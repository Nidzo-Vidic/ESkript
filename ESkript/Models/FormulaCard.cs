namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FormulaCard")]
    public partial class FormulaCard
    {
        [Key]
        public int IdFormulaCard { get; set; }

        public int? FormulaContent { get; set; }

        [Required]
        [Display(Name ="Formel")]
        public string Formula { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
