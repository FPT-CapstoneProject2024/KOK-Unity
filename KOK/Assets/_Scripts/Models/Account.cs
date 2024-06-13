using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    [Serializable]
    public partial class Account
    {
        public Account()
        {
            AccountInventoryItems = new HashSet<AccountInventoryItem>();
            ConversationMemberId1Navigations = new HashSet<Conversation>();
            ConversationMemberId2Navigations = new HashSet<Conversation>();
            FavouriteSongs = new HashSet<FavouriteSong>();
            FriendReceivers = new HashSet<Friend>();
            FriendSenders = new HashSet<Friend>();
            InAppTransactions = new HashSet<InAppTransaction>();
            Items = new HashSet<Item>();
            KaraokeRooms = new HashSet<KaraokeRoom>();
            LoginActivities = new HashSet<LoginActivity>();
            Messages = new HashSet<Message>();
            MoneyTransactions = new HashSet<MoneyTransaction>();
            Packages = new HashSet<Package>();
            PostComments = new HashSet<PostComment>();
            PostShares = new HashSet<PostShare>();
            PostVotes = new HashSet<PostVote>();
            Posts = new HashSet<Post>();
            PurchasedSongs = new HashSet<PurchasedSong>();
            RecordingHosts = new HashSet<Recording>();
            RecordingOwners = new HashSet<Recording>();
            ReportReportedAccounts = new HashSet<Report>();
            ReportReporters = new HashSet<Report>();
            Songs = new HashSet<Song>();
            SupportRequests = new HashSet<SupportRequest>();
            VoiceAudios = new HashSet<VoiceAudio>();
        }

        public Guid AccountId { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Gender { get; set; }
        public string AccountName { get; set; } = null!;
        public bool IsVerified { get; set; }
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

        public virtual AccountInventoryItem? CharacterItem { get; set; }
        public virtual AccountInventoryItem? RoomItem { get; set; }
        public virtual ICollection<AccountInventoryItem> AccountInventoryItems { get; set; }
        public virtual ICollection<Conversation> ConversationMemberId1Navigations { get; set; }
        public virtual ICollection<Conversation> ConversationMemberId2Navigations { get; set; }
        public virtual ICollection<FavouriteSong> FavouriteSongs { get; set; }
        public virtual ICollection<Friend> FriendReceivers { get; set; }
        public virtual ICollection<Friend> FriendSenders { get; set; }
        public virtual ICollection<InAppTransaction> InAppTransactions { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<KaraokeRoom> KaraokeRooms { get; set; }
        public virtual ICollection<LoginActivity> LoginActivities { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MoneyTransaction> MoneyTransactions { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<PostComment> PostComments { get; set; }
        public virtual ICollection<PostShare> PostShares { get; set; }
        public virtual ICollection<PostVote> PostVotes { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<PurchasedSong> PurchasedSongs { get; set; }
        public virtual ICollection<Recording> RecordingHosts { get; set; }
        public virtual ICollection<Recording> RecordingOwners { get; set; }
        public virtual ICollection<Report> ReportReportedAccounts { get; set; }
        public virtual ICollection<Report> ReportReporters { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
        public virtual ICollection<SupportRequest> SupportRequests { get; set; }
        public virtual ICollection<VoiceAudio> VoiceAudios { get; set; }
    }
}
