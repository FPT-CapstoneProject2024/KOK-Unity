using System.Collections.Generic;
using System;

namespace KOK.ApiHandler.DTOModels
{
    public class PurchasedSong
    {
        public Guid PurchasedSongId { get; set; }
        public Guid SongId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Guid MemberId { get; set; }
        public string SongName { get; set; }
        public decimal Price { get; set; }
        public string SongUrl { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Singers { get; set; }
        public List<string> Artists { get; set; }
        public bool IsFavorite { get; set; } = false;
    }
}
