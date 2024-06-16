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

        public Account(Guid accountId, string userName, string password, string email, int gender, string accountName, int role, int star, bool isOnline, string fullname, int? yob, string identityCardNumber, string phoneNumber, DateTime? createdTime, Guid? characterItemId, Guid? roomItemId, int? accountStatus)
        {
            AccountId = accountId;
            UserName = userName;
            Password = password;
            Email = email;
            Gender = gender;
            AccountName = accountName;
            Role = role;
            Star = star;
            IsOnline = isOnline;
            Fullname = fullname;
            Yob = yob;
            IdentityCardNumber = identityCardNumber;
            PhoneNumber = phoneNumber;
            CreatedTime = createdTime;
            CharacterItemId = characterItemId;
            RoomItemId = roomItemId;
            AccountStatus = accountStatus;
        }

        public Guid AccountId { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Gender { get; set; }
        public string AccountName { get; set; } = null!;
        public int Role { get; set; }
        public int Star { get; set; }
        public bool IsOnline { get; set; }
        public string? Fullname { get; set; }
        public int? Yob { get; set; }
        public string? IdentityCardNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedTime { get; set; }
        public Guid? CharacterItemId { get; set; }
        public Guid? RoomItemId { get; set; }
        public int? AccountStatus { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
