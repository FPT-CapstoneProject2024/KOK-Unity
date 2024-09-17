using Newtonsoft.Json;
using System;

namespace KOK.ApiHandler.DTOModels
{
    public class UpdateAccountRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  string UserName { get; set; } = null;
        //public string Email { get; set; } = string.Empty;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; } = null;
        //public decimal UpBalance { get; set; } = -1;
        //public int? Yob { get; set; } = 2000;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  string PhoneNumber { get; set; } = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  Guid? CharacterItemId { get; set; } = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  Guid? RoomItemId { get; set; } = null;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
