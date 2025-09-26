using Ecommerce.Mailer.DTOs;
using HtmlAgilityPack;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Text.Json;
using System;
using System.Xml;
using UniqueId = MailKit.UniqueId;

namespace Ecommerce.Mailer
{
    public class MailerServices : IMailerServices
    {
        //private string ImapServer = "imap.mailprotect.be";
        //private string ImapUser = "administrator@registerbyasset.com";
        //private int ImapPort = 993;
        //private string ImapPass = "Gazelle11";


        //private string SMTP_Server = "smtp-auth.mailprotect.be";
        //private string SMTP_User = "administrator@registerbyasset.com";
        //private int SMTP_Port = 465;
        //private string SMTP_Pass = "Gazelle11";
        private string _credentialPath;
        public string CredentialPath
        {
            get => _credentialPath;
            set => _credentialPath = value;
        }
        private static MailCredential credential;
        private static readonly object _lock = new object();

        public async Task Init()
        {
            var credentials = await ReadCredentials();

            if (credentials == null || credentials.Count == 0)
            {
                throw new InvalidOperationException("Please add email first from settings");
            }

            lock (_lock)
            {
                if (credential == null)
                {
                    credential = credentials.First();
                }
            }
        }
        private async Task<List<MailCredential>> ReadCredentials()
        {
            //if(CredentialPath==null)
            //{
            //    CredentialPath = "F:\\CICOCIC_Ecommerce\\Ecommerce.Admin\\Data\\mailCredentials.json";
            //}
            if (CredentialPath == null)
                {
                // Example: put it under your wwwroot\Data folder
                CredentialPath = Path.Combine(AppContext.BaseDirectory, "Data", "mailCredentials.json");
                }

            if (!System.IO.File.Exists(CredentialPath))
            {
                return new List<MailCredential>();
            }

            var jsonString = await System.IO.File.ReadAllTextAsync(CredentialPath);
            var mailCredentials = JsonSerializer.Deserialize<List<MailCredential>>(jsonString);
            if (mailCredentials == null)
            {
                mailCredentials = new List<MailCredential>();
            }
            return mailCredentials;
        }

        public async Task<PaginatedResult<ListedItem>> FetchEmailsFromFolderAsync(SpecialFolder folderType, int pageNumber, int pageSize)
        {
            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);
                }
                catch (Exception)
                {
                    throw;
                }

                var folder = folderType == SpecialFolder.All ? client.Inbox : client.GetFolder(folderType);
                await folder.OpenAsync(FolderAccess.ReadOnly);

                var totalMessages = folder.Count;
                var start = (pageNumber - 1) * pageSize;
                var end = Math.Min(start + pageSize, totalMessages);

                var messages = new List<MimeMessage>();
                var alluids = await folder.FetchAsync(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure | MessageSummaryItems.Flags | MessageSummaryItems.Headers | MessageSummaryItems.UniqueId);

                // Sort the message summaries by date in descending order
                var uids = alluids
                    .Where(item => item.Envelope != null) // Ensure envelope is not null
                    .OrderByDescending(item => item.Envelope.Date)
                    .Skip(start)
                    .Take(pageSize)
                    .ToList();

                var emailDtos = uids.Select(item => new ListedItem
                {
                    From = item.Envelope.From.Mailboxes.FirstOrDefault()?.Address,
                    Subject = item.Envelope.Subject,
                    Date = item.Envelope.Date?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Body = null,
                    HasAttachments = item.Attachments.Any(),
                    MessageId = item.Envelope.MessageId,
                    UniqueId = item.UniqueId.IsValid ? item.UniqueId.Id : null,
                    ResentMessageId = item.Headers[HeaderId.ResentMessageId],
                    IsUnread = !item?.Flags?.HasFlag(MessageFlags.Seen) ?? false
                }).ToList();


