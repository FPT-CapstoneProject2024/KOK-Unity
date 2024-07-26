using System;
using System.Collections.Generic;

namespace KOK.ApiHandler.DTOModels
{
    public class CreateRecordingRequest
    {
        public string RecordingName { get; set; } = null!;
        public int RecordingType { get; set; }
        public int Score { get; set; }
        public Guid SongId { get; set; }
        public Guid HostId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid KaraokeRoomId { get; set; }
    }
}
