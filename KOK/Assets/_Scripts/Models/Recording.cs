using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class Recording
    {
        public Recording()
        {
            Posts = new HashSet<Post>();
            VoiceAudios = new HashSet<VoiceAudio>();
        }

        public Guid RecordingId { get; set; }
        public string RecordingName { get; set; } = null!;
        public int RecordingType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Score { get; set; }
        public Guid SongId { get; set; }
        public Guid HostId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid KaraokeRoomId { get; set; }

        public virtual Account Host { get; set; } = null!;
        public virtual KaraokeRoom KaraokeRoom { get; set; } = null!;
        public virtual Account Owner { get; set; } = null!;
        public virtual Song Song { get; set; } = null!;
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<VoiceAudio> VoiceAudios { get; set; }
    }
}
