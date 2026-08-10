import fitz

def ConvertPdfToTxt(pdfFilePath, outputPath, startPageStr, endPageStr):
    doc = fitz.open(pdfFilePath)
    text = ""

    # تبدیل ورودی‌های رشته‌ای به عدد
    start_idx = int(startPageStr)
    end_idx = int(endPageStr)

    # بررسی اینکه آیا کاربر همه صفحات را خواسته است یا خیر
    # در کد C# اگر کاربر 0 بفرستد، مقدار start_idx برابر 1- خواهد بود
    if start_idx == -1:
        pages_to_read = range(len(doc))
    else:
        # برای جلوگیری از خطای خارج از محدوده (Out of Range) در فایل‌های کوچکتر
        upper_bound = min(end_idx, len(doc))
        pages_to_read = range(start_idx, upper_bound)

    # خواندن صفحات بر اساس بازه‌ی مشخص شده
    for i in pages_to_read:
        page = doc.load_page(i)
        text += page.get_text()

    doc.close()

    # ذخیره در فایل متنی
    with open(outputPath, "w", encoding="utf-8") as f:
        f.write(text)