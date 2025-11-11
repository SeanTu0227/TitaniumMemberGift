using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumMemberGift
{
    public class CardTypeBenefits
    {
        public string? Benefit_Id { get; set; }
        public string? Content { get; set; }
        public string? Message { get; set; }
        public string? Card_Type { get; set; }
        public int? Bonus { get; set; }
        public string? PLU_Magno { get; set; }
        public DateTime? Crt_Date { get; set; }
        public string? Crt_Usr_No { get; set; }
        public string? Crt_Usr_Name { get; set; }
        public DateTime? Upd_Date { get; set; }
        public string? Upd_Usr_No { get; set; }
        public string? Upd_Usr_Name { get; set; }
        public long PKNO { get; set; }
    }
}
