using System;

namespace KOK.ApiHandler.DTOModels
{
    public class PayOSPackagePaymentMethodResponse
    {
        public string checkoutUrl { get; set; } = string.Empty;
        public string qrCode { get; set; } = string.Empty;

        // Package & transaction detail
        public string PackageId { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public int UpAmount { get; set; }
        public decimal MoneyAmount { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string MonetaryTransactionId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
