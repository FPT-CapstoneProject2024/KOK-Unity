using System;

namespace KOK.ApiHandler.DTOModels
{
    public class FavoriteSongFilter
    {
        public string SongName { get; set; } = string.Empty;
        public Guid MemberId { get; set; }
    }
}
