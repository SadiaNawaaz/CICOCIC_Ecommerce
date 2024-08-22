namespace Ecommerce.Mailer.DTOs
{
    public class AttachmentResult
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
