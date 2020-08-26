using System;
namespace Helper.Pagination
{
    public static class Paging
    {
        public const int PAGESIZE = 20;
    }
    public class PagingModel
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int TotalPage
        {
            get
            {
                if (Total % PageSize != 0)
                    return Convert.ToInt32(Total / PageSize) + 1;
                else
                    return Total / PageSize;
            }
        }
    }
}