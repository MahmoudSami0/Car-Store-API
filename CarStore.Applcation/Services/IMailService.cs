namespace CarStore.Applcation.Services;

public interface IMailService
{
    Task SendEmailConfirmationAsync(string email, string confirmatiionLink);
}