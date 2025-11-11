using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumMemberGift
{
    public class MBS4003
    {
        /// <summary>
        /// REC_KEY，系統產生(流水號)
        /// </summary>
        public long PKNO { get; set; }

        /// <summary>
        /// 會員編號 PointCard.Card_no
        /// </summary>
        public string MEB_No { get; set; } = null!;

        /// <summary>
        /// 票券號碼 CPS3001.COUP_No
        /// </summary>
        public string COUP_No { get; set; } = null!;

        /// <summary>
        /// 票券種類 01=兌換券 02=抵用券 03=優惠券 04=紅利商品券
        /// </summary>
        public string? COUP_Type { get; set; }

        /// <summary>
        /// 狀態 2=已領取 3=已兌回/已核銷 5=作廢/註銷(不使用) 6=轉贈(不使用)
        /// </summary>
        public string CPN_Status { get; set; } = null!;

        /// <summary>
        /// 領取日期
        /// </summary>
        public DateTime CPN_GetDate { get; set; }

        /// <summary>
        /// 有效起始日期
        /// </summary>
        public DateTime? CPN_ExpireBegDate { get; set; } = null;

        /// <summary>
        /// 有效終止日期
        /// </summary>
        public DateTime? CPN_ExpireEndDate { get; set; } = null;

        /// <summary>
        /// 票券商品貨號
        /// </summary>
        public string CPN_PLU_MagNo { get; set; } = null!;

        /// <summary>
        /// 銷售日期，非銷貨交易取得的票券不會紀錄銷售日期(不確定)
        /// </summary>
        public DateTime? CPN_SaleDate { get; set; }

        /// <summary>
        /// 兌回日期/核銷日期
        /// </summary>
        public DateTime? CPN_BackDate { get; set; }

        /// <summary>
        /// 作廢日期(不使用)
        /// </summary>
        public DateTime? CPN_VoidDate { get; set; }

        /// <summary>
        /// 轉贈日期(不使用)
        /// </summary>
        public DateTime? CPN_GiveDate { get; set; }

        /// <summary>
        /// 轉贈對象(不使用)
        /// </summary>
        public string? CPN_GiveMebNo { get; set; }

        /// <summary>
        /// 序號管理 Y=序號管理 N=通用券，預設值為N
        /// </summary>
        public string PLU_SerialControl { get; set; } = "N";

        /// <summary>
        /// 訂購人姓名
        /// </summary>
        public string? Order_Name { get; set; }

        /// <summary>
        /// 取貨商店代號
        /// </summary>
        public string? STO_No { get; set; }

        /// <summary>
        /// 取貨店名
        /// </summary>
        public string? STO_Name { get; set; }

        /// <summary>
        /// 聯繫電話
        /// </summary>
        public string? Contact_Telno { get; set; }

        /// <summary>
        /// 異動備註
        /// </summary>
        public string? CPN_Remark { get; set; }

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
    }
}
