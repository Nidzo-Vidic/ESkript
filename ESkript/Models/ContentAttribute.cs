namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContentAttribute")]
    public partial class ContentAttribute
    {
        public ContentAttribute()
        {
            ContentAttributeList = new HashSet<ContentAttributeList>();
        }

        [Key]
        public int IdAttribute { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name ="Attribut")]
        public string AttributeName { get; set; }

        public int? Script { get; set; }

        public virtual Script Script1 { get; set; }

        public virtual ICollection<ContentAttributeList> ContentAttributeList { get; set; }
    }
}
