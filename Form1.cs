using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading;
using Dapper;
using System.Windows.Forms;
using System.Transactions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Data.Common;

namespace TitaniumMemberGift
{
    public partial class Form1 : Form
    {
        private System.Threading.Timer? _timer;
        private readonly string _connectionString = "Server=localhost;Database=tom;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;";
        private DateTime? _nextRunTime;

        public Form1()
        {
            InitializeComponent();
            Log("系統已啟動，請按「開始每月排程」。");
            lblStartTime.Text = $"程式啟動時間：{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
        }

        private void btnStartSchedule_Click(object sender, EventArgs e)
        {
            Log("已啟動每月鈦金會員獨享禮排程...");
            RunTitaniumGifts();
            ScheduleTitaniumMemberGiftMonthlyRun();

            // 禁用按鈕，避免重複啟動
            btnStartSchedule.Enabled = false;
        }

        private void ScheduleTitaniumMemberGiftMonthlyRun()
        {
            DateTime now = DateTime.Now;
            DateTime nextRun = new DateTime(now.Year, now.Month, 1, 0, 5, 0);
            if (now.Day >= 1)
                nextRun = nextRun.AddMonths(1);

            TimeSpan timeToGo = nextRun - now;

            _timer?.Dispose();
            _timer = new System.Threading.Timer(x =>
            {
                RunTitaniumGifts();
                ScheduleTitaniumMemberGiftMonthlyRun(); // 排下一次
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

            UpdateNextRunLabel($"下次自動執行：{nextRun:yyyy/MM/dd HH:mm:ss}");
        }

        private void btnManualRun_Click(object sender, EventArgs e)
        {
            Log("手動發送當月鈦金會員獨享禮...");
            RunTitaniumGifts();
        }

        private void RunTitaniumGifts()
        {
            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            int currentMonth = now.Month;

            UpdateLastRunLabel($"上次執行：{DateTime.Now:yyyy/MM/dd HH:mm:ss}");

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 取得鈦金禮會員(等級=鈦金、成為會員日<本月1號)
                    var titaniumMembers = conn.Query<(string Card_no, string Card_Type)>(@"
                        SELECT Card_no, Card_Type
                        FROM [dbo].[PointCard]
                        WHERE Card_Type = '9003'
                        AND CardActivationDate < CONVERT(varchar(8), DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1), 112)
                    ").ToList();

                    string year = DateTime.Today.Year.ToString();
                    string month = DateTime.Today.Month.ToString("D2"); // 01, 02 ... 12

                    var alreadySent = conn.Query<string>(@"
                        SELECT Card_no 
                        FROM MemberGiftsLog
                        WHERE GiftType = '2'  -- 獨享禮
                          AND Year = @Year
                          AND Month = @Month;",
                        new { Year = year, Month = month }).ToList();

                    // 過濾掉已經發過的會員
                    var toSendMembers = titaniumMembers
                        .Where(m => !alreadySent.Contains(m.Card_no))
                        .ToList();

                    if (toSendMembers.Count == 0)
                    {
                        Log("本月所有鈦金會員已發過獨享禮。");
                        return;
                    }

                    string projectPath = AppDomain.CurrentDomain.BaseDirectory;
                    string logFolder = System.IO.Path.Combine(projectPath, "LOG");
                    if (!System.IO.Directory.Exists(logFolder))
                        System.IO.Directory.CreateDirectory(logFolder);

                    string logFilePath = System.IO.Path.Combine(logFolder, $"TitaniumGifts_{currentYear}{currentMonth:D2}.log");

                    int successCount = 0;

                    foreach (var member in toSendMembers)
                    {
                        try
                        {
                            using (var tran = conn.BeginTransaction())
                            {

                                //取得獨享禮票券資訊
                                var titaniumGift = GetMemberBenefits("005", member.Card_Type, conn, tran);
                                string pluMagno = titaniumGift?.PLU_Magno ;

                                // 插入 MemberGiftsLog
                                conn.Execute(@"
                                    INSERT INTO MemberGiftsLog
                                    (Card_no, GiftType, Year, Month, Transaction_Time)
                                    VALUES (@Card_no, @GiftType, @Year, @Month, GETDATE())
                                ", new { member.Card_no, GiftType = "2", Year = currentYear.ToString(), Month = currentMonth.ToString("D2") }, tran);


                                var effectiveDay = conn.QueryFirstOrDefault<int?>(@"
                                    SELECT [Effectiveday]
                                    FROM POS2004
                                    WHERE [PLU_Magno] = @PLU_Magno;",
                                    new { PLU_Magno = pluMagno }, tran) ?? 365;

                                CPS3001 cps3001 = new()
                                {
                                    Coup_PLU = pluMagno,
                                    Coup_Type = "01",
                                    Coup_Status = "1",
                                    Effect_SDate = DateTime.Now.Date,
                                    Effect_EDate = DateTime.Now.Date.AddDays(effectiveDay), 
                                    Get_Date = DateTime.Now,
                                    ORG_MEB_No = member.Card_no,
                                    MEB_No = member.Card_no,
                                    Crt_Date = DateTime.Now
                                };

                                CouponClaimResult claimResult = CreateAndClaimCouponWithTransaction(cps3001, conn, tran);

                                MBS4003 mbs4003 = new()
                                {
                                    MEB_No = member.Card_no,
                                    COUP_No = claimResult.CoupNo,
                                    COUP_Type = "01",
                                    CPN_Status = "2",
                                    CPN_GetDate = DateTime.Now,
                                    CPN_ExpireBegDate = DateTime.Now.Date,
                                    CPN_ExpireEndDate = DateTime.Now.Date.AddDays(effectiveDay), 
                                    CPN_PLU_MagNo = claimResult.CoupPLU,
                                    PLU_SerialControl = "Y",
                                    Crt_Date = DateTime.Now
                                };


                                bool mbs4003Success = InsertCouponBenefitWithTransaction(mbs4003, conn, tran);

                                if (!mbs4003Success)
                                {
                                    tran.Rollback();
                                }

                                //發送訊息
                                string couponTemplate = titaniumGift?.Message;
                                string couponMessage = !string.IsNullOrEmpty(couponTemplate)
                                    ? string.Format(couponTemplate, titaniumGift.Bonus)
                                    : "鈦金會員獨享禮";


                                MemberNotifications couponNotifications = new MemberNotifications()
                                {
                                    Card_No = member.Card_no,
                                    Read = "N",
                                    Title = "鈦金會員獨享禮",
                                    Content = couponMessage,
                                    Type = "3",
                                    IsMessageNotification = "Y",
                                    IsPushNotification = "N",
                                    Crt_Date = DateTime.Now,
                                    Crt_Usr_No = "System",
                                    Crt_Usr_Name = "排程自動發送"
                                };

                                var couponSuccess = InsertNotification(couponNotifications, conn, tran);

                                tran.Commit();

                                // 寫入 log
                                using (var file = new System.IO.StreamWriter(logFilePath, true))
                                {
                                    file.WriteLine($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.ff}] 會員 {member.Card_no} 鈦金會員獨享禮發放成功");
                                }

                                successCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"會員 {member.Card_no} 發鈦金會員獨享禮失敗：{ex.Message}");
                            try
                            {
                                string errorFilePath = System.IO.Path.Combine(logFolder, $"Error_{now:yyyyMMdd}.log");
                                using (var file = new System.IO.StreamWriter(errorFilePath, true))
                                {
                                    file.WriteLine($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}] 會員 {member.Card_no} 發鈦金會員獨享禮失敗：{ex.Message}");
                                    if (ex.StackTrace != null)
                                        file.WriteLine(ex.StackTrace);
                                    file.WriteLine(new string('-', 80));
                                }
                            }
                            catch { /* LOG 寫失敗也不影響其他會員 */ }
                        }
                    }

                    Log($"已成功發送 {successCount}/{toSendMembers.Count} 筆鈦金會員獨享禮。");
                    if (_nextRunTime != null)
                        UpdateNextRunLabel($"下次自動發送：{_nextRunTime:yyyy/MM/dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                Log($"[Error] 發鈦金會員獨享禮過程中發生錯誤：{ex.Message}");
            }
        }

        #region UI Helpers

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff");
            string fullMessage = $"[{timestamp}] {message}";
            if (txtLog.InvokeRequired)
                txtLog.Invoke(new Action(() => txtLog.AppendText(fullMessage + "\r\n")));
            else
                txtLog.AppendText(fullMessage + "\r\n");
        }

