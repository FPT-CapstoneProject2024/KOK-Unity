using System;

namespace KOK.ApiHandler.DTOModels
{
    public class SongPurchaseRequest
    {
        public Guid SongId { get; set; }
        public Guid MemberId { get; set; }
    }
}
