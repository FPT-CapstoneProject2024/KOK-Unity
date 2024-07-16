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
        public int? Status { get; set; }
        public int? PostType { get; set; }
        public Guid? OriginPostId { get; set; }
    }
}
