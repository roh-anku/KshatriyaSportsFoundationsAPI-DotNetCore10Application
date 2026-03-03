namespace KshatriyaSportsFoundations.API.Utilities.WhatsappMessageSender
{
    public interface IWhatsAppService
    {
        Task SendWhatsAppAsync(string to, string message);
        Task SendBulkWhatsAppAsync(List<string> recipients, string message);
        List<string> GetRecipients();
        string GetMessage(string name, string phone, string email, string location, string message);
    }
}
