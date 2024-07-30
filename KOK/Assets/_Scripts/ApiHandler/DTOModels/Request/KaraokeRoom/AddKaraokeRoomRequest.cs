using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.ApiHandler.DTOModels
{
    public class AddKaraokeRoomRequest
    {
        public string RoomLog { get; set; } = null!;
        public Guid CreatorId { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
