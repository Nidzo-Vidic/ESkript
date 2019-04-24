namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImageCard")]
    public partial class ImageCard
    {
        [Key]
        public int IdImageCard { get; set; }

        public int? ImageContent { get; set; }

        [Required]
        public byte[] Image { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
