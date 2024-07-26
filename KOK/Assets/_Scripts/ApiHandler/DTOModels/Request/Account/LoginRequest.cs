using System;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
