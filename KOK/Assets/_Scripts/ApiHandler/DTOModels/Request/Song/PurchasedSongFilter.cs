using Newtonsoft.Json;
using System;

namespace KOK.ApiHandler.DTOModels
{
    public class PurchasedSongFilter
    {
        public string SongName { get; set; } = string.Empty;
        public Guid MemberId { get; set; }
        public Guid SongId { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
