using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request.PostComment
{
    [Serializable]
    public class PostCommentFilter
    {
        public Guid MemberId { get; set; } = Guid.Empty;
        public Guid PostId { get; set; } = Guid.Empty;
        public int CommentType { get; set; } = 0;
        public string Comment { get; set; } = string.Empty;
        public int Status { get; set; } = 0;
    }
}
