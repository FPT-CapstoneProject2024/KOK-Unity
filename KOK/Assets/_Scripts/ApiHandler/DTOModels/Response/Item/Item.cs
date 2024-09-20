using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response
{
    [Serializable]
    public class Item
    {
        public Guid? ItemId { get; set; }
        public string? ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string ItemDescription { get; set; } = null!;
        public ItemType? ItemType { get; set; }
        public ItemStatus? ItemStatus { get; set; }
        public bool? CanExpire { get; set; }
        public bool? CanStack { get; set; }
        public bool? IsOwned { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatorId { get; set; }
        public string? PrefabCode { get; set; }

        public decimal ItemBuyPrice { get; set; }
        public decimal ItemSellPrice { get; set; }
    }
}
