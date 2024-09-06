using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response
{
    public class Report
    {
        public Guid ReportId { get; set; }
        public Guid ReporterId { get; set; }
        public Guid ReportedAccountId { get; set; }
        public string ReportCategory { get; set; }
        public string Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreateTime { get; set; }
        public string ReportType { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? RoomId { get; set; }
        public string? Title { get; set; }

        public virtual Post.Post? Post { get; set; } = null!;
        public virtual Account ReportedAccount { get; set; } = null!;
        public virtual Account Reporter { get; set; } = null!;
        public virtual KaraokeRoom? Room { get; set; } = null!;
        public virtual PostComment.PostComment? Comment { get; set; } = null!;
    }
}
