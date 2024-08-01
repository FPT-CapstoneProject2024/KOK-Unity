using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KOK.ApiHandler.DTOModels
{
    public class SongDetail
    {
        public Guid? SongId { get; set; }
        public string SongName { get; set; }
        public string SongDescription { get; set; }
        public string SongUrl { get; set; }
        public SongStatus? SongStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string SongCode { get; set; }
        public DateTime? PublicDate { get; set; }
        public Guid? CreatorId { get; set; }
        public decimal Price { get; set; }
        public List<string> Genre { get; set; }
        public List<string> Singer { get; set; }
        public List<string> Artist { get; set; }

        //public List<SongArtist> SongArtists { get; set; }
        //public List<SongGenre> SongGenres { get; set; }
        //public List<SongSinger> SongSingers { get; set; }

        public bool isFavorite = false;
        public bool isPurchased = false;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
