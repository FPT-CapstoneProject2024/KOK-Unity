using System;

namespace KOK.ApiHandler.DTOModels
{
    public class FavoriteSongRequest
    {
        public Guid MemberId { get; set; }
        public Guid SongId { get; set; }
    }
}
