from docx import Document
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.oxml.ns import qn
from docx2pdf import convert
import os


def ConvertTxtToDocx(txtFilePath, docxOutputPath):
    with open(txtFilePath, "r", encoding="utf-8") as f:
        text = f.read()

    doc = Document()

    for line in text.split("\n"):
        paragraph = doc.add_paragraph(line)
        paragraph.alignment = WD_ALIGN_PARAGRAPH.RIGHT  # RTL reading direction

        pPr = paragraph._p.get_or_add_pPr()
        bidi = pPr.makeelement(qn('w:bidi'), {})
        pPr.append(bidi)

        for run in paragraph.runs:
            run.font.name = "Vazirmatn"
            rPr = run._element.get_or_add_rPr()
            rFonts = rPr.makeelement(qn('w:rFonts'), {qn('w:cs'): "Vazirmatn"})
            rPr.append(rFonts)

    doc.save(docxOutputPath)


def ConvertTxtToPdf(txtFilePath, pdfOutputPath):
    # Step 1: build an intermediate .docx path next to the final pdf output
    tempDocxPath = os.path.splitext(pdfOutputPath)[0] + "_temp.docx"

    # Step 2: txt -> docx
    ConvertTxtToDocx(txtFilePath, tempDocxPath)

    # Step 3: docx -> pdf
    convert(tempDocxPath, pdfOutputPath)

    # Step 4: clean up the intermediate docx file
    if os.path.exists(tempDocxPath):
        os.remove(tempDocxPath)