namespace KOK.ApiHandler.DTOModels
{
    public class MemberRegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public AccountGender Gender { get; set; } = AccountGender.OTHERS;
    }
}
