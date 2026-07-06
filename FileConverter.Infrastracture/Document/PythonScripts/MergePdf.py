from pypdf import PdfWriter

def MergePdfs(pdfFilePaths, outputPath):
    merger = PdfWriter()

    for path in pdfFilePaths:
        merger.append(path)

    merger.write(outputPath)
    merger.close()