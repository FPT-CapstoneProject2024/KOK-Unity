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
}
