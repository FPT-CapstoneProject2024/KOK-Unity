using System;

namespace KOK.ApiHandler.DTOModels
{
    public class SongPurchaseResponse
    {
        public Guid? InAppTransactionId { get; set; }
        public InAppTransactionStatus Status { get; set; } = InAppTransactionStatus.PENDING;
        public DateTime? CreatedDate { get; set; }
        public InAppTransactionType TransactionType { get; set; } = InAppTransactionType.BUY_SONG;
        public Guid? MemberId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? SongId { get; set; }
        public decimal? UpAmountBefore { get; set; }
        public Guid? MonetaryTransactionId { get; set; }
        public decimal? UpTotalAmount { get; set; }
    }
}