        private void UpdateLastRunLabel(string text)
        {
            if (lblLastRun.InvokeRequired)
                lblLastRun.Invoke(new Action(() => lblLastRun.Text = text));
            else
                lblLastRun.Text = text;
        }

        private void UpdateNextRunLabel(string text)
        {
            if (lblNextRun.InvokeRequired)
                lblNextRun.Invoke(new Action(() => lblNextRun.Text = text));
            else
                lblNextRun.Text = text;
        }

        #endregion


        /// <summary>
        /// 動態產生並領取票券
        /// </summary>
        public CouponClaimResult CreateAndClaimCouponWithTransaction(CPS3001 cps3001, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                CouponClaimResult result = new() { IsSuccess = false };

                // 新增票券記錄到 CPS3001
                string insertSql = @"
                    INSERT INTO CPS3001 (
                        Coup_No,
                        Coup_PLU,
                        Coup_Type,
                        Coup_Status,
                        Effect_SDate,
                        Effect_EDate,
                        Get_Date,
                        ORG_MEB_No,
                        MEB_No,
                        Crt_Date
                    ) VALUES (
                        @Coup_No,
                        @Coup_PLU,
                        @Coup_Type,
                        @Coup_Status,
                        @Effect_SDate,
                        @Effect_EDate,
                        @Get_Date,
                        @ORG_MEB_No,
                        @MEB_No,
                        @Crt_Date
                    )";

                SqlCommand insertCmd = new(insertSql, conn, transaction);

                // 產生票券編號 - 使用現在時間字串
                string coupNo = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                // 設定參數
                insertCmd.Parameters.AddWithValue("@Coup_No", coupNo);
                insertCmd.Parameters.AddWithValue("@Coup_PLU", cps3001.Coup_PLU);
                insertCmd.Parameters.AddWithValue("@Coup_Type", cps3001.Coup_Type);
                insertCmd.Parameters.AddWithValue("@Coup_Status", cps3001.Coup_Status);
                insertCmd.Parameters.AddWithValue("@Effect_SDate", (object?)cps3001.Effect_SDate ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Effect_EDate", (object?)cps3001.Effect_EDate ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Get_Date", cps3001.Get_Date);
                insertCmd.Parameters.AddWithValue("@ORG_MEB_No", cps3001.ORG_MEB_No);
                insertCmd.Parameters.AddWithValue("@MEB_No", cps3001.MEB_No);
                insertCmd.Parameters.AddWithValue("@Crt_Date", cps3001.Crt_Date);


                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    result.IsSuccess = true;
                    result.CoupNo = coupNo;  // 實際編號會由 stored procedure 更新
                    result.CoupPLU = cps3001.Coup_PLU;
                }

