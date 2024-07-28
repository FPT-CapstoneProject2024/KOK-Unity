using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response
{
    [Serializable]
    public class Recording
    {
        public Guid RecordingId { get; set; }
        public string RecordingName { get; set; } = null!;
        public string RecordingType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int Score { get; set; }
        public Guid PurchasedSongId { get; set; }
        public Guid HostId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid KaraokeRoomId { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
