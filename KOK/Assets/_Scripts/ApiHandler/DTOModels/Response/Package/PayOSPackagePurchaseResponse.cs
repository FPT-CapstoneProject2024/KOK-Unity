namespace KOK.ApiHandler.DTOModels
{
    public class PayOSPackagePurchaseResponse
    {
        // PayOS payment link detail
        public string bin { get; set; } = string.Empty;
        public string accountNumber { get; set; } = string.Empty;
        public int amount { get; set; }
        public string description { get; set; } = string.Empty;
        public long orderCode { get; set; }
        public string currency { get; set; } = string.Empty;
        public string paymentLinkId { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string checkoutUrl { get; set; } = string.Empty;
        public string qrCode { get; set; } = string.Empty;

        // Package & transaction detail
        public string PackageId { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public int UpAmount { get; set; }
        public decimal MoneyAmount { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string MonetaryTransactionId { get; set; } = string.Empty;
    }
}
