namespace Application.Interfaces;

public interface IEmailService
{
    Task<string> SendEmailAsync(string to, string subject, string htmlText);
}