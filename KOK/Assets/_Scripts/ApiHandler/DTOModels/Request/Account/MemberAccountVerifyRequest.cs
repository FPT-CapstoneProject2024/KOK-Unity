namespace KOK.ApiHandler.DTOModels
{
    public class MemberAccountVerifyRequest
    {
        public string AccountEmail { get; set; } = string.Empty;
        public string VerifyCode { get; set; } = string.Empty;
    }
}
