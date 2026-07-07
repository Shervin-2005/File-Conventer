using FileConverter.Domain.Enums;

namespace FileConverter.Application.Interfaces.AudioServiceInterfaces
{
    public interface IAudioInspectionService
    {
        bool GetAudioFormatFromExtension(string extension, out AudioFormat initialFormat);
        bool GetAudioTargetForamtFromEnum(AudioFormat targetFormat, out string targetExtension);
    }
}
