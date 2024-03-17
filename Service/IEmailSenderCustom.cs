public interface IEmailSenderCustom
{
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
}