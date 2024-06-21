using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class Report
    {
        public Guid ReportId { get; set; }
        public Guid ReporterId { get; set; }
        public Guid ReportedAccountId { get; set; }
        public int ReportCategory { get; set; }
        public int Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreateTime { get; set; }
        public int ReportType { get; set; }
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid RoomId { get; set; }

        public virtual PostRate Comment { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
        public virtual Account ReportedAccount { get; set; } = null!;
        public virtual Account Reporter { get; set; } = null!;
        public virtual KaraokeRoom Room { get; set; } = null!;
    }
}
