using System;
using System.Collections.Generic;

namespace KOK.ApiHandler.DTOModels
{
    public class FavoriteSong
    {
        public Guid? MemberId { get; set; }
        public Guid? SongId { get; set; }
        public string SongName { get; set; }
        public List<string> Singers { get; set; }
        public List<string> Artists { get; set; }
        public List<string> Genres { get; set; }
    }
}
