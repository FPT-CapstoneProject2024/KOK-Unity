using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.ApiHandler.DTOModels
{
    public class FavoriteSongRequest
    {
        public Guid MemberId { get; set; }
        public Guid SongId { get; set; }
    }
}
