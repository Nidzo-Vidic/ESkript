namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VideoCard")]
    public partial class VideoCard
    {
        [Key]
        public int IdVideoCard { get; set; }

        public int? VideoContent { get; set; }

        public byte[] Video { get; set; }

        public virtual ScriptContent ScriptContent { get; set; }
    }
}