                return result;
            }
            catch (Exception ex)
            {
                // 記錄錯誤日誌
                return new CouponClaimResult { IsSuccess = false };
            }
        }


        public bool InsertCouponBenefitWithTransaction(MBS4003 mbs4003, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                string sql = @"
                INSERT INTO MBS4003 
                (MEB_No, COUP_No, COUP_Type, CPN_Status, CPN_GetDate, CPN_ExpireBegDate, CPN_ExpireEndDate,
                 CPN_PLU_MagNo, PLU_SerialControl, Crt_Date,[Order_Name],[STO_No],[STO_Name],[Contact_Telno])
                VALUES 
                (@MEB_No, @COUP_No, @COUP_Type ,@CPN_Status, @CPN_GetDate, @CPN_ExpireBegDate, @CPN_ExpireEndDate,
                 @CPN_PLU_MagNo,@PLU_SerialControl, GETDATE(),@Order_Name,@STO_No,@STO_Name,@Contact_Telno)";

                SqlCommand cmd = new(sql, conn, transaction);
                cmd.Parameters.AddWithValue("@MEB_No", mbs4003.MEB_No);
                cmd.Parameters.AddWithValue("@COUP_No", mbs4003.COUP_No);          // 從 CPS3001 來
                cmd.Parameters.AddWithValue("@COUP_Type", mbs4003.COUP_Type);
                cmd.Parameters.AddWithValue("@CPN_Status", mbs4003.CPN_Status);
                cmd.Parameters.AddWithValue("@CPN_GetDate", mbs4003.CPN_GetDate);
                cmd.Parameters.AddWithValue("@CPN_ExpireBegDate", (object?)mbs4003.CPN_ExpireBegDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CPN_ExpireEndDate", (object?)mbs4003.CPN_ExpireEndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CPN_PLU_MagNo", mbs4003.CPN_PLU_MagNo);
                cmd.Parameters.AddWithValue("@PLU_SerialControl", mbs4003.PLU_SerialControl);
                cmd.Parameters.AddWithValue("@Order_Name", (object?)mbs4003.Order_Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STO_No", (object?)mbs4003.STO_No ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STO_Name", (object?)mbs4003.STO_Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Contact_Telno", (object?)mbs4003.Contact_Telno ?? DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // 讓 Service 層處理異常和交易回滾
                throw;
            }
        }



        /*取得會員權益*/
        public CardTypeBenefits? GetMemberBenefits(string benefitId, string? cardType, SqlConnection conn, SqlTransaction tran)
        {
            CardTypeBenefits? benefits = null;

            string sql = @"
                SELECT TOP 1 * 
                FROM [dbo].[CardType_Benefits]
                WHERE [Benefit_Id] = @Benefit_Id
                  AND (@Card_Type IS NULL OR [Card_Type] = @Card_Type)";

            using SqlCommand cmd = new(sql, conn, tran);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Benefit_Id", benefitId);
            cmd.Parameters.AddWithValue("@Card_Type", (object?)cardType ?? DBNull.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                benefits = new()
                {
                    Benefit_Id = reader["Benefit_Id"].ToString(),
                    Content = reader["Content"].ToString(),
                    Card_Type = reader["Card_Type"].ToString(),
                    Message = reader["Message"].ToString(),
                    Bonus = reader["Bonus"] == DBNull.Value ? null : Convert.ToInt32(reader["Bonus"]),
                    PLU_Magno = reader["PLU_Magno"].ToString(),
                    Crt_Date = reader["Crt_Date"] == DBNull.Value ? null : Convert.ToDateTime(reader["Crt_Date"]),
                    Crt_Usr_No = reader["Crt_Usr_No"].ToString(),
                    Crt_Usr_Name = reader["Crt_Usr_Name"].ToString(),
                    Upd_Date = reader["Upd_Date"] == DBNull.Value ? null : Convert.ToDateTime(reader["Upd_Date"]),
                    Upd_Usr_No = reader["Upd_Usr_No"].ToString(),
                    Upd_Usr_Name = reader["Upd_Usr_Name"].ToString(),
                    PKNO = Convert.ToInt64(reader["PKNO"])
                };
            }

            reader.Close();
            return benefits;
        }


        /// <summary>
        /// 新增通知記錄 (使用 Transaction)
        /// </summary>
        /// <param name="memberNotifications">通知資料模型</param>
        /// <param name="conn">資料庫連線</param>
        /// <param name="transaction">交易物件</param>
        /// <returns>成功則回傳 true，否則 false</returns>
        public bool InsertNotification(MemberNotifications memberNotifications, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                string sql = @"
                    INSERT INTO [MemberNotifications] (
                        Card_No,
                        [Read],
                        Msg_Id,
                        Push_Set_Id,
                        Push_Id,
                        Title,
                        Content,
                        [Type],
                        IsMessageNotification,
                        IsPushNotification,
                        Effective_Date,
                        Push_Date,
                        Crt_Date,
                        Crt_Usr_No,
                        Crt_Usr_Name,
                        Upd_Date,
                        Upd_Usr_No,
                        Upd_Usr_Name
                    ) VALUES (
                        @Card_No,
                        @Read,
                        @Msg_Id,
                        @Push_Set_Id,
                        @Push_Id,
                        @Title,
                        @Content,
                        @Type,
                        @IsMessageNotification,
                        @IsPushNotification,
                        @Effective_Date,
                        @Push_Date,
                        @Crt_Date,
                        @Crt_Usr_No,
                        @Crt_Usr_Name,
                        @Upd_Date,
                        @Upd_Usr_No,
                        @Upd_Usr_Name
                    )";

                using (SqlCommand cmd = new(sql, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Card_No", (object?)memberNotifications.Card_No ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Read", (object?)memberNotifications.Read ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Msg_Id", (object?)memberNotifications.Msg_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Push_Set_Id", (object?)memberNotifications.Push_Set_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Push_Id", (object?)memberNotifications.Push_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Title", (object?)memberNotifications.Title ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Content", (object?)memberNotifications.Content ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", (object?)memberNotifications.Type ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsMessageNotification", (object?)memberNotifications.IsMessageNotification ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPushNotification", (object?)memberNotifications.IsPushNotification ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Effective_Date", (object?)memberNotifications.Effective_Date ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Push_Date", (object?)memberNotifications.Push_Date ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Crt_Date", (object?)memberNotifications.Crt_Date ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Crt_Usr_No", (object?)memberNotifications.Crt_Usr_No ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Crt_Usr_Name", (object?)memberNotifications.Crt_Usr_Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Upd_Date", (object?)memberNotifications.Upd_Date ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Upd_Usr_No", (object?)memberNotifications.Upd_Usr_No ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Upd_Usr_Name", (object?)memberNotifications.Upd_Usr_Name ?? DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
