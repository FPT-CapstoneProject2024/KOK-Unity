using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post
{
    [Serializable]
    public class PostFilter
    {
        //public Guid MemberId { get; set; } = Guid.Empty;
        //public Guid PostId { get; set; } = Guid.Empty;
        public string Caption { get; set; } = string.Empty;
    }
}
