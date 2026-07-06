namespace FileConverter.Domain.Models;

public record ConversionStreamResult(
    Stream? OutputStream, 
    string? FileName, 
    string ContentType,
    double SizeInBytes,
    bool Success,
    string? ErrorMessage = null);
