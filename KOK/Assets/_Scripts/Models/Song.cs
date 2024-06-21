using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class Song
    {
        public Song()
        {
            FavouriteSongs = new HashSet<FavouriteSong>();
            InAppTransactions = new HashSet<InAppTransaction>();
            PurchasedSongs = new HashSet<PurchasedSong>();
            Recordings = new HashSet<Recording>();
        }

        public Guid SongId { get; set; }
        public string SongName { get; set; } = null!;
        public string? SongDescription { get; set; }
        public string? SongUrl { get; set; }
        public int SongStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? SongCode { get; set; }
        public DateTime? PublicDate { get; set; }
        public Guid CreatorId { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Singer { get; set; }

        public virtual Account Creator { get; set; } = null!;
        public virtual ICollection<FavouriteSong> FavouriteSongs { get; set; }
        public virtual ICollection<InAppTransaction> InAppTransactions { get; set; }
        public virtual ICollection<PurchasedSong> PurchasedSongs { get; set; }
        public virtual ICollection<Recording> Recordings { get; set; }
    }
}
