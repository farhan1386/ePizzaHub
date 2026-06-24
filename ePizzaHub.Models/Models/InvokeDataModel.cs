using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ePizzaHub.Models
{
    public class InvokeDataModel
    {
        public Guid userUid { get; set; }
        public string schema { get; set; }
        public int? commandTimeout { get; set; }
    }
}
