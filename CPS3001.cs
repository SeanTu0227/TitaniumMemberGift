using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumMemberGift
{
    public class CPS3001
    {
        /// <summary>
        /// 票券印刷碼
        /// </summary>
        public string? Blank_No { get; set; }

        /// <summary>
        /// 印刷券批次
        /// </summary>
        public decimal? Blank_Batch { get; set; }

        /// <summary>
        /// 庫存單位
        /// </summary>
        public string? Stock_StoreNo { get; set; }

        /// <summary>
        /// 票券發行批號
        /// </summary>
        public string? Coup_IssueNo { get; set; }

        /// <summary>
        /// 票券號碼(自編碼=商品編號+流水號)
        /// </summary>
        public string? Coup_No { get; set; }

        /// <summary>
        /// 商品編號(與 POS2004.PLU_Magno同)
        /// </summary>
        public string? Coup_PLU { get; set; }

        /// <summary>
        /// 票券種類 01=兌幣券 02=折扣券 3=贈品優惠券 4=商品券(領用券) 5=熊好康券(實體轉虛擬)
        /// </summary>
        public string? Coup_Type { get; set; }

        /// <summary>
        /// 票券流水號
        /// </summary>
        public decimal? Coup_SN { get; set; }

        /// <summary>
        /// 票券狀態 0=發行 1=領用中 2.已兌回/核銷
        /// </summary>
        public string? Coup_Status { get; set; }

        /// <summary>
        /// 票券面額
        /// </summary>
        public decimal? Coup_Value { get; set; }

        /// <summary>
        /// 折扣數
        /// </summary>
        public decimal? Coup_Disc { get; set; }

        /// <summary>
        /// 實際售價
        /// </summary>
        public decimal? PLU_SalePrc { get; set; }

        /// <summary>
        /// 發行日期 YYYY/MM/DD
        /// </summary>
        public DateTime? Coup_IssueDate { get; set; }

        /// <summary>
        /// 有效期限-起 (*代表沒有期限) YYYY/MM/DD
        /// </summary>
        public DateTime? Effect_SDate { get; set; }

        /// <summary>
        /// 有效期限-迄 (*代表沒有期限) YYYY/MM/DD
        /// </summary>
        public DateTime? Effect_EDate { get; set; }

        /// <summary>
        /// 領用日期 YYYY/MM/DD
        /// </summary>
        public DateTime? Get_Date { get; set; }

        /// <summary>
        /// 銷售日期 YYYY/MM/DD
        /// </summary>
        public DateTime? Sale_Date { get; set; }

        /// <summary>
        /// 兌回/核銷 日期 YYYY/MM/DD
        /// </summary>
        public DateTime? BACK_Date { get; set; }

        /// <summary>
        /// 作廢日期 YYYY/MM/DD
        /// </summary>
        public DateTime? VOID_Date { get; set; }

        /// <summary>
        /// 銷售店號
        /// </summary>
        public string? Sale_StoreNo { get; set; }

        /// <summary>
        /// 銷售機台
        /// </summary>
        public string? Sale_ECR_No { get; set; }

        /// <summary>
        /// 銷售交易序號
        /// </summary>
        public decimal? Sale_TXN_No { get; set; }

        /// <summary>
        /// 兌回單位
        /// </summary>
        public string? Back_StoreNo { get; set; }

        /// <summary>
        /// 兌回機台
        /// </summary>
        public string? Back_ECR_No { get; set; }

        /// <summary>
        /// 兌回交易序號
        /// </summary>
        public decimal? Back_TXN_No { get; set; }

        /// <summary>
        /// 兌回店號(前台抵用)
        /// </summary>
        public string? Use_StoreNo { get; set; }

        /// <summary>
        /// 兌回機台(前台抵用)
        /// </summary>
        public string? Use_ECR_No { get; set; }

        /// <summary>
        /// 兌回交易序號(前台抵用)
        /// </summary>
        public decimal? Use_TXN_No { get; set; }

        /// <summary>
        /// 行銷代碼
        /// </summary>
        public string? Campaign_id { get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
        public string? Promo_id { get; set; }

        /// <summary>
        /// 票券歸屬會員編號
        /// </summary>
        public string? MEB_No { get; set; }

        /// <summary>
        /// 轉贈日期
        /// </summary>
        public DateTime? CPN_GiveDate { get; set; }

        /// <summary>
        /// 票券原歸屬會員編號
        /// </summary>
        public string? ORG_MEB_No { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 展延日期 YYYY/MM/DD
        /// </summary>
        public DateTime? Extd_Date { get; set; }

        /// <summary>
        /// 展延編號
        /// </summary>
        public string? Extd_No { get; set; }

        /// <summary>
        /// 展延前有效期限-迄 (*代表沒有期限) YYYY/MM/DD
        /// </summary>
        public DateTime? Extd_Effect_EDate { get; set; }

        /// <summary>
        /// 銷售人員
        /// </summary>
        public string? Sale_USR_No { get; set; }

        /// <summary>
        /// 兌換人員
        /// </summary>
        public string? Back_USR_No { get; set; }

        /// <summary>
        /// 作廢人員
        /// </summary>
        public string? Void_USR_No { get; set; }

        /// <summary>
        /// 展期人員
        /// </summary>
        public string? Extd_USR_No { get; set; }

        /// <summary>
        /// 兌回人員
        /// </summary>
        public string? Use_USR_No { get; set; }

        /// <summary>
        /// 票券列印日期 (實體券)
        /// </summary>
        public DateTime? Print_Date { get; set; }


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
        /// 
        public long PKNO { get; set; }
    }
}
