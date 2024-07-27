using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post
{
    public class CreatePostRequest
    {
        public string? Caption { get; set; }
        public Guid MemberId { get; set; }
        public Guid RecordingId { get; set; }
        public int Status { get; set; }
        public int PostType { get; set; }
        public Guid? OriginPostId { get; set; }
    }
}
