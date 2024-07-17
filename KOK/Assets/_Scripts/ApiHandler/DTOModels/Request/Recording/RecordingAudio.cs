using System;

namespace KOK.ApiHandler.DTOModels
{
    public class RecordingAudio
    {
        public string VoiceUrl { get; set; } = null!;
        public double DurationSecond { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid MemberId { get; set; }
    }
}
