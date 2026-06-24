using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models;

public class PizzaModel : BaseModel
{
    [Required]
    public required string f_name { get; set; }
    [Required]
    public required string f_description { get; set; }
    public decimal f_price { get; set; }
    [Required]
    public required string f_image_url { get; set; }
    public bool f_is_available { get; set; } = true;
    public Guid f_category_uid { get; set; }
    public PizzaCategoryModel? f_category { get; set; }
}