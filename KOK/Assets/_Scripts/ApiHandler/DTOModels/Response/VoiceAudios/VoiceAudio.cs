using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios
{
    [Serializable]
    public class VoiceAudio
    {
        public Guid VoiceId { get; set; }
        public string VoiceUrl { get; set; } = null!;
        public double DurationSecond { get; set; }
        public DateTime UploadTime { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float? Volume { get; set; }
        public int Pitch { get; set; }
        public Guid RecordingId { get; set; }
        public Guid MemberId { get; set; }
    }
}
