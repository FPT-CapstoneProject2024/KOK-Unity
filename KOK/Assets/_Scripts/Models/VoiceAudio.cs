using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class VoiceAudio
    {
        public Guid VoiceId { get; set; }
        public string VoiceUrl { get; set; } = null!;
        public double DurationSecond { get; set; }
        public DateTime UploadTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Pitch { get; set; }
        public Guid RecordingId { get; set; }
        public Guid MemberId { get; set; }

        public virtual Account Member { get; set; } = null!;
        public virtual Recording Recording { get; set; } = null!;
    }
}
