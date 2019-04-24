namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NoteCard")]
    public partial class NoteCard
    {
        [Key]
        public int IdNote { get; set; }

        public int? ScriptContent { get; set; }

        public int? Author { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreationDate { get; set; }

        public string Text { get; set; }

        public virtual Account Account { get; set; }

        public virtual ScriptContent ScriptContent1 { get; set; }
    }
}
