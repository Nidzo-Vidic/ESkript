namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScriptContent")]
    public partial class ScriptContent
    {
        public ScriptContent()
        {
            CommentCard = new HashSet<CommentCard>();
            ContentAttributeList = new HashSet<ContentAttributeList>();
            FormulaCard = new HashSet<FormulaCard>();
            ImageCard = new HashSet<ImageCard>();
            LinkCard = new HashSet<LinkCard>();
            LiteratureCard = new HashSet<LiteratureCard>();
            NoteCard = new HashSet<NoteCard>();
            ScriptContent1 = new HashSet<ScriptContent>();
            SourceCodeCard = new HashSet<SourceCodeCard>();
            TextCard = new HashSet<TextCard>();
            VideoCard = new HashSet<VideoCard>();
        }

        [Key]
        public int IdScriptContent { get; set; }

        [Display(Name=("Skript"))]
        public int? Script { get; set; }

        public int? FatherContent { get; set; }

        [StringLength(100)]
        [Required]
        public string Titel { get; set; }

        public int? Predecessor { get; set; }

        public virtual ICollection<CommentCard> CommentCard { get; set; }

        public virtual ICollection<ContentAttributeList> ContentAttributeList { get; set; }

        public virtual ICollection<FormulaCard> FormulaCard { get; set; }

        public virtual ICollection<ImageCard> ImageCard { get; set; }

        public virtual ICollection<LinkCard> LinkCard { get; set; }

        public virtual ICollection<LiteratureCard> LiteratureCard { get; set; }

        public virtual ICollection<NoteCard> NoteCard { get; set; }

        public virtual Script Script1 { get; set; }

        public virtual ICollection<ScriptContent> ScriptContent1 { get; set; }

        public virtual ScriptContent ScriptContent2 { get; set; }

        public virtual ICollection<SourceCodeCard> SourceCodeCard { get; set; }

        public virtual ICollection<TextCard> TextCard { get; set; }

        public virtual ICollection<VideoCard> VideoCard { get; set; }
    }
}
