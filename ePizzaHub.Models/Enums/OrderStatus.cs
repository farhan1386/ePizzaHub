using System;
using System.Collections.Generic;
using System.Text;

namespace ePizzaHub.Models.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Baking = 2,
        Dispatched = 3,
        Delivered = 4,
        Cancelled = 5
    }
}

