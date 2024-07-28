using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class CreateRecordingRequest
    {
        public string RecordingName { get; set; } = null!;
        public int RecordingType { get; set; }
        public int Score { get; set; }
        public Guid PurchasedSongId { get; set; }
        public Guid HostId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid KaraokeRoomId { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public ICollection<CreateVoiceAudioRequest> VoiceAudios { get; set; } = new List<CreateVoiceAudioRequest>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
