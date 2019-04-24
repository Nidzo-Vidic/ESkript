namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContentAttributeList")]
    public partial class ContentAttributeList
    {
        [Key]
        public int IdContentAttributeList { get; set; }

        [Display(Name ="Attribut")]
        public int? Attribute { get; set; }

        public int? ScriptContent { get; set; }

        public virtual ContentAttribute ContentAttribute { get; set; }

        public virtual ScriptContent ScriptContent1 { get; set; }
    }
}
