using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request
{
    public class CreatePostRatingRequest
    {
        public Guid MemberId { get; set; }
        public Guid PostId { get; set; }
        public int Score { get; set; }
    }
}
