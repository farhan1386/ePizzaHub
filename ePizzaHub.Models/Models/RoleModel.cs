using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ePizzaHub.Models
{
    public class RoleModel : BaseModel
    {
        [Required]
        public required string f_name { get; set; }

        public bool f_is_active { get; set; } = true;
    }
}
