using System;
using System.Collections.Generic;
using System.Text;

namespace ePizzaHub.Models
{
    public class GridDataModel<T> where T : class
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int TotalRecords { get; set; }
        public int FilteredRecords { get; set; }
    }
}
