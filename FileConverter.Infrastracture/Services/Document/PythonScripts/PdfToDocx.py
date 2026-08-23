from pdf2docx import Converter

def ConvertPdfToDocx(pdfFilePath, outputPath, startPage, endPage):
    cv = Converter(pdfFilePath)
    cv.convert(outputPath, start=startPage, end=endPage)
    cv.close()