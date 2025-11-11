using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumMemberGift
{
    public class CouponClaimResult
    {
        /// <summary>
        /// 票券號碼
        /// </summary>
        public string CoupNo { get; set; } = string.Empty;

        /// <summary>
        /// 商品編號(票券PLU)
        /// </summary>
        public string CoupPLU { get; set; } = string.Empty;

        /// <summary>
        /// 有效期限-起
        /// </summary>
        public DateTime? EffectSDate { get; set; }

        /// <summary>
        /// 有效期限-迄
        /// </summary>
        public DateTime? EffectEDate { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
