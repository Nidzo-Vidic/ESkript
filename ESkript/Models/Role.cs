namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Role")]
    public partial class Role
    {
        public Role()
        {
            Account = new HashSet<Account>();
        }

        [Key]
        public int IdRole { get; set; }

        [Required]
        [Display(Name=("Rollenname"))]
        [StringLength(50)]
        public string RoleName { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
