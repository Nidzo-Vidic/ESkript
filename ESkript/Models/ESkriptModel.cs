namespace ESkript.Models {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ESkriptModel : DbContext {
        public ESkriptModel()
            : base("name=ESkriptModel") {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<CommentCard> CommentCard { get; set; }
        public virtual DbSet<ContentAttribute> ContentAttribute { get; set; }
        public virtual DbSet<ContentAttributeList> ContentAttributeList { get; set; }
        public virtual DbSet<FormulaCard> FormulaCard { get; set; }
        public virtual DbSet<ImageCard> ImageCard { get; set; }
        public virtual DbSet<LinkCard> LinkCard { get; set; }
        public virtual DbSet<LiteratureCard> LiteratureCard { get; set; }
        public virtual DbSet<NoteCard> NoteCard { get; set; }
        public virtual DbSet<Reference> Reference { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Script> Script { get; set; }
        public virtual DbSet<ScriptContent> ScriptContent { get; set; }
        public virtual DbSet<SourceCodeCard> SourceCodeCard { get; set; }
        public virtual DbSet<TextCard> TextCard { get; set; }
        public virtual DbSet<VideoCard> VideoCard { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Account>()
                .HasMany(e => e.CommentCard)
                .WithOptional(e => e.Account)
                .HasForeignKey(e => e.Author);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.NoteCard)
                .WithOptional(e => e.Account)
                .HasForeignKey(e => e.Author);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Script)
                .WithRequired(e => e.Account)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ContentAttribute>()
                .HasMany(e => e.ContentAttributeList)
                .WithOptional(e => e.ContentAttribute)
                .HasForeignKey(e => e.Attribute);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Account)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.AccountRole)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Script>()
                .HasMany(e => e.ContentAttribute)
                .WithOptional(e => e.Script1)
                .HasForeignKey(e => e.Script);

            modelBuilder.Entity<Script>()
                .HasMany(e => e.ScriptContent)
                .WithOptional(e => e.Script1)
                .HasForeignKey(e => e.Script);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.CommentCard)
                .WithOptional(e => e.ScriptContent1)
                .HasForeignKey(e => e.ScriptContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.ContentAttributeList)
                .WithOptional(e => e.ScriptContent1)
                .HasForeignKey(e => e.ScriptContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.FormulaCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.FormulaContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.ImageCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.ImageContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.LinkCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.LinkContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.LiteratureCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.LiteratureContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.NoteCard)
                .WithOptional(e => e.ScriptContent1)
                .HasForeignKey(e => e.ScriptContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.ScriptContent1)
                .WithOptional(e => e.ScriptContent2)
                .HasForeignKey(e => e.FatherContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.SourceCodeCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.SourceCodeContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.TextCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.TextContent);

            modelBuilder.Entity<ScriptContent>()
                .HasMany(e => e.VideoCard)
                .WithOptional(e => e.ScriptContent)
                .HasForeignKey(e => e.VideoContent);
        }
    }
}
