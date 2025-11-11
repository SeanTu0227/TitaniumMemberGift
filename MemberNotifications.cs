using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumMemberGift
{
    public class MemberNotifications
    {
        /// <summary>
        /// 會員編號 PointCard.Card_no
        /// </summary>
        public string? Card_No { get; set; }

        /// <summary>
        /// 讀取訊息 Y:已讀, N:未讀
        /// </summary>
        public string? Read { get; set; }

        /// <summary>
        /// 訊息代碼
        /// </summary>
        public string? Msg_Id { get; set; }

        /// <summary>
        /// 設定檔代碼
        /// </summary>
        public string? Push_Set_Id { get; set; }

        /// <summary>
        /// 發送代碼
        /// </summary>
        public string? Push_Id { get; set; }

        /// <summary>
        /// 訊息標題
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 訊息類別 0: 系統公告, 1: 一般訊息, 2: 活動訊息, 3: 權益提醒
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 是否為一般通知(小鈴鐺) Y:是, N:不是
        /// </summary>
        public string? IsMessageNotification { get; set; }

        /// <summary>
        /// 是否為推播通知 Y:是, N:不是
        /// </summary>
        public string? IsPushNotification { get; set; }

        /// <summary>
        /// 預計生效時間
        /// </summary>
        public DateTime? Effective_Date { get; set; }

        /// <summary>
        /// 預計推播時間
        /// </summary>
        public DateTime? Push_Date { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime? Crt_Date { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        public string? Crt_Usr_No { get; set; }

        /// <summary>
        /// 建立人員姓名
        /// </summary>
        public string? Crt_Usr_Name { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? Upd_Date { get; set; }

        /// <summary>
        /// 更新人員
        /// </summary>
        public string? Upd_Usr_No { get; set; }

        /// <summary>
        /// 更新人員姓名
        /// </summary>
        public string? Upd_Usr_Name { get; set; }

        /// <summary>
        /// REC_KEY，系統產生(流水號)
        /// </summary>
        public long PKNO { get; set; }
    }
}
