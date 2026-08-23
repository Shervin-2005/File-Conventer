using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using System.Text;

namespace FileConverter.Infrastracture.Services.Document
{
    public class PdfInspectionService : IPdfInspectionService
    {
        public async Task<bool> IsPdfEncryptedAsync(Stream pdfStream, CancellationToken cancellationToken)
        {
            if (!pdfStream.CanSeek)
            {
                return false;
            }

            long originalPosition = pdfStream.Position;
            pdfStream.Position = 0;

            try
            {
                using var reader = new StreamReader(pdfStream, Encoding.Latin1, false, 4096, leaveOpen: true);
                var buffer = new char[8192];
                int totalRead = 0;
                int charsRead;

                while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    totalRead += charsRead;
                    string chunk = new string(buffer, 0, charsRead);
                    if (chunk.Contains("/Encrypt"))
                    {
                        return true;
                    }

                    if (totalRead > 5 * 1024 * 1024)
                    {
                        break;
                    }
                }

                return false;
            }
            finally
            {
                pdfStream.Position = originalPosition;
            }
        }
    }
}