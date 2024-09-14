using KOK.ApiHandler.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post
{
    [Serializable]
    public class Post
    {
        public Guid? PostId { get; set; }
        public string? Caption { get; set; }
        public DateTime? UploadTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? MemberId { get; set; }

        public Guid? RecordingId { get; set; }
        public string SongUrl { get; set; }
        public PostStatus? Status { get; set; }
        public PostType? PostType { get; set; }
        public float? Score { get; set; }
        public Guid? OriginPostId { get; set; }
        public virtual Account Member { get; set; } = null!;
    }
}
