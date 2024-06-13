using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class Post
    {
        public Post()
        {
            PostComments = new HashSet<PostComment>();
            PostShares = new HashSet<PostShare>();
            PostVotes = new HashSet<PostVote>();
            Reports = new HashSet<Report>();
        }

        public Guid PostId { get; set; }
        public string? Caption { get; set; }
        public DateTime UploadTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public Guid MemberId { get; set; }
        public Guid RecordingId { get; set; }

        public virtual Account Member { get; set; } = null!;
        public virtual Recording Recording { get; set; } = null!;
        public virtual ICollection<PostComment> PostComments { get; set; }
        public virtual ICollection<PostShare> PostShares { get; set; }
        public virtual ICollection<PostVote> PostVotes { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
