namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SourceCodeCard")]
    public partial class SourceCodeCard
    {
        [Key]
        public int IdSourceCodeCard { get; set; }

        public int? SourceCodeContent { get; set; }

        public string SourcceCode { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
