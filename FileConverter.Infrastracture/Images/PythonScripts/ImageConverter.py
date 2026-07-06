from PIL import Image
import pillow_heif

pillow_heif.register_heif_opener()  # enables HEIC support

def ConvertImage(imagePath, outputPath, targetFormat):
    image = Image.open(imagePath)

    target = targetFormat.upper()
    if target == "JPG":
        target = "JPEG"  # Pillow calls it "JPEG" internally, not "JPG"

    if target == "JPEG" and image.mode in ("RGBA", "P"):
        background = Image.new("RGB", image.size, (255, 255, 255))
        if image.mode == "P":
            image = image.convert("RGBA")
        background.paste(image, mask=image.split()[-1])
        image = background
    elif image.mode == "P" and target != "GIF":
        image = image.convert("RGB")

    image.save(outputPath, target)