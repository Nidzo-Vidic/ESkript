namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("TextCard")]
    public partial class TextCard
    {
        [Key]
        public int IdTextCard { get; set; }

        public int? TextContent { get; set; }

        [Required]
        [AllowHtml]
        public string Text { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
