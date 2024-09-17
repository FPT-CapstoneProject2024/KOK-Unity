using Newtonsoft.Json;
using System;

namespace KOK.ApiHandler.DTOModels
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Description { get; set; } = string.Empty;
        public NotificationType NotificationType { get; set; } = NotificationType.MESSAGE_COMMING;
        public NotificationStatus Status { get; set; } = NotificationStatus.UNREAD;
        public DateTime CreateDate { get; set; }
        public Guid AccountId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
