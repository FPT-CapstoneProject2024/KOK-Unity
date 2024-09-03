using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.Assets._Scripts.ApiHandler.DTOModels.Request
{
    public class CreateReportRequest
    {
        public Guid ReporterId { get; set; }
        public Guid ReportedAccountId { get; set; }
        public int ReportCategory { get; set; }
        public string? Reason { get; set; }
        public int ReportType { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? RoomId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
