using System;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class AccountFilter
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
