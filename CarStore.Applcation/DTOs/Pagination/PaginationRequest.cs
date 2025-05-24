using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.DTOs.Pagination
{
    public class PaginationRequest
    {
        private const int MaxPageSize = 40;
        private int _pageSize = 10;
        private int _pageNumber = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value;
        }
    }
}
