using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class Conversation
    {
        public Conversation()
        {
            Messages = new HashSet<Message>();
        }

        public Guid ConversationId { get; set; }
        public Guid MemberId1 { get; set; }
        public Guid MemberId2 { get; set; }
        public int ConversationType { get; set; }
        public Guid SupportRequestId { get; set; }

        public virtual Account MemberId1Navigation { get; set; } = null!;
        public virtual Account MemberId2Navigation { get; set; } = null!;
        public virtual SupportRequest SupportRequest { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; }
    }
}
