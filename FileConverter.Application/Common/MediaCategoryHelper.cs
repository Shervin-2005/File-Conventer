using FileConverter.Domain.Enums;

namespace FileConverter.Application.Common
{
    public static class MediaCategoryHelper
    {
        private static readonly Dictionary<string, MediaCategory> ExtensionMap = new()
        {
            // Document
            [".pdf"] = MediaCategory.Document,
            [".docx"] = MediaCategory.Document,
            [".doc"] = MediaCategory.Document,
            [".txt"] = MediaCategory.Document,
            [".rtf"] = MediaCategory.Document,
            [".odt"] = MediaCategory.Document,
            [".xlsx"] = MediaCategory.Document,
            [".xls"] = MediaCategory.Document,
            [".csv"] = MediaCategory.Document,
            [".pptx"] = MediaCategory.Document,
            [".ppt"] = MediaCategory.Document,

            // Image
            [".jpg"] = MediaCategory.Image,
            [".jpeg"] = MediaCategory.Image,
            [".png"] = MediaCategory.Image,
            [".webp"] = MediaCategory.Image,
            [".bmp"] = MediaCategory.Image,
            [".gif"] = MediaCategory.Image,
            [".tiff"] = MediaCategory.Image,

            // Audio
            [".mp3"] = MediaCategory.Audio,
            [".wav"] = MediaCategory.Audio,
            [".aac"] = MediaCategory.Audio,
            [".ogg"] = MediaCategory.Audio,
            [".flac"] = MediaCategory.Audio,
            [".m4a"] = MediaCategory.Audio,

            // Video
            [".mp4"] = MediaCategory.Video,
            [".mov"] = MediaCategory.Video,
            [".avi"] = MediaCategory.Video,
            [".mkv"] = MediaCategory.Video,
            [".webm"] = MediaCategory.Video,
        };

        public static MediaCategory GetCategory(string fileNameOrExtension)
        {
            var ext = fileNameOrExtension.StartsWith('.')
                ? fileNameOrExtension.ToLowerInvariant()
                : Path.GetExtension(fileNameOrExtension).ToLowerInvariant();

            if (ExtensionMap.TryGetValue(ext, out var category))
                return category;

            throw new NotSupportedException($"Unsupported file extension: {ext}");
        }
    }
}