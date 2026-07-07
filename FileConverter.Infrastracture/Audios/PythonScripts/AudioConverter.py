from pydub import AudioSegment

def ConvertAudio(inputPath, outputPath, targetFormat):
    audio = AudioSegment.from_file(inputPath)

    format_map = {
        "wma": "asf",   # WMA audio lives inside an ASF container
        "aac": "adts",  # raw AAC audio uses the ADTS container
        "m4a": "ipod",  # m4a (AAC in MP4 container) uses ffmpeg's "ipod" muxer
    }

    ffmpeg_format = format_map.get(targetFormat.lower(), targetFormat.lower())

    audio.export(outputPath, format=ffmpeg_format)