using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class PostShare
    {
        public Guid PostShareId { get; set; }
        public string? Caption { get; set; }
        public DateTime ShareTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public Guid MemberId { get; set; }
        public Guid PostId { get; set; }

        public virtual Account Member { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
