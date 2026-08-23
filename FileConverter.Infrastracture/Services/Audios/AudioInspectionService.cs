using FileConverter.Application.Interfaces.AudioServiceInterfaces;
using FileConverter.Domain.Enums;

namespace FileConverter.Infrastracture.Services.Audios
{
    public class AudioInspectionService : IAudioInspectionService
    {
        public bool GetAudioFormatFromExtension(string extension, out AudioFormat initialFormat)
        {
            switch (extension)
            {
                case ".mp3":
                    initialFormat = AudioFormat.Mp3;
                    return true;

                case ".wav":
                    initialFormat = AudioFormat.Wav;
                    return true;

                case ".m4a":
                    initialFormat = AudioFormat.M4a;
                    return true;

                case ".aac":
                    initialFormat = AudioFormat.Aac;
                    return true;

                case ".ogg":
                    initialFormat = AudioFormat.Ogg;
                    return true;

                case ".flac":
                    initialFormat = AudioFormat.Flac;
                    return true;

                default:
                    initialFormat = default;
                    return false;
            }
        }

        public bool GetAudioTargetForamtFromEnum(AudioFormat targetFormat, out string targetExtension)
        {
            switch (targetFormat)
            {
                case AudioFormat.Mp3:
                    targetExtension = "mp3";
                    return true;

                case AudioFormat.Wav:
                    targetExtension = "wav";
                    return true;

                case AudioFormat.M4a:
                    targetExtension = "m4a";
                    return true;

                case AudioFormat.Aac:
                    targetExtension = "aac";
                    return true;

                case AudioFormat.Ogg:
                    targetExtension = "ogg";
                    return true;

                case AudioFormat.Flac:
                    targetExtension = "flac";
                    return true;

                default:
                    targetExtension = default!;
                    return false;
            }
        }
    }
}
