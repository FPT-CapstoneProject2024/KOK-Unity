using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class KaraokeRoom
    {
        public KaraokeRoom()
        {
            Recordings = new HashSet<Recording>();
            Reports = new HashSet<Report>();
        }

        public Guid RoomId { get; set; }
        public string RoomLog { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public Guid CreatorId { get; set; }

        public virtual Account Creator { get; set; } = null!;
        public virtual ICollection<Recording> Recordings { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
