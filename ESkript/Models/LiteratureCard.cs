namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LiteratureCard")]
    public partial class LiteratureCard
    {
        [Key]
        public int IdLiterature { get; set; }

        public int? LiteratureContent { get; set; }

        [Required]
        [Display(Name=("Autor/en"))]
        [StringLength(250)]
        public string Authors { get; set; }

        [Required]
        [StringLength(150)]
        public string Titel { get; set; }

        [Required]
        [Display(Name=("Verlag"))]
        [StringLength(150)]
        public string Publisher { get; set; }

        [Column(TypeName = "date")]
        [Display(Name="Datum")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? Year { get; set; }

        [Display(Name=("Ort"))]
        [StringLength(50)]
        public string Location { get; set; }

        [StringLength(20)]
        public string Edition { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
