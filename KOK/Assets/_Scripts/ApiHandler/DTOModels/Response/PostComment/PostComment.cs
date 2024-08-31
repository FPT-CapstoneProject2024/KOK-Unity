using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment
{
    [Serializable]
    public class PostComment
    {
        public Guid? CommentId { get; set; }
        public string Comment { get; set; } = null!;
        public int CommentType { get; set; }
        public PostCommentStatus? Status { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Guid MemberId { get; set; }
        public Guid PostId { get; set; }
        public DateTime UploadTime { get; set; }

        public Account Member { get; set; }
        public List<PostComment> InverseParentComment { get; set; }
    }
}
