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
// === PdfSharp ===
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
// === Для работы с изображениями ===
using PdfSharp.Pdf.Advanced;
// === Для кодировок ===
using PdfSharp.Pdf.Security;
// === Для шрифтов ===
using PdfSharp.Fonts;
// === Для геометрии ===
using PdfSharp.Pdf.Annotations;
// === Для извлечения текста ===
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Content.Objects;
using System.Drawing.Text;

namespace DocumentMark
{
    public partial class Form1 : Form
    {
        // === Настройки по умолчанию ===
        private const string DefaultInputPdfFolderPath = @"C:\PDF\1\Rename";
        private const string DefaultFontPath = @"C:\Users\maksimov\AppData\Local\Microsoft\Windows\Fonts\GOST type A Italic.ttf";

        // === Настройки относительно опорной точки "Разраб." ===
        private const float Text_OffsetFromReferenceX = 55f;  // Смещение вправо от опорной точки
        private const float Text_OffsetFromReferenceY = 0f;    // Смещение вверх от опорной точки
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
                UpdateStatus($"Выбрана папка: {Path.GetFileName(fbd.SelectedPath)}");
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
                UpdateStatus($"Выбран шрифт: {Path.GetFileName(ofd.FileName)}");
                LogMessage($"Выбран шрифт: {Path.GetFileName(ofd.FileName)}");
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
                    UpdateStatus("Ошибка: Неверный формат даты передачи документов в textBox1. Используется текущая дата.");
                    LogMessage("Ошибка: Неверный формат даты передачи документов в textBox1. Используется текущая дата.");
                    datePeredachi = DateTime.Now;
                }

