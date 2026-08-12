using FileConverter.Domain.Enums;

namespace FileConverter.Domain.Entities
{
    public class MediaFormatResolver
    {
        public static bool TryValidate(MediaCategory category, string format) => category switch
        {
            MediaCategory.Document => Enum.TryParse<DocumentFormat>(format, ignoreCase: true, out _),
            MediaCategory.Image => Enum.TryParse<ImageFormat>(format, ignoreCase: true, out _),
            MediaCategory.Audio => Enum.TryParse<AudioFormat>(format, ignoreCase: true, out _),
            MediaCategory.Video => Enum.TryParse<VideoFormat>(format, ignoreCase: true, out _),
            _ => false
        };

        public static Enum Parse(MediaCategory category, string format) => category switch
        {
            MediaCategory.Document => Enum.Parse<DocumentFormat>(format, ignoreCase: true),
            MediaCategory.Image => Enum.Parse<ImageFormat>(format, ignoreCase: true),
            MediaCategory.Audio => Enum.Parse<AudioFormat>(format, ignoreCase: true),
            MediaCategory.Video => Enum.Parse<VideoFormat>(format, ignoreCase: true),
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };
    }
}
