using System;

namespace KOK.ApiHandler.DTOModels
{
    public class PayOSPackagePurchaseRequest
    {
        public PaymentType PaymentType { get; set; } = PaymentType.PAYOS;
        public string PaymentCode { get; set; } = "N/A";
        public string Currency { get; set; } = "VND";
        public Guid PackageId { get; set; }
        public Guid MemberId { get; set; }
    }
}