                if (string.IsNullOrEmpty(pdfFolderPath) || !Directory.Exists(pdfFolderPath))
                {
                    UpdateStatus($"Ошибка: Папка не найдена или путь пуст: '{pdfFolderPath}'");
                    LogMessage($"Ошибка: Папка не найдена или путь пуст: '{pdfFolderPath}'");
                    MessageBox.Show($"Папка не найдена или путь пуст: '{pdfFolderPath}'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] pdfFiles = Directory.GetFiles(pdfFolderPath, "*.pdf", SearchOption.TopDirectoryOnly);
                if (pdfFiles.Length == 0)
                {
                    UpdateStatus("Ошибка: В папке не найдено PDF файлов.");
                    LogMessage("Ошибка: В папке не найдено PDF файлов.");
                    MessageBox.Show($"В папке не найдено PDF файлов: {pdfFolderPath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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

                        UpdateStatus($"Обработка файла: {Path.GetFileName(inputPdfPath)}...");
                        LogMessage($"Обработка файла: {Path.GetFileName(inputPdfPath)}...");
                        if (ProcessSinglePdf(inputPdfPath, fontPath, nachBuroDateString, utverdilDateString, razrabotalDateString))
                        {
                            successCount++;
                            UpdateStatus($"Файл {Path.GetFileName(inputPdfPath)} успешно обработан.");
                            LogMessage($"Файл {Path.GetFileName(inputPdfPath)} успешно обработан.");
                        }
                        else
                        {
                            errorCount++;
                            UpdateStatus($"Ошибка при обработке файла {Path.GetFileName(inputPdfPath)}.");
                            LogMessage($"Ошибка при обработке файла {Path.GetFileName(inputPdfPath)}.");
                        }
                    }
                    catch (Exception fileEx)
                    {
                        errorCount++;
                        UpdateStatus($"Критическая ошибка при обработке файла {Path.GetFileName(inputPdfPath)}: {fileEx.Message}");
                        LogMessage($"Критическая ошибка при обработке файла {Path.GetFileName(inputPdfPath)}: {fileEx.Message}");
                    }
                }

                UpdateStatus($"Обработка завершена. Успешно: {successCount}, Ошибок: {errorCount}.");
                LogMessage($"Обработка завершена. Успешно: {successCount}, Ошибок: {errorCount}.");
                MessageBox.Show($"Обработка завершена.\nУспешно обработано: {successCount}\nОшибок: {errorCount}", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка в процессе обработки: {ex.Message}");
                LogMessage($"Ошибка в процессе обработки: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonProcessAll.Enabled = true;
                UpdateStatus("Обработка закончена.");
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
        /// Обрабатывает один PDF файл с новой логикой позиционирования относительно найденного текста "Разраб."
        /// </summary>
        private bool ProcessSinglePdf(string inputPdfPath, string fontPath, string dateForNachBuro, string dateForUtverdil, string dateForRazrabotal)
        {
            try
            {
                string outputPdfPath = Path.Combine(
                    Path.GetDirectoryName(inputPdfPath),
                    Path.GetFileNameWithoutExtension(inputPdfPath) + "_edited.pdf"
                );

                // === PdfSharp: Открываем PDF для редактирования ===
                PdfDocument pdfDoc = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify);
                PdfPage page = pdfDoc.Pages[0]; // Работаем с первой страницей
                if (page == null)
                {
                    LogMessage($"  Ошибка: Не удалось получить первую страницу PDF '{Path.GetFileName(inputPdfPath)}'.");
                    return false;
                }

                var pageSize = page.MediaBox;
                float pageWidth = pageSize.Width;
                float pageHeight = pageSize.Height;
                LogMessage($"  Размер страницы: {pageWidth:F1} x {pageHeight:F1} точек");

                // === Поиск опорной точки "Разраб." ===
                float referenceX = 0;
                float referenceY = 0;
                bool referenceFound = false;

                try
                {
                    LogMessage("  Поиск опорной точки 'Разраб.'...");

                    // === PdfSharp: Извлекаем текст с координатами ===
                    // PdfSharp не предоставляет прямого API для извлечения текста с координатами
                    // Поэтому используем обходной путь: открываем PDF как текстовый файл и ищем "Разраб."
                    // Это не идеально, но может сработать для простых случаев

                    // string pdfText = PdfReader.ExtractText(pdfDoc); // <-- ОШИБКА CS0117
                    // ИСПРАВЛЕНО: CS0117 - используем PdfTextExtractor
                    string pdfText = PdfTextExtractor.GetTextFromPage(page); // <-- ПРАВИЛЬНЫЙ МЕТОД

                    if (!string.IsNullOrEmpty(pdfText))
                    {
                        string[] lines = pdfText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            if (line.IndexOf("Разраб.", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // Найдено слово "Разраб."
                                // В PdfSharp нет прямого способа получить координаты текста
                                // Поэтому используем заранее определенные координаты
                                referenceX = -361f; // Пример, подберите
                                referenceY = -506f; // Пример, подберите
                                referenceFound = true;
                                LogMessage($"  Опорная точка 'Разраб.' найдена (по заранее определенным координатам): X={referenceX:F2}, Y={referenceY:F2}");
                                break;
                            }
                        }
                    }

                    if (!referenceFound)
                    {
                        LogMessage("  Опорная точка 'Разраб.' НЕ НАЙДЕНА. Используются координаты (0, 0) по умолчанию.");
                    }
                }
                catch (Exception refEx)
                {
                    LogMessage($"  Ошибка поиска опорной точки 'Разраб.': {refEx.Message}. Используются координаты (0, 0) по умолчанию.");
                }
                // === КОНЕЦ: Поиск опорной точки ===

                // === Настройки относительно опорной точки "Разраб." ===
                // === Координаты: Смещение ОТ опорной точки (referenceX, referenceY) ===

                // --- "Утвердил" ---
                const float Text_OffsetFromReferenceX = 55f;  // Смещение вправо от опорной точки
                const float Text_OffsetFromReferenceY = 0f;    // Смещение вверх от опорной точки
                const int FontSize_Main = 16;

                // --- "УтвердилДата" ---
                const float SecondText_OffsetFromReferenceX = 159f;
                const float SecondText_OffsetFromReferenceY = -2f;
                const int SecondFontSize_Date = 8;

                // --- "Нач.бюро" ---
                const float ThirdText_OffsetFromReferenceX = 55f;
                const float ThirdText_OffsetFromReferenceY = 28f;
                const int ThirdFontSize_Main = 16;

                // --- "Нач.БюроДата" ---
                const float FourthText_OffsetFromReferenceX = 159f;
                const float FourthText_OffsetFromReferenceY = 26f;
                const int FourthFontSize_Date = 8;

                // --- "РазработалДата" ---
                const float SixthText_OffsetFromReferenceX = 159f;
                const float SixthText_OffsetFromReferenceY = 73f;
                const int SixthFontSize_Date = 8;

                // --- Изображение 1 (Подп001.tif) ---
                const float Image1_OffsetFromReferenceX = 90f;
                const float Image1_OffsetFromReferenceY = -15f;
                const float Image1_TargetWidth = 90f;
                const float Image1_TargetHeight = 40f;
                const string ImagePath1 = @"C:\PDF\1\Rename\Подп001.tif";

                // --- Изображение 2 (Подп002.tif) ---
                const float Image2_OffsetFromReferenceX = 105f;
                const float Image2_OffsetFromReferenceY = 15f;
                const float Image2_TargetWidth = 80f;
                const float Image2_TargetHeight = 50f;
                const string ImagePath2 = @"C:\PDF\1\Rename\Подп002.tif";

                // --- Изображение 3 (Подп003.tif и альтернативы) ---
                const float Image3_OffsetFromReferenceX = 105f; // Пример, подберите
                const float Image3_OffsetFromReferenceY = 62f;  // Пример, подберите
                const float Image3_TargetWidth = 80f;          // Пример, подберите
                const float Image3_TargetHeight = 50f;         // Пример, подберите
                const string ImagePath3_Default = @"C:\PDF\1\Rename\Подп003.tif";
                const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\Подп002.tif";
                const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\Подп006.tif";
                const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\Подп005.tif";
                const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\Подп004.tif";

                // --- Зона считывания текста ---
                const float ReadArea_OffsetFromReferenceX = 55f;    // Пример, подберите (относительно опорной точки "Разраб.")
                const float ReadArea_OffsetFromReferenceY = 73f;   // Пример, подберите (относительно опорной точки "Разраб.")
                const float ReadArea_Width = 100f;                 // Пример, подберите
                const float ReadArea_Height = 15f;                // Пример, подберите

                // === Расчет АБСОЛЮТНЫХ координат относительно опорной точки ===

                // --- Расчет АБСОЛЮТНЫХ координат X (от левого края) ---
                float textX_Pdf = referenceX + Text_OffsetFromReferenceX;
                float secondTextX_Pdf = referenceX + SecondText_OffsetFromReferenceX;
                float thirdTextX_Pdf = referenceX + ThirdText_OffsetFromReferenceX;
                float fourthTextX_Pdf = referenceX + FourthText_OffsetFromReferenceX;
                float sixthTextX_Pdf = referenceX + SixthText_OffsetFromReferenceX;
                float image1X_Pdf = referenceX + Image1_OffsetFromReferenceX;
                float image2X_Pdf = referenceX + Image2_OffsetFromReferenceX;
                float image3X_Pdf = referenceX + Image3_OffsetFromReferenceX;
                float readAreaX_Pdf = referenceX + ReadArea_OffsetFromReferenceX; // Для зоны считывания

                // --- Расчет АБСОЛЮТНЫХ координат Y (от нижнего края) ---
                // В PDF (0,0) в нижнем левом углу, Y растет вверх
                float textY_Pdf = referenceY + Text_OffsetFromReferenceY;
                float secondTextY_Pdf = referenceY + SecondText_OffsetFromReferenceY;
                float thirdTextY_Pdf = referenceY + ThirdText_OffsetFromReferenceY;
                float fourthTextY_Pdf = referenceY + FourthText_OffsetFromReferenceY;
                float sixthTextY_Pdf = referenceY + SixthText_OffsetFromReferenceY;
                float image1Y_Pdf = referenceY + Image1_OffsetFromReferenceY;
                float image2Y_Pdf = referenceY + Image2_OffsetFromReferenceY;
                float image3Y_Pdf = referenceY + Image3_OffsetFromReferenceY;
                float readAreaY_Pdf = referenceY + ReadArea_OffsetFromReferenceY; // Для зоны считывания

                // --- Размеры для зоны считывания ---
                float readAreaWidth_Pdf = ReadArea_Width;
                float readAreaHeight_Pdf = ReadArea_Height;

                // 5. === Создаем XGraphics для рисования прямоугольников ===
                XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);

                // === НАЧАЛО: Отрисовка прямоугольников ===
                // Прямоугольник 1 
                float rect1X = referenceX + 106f; // Пример, замените на нужные относительные координаты
                float rect1Y = referenceY + 13f; // Пример, замените на нужные относительные координаты
                float rect1Width = 27f;
                float rect1Height = 13f;

                // Прямоугольник 2 
                float rect2X = referenceX + 135.4f; // Пример, замените на нужные относительные координаты
                float rect2Y = referenceY + 13f;   // Пример, замените на нужные относительные координаты
                float rect2Width = 5f;
                float rect2Height = 13f;

                // Рисуем первый прямоугольник (белый, закрашенный)
                gfx.SaveState();
                gfx.DrawRectangle(XBrushes.White, rect1X, rect1Y, rect1Width, rect1Height);
                gfx.RestoreState();

                // Рисуем второй прямоугольник (белый, закрашенный)
                gfx.SaveState();
                gfx.DrawRectangle(XBrushes.White, rect2X, rect2Y, rect2Width, rect2Height);
                gfx.RestoreState();

                // === НОВОЕ: Красный прямоугольник для зоны считывания ===
                gfx.SaveState();
                XPen redPen = new XPen(XColor.FromArgb(255, 0, 0), 1); // Красная обводка
                XBrush whiteBrush = XBrushes.White; // Белая заливка
                // Используем рассчитанные абсолютные координаты
                gfx.DrawRectangle(whiteBrush, redPen, readAreaX_Pdf, readAreaY_Pdf, readAreaWidth_Pdf, readAreaHeight_Pdf);
                gfx.RestoreState();

                LogMessage("  Прямоугольники нарисованы (включая красный для зоны считывания).");
                // === КОНЕЦ: Отрисовка прямоугольников ===

                // === НОВОЕ: Считывание текста из области ===
                string readText = "";
                try
                {
                    LogMessage($"  Попытка считывания текста из области: X={readAreaX_Pdf:F1}, Y={readAreaY_Pdf:F1}, W={readAreaWidth_Pdf:F1}, H={readAreaHeight_Pdf:F1}");

                    // === PdfSharp: Извлекаем текст с координатами ===
                    // PdfSharp не предоставляет прямого API для извлечения текста с координатами
                    // Поэтому используем обходной путь: открываем PDF как текстовый файл и ищем ключевые слова

                    // string pdfText = PdfReader.ExtractText(pdfDoc); // <-- ОШИБКА CS0117
                    // ИСПРАВЛЕНО: CS0117 - используем PdfTextExtractor
                    string pdfText = PdfTextExtractor.GetTextFromPage(page); // <-- ПРАВИЛЬНЫЙ МЕТОД

                    if (!string.IsNullOrEmpty(pdfText))
                    {
                        string[] lines = pdfText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
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

                // 6. === Создаем новый XGraphics для рисования текста ===
                XGraphics textGfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);

                // 7. === Загружаем шрифт ===
                XFont font;
                try
                {
                    if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
                    {
                        // === PdfSharp: Загружаем шрифт ===
                        // PdfSharp может загружать системные шрифты или шрифты из файлов
                        // Для пользовательских шрифтов используем XPrivateFontCollection
                        XPrivateFontCollection privateFonts = new XPrivateFontCollection();
                        privateFonts.Add(fontPath);
                        font = new XFont(privateFonts.Families[0], FontSize_Main, XFontStyle.Italic); // Курсив
                        LogMessage($"  Шрифт загружен: {Path.GetFileName(fontPath)}");
                    }
                    else
                    {
                        // === PdfSharp: Используем стандартный шрифт ===
                        font = new XFont("Helvetica", FontSize_Main, XFontStyle.Italic); // Курсив
                        LogMessage($"  Шрифт не найден. Используется Helvetica (курсив).");
                    }
                }
                catch (Exception fontEx)
                {
                    font = new XFont("Helvetica", FontSize_Main, XFontStyle.Italic); // Курсив
                    LogMessage($"  Ошибка загрузки шрифта: {fontEx.Message}. Используется Helvetica (курсив).");
                }

                // 8. === Вставляем текстовые элементы ===
                // Утвердил
                textGfx.Save();
                textGfx.DrawString("Утвердил", font, XBrushes.Black, textX_Pdf, textY_Pdf);
                textGfx.Restore();

                // УтвердилДата
                textGfx.Save();
                textGfx.DrawString(dateForUtverdil, font, XBrushes.Black, secondTextX_Pdf, secondTextY_Pdf);
                textGfx.Restore();

                // Нач.бюро
                textGfx.Save();
                textGfx.DrawString("Нач.бюро", font, XBrushes.Black, thirdTextX_Pdf, thirdTextY_Pdf);
                textGfx.Restore();

                // Нач.БюроДата
                textGfx.Save();
                textGfx.DrawString(dateForNachBuro, font, XBrushes.Black, fourthTextX_Pdf, fourthTextY_Pdf);
                textGfx.Restore();

                // РазработалДата
                textGfx.Save();
                textGfx.DrawString(dateForRazrabotal, font, XBrushes.Black, sixthTextX_Pdf, sixthTextY_Pdf);
                textGfx.Restore();

                LogMessage("  Текстовые элементы вставлены.");

                // 9. === Вставляем изображения ===
                // Изображение 1
                try
                {
                    if (File.Exists(ImagePath1))
                    {
                        // === PdfSharp: Загружаем изображение ===
                        XImage image1 = XImage.FromFile(ImagePath1);
                        if (image1 != null)
                        {
                            // === PdfSharp: Вставляем изображение ===
                            textGfx.Save();
                            textGfx.DrawImage(image1, image1X_Pdf, image1Y_Pdf, Image1_TargetWidth, Image1_TargetHeight);
                            textGfx.Restore();
                            LogMessage($"  Изображение 1 вставлено: {Path.GetFileName(ImagePath1)}");
                        }
                        else
                        {
                            LogMessage("  Ошибка: Не удалось загрузить данные изображения 1.");
                        }
                    }
                    else
                    {
                        LogMessage($"  Файл изображения 1 не найден: {Path.GetFileName(ImagePath1)}");
                    }
                }
                catch (Exception imgEx)
                {
                    LogMessage($"  Ошибка вставки изображения 1: {imgEx.Message}");
                }

                // Изображение 2
                try
                {
                    if (File.Exists(ImagePath2))
                    {
                        // === PdfSharp: Загружаем изображение ===
                        XImage image2 = XImage.FromFile(ImagePath2);
                        if (image2 != null)
                        {
                            // === PdfSharp: Вставляем изображение ===
                            textGfx.Save();
                            textGfx.DrawImage(image2, image2X_Pdf, image2Y_Pdf, Image2_TargetWidth, Image2_TargetHeight);
                            textGfx.Restore();
                            LogMessage($"  Изображение 2 вставлено: {Path.GetFileName(ImagePath2)}");
                        }
                        else
                        {
                            LogMessage("  Ошибка: Не удалось загрузить данные изображения 2.");
                        }
                    }
                    else
                    {
                        LogMessage($"  Файл изображения 2 не найден: {Path.GetFileName(ImagePath2)}");
                    }
                }
                catch (Exception imgEx)
                {
                    LogMessage($"  Ошибка вставки изображения 2: {imgEx.Message}");
                }

                // === НОВОЕ: Изображение 3 (с условной логикой) ===
                if (!string.IsNullOrEmpty(imagePath3_Selected) && File.Exists(imagePath3_Selected))
                {
                    try
                    {
                        // === PdfSharp: Загружаем изображение ===
                        XImage image3 = XImage.FromFile(imagePath3_Selected);
                        if (image3 != null)
                        {
                            // === PdfSharp: Вставляем изображение ===
                            textGfx.Save();
                            textGfx.DrawImage(image3, image3X_Pdf, image3Y_Pdf, Image3_TargetWidth, Image3_TargetHeight);
                            textGfx.Restore();
                            LogMessage($"  Изображение 3 вставлено: {Path.GetFileName(imagePath3_Selected)}");
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
                    LogMessage($"  Файл изображения 3 не найден: {Path.GetFileName(imagePath3_Selected)}");
                }
                // Если imagePath3_Selected пустая строка, ничего не делаем - изображение не вставляется

                LogMessage("  Обработка изображений завершена.");

                // === Сохраняем PDF ===
                pdfDoc.Save(outputPdfPath);
                pdfDoc.Close();
                LogMessage($"  Файл сохранен как: {Path.GetFileName(outputPdfPath)}");
                return true;

            }
            catch (Exception ex)
            {
                LogMessage($"  Ошибка обработки файла '{Path.GetFileName(inputPdfPath)}': {ex.Message}");
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
}