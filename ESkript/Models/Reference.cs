namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reference")]
    public partial class Reference
    {
        [Key]
        public int idReference { get; set; }

        public int? Script { get; set; }

        public int? FromContent { get; set; }

        public int? ToContent { get; set; }
    }
}
