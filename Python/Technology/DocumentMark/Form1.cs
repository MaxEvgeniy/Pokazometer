using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// === Проверенные using для iText 9 (с itext.kernel) ===
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
// === Дополнительные using для парсинга текста ===
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
// === Для работы с базовыми элементами геометрии ===
using iText.Kernel.Pdf.Canvas.Parser.Data;
// === Разрешение конфликтов имен ===
using SysPath = System.IO.Path;           // Псевдоним для System.IO.Path
using SysRectangle = System.Drawing.Rectangle; // Псевдоним для System.Drawing.Rectangle
using IT_Rectangle = iText.Kernel.Geom.Rectangle; // Псевдоним для iText.Kernel.Geom.Rectangle
using IT_Image = iText.Layout.Element.Image;     // Псевдоним для iText.Layout.Element.Image
using IT_PdfDocument = iText.Kernel.Pdf.PdfDocument; // Псевдоним для iText.Kernel.Pdf.PdfDocument
using IT_ImageData = iText.IO.Image.ImageData;     // Псевдоним для iText.IO.Image.ImageData
using IT_ImageDataFactory = iText.IO.Image.ImageDataFactory; // Псевдоним для iText.IO.Image.ImageDataFactory

namespace DocumentMark
{
    public partial class Form1 : Form
    {
        // === Настройки по умолчанию ===
        private const string DefaultInputPdfFolderPath = @"C:\PDF\1\Rename";
        private const string DefaultFontPath = @"C:\Users\maksimov\AppData\Local\Microsoft\Windows\Fonts\GOST type A Italic.ttf";

        // === Настройки относительно опорной точки "Разраб." ===
        private const float Text_OffsetFromReferenceX = 55f;
        private const float Text_OffsetFromReferenceY = 0f;
        private const int FontSize_Main = 16;

        private const float SecondText_OffsetFromReferenceX = 159f;
        private const float SecondText_OffsetFromReferenceY = -2f;
        private const int FontSize_Date = 8;

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

        public Form1()
        {
            InitializeComponent();
            // Инициализируем поля ввода значениями по умолчанию
            textBoxPdfFolderPath.Text = DefaultInputPdfFolderPath;
            textBoxFontPath.Text = DefaultFontPath;
            textBox1.Text = "01.07.2019";
            UpdateStatus("Приложение готово.");
        }

        /// <summary>
        /// Обновляет текст в строке состояния
        /// </summary>
        /// <param name="message">Сообщение для добавления</param>
        private void UpdateStatus(string message)
        {
            if (toolStripStatusLabel1 != null)
            {
                toolStripStatusLabel1.Text = message;
                statusStrip1.Refresh();
            }
        }

