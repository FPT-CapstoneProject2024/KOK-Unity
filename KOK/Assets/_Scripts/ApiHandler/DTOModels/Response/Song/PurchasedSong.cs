using System.Collections.Generic;
using System;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransactions;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Songs;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;

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

        public virtual Account Member { get; set; } = null!;
        public virtual Song Song { get; set; } = null!;
        public virtual InAppTransaction? InAppTransaction { get; set; }
        public virtual ICollection<Recording> Recordings { get; set; }
    }
}
