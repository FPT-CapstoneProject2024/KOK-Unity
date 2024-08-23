using Newtonsoft.Json;
using System;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class Account
    {
        public Guid? AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public AccountGender? Gender { get; set; }
        public AccountRole? Role { get; set; }
        public decimal? UpBalance { get; set; }
        public bool? IsOnline { get; set; }
        public string Fullname { get; set; }
        public int? Yob { get; set; }
        public string IdentityCardNumber { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedTime { get; set; }
        public Guid? CharacterItemId { get; set; }
        public Guid? RoomItemId { get; set; }
        public string? CharaterItemCode { get; set; }
        public string? RoomItemCode { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
