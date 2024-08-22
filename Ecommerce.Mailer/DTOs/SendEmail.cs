using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Mailer.DTOs
{
    public class SendEmail : IValidatableObject
    {
        public IList<EmailAddress> To { get; set; }
        public IList<EmailAddress>? Cc { get; set; }
        public IList<EmailAddress>? Bcc { get; set; }
        public string? From { get; set; }
        public string Subject { get; set; }
        [ValidateHtmlContent]
        public string Body { get; set; }
        public IList<IFormFile>? Attachments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            if(Body == null || Body.Length <= 0)
            {
                result.Add(new ValidationResult("Body is Empty", new[] { nameof(Body) }));
            }
            else if(string.IsNullOrEmpty(Subject))
            {
                result.Add(new ValidationResult("Subject can not empty", new[] { nameof(Subject) }));

            }
            return result;
        }
    }


    public class ReplyEmail : IValidatableObject
    {
        public IList<EmailAddress> To { get; set; }
        public IList<EmailAddress>? Cc { get; set; }
        public string? InReplyTo { get; set; }
        public string? From { get; set; }
        public IList<EmailAddress>? Bcc { get; set; }
        public string Subject { get; set; }
        [ValidateHtmlContent]
        public string Body { get; set; }
        public IList<IFormFile>? Attachments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            if (Body == null || Body.Length <= 0)
            {
                result.Add(new ValidationResult("Body is Empty", new[] { nameof(Body) }));
            }
            else if (string.IsNullOrEmpty(Subject))
            {
                result.Add(new ValidationResult("Subject can not empty", new[] { nameof(Subject) }));

            }
            else if (string.IsNullOrEmpty(InReplyTo))
            {
                result.Add(new ValidationResult("Something went wrong, please try again"));
            }
            return result;
        }
    }
    public class EmailAddress
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set;}
    }
    public class ValidateHtmlContentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var htmlContent = value as string;

            // Validate htmlContent here against your security requirements
            // For example, you can use a library like HtmlSanitizer to sanitize HTML input

            // Return ValidationResult.Success if valid, otherwise return an appropriate ValidationResult
            // Example validation:
            if(htmlContent == null)
            {
                return new ValidationResult("Body content cannot null or empty");
            }
            if (htmlContent.Contains("<script>"))
            {
                return new ValidationResult("HTML content cannot contain <script> tags.");
            }

            return ValidationResult.Success;
        }
    }
}
