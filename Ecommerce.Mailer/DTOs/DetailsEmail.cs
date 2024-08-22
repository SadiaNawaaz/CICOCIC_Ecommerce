using MimeKit;

namespace Ecommerce.Mailer.DTOs
{
    public class DetailsEmail
    {
        public uint UniqueId { get; set; }
        public List<EmailAddressDTO> From { get; set; } = new List<EmailAddressDTO>();
        public List<EmailAddressDTO> To { get; set; } = new List<EmailAddressDTO>();
        public List<EmailAddressDTO> Cc { get; set; } = new List<EmailAddressDTO>();
        public List<EmailAddressDTO> Bcc { get; set; } = new List<EmailAddressDTO>();
        public List<EmailAddressDTO> ReplyTo { get; set; } = new List<EmailAddressDTO>();
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
        public bool HasAttachments { get; set; }
        public IList<DetailsEmailAttachement> Attachments { get; set; }
        public List<ListedItem> References { get; set; } = new List<ListedItem>();
    }
    public class EmailAddressDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class DetailsEmailAttachement
    {
        public string? Url { get; set; }
        public string? ContentType { get; set; }
        public string? Name { get; set; }
    }
    public class DetailsEmailMessage{
        public string?  From { get; set; }
        public string?  To { get; set; }
        public string?  ReplyTo { get; set; }
        public string?  Cc { get; set; }
        public string?  Bcc { get; set; }
        public string?  Subject { get; set; }
        public string? MailedBy { get; set; }
        public string? SignedBy { get; set; }
    }
}
