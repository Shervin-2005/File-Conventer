from pydub import AudioSegment

def ConvertAudio(inputPath, outputPath, targetFormat):
    audio = AudioSegment.from_file(inputPath)
    audio.export(outputPath, format=targetFormat.lower())
