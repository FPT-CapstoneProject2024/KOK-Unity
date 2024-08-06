using Newtonsoft.Json;
using System;

namespace KOK.ApiHandler.DTOModels
{
    public class UpdateAccountRequest
    {
        public string UserName { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = AccountGender.OTHERS.ToString();
        public decimal UpBalance { get; set; } = 0;
        //public int? Yob { get; set; } = 2000;
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid? CharacterItemId { get; set; } = null;
        public Guid? RoomItemId { get; set; } = null;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
