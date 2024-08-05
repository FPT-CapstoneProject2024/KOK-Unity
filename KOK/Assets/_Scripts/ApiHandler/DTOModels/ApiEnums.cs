namespace KOK.ApiHandler.DTOModels
{
    #region Filter_and_Sorting

    public enum SortOrder
    {
        Descending,
        Ascending,
    }

    public enum AccountOrderFilter
    {
        UserName = 1,
        Email = 3,
        PhoneNumber = 13,
    }

    public enum SongOrderFilter
    {
        SongId = 0,
        SongName = 1,
        SongCode = 7,
    }

    public enum FavoriteSongOrderFilter
    {
        MemberId = 0,
        SongId = 1
    }

    public enum ItemOrderFilter
    {
        ItemCode = 1,
        ItemName = 2,
    }

    public enum PostCommentOrderFilter
    {
        Comment = 1,
    }

    public enum PostOrderFilter
    {
        Caption = 1,
        PostId = 0,
    }

    public enum RecordingOrderFilter
    {
        RecordingName = 1,
    }

    #endregion

    #region Account

    public enum AccountGender
    {
        MALE = 1,
        FEMALE = 2,
        OTHERS = 3,
    }

    public enum AccountRole
    {
        ADMIN = 1,
        STAFF = 2,
        MEMBER = 3,
    }

    public enum AccountStatus
    {
        NOT_VERIFY = 0,
        ACTIVE = 1,
        INACTIVE = 2,
    }

    #endregion

    #region Item

    public enum ItemStatus
    {
        DISABLE = 0,
        ENABLE = 1,
        PENDING = 2,
    }
    public enum ItemType
    {
        CHARACTER,
        ROOM,
        DEFAULT,
    }



    #endregion

    #region Song

    public enum SongStatus
    {
        DISABLE = 0,
        ENABLE = 1,
    }

    public enum SongType
    {
        INTERNAL = 1,
        EXTERNAL = 2,
    }
    public enum SongCategory
    {
        VPOP = 0,
        POP = 1,
        KPOP = 2,
        ROCK = 3,
    }

    #endregion

    #region PostComment
    public enum PostCommentType
    {
        PARENT,
        CHILD,
    }

    public enum PostCommentStatus
    {
        DEACTIVE,
        ACTIVE,
    }
    #endregion

    #region FavoriteSong



    #endregion

    #region Recording
    public enum RecordingType
    {
        SINGLE,
        MULTIPLE,
    }
    #endregion

    #region PurchasedSong

    public enum PurchasedSongOrderFilter
    {
        PurchasedSongId,
        PurchaseDate,
        MemberId,
        SongId,
    }

    #endregion

    #region Transaction

    public enum InAppTransactionStatus
    {
        PENDING,
        COMPLETE,
        CANCELED,
    }

    public enum InAppTransactionType
    {
        BUY_ITEM = 1,
        BUY_SONG = 2,
    }

    #endregion

    #region UpPackage

    public enum PackageStatus
    {
        INACTIVE = 0,
        ACTIVE = 1,
    }

    public enum PackageOrderFilter
    {
        PackageName = 1,
        MoneyAmount = 3,
        StarNumber = 4,
    }

    #endregion

    #region Post
    public enum PostType
    {
        POST,
        SHARE,
    }

    public enum PostStatus
    {
        DEACTIVE,
        ACTIVE,
    }
    #endregion
}
