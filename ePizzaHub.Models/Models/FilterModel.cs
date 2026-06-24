using System;
using System.Collections.Generic;
using System.Text;

namespace ePizzaHub.Models
{
    public class FilterModel
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; } // "ASC" or "DESC"
    }
}
