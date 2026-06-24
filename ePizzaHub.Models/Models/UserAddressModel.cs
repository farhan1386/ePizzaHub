using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class UserAddressModel : BaseModel
    {
        public Guid f_user_iid { get; set; }
        [Required]
        public required string f_address_line { get; set; }
        [Required]
        public required string f_city { get; set; }
        [Required]
        public required string f_postal_code { get; set; }
        public bool f_is_default { get; set; } = false;
        public UserModel? f_user { get; set; }
    }
}