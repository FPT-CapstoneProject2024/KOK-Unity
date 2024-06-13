using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class MoneyTransaction
    {
        public Guid MoneyTransactionId { get; set; }
        public int PaymentType { get; set; }
        public string PaymentCode { get; set; } = null!;
        public decimal MoneyAmount { get; set; }
        public string Currency { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid PackageId { get; set; }
        public Guid MemberId { get; set; }

        public virtual Account Member { get; set; } = null!;
        public virtual Package Package { get; set; } = null!;
    }
}
