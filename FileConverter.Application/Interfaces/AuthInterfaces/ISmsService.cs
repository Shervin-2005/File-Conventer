namespace FileConverter.Application.Interfaces.AuthInterfaces
{
    public interface ISmsService
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
