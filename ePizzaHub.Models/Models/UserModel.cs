using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class UserModel : BaseModel
    {
        [Required]
        public required string f_fname { get; set; }
        [Required]
        public required string f_lname { get; set; }
        [Required]
        public required string f_phone { get; set; }
        [Required]
        public required string f_email { get; set; }
        [Required]
        public required string f_password_hash { get; set; }
        public bool f_is_active { get; set; } = true;
        public Guid f_role_uid { get; set; }
        public RoleModel? f_role { get; set; }
    }
}