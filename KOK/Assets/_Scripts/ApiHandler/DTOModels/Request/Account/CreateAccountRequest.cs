using System;

namespace KOK.ApiHandler.DTOModels
{
    public class CreateAccountRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Gender { get; set; } = (int)AccountGender.OTHERS;
        public int Role { get; set; } = (int)AccountRole.MEMBER;
        public decimal UpBalance { get; set; } = 0;
        public string Fullname { get; set; } = null;
        public int? Yob { get; set; } = 2000;
        public string IdentityCardNumber { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public Guid? CharacterItemId { get; set; } = null;
        public Guid? RoomItemId { get; set; } = null;
        public int AccountStatus { get; set; } = (int)DTOModels.AccountStatus.ACTIVE;
    }
}
