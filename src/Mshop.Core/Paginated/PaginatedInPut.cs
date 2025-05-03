
using Mshop.Core.Enum.Paginated;

namespace Mshop.Core.Paginated
{
    public class PaginatedInPut
    {
        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        public string Search { get; set; }

        public string OrderBy { get; set; }

        public SearchOrder Order { get; set; }

        //(1-1)*10=0
        //(2-1)*10=10
        //(3-1)*10=20
        //public int From => (CurrentPage - 1) * PerPage;

        public PaginatedInPut(int currentPage = 1, int perPage = 20, string search = "", string orderBy = "", SearchOrder order = SearchOrder.Asc)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Search = search;
            OrderBy = orderBy;
            Order = order;
        }



    }
}
