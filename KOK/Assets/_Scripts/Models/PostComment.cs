using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class PostComment
    {
        public PostComment()
        {
            Reports = new HashSet<Report>();
        }

        public Guid CommentId { get; set; }
        public string Comment { get; set; } = null!;
        public Guid MemberId { get; set; }
        public Guid PostId { get; set; }

        public virtual Account Member { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
        public virtual ICollection<Report> Reports { get; set; }
    }
}
