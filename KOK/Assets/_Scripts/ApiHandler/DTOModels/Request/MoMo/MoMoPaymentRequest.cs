using System;

namespace KOK.ApiHandler.DTOModels
{
    public class MoMoPaymentRequest
    {
        public PaymentType PaymentType { get; set; }
        public string PaymentCode { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public Guid PackageId { get; set; }
        public Guid MemberId { get; set; }
    }
}
