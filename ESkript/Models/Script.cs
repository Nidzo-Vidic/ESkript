namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Script")]
    public partial class Script
    {
        public Script()
        {
            ContentAttribute = new HashSet<ContentAttribute>();
            ScriptContent = new HashSet<ScriptContent>();
        }

        [Key]
        public int IdScript { get; set; }

        [Required]
        [Display(Name=("Fach"))]
        [StringLength(100)]
        public string Subject { get; set; }

        [Display(Name=("Autor"))]
        public int Author { get; set; }

        [Column(TypeName = "date")]
        [Display(Name=("Erstellungsdatum"))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime CreationDate { get; set; }

        [Display(Name=("Bearbeitungsstatus"))]
        public bool? InWork { get; set; }

        public virtual Account Account { get; set; }

        public virtual ICollection<ContentAttribute> ContentAttribute { get; set; }

        public virtual ICollection<ScriptContent> ScriptContent { get; set; }
    }
}
