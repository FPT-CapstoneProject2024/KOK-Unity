using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class PostVote
    {
        public Guid MemberId { get; set; }
        public Guid PostId { get; set; }
        public int VoteType { get; set; }

        public virtual Account Member { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