        /// <summary>
        /// Добавляет сообщение в текстовое поле логов
        /// </summary>
        /// <param name="message">Сообщение для добавления</param>
        private void LogMessage(string message)
        {
            if (textBoxLog != null)
            {
                textBoxLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
                textBoxLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Обзор..." для выбора папки PDF
        /// </summary>
        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Выберите папку с PDF файлами";
            fbd.SelectedPath = textBoxPdfFolderPath.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBoxPdfFolderPath.Text = fbd.SelectedPath;
                UpdateStatus($"Выбрана папка: {SysPath.GetFileName(fbd.SelectedPath)}");
                LogMessage($"Выбрана папка: {fbd.SelectedPath}");
            }
            else
            {
                UpdateStatus("Выбор папки отменен.");
                LogMessage("Выбор папки отменен.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "..." для выбора шрифта
        /// </summary>
        private void buttonBrowseFont_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы шрифтов (*.ttf, *.otf)|*.ttf;*.otf|Все файлы (*.*)|*.*";
            ofd.Title = "Выберите файл шрифта";
            ofd.FileName = textBoxFontPath.Text;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxFontPath.Text = ofd.FileName;
                UpdateStatus($"Выбран шрифт: {SysPath.GetFileName(ofd.FileName)}");
                LogMessage($"Выбран шрифт: {SysPath.GetFileName(ofd.FileName)}");
            }
            else
            {
                UpdateStatus("Выбор шрифта отменен.");
                LogMessage("Выбор шрифта отменен.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Обработать все PDF"
        /// </summary>
        private void buttonProcessAll_Click(object sender, EventArgs e)
        {
            buttonProcessAll.Enabled = false;
            UpdateStatus("Начало обработки...");
            LogMessage("Начало обработки всех PDF файлов...");

            try
            {
                string pdfFolderPath = textBoxPdfFolderPath.Text.Trim();
                string fontPath = textBoxFontPath.Text.Trim();
                string datePeredachiString = textBox1.Text.Trim();

                DateTime datePeredachi;
                if (string.IsNullOrEmpty(datePeredachiString) || !DateTime.TryParseExact(datePeredachiString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out datePeredachi))
                {
                    UpdateStatus("Ошибка даты. Используется текущая.");
                    LogMessage("Ошибка: Неверный формат даты передачи документов в textBox1. Используется текущая дата.");
                    datePeredachi = DateTime.Now;
                }

                if (string.IsNullOrEmpty(pdfFolderPath) || !Directory.Exists(pdfFolderPath))
                {
                    UpdateStatus("Ошибка: Папка не найдена.");
                    LogMessage($"Ошибка: Папка не найдена или путь пуст: '{pdfFolderPath}'");
                    MessageBox.Show($"Папка не найдена или путь пуст: '{pdfFolderPath}'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] pdfFiles = Directory.GetFiles(pdfFolderPath, "*.pdf", SearchOption.TopDirectoryOnly);
                if (pdfFiles.Length == 0)
                {
                    UpdateStatus("Ошибка: В папке нет PDF.");
                    LogMessage("Ошибка: В папке не найдено PDF файлов.");
                    MessageBox.Show($"В папке не найдено PDF файлов: {pdfFolderPath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UpdateStatus($"Найдено PDF файлов: {pdfFiles.Length}");
                LogMessage($"Найдено PDF файлов для обработки: {pdfFiles.Length}");

                int successCount = 0;
                int errorCount = 0;

                foreach (string inputPdfPath in pdfFiles)
                {
                    try
                    {
                        DateTime minUtverdilDate = datePeredachi.AddDays(-20);
                        DateTime maxUtverdilDate = datePeredachi;
                        string utverdilDateString = GetRandomWorkday(minUtverdilDate, maxUtverdilDate);
                        LogMessage($"Сгенерирована 'УтвердилДата': {utverdilDateString}");

                        DateTime utverdilDate = DateTime.ParseExact(utverdilDateString, "dd.MM.yyyy", null);
                        DateTime minNachBuroDate = utverdilDate.AddDays(-10);
                        DateTime maxNachBuroDate = utverdilDate;
                        string nachBuroDateString = GetRandomWorkday(minNachBuroDate, maxNachBuroDate);
                        LogMessage($"Сгенерирована 'Нач.БюроДата': {nachBuroDateString}");

                        DateTime nachBuroDate = DateTime.ParseExact(nachBuroDateString, "dd.MM.yyyy", null);
                        DateTime minRazrabotalDate = nachBuroDate.AddDays(-50);
                        DateTime maxRazrabotalDate = nachBuroDate;
                        string razrabotalDateString = GetRandomWorkday(minRazrabotalDate, maxRazrabotalDate);
                        LogMessage($"Сгенерирована 'РазработалДата': {razrabotalDateString}");

                        UpdateStatus($"Обработка: {SysPath.GetFileName(inputPdfPath)}...");
                        LogMessage($"Обработка файла: {SysPath.GetFileName(inputPdfPath)}...");
                        if (ProcessSinglePdf(inputPdfPath, fontPath, nachBuroDateString, utverdilDateString, razrabotalDateString))
                        {
                            successCount++;
                            UpdateStatus($"Успешно: {SysPath.GetFileName(inputPdfPath)}");
                            LogMessage($"Файл {SysPath.GetFileName(inputPdfPath)} успешно обработан.");
                        }
                        else
                        {
                            errorCount++;
                            UpdateStatus($"Ошибка: {SysPath.GetFileName(inputPdfPath)}");
                            LogMessage($"Ошибка при обработке файла {SysPath.GetFileName(inputPdfPath)}.");
                        }
                    }
                    catch (Exception fileEx)
                    {
                        errorCount++;
                        UpdateStatus($"Ошибка файла: {SysPath.GetFileName(inputPdfPath)}");
                        LogMessage($"Критическая ошибка при обработке файла {SysPath.GetFileName(inputPdfPath)}: {fileEx.Message}");
                    }
                }

                UpdateStatus($"Завершено. Успешно: {successCount}, Ошибок: {errorCount}.");
                LogMessage($"Обработка завершена. Успешно: {successCount}, Ошибок: {errorCount}.");
                MessageBox.Show($"Обработка завершена.\nУспешно обработано: {successCount}\nОшибок: {errorCount}", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка обработки: {ex.Message}");
                LogMessage($"Ошибка в процессе обработки: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonProcessAll.Enabled = true;
                LogMessage("Обработка закончена.");
            }
        }

        /// <summary>
        /// Генерирует случайную рабочую дату (не суббота и не воскресенье) в заданном диапазоне
        /// </summary>
        private string GetRandomWorkday(DateTime startDate, DateTime endDate)
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
        /// Обрабатывает один PDF файл с логикой выбора изображений относительно считываемого слова
        /// </summary>
        private bool ProcessSinglePdf(string inputPdfPath, string fontPath, string dateForNachBuro, string dateForUtverdil, string dateForRazrabotal)
        {
            try
            {
                string outputPdfPath = SysPath.Combine(
                    SysPath.GetDirectoryName(inputPdfPath),
                    SysPath.GetFileNameWithoutExtension(inputPdfPath) + "_edited.pdf"
                );

                using (var reader = new PdfReader(inputPdfPath))
                using (var writer = new PdfWriter(outputPdfPath))
                using (var pdfDoc = new IT_PdfDocument(reader, writer)) // Используем псевдоним
                {
                    var page = pdfDoc.GetFirstPage();
                    if (page == null)
                    {
                        LogMessage($"  Ошибка: Не удалось получить первую страницу PDF '{SysPath.GetFileName(inputPdfPath)}'.");
                        return false;
                    }

                    var pageSize = page.GetPageSize();
                    float pageWidth = pageSize.GetWidth();
                    float pageHeight = pageSize.GetHeight();
                    LogMessage($"  Размер страницы: {pageWidth:F1} x {pageHeight:F1} точек");

                    // === Поиск опорной точки "Разраб." ===
                    float referenceX = 0;
                    float referenceY = 0;
                    bool referenceFound = false;

                    try
                    {
                        LogMessage("  Поиск опорной точки 'Разраб.'...");

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
                            LogMessage($"  Опорная точка 'Разраб.' найдена: X={referenceX:F2}, Y={referenceY:F2}");
                        }
                        else
                        {
                            LogMessage("  Опорная точка 'Разраб.' НЕ НАЙДЕНА. Используются координаты (0, 0) по умолчанию.");
                        }
                    }
                    catch (Exception refEx)
                    {
                        LogMessage($"  Ошибка поиска опорной точки 'Разраб.': {refEx.Message}. Используются координаты (0, 0) по умолчанию.");
                    }

                    // === Настройки относительно опорной точки "Разраб." ===
                    const float Text_OffsetFromReferenceX = 55f;
                    const float Text_OffsetFromReferenceY = 0f;
                    const int FontSize_Main = 16;

                    const float SecondText_OffsetFromReferenceX = 159f;
                    const float SecondText_OffsetFromReferenceY = -2f;
                    const int FontSize_Date = 8;

                    const float ThirdText_OffsetFromReferenceX = 55f;
                    const float ThirdText_OffsetFromReferenceY = 28f;
                    const int ThirdFontSize_Main = 16;

                    const float FourthText_OffsetFromReferenceX = 159f;
                    const float FourthText_OffsetFromReferenceY = 26f;
                    const int FourthFontSize_Date = 8;

                    const float SixthText_OffsetFromReferenceX = 159f;
                    const float SixthText_OffsetFromReferenceY = 73f;
                    const int SixthFontSize_Date = 8;

                    const float Image1_OffsetFromReferenceX = 90f;
                    const float Image1_OffsetFromReferenceY = -15f;
                    const float Image1_TargetWidth = 90f;
                    const float Image1_TargetHeight = 40f;
                    const string ImagePath1 = @"C:\PDF\1\Rename\Подп001.tif";

                    const float Image2_OffsetFromReferenceX = 105f;
                    const float Image2_OffsetFromReferenceY = 15f;
                    const float Image2_TargetWidth = 80f;
                    const float Image2_TargetHeight = 50f;
                    const string ImagePath2 = @"C:\PDF\1\Rename\Подп002.tif";

                    const float Image3_OffsetFromReferenceX = 105f; // Пример, подберите
                    const float Image3_OffsetFromReferenceY = 62f;  // Пример, подберите
                    const float Image3_TargetWidth = 80f;          // Пример, подберите
                    const float Image3_TargetHeight = 50f;         // Пример, подберите
                    const string ImagePath3_Default = @"C:\PDF\1\Rename\Подп003.tif";
                    const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\Подп002.tif";
                    const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\Подп006.tif";
                    const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\Подп005.tif";
                    const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\Подп004.tif";

                    // === Настройки для зоны считывания текста ===
                    const float ReadArea_OffsetFromReferenceX = 55f;    // Пример, подберите (относительно опорной точки "Разраб.")
                    const float ReadArea_OffsetFromReferenceY = 73f;   // Пример, подберите (относительно опорной точки "Разраб.")
                    const float ReadArea_Width = 100f;                 // Пример, подберите
                    const float ReadArea_Height = 15f;                // Пример, подберите

                    // === Расчет АБСОЛЮТНЫХ координат относительно опорной точки ===
                    float textX_Pdf = referenceX + Text_OffsetFromReferenceX;
                    float textY_Pdf = referenceY + Text_OffsetFromReferenceY;
                    float secondTextX_Pdf = referenceX + SecondText_OffsetFromReferenceX;
                    float secondTextY_Pdf = referenceY + SecondText_OffsetFromReferenceY;
                    float thirdTextX_Pdf = referenceX + ThirdText_OffsetFromReferenceX;
                    float thirdTextY_Pdf = referenceY + ThirdText_OffsetFromReferenceY;
                    float fourthTextX_Pdf = referenceX + FourthText_OffsetFromReferenceX;
                    float fourthTextY_Pdf = referenceY + FourthText_OffsetFromReferenceY;
                    float sixthTextX_Pdf = referenceX + SixthText_OffsetFromReferenceX;
                    float sixthTextY_Pdf = referenceY + SixthText_OffsetFromReferenceY;

                    float image1X_Pdf = referenceX + Image1_OffsetFromReferenceX;
                    float image1Y_Pdf = referenceY + Image1_OffsetFromReferenceY;
                    float image2X_Pdf = referenceX + Image2_OffsetFromReferenceX;
                    float image2Y_Pdf = referenceY + Image2_OffsetFromReferenceY;
                    float image3X_Pdf = referenceX + Image3_OffsetFromReferenceX;
                    float image3Y_Pdf = referenceY + Image3_OffsetFromReferenceY;

                    float readAreaX_Pdf = referenceX + ReadArea_OffsetFromReferenceX;
                    float readAreaY_Pdf = referenceY + ReadArea_OffsetFromReferenceY;
                    float readAreaWidth_Pdf = ReadArea_Width;
                    float readAreaHeight_Pdf = ReadArea_Height;

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
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f); // RGB для белого цвета
                    canvas.Rectangle(rect1X, rect1Y, rect1Width, rect1Height);
                    canvas.Fill(); // Закрашиваем прямоугольник
                    canvas.RestoreState();

                    // Рисуем второй прямоугольник (белый, закрашенный)
                    canvas.SaveState();
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f); // RGB для белого цвета
                    canvas.Rectangle(rect2X, rect2Y, rect2Width, rect2Height);
                    canvas.Fill(); // Закрашиваем прямоугольник
                    canvas.RestoreState();

                    // === НОВОЕ: Красный прямоугольник для зоны считывания ===
                    canvas.SaveState();
                    canvas.SetStrokeColorRgb(1.0f, 0.0f, 0.0f); // RGB для красного цвета (обводка)
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);   // RGB для белого цвета (заливка)
                    canvas.SetLineWidth(1); // Толщина линии
                    // Используем рассчитанные абсолютные координаты
                    canvas.Rectangle(readAreaX_Pdf, readAreaY_Pdf, readAreaWidth_Pdf, readAreaHeight_Pdf);
                    canvas.FillStroke(); // Закрашиваем и рисуем обводку
                    canvas.RestoreState();

                    LogMessage("  Прямоугольники нарисованы (включая красный для зоны считывания).");
                    // === КОНЕЦ: Отрисовка прямоугольников ===

                    // === НОВОЕ: Считывание текста из области ===
                    string readText = "";
                    try
                    {
                        LogMessage($"  Попытка считывания текста из области: X={readAreaX_Pdf:F1}, Y={readAreaY_Pdf:F1}, W={readAreaWidth_Pdf:F1}, H={readAreaHeight_Pdf:F1}");

                        // Создаем прямоугольник для области интереса
                        IT_Rectangle region = new IT_Rectangle(readAreaX_Pdf, readAreaY_Pdf, readAreaWidth_Pdf, readAreaHeight_Pdf); // Используем псевдоним

                        // Используем LocationTextExtractionStrategy для извлечения текста
                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();

                        // Извлекаем текст из страницы
                        string allText = PdfTextExtractor.GetTextFromPage(page, strategy);

                        // Простой поиск ключевых слов в извлеченном тексте
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
                        LogMessage($"  Считанный текст (примитивный поиск): '{readText}'");
                    }
                    catch (Exception readEx)
                    {
                        LogMessage($"  Ошибка считывания текста: {readEx.Message}");
                        readText = "";
                    }
                    // === КОНЕЦ: Считывание текста из области ===

                    // === НОВОЕ: Определение пути к изображению 3 на основе считанного текста ===
                    string imagePath3_Selected = "";
                    try
                    {
                        if (readText.Equals("Старцев", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt1; // Подп002.tif
                            LogMessage($"  Определено изображение для вставки: Подп002.tif (по слову 'Старцев')");
                        }
                        else if (readText.Equals("Максимов", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Default; // Подп003.tif
                            LogMessage($"  Определено изображение для вставки: Подп003.tif (по слову 'Максимов')");
                        }
                        else if (readText.Equals("Тихомиров", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt2; // Подп006.tif
                            LogMessage($"  Определено изображение для вставки: Подп006.tif (по слову 'Тихомиров')");
                        }
                        else if (readText.Equals("Седюк", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt3; // Подп005.tif
                            LogMessage($"  Определено изображение для вставки: Подп005.tif (по слову 'Седюк')");
                        }
                        else if (readText.Equals("Русских", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt4; // Подп004.tif
                            LogMessage($"  Определено изображение для вставки: Подп004.tif (по слову 'Русских')");
                        }
                        else
                        {
                            imagePath3_Selected = "";
                            if (string.IsNullOrEmpty(readText))
                            {
                                LogMessage($"  Текст не считан или пуст. Изображение 3 вставлено не будет.");
                            }
                            else
                            {
                                LogMessage($"  Считано неизвестное слово '{readText}'. Изображение 3 вставлено не будет.");
                            }
                        }
                    }
                    catch (Exception selectEx)
                    {
                        LogMessage($"  Ошибка определения изображения: {selectEx.Message}");
                        imagePath3_Selected = "";
                    }
                    // === КОНЕЦ: Определение пути к изображению 3 ===

                    // 6. === Создаем новый PdfCanvas для рисования текста ===
                    PdfCanvas textCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // 7. === Загружаем шрифт ===
                    PdfFont font;
                    try
                    {
                        if (!string.IsNullOrEmpty(fontPath) && System.IO.File.Exists(fontPath))
                        {
                            byte[] fontBytes = System.IO.File.ReadAllBytes(fontPath);
                            font = PdfFontFactory.CreateFont(fontBytes, "Identity-H");
                            LogMessage($"  Шрифт загружен: {SysPath.GetFileName(fontPath)}");
                        }
                        else
                        {
                            font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
                            LogMessage($"  Шрифт не найден. Используется Helvetica-Oblique.");
                        }
                    }
                    catch (Exception fontEx)
                    {
                        font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
                        LogMessage($"  Ошибка загрузки шрифта: {fontEx.Message}. Используется Helvetica-Oblique.");
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
                    textCanvas.SetFontAndSize(font, FontSize_Date);
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

                    // === НОВОЕ: РазработалДата ===
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SixthFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(sixthTextX_Pdf, sixthTextY_Pdf);
                    textCanvas.ShowText(dateForRazrabotal);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    LogMessage("  Текстовые элементы вставлены.");

                    // 9. === Вставляем изображения ===
                    // Изображение 1
                    try
                    {
                        if (System.IO.File.Exists(ImagePath1))
                        {
                            IT_ImageData imageData1 = IT_ImageDataFactory.Create(ImagePath1); // Используем псевдонимы
                            if (imageData1 != null)
                            {
                                IT_Image imageElement1 = new IT_Image(imageData1); // Используем псевдоним
                                imageElement1.SetFixedPosition(image1X_Pdf, image1Y_Pdf);
                                imageElement1.SetWidth(Image1_TargetWidth);
                                imageElement1.SetHeight(Image1_TargetHeight);

                                Canvas layoutCanvas1 = new Canvas(page, page.GetPageSize());
                                layoutCanvas1.Add(imageElement1);
                                layoutCanvas1.Close();
                            }
                            else
                            {
                                LogMessage("  Ошибка: Не удалось загрузить данные изображения 1.");
                            }
                        }
                        else
                        {
                            LogMessage($"  Файл изображения 1 не найден: {SysPath.GetFileName(ImagePath1)}");
                        }
                    }
                    catch (Exception imgEx)
                    {
                        LogMessage($"  Ошибка вставки изображения 1: {imgEx.Message}");
                    }

                    // Изображение 2
                    try
                    {
                        if (System.IO.File.Exists(ImagePath2))
                        {
                            IT_ImageData imageData2 = IT_ImageDataFactory.Create(ImagePath2); // Используем псевдонимы
                            if (imageData2 != null)
                            {
                                IT_Image imageElement2 = new IT_Image(imageData2); // Используем псевдоним
                                imageElement2.SetFixedPosition(image2X_Pdf, image2Y_Pdf);
                                imageElement2.SetWidth(Image2_TargetWidth);
                                imageElement2.SetHeight(Image2_TargetHeight);

                                Canvas layoutCanvas2 = new Canvas(page, page.GetPageSize());
                                layoutCanvas2.Add(imageElement2);
                                layoutCanvas2.Close();
                            }
                            else
                            {
                                LogMessage("  Ошибка: Не удалось загрузить данные изображения 2.");
                            }
                        }
                        else
                        {
                            LogMessage($"  Файл изображения 2 не найден: {SysPath.GetFileName(ImagePath2)}");
                        }
                    }
                    catch (Exception imgEx)
                    {
                        LogMessage($"  Ошибка вставки изображения 2: {imgEx.Message}");
                    }

                    // === НОВОЕ: Изображение 3 (с условной логикой) ===
                    if (!string.IsNullOrEmpty(imagePath3_Selected) && System.IO.File.Exists(imagePath3_Selected))
                    {
                        try
                        {
                            IT_ImageData imageData3 = IT_ImageDataFactory.Create(imagePath3_Selected); // Используем псевдонимы
                            if (imageData3 != null)
                            {
                                IT_Image imageElement3 = new IT_Image(imageData3); // Используем псевдоним
                                imageElement3.SetFixedPosition(image3X_Pdf, image3Y_Pdf);
                                imageElement3.SetWidth(Image3_TargetWidth);
                                imageElement3.SetHeight(Image3_TargetHeight);

                                Canvas layoutCanvas3 = new Canvas(page, page.GetPageSize());
                                layoutCanvas3.Add(imageElement3);
                                layoutCanvas3.Close();

                                LogMessage($"  Изображение 3 вставлено: {SysPath.GetFileName(imagePath3_Selected)}");
                            }
                            else
                            {
                                LogMessage("  Ошибка: Не удалось загрузить данные изображения 3.");
                            }
                        }
                        catch (Exception imgEx)
                        {
                            LogMessage($"  Ошибка вставки изображения 3: {imgEx.Message}");
                        }
                    }
                    else if (!string.IsNullOrEmpty(imagePath3_Selected))
                    {
                        LogMessage($"  Файл изображения 3 не найден: {SysPath.GetFileName(imagePath3_Selected)}");
                    }
                    // Если imagePath3_Selected пустая строка, ничего не делаем - изображение не вставляется

                    LogMessage("  Обработка изображений завершена.");
                }

                LogMessage($"  Файл сохранен как: {SysPath.GetFileName(outputPdfPath)}");
                return true;

            }
            catch (Exception ex)
            {
                LogMessage($"  Ошибка обработки файла '{SysPath.GetFileName(inputPdfPath)}': {ex.Message}");
                return false;
            }
            finally
            {
                // Всегда включаем кнопку обратно
                button1.Enabled = true;
                UpdateStatus("Обработка закончена.");
            }
        }
    }

    /// <summary>
    /// Пользовательский слушатель событий для извлечения текста с координатами
    /// ИСПРАВЛЕНО: Для работы с iText 9 и LineSegment
    /// </summary>
    public class CustomTextEventListener : IEventListener
    {
        private readonly List<TextItem> _textItems = new List<TextItem>();

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_TEXT)
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                string text = renderInfo.GetText();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        IT_Rectangle textRect = null; // Используем псевдоним

                        try
                        {
                            // === ИСПРАВЛЕНО: CS1061 - правильный способ получения ограничивающего прямоугольника в iText 9 ===
                            // Способ 1: Используем GetDescentLine().GetBoundingRectangle()
                            // Это стандартный способ в iText 7/9
                            var descentLine = renderInfo.GetDescentLine();
                            if (descentLine != null)
                            {
                                textRect = descentLine.GetBoundingRectangle(); // <-- ПРАВИЛЬНЫЙ МЕТОД
                            }
                        }
                        catch (MissingMethodException)
                        {
                            // Если GetBoundingRectangle() недоступен, попробуем другие методы
                            try
                            {
                                // Способ 2: Попробуем получить координаты из StartPoint и EndPoint
                                var descentLine = renderInfo.GetDescentLine();
                                if (descentLine != null)
                                {
                                    var startPoint = descentLine.GetStartPoint();
                                    var endPoint = descentLine.GetEndPoint();

                                    if (startPoint != null && endPoint != null)
                                    {
                                        // Получаем координаты точек
                                        float x1 = startPoint.Get(0);
                                        float y1 = startPoint.Get(1);
                                        float x2 = endPoint.Get(0);
                                        float y2 = endPoint.Get(1);

                                        // Вычисляем bounding box
                                        float minX = Math.Min(x1, x2);
                                        float maxX = Math.Max(x1, x2);
                                        float minY = Math.Min(y1, y2);
                                        float maxY = Math.Max(y1, y2);

                                        // Простая оценка ширины и высоты
                                        float width = maxX - minX;
                                        float height = maxY - minY;

                                        // Если высота слишком маленькая, используем эвристическое значение
                                        if (height <= 0)
                                        {
                                            // Примерная высота символа (очень грубая оценка)
                                            height = 10f;
                                            minY = y1 - height / 2;
                                            maxY = y1 + height / 2;
                                        }

                                        textRect = new IT_Rectangle(minX, minY, width, height); // Используем псевдоним
                                    }
                                }
                            }
                            catch
                            {
                                // Игнорируем ошибки второго способа
                            }
                        }
                        catch
                        {
                            // Игнорируем ошибки первого способа
                        }

                        // Если все еще не получили валидный прямоугольник, создаем минимальный для отладки
                        if (textRect == null || (textRect.GetWidth() <= 0 && textRect.GetHeight() <= 0))
                        {
                            // Последний резорт: очень грубая оценка
                            var descentLine = renderInfo.GetDescentLine();
                            if (descentLine != null)
                            {
                                var startPoint = descentLine.GetStartPoint();
                                if (startPoint != null)
                                {
                                    float x = startPoint.Get(0);
                                    float y = startPoint.Get(1);
                                    // Примерная ширина и высота (очень грубая оценка)
                                    float estimatedWidth = text.Length * 6f; // ~6 точек на символ
                                    float estimatedHeight = 10f; // Примерная высота символа
                                    textRect = new IT_Rectangle(x, y - estimatedHeight / 2, estimatedWidth, estimatedHeight); // Используем псевдоним
                                }
                            }
                        }

                        if (textRect != null && (textRect.GetWidth() > 0 || textRect.GetHeight() > 0))
                        {
                            _textItems.Add(new TextItem(text, textRect));
                        }
                        else
                        {
                            // Даже если прямоугольник не валиден, добавляем текст без координат для отладки
                            // _textItems.Add(new TextItem(text, new IT_Rectangle(0, 0, 0, 0))); // Используем псевдоним
                            // Или просто игнорируем
                        }
                    }
                    catch (Exception /*ex*/) // Переменная ex убрана, так как не используется напрямую
                    {
                        // Игнорируем ошибки для отдельных элементов текста
                        // LogMessage($"Предупреждение при обработке текста '{text}': {ex.Message}");
                    }
                }
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return new HashSet<EventType> { EventType.RENDER_TEXT };
        }

        /// <summary>
        /// Находит ВСЕ вхождения указанного текста
        /// </summary>
        public List<TextItem> FindAllText(string searchText)
        {
            List<TextItem> results = new List<TextItem>();
            foreach (var item in _textItems)
            {
                // ИСПРАВЛЕНО: CS1501 - правильное использование IndexOf с StringComparison
                if (item.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// Находит первое вхождение указанного текста
        /// </summary>
        public TextItem FindText(string searchText)
        {
            var allMatches = FindAllText(searchText);
            return allMatches.Count > 0 ? allMatches[0] : null;
        }

        /// <summary>
        /// Возвращает все найденные текстовые элементы
        /// </summary>
        public List<TextItem> GetAllTextItems()
        {
            return new List<TextItem>(_textItems);
        }
    }

    /// <summary>
    /// Представляет текстовый элемент с его координатами
    /// </summary>
    public class TextItem
    {
        public string Text { get; }
        public IT_Rectangle Rect { get; } // Используем псевдоним

        public TextItem(string text, IT_Rectangle rect) // Используем псевдоним
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Rect = rect ?? throw new ArgumentNullException(nameof(rect));
        }
    }
}