using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Songs
{
    [Serializable]
    public class Song
    {
        public Guid? SongId { get; set; }
        public string SongName { get; set; } = null!;
        public string? SongDescription { get; set; }
        public string? SongUrl { get; set; }
        public SongStatus SongStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? SongCode { get; set; }
        public DateTime? PublicDate { get; set; }
        public Guid CreatorId { get; set; }
        public decimal Price { get; set; }
    }
}