                await client.DisconnectAsync(true);
                return new PaginatedResult<ListedItem>
                {
                    Items = emailDtos,
                    TotalCount = totalMessages,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        /// <summary>
        /// Details Message loading
        /// </summary>
        /// <param name="messageid"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task<DetailsEmail> FetchFullEmailFromInboxAsync(string messageid, SpecialFolder folder)
        {
            try
            {
                var res = new DetailsEmail();
                using (var client = new ImapClient())
                {
                    await client.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);

                    var inbox = folder == SpecialFolder.All ? client.Inbox : client.GetFolder(folder);
                    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                    //  var message = inbox.FirstOrDefault(m => m.MessageId == messageid);
                    // Fetch the main message using the message ID
                    //  var query = SearchQuery.HeaderContains("Message-ID", messageid);
                    var rawMessageId = messageid;
                    var bracketedMessageId = $"<{messageid}>";

                    // Expanded search query to handle variations
                    var query = SearchQuery.Or(
                                SearchQuery.HeaderContains("Message-ID", rawMessageId),
                                SearchQuery.Or(
                                    SearchQuery.HeaderContains("Message-ID", bracketedMessageId),
                                    SearchQuery.Or(
                                        SearchQuery.HeaderContains("In-Reply-To", rawMessageId),
                                        SearchQuery.Or(
                                            SearchQuery.HeaderContains("In-Reply-To", bracketedMessageId),
                                            SearchQuery.Or(
                                                SearchQuery.HeaderContains("References", rawMessageId),
                                                SearchQuery.HeaderContains("References", bracketedMessageId)
                                            )
                                        )
                                    )
                                )
                            );


                    var uids = await inbox.SearchAsync(query);
                    if (uids.Count == 0)
                        throw new Exception("Message not found");

                    var message = await inbox.GetMessageAsync(uids[0]);
                    res.From = message.From.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.To = message.To.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.Cc = message.Cc.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.Bcc = message.Bcc.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.ReplyTo = message.ReplyTo.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.Subject = message.Subject;
                    res.Date = message.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    res.Body = message.HtmlBody ?? message.TextBody;
                    res.HasAttachments = message.Attachments.Any();
                    // Map attachments
                    res.Attachments = new List<DetailsEmailAttachement>();
                    foreach (var attachment in message.Attachments)
                    {
                        int attachmentIndex = 0;
                        if (attachment is MimePart mimePart)
                        {
                            var fileName = mimePart.FileName;
                            if (string.IsNullOrEmpty(fileName) && mimePart.ContentDisposition != null)
                            {
                                fileName = mimePart.ContentDisposition?.FileName;
                            }

                            var attachmentData = new DetailsEmailAttachement
                            {
                                Name = fileName ?? "unnamed",
                                ContentType = mimePart.ContentType.MimeType,
                                Url = $"/crm/attachment?folder={folder}&messageId={messageid}&attix={attachmentIndex}"
                            };
                            res.Attachments.Add(attachmentData);
                        }
                        else if (attachment is MessagePart rfc822Part)
                        {
                            var fileName = rfc822Part.ContentDisposition?.FileName ?? "attached-email";

                            var attachmentData = new DetailsEmailAttachement
                            {
                                Name = fileName,
                                ContentType = rfc822Part.ContentType.MimeType,
                                Url = $"/crm/attachment?folder={folder}&messageId={messageid}&attix={attachmentIndex}"
                            };
                            res.Attachments.Add(attachmentData);
                        }
                        attachmentIndex++;
                    }
                    // Fetch reference messages
                    foreach (var referenceId in message.References)
                    {
                        var refQuery = SearchQuery.HeaderContains("Message-ID", referenceId);
                        var refUids = await inbox.SearchAsync(refQuery);

                        if (refUids.Count > 0)
                        {
                            var refMessage = await inbox.GetMessageAsync(refUids[0]);
                            if (refMessage != null)
                            {
                                res.References.Add(new ListedItem()
                                {
                                    Date = refMessage.Date.ToString(),
                                    MessageId = refMessage.MessageId,
                                    Subject = refMessage.Subject,
                                    Body = GetPlainTextPreview(refMessage.HtmlBody ?? refMessage.TextBody),
                                    From = refMessage.From.Mailboxes.FirstOrDefault()?.Address,
                                    ResentMessageId = refMessage.MessageId,
                                    HasAttachments = refMessage.Attachments.Any()
                                });
                            }
                        }
                    }

                    return res;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<DetailsEmail> FetchFullEmailFromInboxAsync(uint uniqueid, SpecialFolder folder)
        {
            try
            {
                var res = new DetailsEmail();
                using (var client = new ImapClient())
                {
                    await client.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);

                    var inbox = folder == SpecialFolder.All ? client.Inbox : client.GetFolder(folder);
                    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                    //  var message = inbox.FirstOrDefault(m => m.MessageId == messageid);
                    // Fetch the main message using the message ID
                    //  var query = SearchQuery.HeaderContains("Message-ID", messageid);
                    var newuniquie = new UniqueId(uniqueid);
                    var message = await inbox.GetMessageAsync(newuniquie);
                    res.From = message.From.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.To = message.To.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.Cc = message.Cc.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.Bcc = message.Bcc.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.ReplyTo = message.ReplyTo.Mailboxes.Select(mailbox => new EmailAddressDTO { Name = mailbox.Name, Email = mailbox.Address }).ToList();
                    res.Subject = message.Subject;
                    res.Date = message.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    res.Body = message.HtmlBody ?? message.TextBody;
                    res.HasAttachments = message.Attachments.Any();
                    // Map attachments
                    res.Attachments = new List<DetailsEmailAttachement>();
                    foreach (var attachment in message.Attachments)
                    {
                        int attachmentIndex = 0;
                        if (attachment is MimePart mimePart)
                        {
                            var fileName = mimePart.FileName;
                            if (string.IsNullOrEmpty(fileName) && mimePart.ContentDisposition != null)
                            {
                                fileName = mimePart.ContentDisposition?.FileName;
                            }

                            var attachmentData = new DetailsEmailAttachement
                            {
                                Name = fileName ?? "unnamed",
                                ContentType = mimePart.ContentType.MimeType,
                                Url = $"/crm/attachment?folder={folder}&uniqueId={uniqueid}&attix={attachmentIndex}"
                            };
                            res.Attachments.Add(attachmentData);
                        }
                        else if (attachment is MessagePart rfc822Part)
                        {
                            var fileName = rfc822Part.ContentDisposition?.FileName ?? "attached-email";

                            var attachmentData = new DetailsEmailAttachement
                            {
                                Name = fileName,
                                ContentType = rfc822Part.ContentType.MimeType,
                                Url = $"/crm/attachment?folder={folder}&uniqueId={uniqueid}&attix={attachmentIndex}"
                            };
                            res.Attachments.Add(attachmentData);
                        }
                        attachmentIndex++;
                    }
                    // Fetch reference messages
                    foreach (var referenceId in message.References)
                    {
                        var refQuery = SearchQuery.HeaderContains("Message-ID", referenceId);
                        var refUids = await inbox.SearchAsync(refQuery);

                        if (refUids.Count > 0)
                        {
                            var refMessage = await inbox.GetMessageAsync(refUids[0]);
                            if (refMessage != null)
                            {
                                res.References.Add(new ListedItem()
                                {
                                    Date = refMessage.Date.ToString(),
                                    MessageId = refMessage.MessageId,
                                    Subject = refMessage.Subject,
                                    Body = GetPlainTextPreview(refMessage.HtmlBody ?? refMessage.TextBody),
                                    From = refMessage.From.Mailboxes.FirstOrDefault()?.Address,
                                    ResentMessageId = refMessage.MessageId,
                                    HasAttachments = refMessage.Attachments.Any()
                                });
                            }
                        }
                    }


                    res.UniqueId = uniqueid;
                    return res;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// count unread message from folder
        /// </summary>
        /// <returns></returns>
        public async Task<IList<FolderSummary>> FetchFolderSummariesAsync()
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(credential.Email, credential.Password);

                var folderSummaries = new List<FolderSummary>();
                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);
                folderSummaries.Add(new FolderSummary
                {
                    Name = inbox.FullName, // or inbox.Name
                    UnreadCount = inbox.Unread
                });
                // List of folders to summarize
                var folders = new[] { SpecialFolder.Sent, SpecialFolder.Drafts };

                foreach (var folderType in folders)
                {
                    var folder = client.GetFolder(folderType);
                    await folder.StatusAsync(StatusItems.Unread);
                    await folder.OpenAsync(FolderAccess.ReadOnly);

                    folderSummaries.Add(new FolderSummary
                    {
                        Name = folder.FullName, // or folder.Name
                        UnreadCount = folder.Unread,
                        Folder = (int)folderType
                    });
                }

                await client.DisconnectAsync(true);

                return folderSummaries;
            }
        }


        public async Task<AttachmentResult> GetAttachmentAsync(string folder, string messageId, int attachmentIndex)
        {
            try
            {

                using (var client = new ImapClient())
                {
                    await client.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);

                    var inbox = folder == SpecialFolder.All.ToString() ? client.Inbox : client.GetFolder(folder);
                    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                    // Fetch the main message using the message ID
                    var query = SearchQuery.HeaderContains("Message-ID", messageId);
                    var uids = await inbox.SearchAsync(query);
                    if (uids.Count == 0)
                        throw new Exception("Message not found");
                    var message = await inbox.GetMessageAsync(uids[0]);
                    if (message.Attachments.Count() <= attachmentIndex)
                        throw new FileNotFoundException("Attachment not found");

                    var attachment = message.Attachments.ElementAt(attachmentIndex);
                    if (attachment is MimePart mimePart)
                    {
                        var stream = new System.IO.MemoryStream();
                        await mimePart.Content.DecodeToAsync(stream);
                        stream.Position = 0;

                        return new AttachmentResult
                        {
                            FileStream = stream,
                            ContentType = mimePart.ContentType.MimeType,
                            FileName = mimePart.FileName
                        };
                    }

                    throw new FileNotFoundException("Attachment not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<AttachmentResult> GetAttachmentAsync(string folder, uint uniqueId, int attachmentIndex)
        {
            try
            {

                using (var client = new ImapClient())
                {
                    await client.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);

                    var inbox = folder == SpecialFolder.All.ToString() ? client.Inbox : client.GetFolder(folder);
                    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                    var newuniqId = new UniqueId(uniqueId);
                    var message = await inbox.GetMessageAsync(newuniqId);
                    if (message.Attachments.Count() <= attachmentIndex)
                        throw new FileNotFoundException("Attachment not found");

                    var attachment = message.Attachments.ElementAt(attachmentIndex);
                    if (attachment is MimePart mimePart)
                    {
                        var stream = new System.IO.MemoryStream();
                        await mimePart.Content.DecodeToAsync(stream);
                        stream.Position = 0;

                        return new AttachmentResult
                        {
                            FileStream = stream,
                            ContentType = mimePart.ContentType.MimeType,
                            FileName = mimePart.FileName
                        };
                    }

                    throw new FileNotFoundException("Attachment not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private string GetPlainTextPreview(string htmlBody)
        {
            // Convert HTML to plain text and take first 100 characters
            if (!string.IsNullOrEmpty(htmlBody))
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlBody);

                string plainText = htmlDocument.DocumentNode.InnerText;

                // Trim and take first 100 characters
                var text = plainText.Trim().Replace("\r\n", "").Replace("\n", " ").Replace("  ", "");
                if (text.Length > 100)
                    return text.Substring(0, 100);
                else
                    return text.Trim();
            }

            return string.Empty;
        }

        public async Task<bool> SendEmailAsync(SendEmail data, string? FromEmail)
        {
            try
            {
                var message = new MimeMessage();
                if (!string.IsNullOrEmpty(FromEmail)) { message.From.Add(MailboxAddress.Parse(FromEmail)); }
                // Assuming 'From' is a valid email address

                // Add recipients (To)
                foreach (var to in data.To)
                {
                    message.To.Add(MailboxAddress.Parse(to.Email));
                }

                // Add CC recipients if provided
                if (data.Cc != null)
                {
                    foreach (var cc in data.Cc)
                    {
                        message.Cc.Add(MailboxAddress.Parse(cc.Email));
                    }
                }

                // Add BCC recipients if provided
                if (data.Bcc != null)
                {
                    foreach (var bcc in data.Bcc)
                    {
                        message.Bcc.Add(MailboxAddress.Parse(bcc.Email));
                    }
                }

                message.Subject = data.Subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = data.Body
                };

                // Create the body of the email
                var builder = new BodyBuilder();
                builder.HtmlBody = data.Body; // HTML body

                // Attachments
                if (data.Attachments != null && data.Attachments.Count > 0)
                {
                    foreach (var attachment in data.Attachments)
                    {
                        using (var stream = attachment.OpenReadStream())
                        {
                            builder.Attachments.Add(attachment.FileName, stream);
                        }
                    }
                }

                // Set the email body
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(credential.SMTPHOST, credential.SMTPPORT, credential.SMTPPORT == 587 ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                using (var imapClient = new ImapClient())
                {
                    await imapClient.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(credential.Email, credential.Password);

                    var sentFolder = imapClient.GetFolder(SpecialFolder.Sent);
                    await sentFolder.OpenAsync(FolderAccess.ReadWrite);

                    await sentFolder.AppendAsync(message);

                    await imapClient.DisconnectAsync(true);
                }

                return true; // Email sent and saved successfully
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ReplyEmailAsync(ReplyEmail data, string? FromEmail)
        {
            try
            {
                var message = new MimeMessage();
                if (!string.IsNullOrEmpty(FromEmail)) { message.From.Add(MailboxAddress.Parse(FromEmail)); }

                // Add recipients (To)
                foreach (var to in data.To)
                {
                    message.To.Add(MailboxAddress.Parse(to.Email));
                }

                // Add CC recipients if provided
                if (data.Cc != null)
                {
                    foreach (var cc in data.Cc)
                    {
                        message.Cc.Add(MailboxAddress.Parse(cc.Email));
                    }
                }

                // Add BCC recipients if provided
                if (data.Bcc != null)
                {
                    foreach (var bcc in data.Bcc)
                    {
                        message.Bcc.Add(MailboxAddress.Parse(bcc.Email));
                    }
                }
                message.InReplyTo = data.InReplyTo;
                message.Subject = data.Subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = data.Body
                };

                // Create the body of the email
                var builder = new BodyBuilder();
                builder.HtmlBody = data.Body; // HTML body

                // Attachments
                if (data.Attachments != null && data.Attachments.Count > 0)
                {
                    foreach (var attachment in data.Attachments)
                    {
                        using (var stream = attachment.OpenReadStream())
                        {
                            builder.Attachments.Add(attachment.FileName, stream);
                        }
                    }
                }

                // Set the email body
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(credential.SMTPHOST, credential.SMTPPORT, credential.SMTPPORT == 587 ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(credential.Email, credential.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                using (var imapClient = new ImapClient())
                {
                    await imapClient.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(credential.Email, credential.Password);

                    var sentFolder = imapClient.GetFolder(SpecialFolder.Sent);
                    await sentFolder.OpenAsync(FolderAccess.ReadWrite);

                    await sentFolder.AppendAsync(message);

                    await imapClient.DisconnectAsync(true);
                }

                return true; // Email sent and saved successfully
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> SaveEmailAsync(SendEmail data)
        {
            try
            {
                var message = new MimeMessage();
                //  message.From.Add(MailboxAddress.Parse(FromEmail)); // Assuming 'From' is a valid email address

                // Add recipients (To)
                foreach (var to in data.To)
                {
                    message.To.Add(MailboxAddress.Parse(to.Email));
                }

                // Add CC recipients if provided
                if (data.Cc != null)
                {
                    foreach (var cc in data.Cc)
                    {
                        message.Cc.Add(MailboxAddress.Parse(cc.Email));
                    }
                }

                // Add BCC recipients if provided
                if (data.Bcc != null)
                {
                    foreach (var bcc in data.Bcc)
                    {
                        message.Bcc.Add(MailboxAddress.Parse(bcc.Email));
                    }
                }

                message.Subject = data.Subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = data.Body
                };

                // Create the body of the email
                var builder = new BodyBuilder();
                builder.HtmlBody = data.Body; // HTML body

                // Attachments
                if (data.Attachments != null && data.Attachments.Count > 0)
                {
                    foreach (var attachment in data.Attachments)
                    {
                        using (var stream = attachment.OpenReadStream())
                        {
                            builder.Attachments.Add(attachment.FileName, stream);
                        }
                    }
                }

                // Set the email body
                message.Body = builder.ToMessageBody();
                using (var imapClient = new ImapClient())
                {
                    await imapClient.ConnectAsync(credential.IMAPHOST, credential.IMAPPORT, SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(credential.Email, credential.Password);

                    var sentFolder = imapClient.GetFolder(SpecialFolder.Drafts);
                    await sentFolder.OpenAsync(FolderAccess.ReadWrite);

                    await sentFolder.AppendAsync(message);

                    await imapClient.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
