using KOK.ApiHandler.DTOModels;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransactions;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PurchasedSongs
{
    [Serializable]
    public class PurchasedSong
    {
        public Guid PurchasedSongId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Guid MemberId { get; set; }
        public Guid SongId { get; set; }
        public string? SongName { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Singers { get; set; }       
        public decimal? SongPrice { get; set; }
        public List<string> Artists { get; set; } = new List<string>();
        public virtual Account Member { get; set; } = null!;
        public virtual Song Song { get; set; } = null!;
        public virtual InAppTransaction? InAppTransaction { get; set; }
        public virtual ICollection<Recording> Recordings { get; set; }
    }
}
