import fitz

def ConvertPdfToTxt(pdfFilePath, outputPath):
    doc = fitz.open(pdfFilePath)
    text = ""
    for page in doc:
        text += page.get_text()
    doc.close()

    with open(outputPath, "w", encoding="utf-8") as f:
        f.write(text)
    

        