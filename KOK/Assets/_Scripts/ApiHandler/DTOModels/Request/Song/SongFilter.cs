using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Song
{
    [Serializable]
    public class SongFilter
    {
        public string SongName { get; set; } = string.Empty;
        public string SongCode { get; set; } = string.Empty;
    }
}
