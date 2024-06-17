using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK.ApiHandler.Model
{
    [Serializable]
    public class Account 
    {
        public Account()
        {
        }


        public Guid accountId { get; set; } 
        public string userName { get; set; } = null!;
        public string password { get; set; } = null!;
        public string email { get; set; } = null!;
        public int gender { get; set; }
        public string accountName { get; set; } = null!;
        public int role { get; set; }
        public int star { get; set; }
        public bool isOnline { get; set; }
        public string? fullname { get; set; }
        public int? yob { get; set; }
        public string? pdentityCardNumber { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? createdTime { get; set; }
        public Guid? characterItemId { get; set; }
        public Guid? roomItemId { get; set; }
        public int? accountStatus { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
