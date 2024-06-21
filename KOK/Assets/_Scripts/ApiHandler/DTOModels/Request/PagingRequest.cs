using System;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class PagingRequest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public SortOrder OrderType { get; set; } = SortOrder.Ascending;
    }
}