from pypdf import PdfReader, PdfWriter

def EncryptPdf(pdfFilePath, outputPath, password):
    reader = PdfReader(pdfFilePath)
    writer = PdfWriter()

    for page in reader.pages:
        writer.add_page(page)

    writer.encrypt(password)

    with open(outputPath, "wb") as f:
        writer.write(f)


def DecryptPdf(pdfFilePath, outputPath, password):
    reader = PdfReader(pdfFilePath)

    if reader.is_encrypted:
        result = reader.decrypt(password)
        if result == 0:
            raise ValueError("رمز عبور اشتباه است")
    else:
        raise ValueError("این فایل رمز نگاری نشده است")

    writer = PdfWriter()
    for page in reader.pages:
        writer.add_page(page)

    with open(outputPath, "wb") as f:
        writer.write(f)