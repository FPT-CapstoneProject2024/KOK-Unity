using Fusion.Statistics;

namespace KOK.ApiHandler.Context
{
    public static class KokApiContext
    {
        //public static readonly string KOK_Host_Url = "https://kok-api.azurewebsites.net/api/";
        public static readonly string KOK_Host_Url = "https://localhost:7017/api/";
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
    }
}
