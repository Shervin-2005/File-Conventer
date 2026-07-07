import subprocess

def ConvertVideo(inputPath, outputPath, targetFormat):
    command = [
        "ffmpeg",
        "-y",  # overwrite output if it exists
        "-i", inputPath,
        outputPath
    ]

    result = subprocess.run(command, capture_output=True, text=True)

    if result.returncode != 0:
        raise Exception(f"FFmpeg failed: {result.stderr}")