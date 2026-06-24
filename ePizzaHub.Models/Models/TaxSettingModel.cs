using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class TaxSettingModel : BaseModel
    {
        [Required]
        public required string f_tax_name { get; set; }
        public decimal f_percentage { get; set; }
        public bool f_is_active { get; set; } = true;
    }
}
