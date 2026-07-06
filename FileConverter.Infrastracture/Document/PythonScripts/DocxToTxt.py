from docx import Document

def ConvertDocxToTxt(docxFilePath, outputPath):
    doc = Document(docxFilePath)

    lines = []
    for paragraph in doc.paragraphs:
        lines.append(paragraph.text)

    full_text = "\n".join(lines)

    with open(outputPath, "w", encoding="utf-8") as f:
        f.write(full_text)