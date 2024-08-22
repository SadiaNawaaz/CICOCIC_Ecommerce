using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Mailer.DTOs
{
    public class MailCredential
    {
        [Display(Name="Display Name")]
        public string? Name { get; set; }
        [DataType(DataType.EmailAddress), Required]
        public string? Email { get; set; }
        [Display(Name = "SMTP Port")]
        public int SMTPPORT { get; set; }
        [Display(Name = "SMTP Host")]
        public string? SMTPHOST { get; set; }      
        
        [Display(Name = "IMAP Port")]
        public int IMAPPORT { get; set; }
        [Display(Name = "IMAP Host")]
        public string? IMAPHOST { get; set; }
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
