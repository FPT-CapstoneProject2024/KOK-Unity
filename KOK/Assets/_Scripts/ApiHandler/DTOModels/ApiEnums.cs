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

    #region Song

    public enum SongStatus
    {
        DISABLE = 0,
        ENABLE = 1,
    }

    #endregion

    #region FavoriteSong



    #endregion
}
