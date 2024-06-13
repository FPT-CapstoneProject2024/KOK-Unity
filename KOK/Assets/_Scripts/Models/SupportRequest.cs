using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class SupportRequest
    {
        public SupportRequest()
        {
            Conversations = new HashSet<Conversation>();
        }

        public Guid RequestId { get; set; }
        public string Problem { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public int Category { get; set; }
        public int Status { get; set; }
        public Guid SenderId { get; set; }

        public virtual Account Sender { get; set; } = null!;
        public virtual ICollection<Conversation> Conversations { get; set; }
    }
}
