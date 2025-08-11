using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
// === iText 9 ===
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
// === Для работы с базовыми элементами геометрии ===
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace DocumentMark
{
    public class PdfProcessor
    {
        // === Настройки относительно опорной точки "Разраб." ===
        private const float Text_OffsetFromReferenceX = 55f;
        private const float Text_OffsetFromReferenceY = 0f;
        private const int FontSize_Main = 16;

        private const float SecondText_OffsetFromReferenceX = 159f;
        private const float SecondText_OffsetFromReferenceY = -2f;
        private const int SecondFontSize_Date = 8;

        private const float ThirdText_OffsetFromReferenceX = 55f;
        private const float ThirdText_OffsetFromReferenceY = 28f;
        private const int ThirdFontSize_Main = 16;

        private const float FourthText_OffsetFromReferenceX = 159f;
        private const float FourthText_OffsetFromReferenceY = 26f;
        private const int FourthFontSize_Date = 8;

        private const float SixthText_OffsetFromReferenceX = 159f;
        private const float SixthText_OffsetFromReferenceY = 73f;
        private const int SixthFontSize_Date = 8;

        private const float Image1_OffsetFromReferenceX = 90f;
        private const float Image1_OffsetFromReferenceY = -15f;
        private const float Image1_TargetWidth = 90f;
        private const float Image1_TargetHeight = 40f;
        private const string ImagePath1 = @"C:\PDF\1\Rename\Подп001.tif";

        private const float Image2_OffsetFromReferenceX = 105f;
        private const float Image2_OffsetFromReferenceY = 15f;
        private const float Image2_TargetWidth = 80f;
        private const float Image2_TargetHeight = 50f;
        private const string ImagePath2 = @"C:\PDF\1\Rename\Подп002.tif";

        private const float Image3_OffsetFromReferenceX = 105f; // Пример, подберите
        private const float Image3_OffsetFromReferenceY = 62f;  // Пример, подберите
        private const float Image3_TargetWidth = 80f;          // Пример, подберите
        private const float Image3_TargetHeight = 50f;         // Пример, подберите
        private const string ImagePath3_Default = @"C:\PDF\1\Rename\Подп003.tif";
        private const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\Подп002.tif";
        private const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\Подп006.tif";
        private const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\Подп005.tif";
        private const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\Подп004.tif";

        // === Настройки для зоны считывания текста ===
        private const float ReadArea_OffsetFromReferenceX = 55f;    // Пример, подберите (относительно опорной точки "Разраб.")
        private const float ReadArea_OffsetFromReferenceY = 73f;   // Пример, подберите (относительно опорной точки "Разраб.")
        private const float ReadArea_Width = 100f;                 // Пример, подберите
        private const float ReadArea_Height = 15f;                // Пример, подберите

        /// <summary>
        /// Генерирует случайную рабочую дату (не суббота и не воскресенье) в заданном диапазоне
        /// </summary>
        public string GetRandomWorkday(DateTime startDate, DateTime endDate)
        {
            Random random = new Random();
            List<DateTime> workdays = new List<DateTime>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workdays.Add(date);
                }
            }

            if (workdays.Count == 0)
            {
                TimeSpan diff = endDate - startDate;
                DateTime randomDate = startDate.AddDays(random.Next((int)diff.TotalDays + 1));
                return randomDate.ToString("dd.MM.yyyy");
            }

            int randomIndex = random.Next(workdays.Count);
            return workdays[randomIndex].ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Обрабатывает один PDF файл
        /// </summary>
        public bool ProcessSinglePdf(string inputPdfPath, string fontPath, string dateForNachBuro, string dateForUtverdil, string dateForRazrabotal)
        {
            try
            {
                string outputPdfPath = Path.Combine(
                    Path.GetDirectoryName(inputPdfPath),
                    Path.GetFileNameWithoutExtension(inputPdfPath) + "_edited.pdf"
                );

                using (var reader = new PdfReader(inputPdfPath))
                using (var writer = new PdfWriter(outputPdfPath))
                using (var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader, writer)) // Используем полное имя
                {
                    var page = pdfDoc.GetFirstPage();
                    if (page == null)
                    {
                        // LogMessage($"  Ошибка: Не удалось получить первую страницу PDF '{Path.GetFileName(inputPdfPath)}'.");
                        return false;
                    }

                    var pageSize = page.GetPageSize();
                    float pageWidth = pageSize.GetWidth();
                    float pageHeight = pageSize.GetHeight();
                    // LogMessage($"  Размер страницы: {pageWidth:F1} x {pageHeight:F1} точек");

                    // === Поиск опорной точки "Разраб." ===
                    float referenceX = 0;
                    float referenceY = 0;
                    bool referenceFound = false;

                    try
                    {
                        // LogMessage("  Поиск опорной точки 'Разраб.'...");

                        var listener = new CustomTextEventListener();
                        var parser = new PdfCanvasProcessor(listener);
                        parser.ProcessContent(page.GetContentBytes(), page.GetResources());

                        var foundTextItems = listener.FindAllText("Разраб.");
                        if (foundTextItems != null && foundTextItems.Count > 0)
                        {
                            var foundText = foundTextItems[0];
                            referenceX = foundText.Rect.GetX();
                            referenceY = foundText.Rect.GetY();
                            referenceFound = true;
                            // LogMessage($"  Опорная точка 'Разраб.' найдена: X={referenceX:F2}, Y={referenceY:F2}");
                        }
                        else
                        {
                            // LogMessage("  Опорная точка 'Разраб.' НЕ НАЙДЕНА. Используются координаты (0, 0) по умолчанию.");
                        }
                    }
                    catch (Exception /*refEx*/)
                    {
                        // LogMessage($"  Ошибка поиска опорной точки 'Разраб.': {refEx.Message}. Используются координаты (0, 0) по умолчанию.");
                    }
                    // === КОНЕЦ: Поиск опорной точки ===

                    // === Настройки относительно опорной точки "Разраб." ===
                    // === Координаты: Смещение ОТ опорной точки (referenceX, referenceY) ===

                    // --- "Утвердил" ---
                    float textX_Pdf = referenceX + Text_OffsetFromReferenceX;
                    float textY_Pdf = referenceY + Text_OffsetFromReferenceY;
                    const int FontSize_Main = 16;

                    // --- "УтвердилДата" ---
                    float secondTextX_Pdf = referenceX + SecondText_OffsetFromReferenceX;
                    float secondTextY_Pdf = referenceY + SecondText_OffsetFromReferenceY;
                    const int SecondFontSize_Date = 8;

                    // --- "Нач.бюро" ---
                    float thirdTextX_Pdf = referenceX + ThirdText_OffsetFromReferenceX;
                    float thirdTextY_Pdf = referenceY + ThirdText_OffsetFromReferenceY;
                    const int ThirdFontSize_Main = 16;

                    // --- "Нач.БюроДата" ---
                    float fourthTextX_Pdf = referenceX + FourthText_OffsetFromReferenceX;
                    float fourthTextY_Pdf = referenceY + FourthText_OffsetFromReferenceY;
                    const int FourthFontSize_Date = 8;

                    // --- "РазработалДата" ---
                    float sixthTextX_Pdf = referenceX + SixthText_OffsetFromReferenceX;
                    float sixthTextY_Pdf = referenceY + SixthText_OffsetFromReferenceY;
                    const int SixthFontSize_Date = 8;

                    // --- Изображение 1 (Подп001.tif) ---
                    float image1X_Pdf = referenceX + Image1_OffsetFromReferenceX;
                    float image1Y_Pdf = referenceY + Image1_OffsetFromReferenceY;
                    const float Image1_TargetWidth = 90f;
                    const float Image1_TargetHeight = 40f;
                    const string ImagePath1 = @"C:\PDF\1\Rename\Подп001.tif";

                    // --- Изображение 2 (Подп002.tif) ---
                    float image2X_Pdf = referenceX + Image2_OffsetFromReferenceX;
                    float image2Y_Pdf = referenceY + Image2_OffsetFromReferenceY;
                    const float Image2_TargetWidth = 80f;
                    const float Image2_TargetHeight = 50f;
                    const string ImagePath2 = @"C:\PDF\1\Rename\Подп002.tif";

                    // --- Изображение 3 (Подп003.tif и альтернативы) ---
                    float image3X_Pdf = referenceX + Image3_OffsetFromReferenceX;
                    float image3Y_Pdf = referenceY + Image3_OffsetFromReferenceY;
                    const float Image3_TargetWidth = 80f;
                    const float Image3_TargetHeight = 50f;
                    const string ImagePath3_Default = @"C:\PDF\1\Rename\Подп003.tif";
                    const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\Подп002.tif";
                    const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\Подп006.tif";
                    const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\Подп005.tif";
                    const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\Подп004.tif";

                    // --- Зона считывания текста ---
                    float readAreaX_Pdf = referenceX + ReadArea_OffsetFromReferenceX;
                    float readAreaY_Pdf = referenceY + ReadArea_OffsetFromReferenceY;
                    const float ReadArea_Width = 100f;
                    const float ReadArea_Height = 15f;

                    // === Расчет АБСОЛЮТНЫХ координат относительно опорной точки ===

                    // --- Расчет АБСОЛЮТНЫХ координат X (от левого края) ---
                    // Уже рассчитаны выше

                    // --- Расчет АБСОЛЮТНЫХ координат Y (от нижнего края) ---
                    // В PDF (0,0) в нижнем левом углу, Y растет вверх
                    // Уже рассчитаны выше

                    // --- Размеры для зоны считывания ---
                    // Уже рассчитаны выше

                    // 5. === Создаем PdfCanvas для рисования прямоугольников ===
                    PdfCanvas canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // === НАЧАЛО: Отрисовка прямоугольников ===
                    // Прямоугольник 1 
                    float rect1X = referenceX + 106f;
                    float rect1Y = referenceY + 13f;
                    float rect1Width = 27f;
                    float rect1Height = 13f;

                    // Прямоугольник 2 
                    float rect2X = referenceX + 135.4f;
                    float rect2Y = referenceY + 13f;
                    float rect2Width = 5f;
                    float rect2Height = 13f;

                    // Рисуем первый прямоугольник (белый, закрашенный)
                    canvas.SaveState();
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);
                    canvas.Rectangle(rect1X, rect1Y, rect1Width, rect1Height);
                    canvas.Fill();
                    canvas.RestoreState();

                    // Рисуем второй прямоугольник (белый, закрашенный)
                    canvas.SaveState();
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);
                    canvas.Rectangle(rect2X, rect2Y, rect2Width, rect2Height);
                    canvas.Fill();
                    canvas.RestoreState();

                    // === НОВОЕ: Красный прямоугольник для зоны считывания ===
                    canvas.SaveState();
                    canvas.SetStrokeColorRgb(1.0f, 0.0f, 0.0f);
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);
                    canvas.SetLineWidth(1);
                    canvas.Rectangle(readAreaX_Pdf, readAreaY_Pdf, ReadArea_Width, ReadArea_Height);
                    canvas.FillStroke();
                    canvas.RestoreState();

                    // LogMessage("  Прямоугольники нарисованы (включая красный для зоны считывания).");
                    // === КОНЕЦ: Отрисовка прямоугольников ===

                    // === НОВОЕ: Считывание текста из области ===
                    string readText = "";
                    try
                    {
                        // LogMessage($"  Попытка считывания текста из области: X={readAreaX_Pdf:F1}, Y={readAreaY_Pdf:F1}, W={ReadArea_Width:F1}, H={ReadArea_Height:F1}");

                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                        string allText = PdfTextExtractor.GetTextFromPage(page, strategy);

                        if (!string.IsNullOrEmpty(allText))
                        {
                            string[] lines = allText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in lines)
                            {
                                if (line.IndexOf("Старцев", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("Максимов", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("Тихомиров", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("Седюк", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("Русских", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    if (line.IndexOf("Старцев", StringComparison.OrdinalIgnoreCase) >= 0) readText = "Старцев";
                                    else if (line.IndexOf("Максимов", StringComparison.OrdinalIgnoreCase) >= 0) readText = "Максимов";
                                    else if (line.IndexOf("Тихомиров", StringComparison.OrdinalIgnoreCase) >= 0) readText = "Тихомиров";
                                    else if (line.IndexOf("Седюк", StringComparison.OrdinalIgnoreCase) >= 0) readText = "Седюк";
                                    else if (line.IndexOf("Русских", StringComparison.OrdinalIgnoreCase) >= 0) readText = "Русских";
                                    break;
                                }
                            }
                        }

                        readText = readText.Trim();
                        // LogMessage($"  Считанный текст (примитивный поиск): '{readText}'");
                    }
                    catch (Exception /*readEx*/)
                    {
                        // LogMessage($"  Ошибка считывания текста: {readEx.Message}");
                        readText = "";
                    }
                    // === КОНЕЦ: Считывание текста из области ===

                    // === НОВОЕ: Определение пути к изображению 3 на основе считанного текста ===
                    string imagePath3_Selected = "";
                    try
                    {
                        if (readText.Equals("Старцев", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt1;
                            // LogMessage($"  Определено изображение для вставки: Подп002.tif (по слову 'Старцев')");
                        }
                        else if (readText.Equals("Максимов", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Default;
                            // LogMessage($"  Определено изображение для вставки: Подп003.tif (по слову 'Максимов')");
                        }
                        else if (readText.Equals("Тихомиров", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt2;
                            // LogMessage($"  Определено изображение для вставки: Подп006.tif (по слову 'Тихомиров')");
                        }
                        else if (readText.Equals("Седюк", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt3;
                            // LogMessage($"  Определено изображение для вставки: Подп005.tif (по слову 'Седюк')");
                        }
                        else if (readText.Equals("Русских", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt4;
                            // LogMessage($"  Определено изображение для вставки: Подп004.tif (по слову 'Русских')");
                        }
                        else
                        {
                            imagePath3_Selected = "";
                            if (string.IsNullOrEmpty(readText))
                            {
                                // LogMessage($"  Текст не считан или пуст. Изображение 3 вставлено не будет.");
                            }
                            else
                            {
                                // LogMessage($"  Считано неизвестное слово '{readText}'. Изображение 3 вставлено не будет.");
                            }
                        }
                    }
                    catch (Exception /*selectEx*/)
                    {
                        // LogMessage($"  Ошибка определения изображения: {selectEx.Message}");
                        imagePath3_Selected = "";
                    }
                    // === КОНЕЦ: Определение пути к изображению 3 ===

                    // 6. === Создаем новый PdfCanvas для рисования текста ===
                    PdfCanvas textCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // 7. === Загружаем шрифт ===
                    PdfFont font;
                    try
                    {
                        if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
                        {
                            byte[] fontBytes = File.ReadAllBytes(fontPath);
                            font = PdfFontFactory.CreateFont(fontBytes, "Identity-H");
                            // LogMessage($"  Шрифт загружен: {Path.GetFileName(fontPath)}");
                        }
                        else
                        {
                            font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
                            // LogMessage($"  Шрифт не найден. Используется Helvetica-Oblique.");
                        }
                    }
                    catch (Exception /*fontEx*/)
                    {
                        font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
                        // LogMessage($"  Ошибка загрузки шрифта: {fontEx.Message}. Используется Helvetica-Oblique.");
                    }

                    // 8. === Вставляем текстовые элементы ===
                    // Утвердил
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FontSize_Main);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(textX_Pdf, textY_Pdf);
                    textCanvas.ShowText("Утвердил");
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // УтвердилДата
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SecondFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(secondTextX_Pdf, secondTextY_Pdf);
                    textCanvas.ShowText(dateForUtverdil);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // Нач.бюро
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, ThirdFontSize_Main);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(thirdTextX_Pdf, thirdTextY_Pdf);
                    textCanvas.ShowText("Нач.бюро");
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // Нач.БюроДата
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FourthFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(fourthTextX_Pdf, fourthTextY_Pdf);
                    textCanvas.ShowText(dateForNachBuro);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // РазработалДата
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SixthFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(sixthTextX_Pdf, sixthTextY_Pdf);
                    textCanvas.ShowText(dateForRazrabotal);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // LogMessage("  Текстовые элементы вставлены.");

                    // 9. === Вставляем изображения ===
                    // Изображение 1
                    try
                    {
                        if (File.Exists(ImagePath1))
                        {
                            ImageData imageData1 = ImageDataFactory.Create(ImagePath1);
                            if (imageData1 != null)
                            {
                                Image imageElement1 = new Image(imageData1); // Используем псевдоним
                                imageElement1.SetFixedPosition(image1X_Pdf, image1Y_Pdf);
                                imageElement1.SetWidth(Image1_TargetWidth);
                                imageElement1.SetHeight(Image1_TargetHeight);

                                Canvas layoutCanvas1 = new Canvas(page, page.GetPageSize());
                                layoutCanvas1.Add(imageElement1);
                                layoutCanvas1.Close();
                            }
                            else
                            {
                                // LogMessage("  Ошибка: Не удалось загрузить данные изображения 1.");
                            }
                        }
                        else
                        {
                            // LogMessage($"  Файл изображения 1 не найден: {Path.GetFileName(ImagePath1)}");
                        }
                    }
                    catch (Exception /*imgEx*/)
                    {
                        // LogMessage($"  Ошибка вставки изображения 1: {imgEx.Message}");
                    }

                    // Изображение 2
                    try
                    {
                        if (File.Exists(ImagePath2))
                        {
                            ImageData imageData2 = ImageDataFactory.Create(ImagePath2);
                            if (imageData2 != null)
                            {
                                Image imageElement2 = new Image(imageData2); // Используем псевдоним
                                imageElement2.SetFixedPosition(image2X_Pdf, image2Y_Pdf);
                                imageElement2.SetWidth(Image2_TargetWidth);
                                imageElement2.SetHeight(Image2_TargetHeight);

                                Canvas layoutCanvas2 = new Canvas(page, page.GetPageSize());
                                layoutCanvas2.Add(imageElement2);
                                layoutCanvas2.Close();
                            }
                            else
                            {
                                // LogMessage("  Ошибка: Не удалось загрузить данные изображения 2.");
                            }
                        }
                        else
                        {
                            // LogMessage($"  Файл изображения 2 не найден: {Path.GetFileName(ImagePath2)}");
                        }
                    }
                    catch (Exception /*imgEx*/)
                    {
                        // LogMessage($"  Ошибка вставки изображения 2: {imgEx.Message}");
                    }

                    // === НОВОЕ: Изображение 3 (с условной логикой) ===
                    if (!string.IsNullOrEmpty(imagePath3_Selected) && File.Exists(imagePath3_Selected))
                    {
                        try
                        {
                            ImageData imageData3 = ImageDataFactory.Create(imagePath3_Selected);
                            if (imageData3 != null)
                            {
                                Image imageElement3 = new Image(imageData3); // Используем псевдоним
                                imageElement3.SetFixedPosition(image3X_Pdf, image3Y_Pdf);
                                imageElement3.SetWidth(Image3_TargetWidth);
                                imageElement3.SetHeight(Image3_TargetHeight);

                                Canvas layoutCanvas3 = new Canvas(page, page.GetPageSize());
                                layoutCanvas3.Add(imageElement3);
                                layoutCanvas3.Close();

                                // LogMessage($"  Изображение 3 вставлено: {Path.GetFileName(imagePath3_Selected)}");
                            }
                            else
                            {
                                // LogMessage("  Ошибка: Не удалось загрузить данные изображения 3.");
                            }
                        }
                        catch (Exception /*imgEx*/)
                        {
                            // LogMessage($"  Ошибка вставки изображения 3: {imgEx.Message}");
                        }
                    }
                    else if (!string.IsNullOrEmpty(imagePath3_Selected))
                    {
                        // LogMessage($"  Файл изображения 3 не найден: {Path.GetFileName(imagePath3_Selected)}");
                    }
                    // Если imagePath3_Selected пустая строка, ничего не делаем - изображение не вставляется

                    // LogMessage("  Обработка изображений завершена.");
                }

                // LogMessage($"  Файл сохранен как: {Path.GetFileName(outputPdfPath)}");
                return true;

            }
            catch (Exception ex)
            {
                // LogMessage($"  Ошибка обработки файла '{Path.GetFileName(inputPdfPath)}': {ex.Message}");
                return false;
            }
        }
    }
}