from PIL import Image

def ConvertImagesToPdf(imageFilePaths, outputPath):
    images = []
    for path in imageFilePaths:
        img = Image.open(path)
        if img.mode in ("RGBA", "P"):
            background = Image.new("RGB", img.size, (255, 255, 255))
            if img.mode == "P":
                img = img.convert("RGBA")
            background.paste(img, mask=img.split()[-1])
            img = background
        elif img.mode != "RGB":
            img = img.convert("RGB")
        images.append(img)

    first_image, remaining_images = images[0], images[1:]
    first_image.save(outputPath, "PDF", save_all=True, append_images=remaining_images)