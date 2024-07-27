using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item
{
    public class AddKaraokeRoomRequest
    {
        public string RoomLog { get; set; } = null!;
        public Guid CreatorId { get; set; }
    }
}
