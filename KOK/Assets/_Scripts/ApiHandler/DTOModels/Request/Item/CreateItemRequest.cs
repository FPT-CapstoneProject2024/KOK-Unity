using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item
{
    public class CreateItemRequest
    {
        public string? ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string ItemDescription { get; set; } = null!;
        public int? Type { get; set; } = (int)ItemType.DEFAULT;
        public int? Status { get; set; } = (int)ItemStatus.PENDING;
        public bool? CanExpire { get; set; } = false;
        public bool? CanStack { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatorId { get; set; }
        public string? PrefabCode { get; set; }
        public decimal? ItemBuyPrice { get; set; }
        public decimal? ItemSellPrice { get; set; }
    }
}
