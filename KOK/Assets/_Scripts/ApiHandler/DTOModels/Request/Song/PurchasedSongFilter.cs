using System;

namespace KOK.ApiHandler.DTOModels
{
    public class PurchasedSongFilter
    {
        public string SongName { get; set; } = string.Empty;
        public Guid MemberId { get; set; }
    }
}
