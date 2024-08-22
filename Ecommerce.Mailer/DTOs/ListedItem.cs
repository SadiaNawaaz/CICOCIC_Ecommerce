using MailKit;

namespace Ecommerce.Mailer.DTOs
{
    public class ListedItem
    {
        public string? From { get; set; }
        public string? Subject { get; set; }
        public string? Date { get; set; }
        public string? Body { get; set; }
        public bool HasAttachments { get; set; }
        public bool IsUnread { get; set; }
        public string? ResentMessageId { get; set; }
        public string? MessageId { get; set; }
        public uint? UniqueId { get; set; }
    }
}
