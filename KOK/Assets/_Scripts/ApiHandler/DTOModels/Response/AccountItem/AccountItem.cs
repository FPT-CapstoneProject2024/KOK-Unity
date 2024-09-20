using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response
{
    public class AccountItem
    {
        public Guid AccountItemId { get; set; }
        public string ItemStatus { get; set; }
        public DateTime ActivateDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public Guid ItemId { get; set; }
        public Guid MemberId { get; set; }
        public int ObtainMethod { get; set; }

        public virtual Item Item { get; set; } = null!;
    }
}
