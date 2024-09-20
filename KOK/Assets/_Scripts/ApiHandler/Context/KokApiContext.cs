namespace KOK.ApiHandler.Context
{
    public static class KokApiContext
    {
        //https://localhost:7017
        public static readonly string KOK_Host_Url = "https://kok-api.azurewebsites.net/api/";
        //public static readonly string KOK_Host_Url = "https://localhost:7017/api/";
        public static readonly string Accounts_Resource = "accounts";
        public static readonly string Authentication_Resource = "authentication";
        public static readonly string Items_Resource = "items";
        public static readonly string Posts_Resource = "posts";
        public static readonly string PostComments_Resource = "post-comments";
        public static readonly string PostRatings_Resource = "post-ratings";
        public static readonly string Songs_Resource = "songs";
        public static readonly string FavouriteSongs_Resource = "favouriteSongs";
        public static readonly string Recordings_Resource = "recordings";
        public static readonly string FavoriteSongs_Resource = "favourite-songs";
        public static readonly string PurchasedSongs_Resource = "purchased-songs";
        public static readonly string Shop_Resource = "shops";
        public static readonly string KaraokeRooms_Resource = "karaoke-rooms";
        public static readonly string UpPackage_Resource = "packages";
        public static readonly string MoMo_Resource = "momo";
        public static readonly string InAppTransaction_Resource = "in-app-transactions";
        public static readonly string AccountItems_Resource = "inventory-items";
        public static readonly string Accounts_Report = "reports";
        public static readonly string Notification_Resource = "notifications";
    }
}
