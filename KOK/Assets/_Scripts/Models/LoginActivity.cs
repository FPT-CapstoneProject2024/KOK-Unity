using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class LoginActivity
    {
        public Guid LoginId { get; set; }
        public DateTime LoginTime { get; set; }
        public string? LoginDevice { get; set; }
        public Guid MemberId { get; set; }

        public virtual Account Member { get; set; } = null!;
    }
}
