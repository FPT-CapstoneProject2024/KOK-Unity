namespace KOK.ApiHandler.DTOModels
{
    public class NotificationStatusUpdateRequest
    {
        public NotificationStatus NewStatus { get; set; } = NotificationStatus.READ;
    }
}
