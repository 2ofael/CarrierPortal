namespace CarrierPortal.Services.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body, string attachmentFilePath = null);
    }
}