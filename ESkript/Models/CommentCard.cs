namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommentCard")]
    public partial class CommentCard
    {
        [Key]
        public int IdComment { get; set; }

        public int? ScriptContent { get; set; }

        [Display(Name ="Autor")]
        public int? Author { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreationDate { get; set; }

        [Required]
        [Display(Name= "Kommentar")]
        public string Text { get; set; }

        public virtual Account Account { get; set; }

        public virtual ScriptContent ScriptContent1 { get; set; }
    }
}
