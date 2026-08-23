from docx2pdf import convert

def ConvertDocxToPdf(docxFilePath, outputPath):

    docx_file = docxFilePath
    pdf_file = outputPath 

    convert(docx_file, pdf_file)