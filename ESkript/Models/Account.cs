namespace ESkript.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        public Account()
        {
            CommentCard = new HashSet<CommentCard>();
            NoteCard = new HashSet<NoteCard>();
            Script = new HashSet<Script>();
        }

        [Key]
        public int IdAccount { get; set; }

        [StringLength(50)]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Nachname")]
        public string LastName { get; set; }

        [StringLength(100)]
        [Display(Name = "Email Adresse")]
        public string EMail { get; set; }

        [StringLength(120)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        public int AccountRole { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<CommentCard> CommentCard { get; set; }

        public virtual ICollection<NoteCard> NoteCard { get; set; }

        public virtual ICollection<Script> Script { get; set; }
    }

    public class LoginModel {
        [Key]
        [Display(Name = "Email Adresse")]
        [Required]
        public string EMail { get; set; }
        
        [Display(Name = "Passwort")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }

    public class RegisterModel {
        [Key]
        public int IdAccount { get; set; }
       
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }
        
        [Display(Name = "Nachname")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email Adresse")]
        public string EMail { get; set; }

        [Required]
        [Display(Name = "Passwort")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Die Passwörter stimmen nicht überein")]
        public string ConfirmPassword { get; set; }
        public int AccountRole { get; set; }

    }
}
