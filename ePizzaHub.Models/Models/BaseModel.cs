using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ePizzaHub.Models
{
    public abstract class BaseModel
    {
        [Key]
        public Guid f_uid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int f_iid { get; set; }

        public DateTime? f_create_date { get; set; }
        public Guid? f_create_by { get; set; }
        public DateTime? f_update_date { get; set; }
        public Guid? f_update_by { get; set; }
        public DateTime? f_delete_date { get; set; }
        public Guid? f_delete_by { get; set; }

        [NotMapped]
        public int? RowsCount { get; set; }
    }
}