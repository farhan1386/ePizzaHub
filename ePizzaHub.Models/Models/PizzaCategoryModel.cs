using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class PizzaCategoryModel : BaseModel
    {
        [Required]
        public required string f_name { get; set; }
        public bool f_is_active { get; set; } = true;
    }
}
