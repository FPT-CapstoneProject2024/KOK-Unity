using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransactions
{
    [Serializable]
    public class InAppTransaction
    {
        public Guid InAppTransactionId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TransactionType { get; set; }
        public Guid MemberId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? SongId { get; set; }
        public decimal UpAmountBefore { get; set; }
        public Guid? MonetaryTransactionId { get; set; }
        public decimal UpTotalAmount { get; set; }
    }
}
