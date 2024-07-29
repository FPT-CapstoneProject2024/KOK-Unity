using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response
{
    [Serializable]
    public class KaraokeRoom
    {        
        public Guid RoomId { get; set; }
        public string RoomLog { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public Guid CreatorId { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
