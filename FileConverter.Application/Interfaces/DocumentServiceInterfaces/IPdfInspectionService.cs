namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IPdfInspectionService
    {
        Task<bool> IsPdfEncryptedAsync(Stream pdfStream, CancellationToken cancellationToken);
    }
}
