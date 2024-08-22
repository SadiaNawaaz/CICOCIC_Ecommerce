using Ecommerce.Mailer.DTOs;
using MailKit;
namespace Ecommerce.Mailer
{
    public interface IMailerServices
    {
        //Task<IList<ListedItem>> FetchEmailsAsync();
        string CredentialPath { get; set; }
        Task Init();
        Task<PaginatedResult<ListedItem>> FetchEmailsFromFolderAsync(SpecialFolder folderType, int pageNumber, int pageSize);
        Task<IList<FolderSummary>> FetchFolderSummariesAsync();
        Task<AttachmentResult> GetAttachmentAsync(string folder, string messageId, int attachmentIndex);
        Task<AttachmentResult> GetAttachmentAsync(string folder, uint uniqueId, int attachmentIndex);
        Task<DetailsEmail> FetchFullEmailFromInboxAsync(string messageid, SpecialFolder folder);
        Task<DetailsEmail> FetchFullEmailFromInboxAsync(uint uniqueid, SpecialFolder folder);
        Task<bool> SaveEmailAsync(SendEmail data);
        Task<bool> SendEmailAsync(SendEmail data, string? FromEmail);  
        Task<bool> ReplyEmailAsync(ReplyEmail data, string? FromEmail);
    }
}
