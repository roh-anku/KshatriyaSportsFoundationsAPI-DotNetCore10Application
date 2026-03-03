namespace KshatriyaSportsFoundations.API.Utilities.SendGridEmailSender
{
    public interface IEmailSender
    {
        Task SendBulkEmailAsync(List<string> recipients, string subject, string htmlContent);
        List<string> GetRecipients();
        string GetSubject();
        string GetEnquiryEmailContent(string name, string phone, string email, string location, string enquiryMessage);
    }
}
