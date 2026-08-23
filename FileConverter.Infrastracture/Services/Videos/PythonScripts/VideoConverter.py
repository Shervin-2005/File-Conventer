import subprocess

def ConvertVideo(inputPath, outputPath, targetFormat):
    command = [
        "ffmpeg",
        "-y",  # overwrite output if it exists
        "-i", inputPath,
        outputPath
    ]

    # اضافه کردن encoding و errors برای جلوگیری از کرش کردن در صورت وجود کاراکترهای خاص در خروجی FFmpeg
    result = subprocess.run(
        command,
        capture_output=True,
        text=True,
        encoding='utf-8',
        errors='ignore'
    )

    if result.returncode != 0:
        # کاراکترهای اضافی انتهای این خط به طور کامل حذف شدند
        raise Exception(f"FFmpeg failed: {result.stderr}")