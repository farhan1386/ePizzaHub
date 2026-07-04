using System;
using System.Collections.Generic;
using System.Text;

namespace ePizzaHub.Models
{
    public class ForgotPasswordRequest
    {
        public required string Email { get; set; }
    }
}
