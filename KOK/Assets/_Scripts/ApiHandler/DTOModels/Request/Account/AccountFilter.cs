using System;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class AccountFilter
    {
        public virtual string UserName { get; set; } = string.Empty;
        public virtual string Email { get; set; } = string.Empty;
        public virtual string PhoneNumber { get; set; } = string.Empty;
    }
}
