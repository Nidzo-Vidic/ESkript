namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LinkCard")]
    public partial class LinkCard
    {
        [Key]
        public int IdLink { get; set; }

        public int? LinkContent { get; set; }

        [Required]
        [StringLength(150)]
        public string Link { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
