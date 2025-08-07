using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
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
// === Для фильтрации событий (если доступно) ===
// using iText.Kernel.Pdf.Canvas.Parser.Filter;

namespace DocumentMark
{
    public partial class Form1 : Form
    {
        // === Настройки по умолчанию ===
        private const string DefaultInputPdfFolderPath = @"C:\PDF\1\Rename";
        private const string DefaultFontPath = @"C:\Users\maksimov\AppData\Local\Microsoft\Windows\Fonts\GOST type A Italic.ttf";

        // === Коэффициенты масштабирования из A4 в A3 ===
        private const float A4_TO_A3_SCALE_X = 1.414f;
        private const float A4_TO_A3_SCALE_Y = 1.414f;

        // === Настройки для A4 ===
        private const string TextToInsert_A4 = "Утвердил";
        private const float TextX_A4 = -361f;
        private const float TextY_A4 = -506f;
        private const int FontSize_A4 = 16;

        private const float SecondTextX_A4 = -257f; // Координаты для "УтвердилДата"
        private const float SecondTextY_A4 = -504f;
        private const int SecondFontSize_A4 = 8; // Размер шрифта для дат

        private const string ThirdTextToInsert_A4 = "Нач.бюро";
        private const float ThirdTextX_A4 = -361f;
        private const float ThirdTextY_A4 = -478f;
        private const int ThirdFontSize_A4 = 16;

        private const float FourthTextX_A4 = -257f; // Координаты для "Нач.БюроДата"
        private const float FourthTextY_A4 = -476f;
        private const int FourthFontSize_A4 = 8; // Размер шрифта для дат

        // === Настройки для "РазработалДата" ===
        private const float SixthTextX_A4 = -257f; // Координаты для "РазработалДата"
        private const float SixthTextY_A4 = -433f;
        private const int SixthFontSize_A4 = 8; // Размер шрифта для даты

        private const string ImagePath1_A4 = @"C:\PDF\1\Rename\Подп001.tif";
        private const float Image1X_A4 = -320f;
        private const float Image1Y_A4 = -515f;
        private const float Image1TargetWidth_A4 = 90f;
        private const float Image1TargetHeight_A4 = 40f;

        private const string ImagePath2_A4 = @"C:\PDF\1\Rename\Подп002.tif";
        private const float Image2X_A4 = -305f;
        private const float Image2Y_A4 = -485f;
        private const float Image2TargetWidth_A4 = 80f;
        private const float Image2TargetHeight_A4 = 50f;

        // === Настройки для третьей картинки "Подп003.tif" и её альтернатив ===
        private const string ImagePath3_Default = @"C:\PDF\1\Rename\Подп003.tif";
        private const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\Подп002.tif";
        private const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\Подп006.tif";
        private const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\Подп005.tif";
        private const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\Подп004.tif";
        //private const float Image3X_A4 = -305f; // Примерные координаты, нужно уточнить
        //private const float Image3Y_A4 = -442f; // Примерные координаты, нужно уточнить
        private const float Image3X_A4 = -305f; // Примерные координаты, нужно уточнить
        private const float Image3Y_A4 = -446f; // Примерные координаты, нужно уточнить
        private const float Image3TargetWidth_A4 = 80f; // Примерные размеры, нужно уточнить
        private const float Image3TargetHeight_A4 = 50f; // Примерные размеры, нужно уточнить

        public Form1()
        {
            InitializeComponent();
            // Инициализируем поля ввода значениями по умолчанию
            textBoxPdfFolderPath.Text = DefaultInputPdfFolderPath;
            textBoxFontPath.Text = DefaultFontPath;

            // Инициализируем textBox1 значением по умолчанию
            textBox1.Text = "01.07.2019";

            LogMessage("Приложение готово.");
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
                LogMessage($"Выбрана папка: {fbd.SelectedPath}");
            }
            else
            {
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

            // === ИСПРАВЛЕНО: CS0103 - заменено fbd на ofd ===
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxFontPath.Text = ofd.FileName;
                LogMessage($"Выбран шрифт: {System.IO.Path.GetFileName(ofd.FileName)}");
            }
            else
            {
                LogMessage("Выбор шрифта отменен.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Обработать все PDF"
        /// </summary>
        private void buttonProcessAll_Click(object sender, EventArgs e)
        {
            // Отключаем кнопку на время обработки
            buttonProcessAll.Enabled = false;
            LogMessage("Начало обработки всех PDF файлов...");

            try
            {
                // Получаем пути из текстовых полей
                string pdfFolderPath = textBoxPdfFolderPath.Text.Trim();
                string fontPath = textBoxFontPath.Text.Trim();

                // === Получаем дату передачи документов из textBox1 ===
                string datePeredachiString = textBox1.Text.Trim();

                // Проверка и парсинг даты передачи
                DateTime datePeredachi;
                if (string.IsNullOrEmpty(datePeredachiString) || !DateTime.TryParseExact(datePeredachiString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out datePeredachi))
                {
                    LogMessage("Ошибка: Неверный формат даты передачи документов в textBox1. Используется текущая дата.");
                    datePeredachi = DateTime.Now;
                }

                // 1. Проверяем существование папки
                if (string.IsNullOrEmpty(pdfFolderPath) || !Directory.Exists(pdfFolderPath))
                {
                    LogMessage($"Ошибка: Папка не найдена или путь пуст: '{pdfFolderPath}'");
                    MessageBox.Show($"Папка не найдена или путь пуст: '{pdfFolderPath}'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Ищем все PDF файлы в папке
                string[] pdfFiles = Directory.GetFiles(pdfFolderPath, "*.pdf", SearchOption.TopDirectoryOnly);
                if (pdfFiles.Length == 0)
                {
                    LogMessage("Ошибка: В папке не найдено PDF файлов.");
                    MessageBox.Show($"В папке не найдено PDF файлов: {pdfFolderPath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LogMessage($"Найдено PDF файлов для обработки: {pdfFiles.Length}");

                int successCount = 0;
                int errorCount = 0;

                // 3. Обрабатываем каждый PDF файл
                foreach (string inputPdfPath in pdfFiles)
                {
                    try
                    {
                        // === ГЕНЕРАЦИЯ ДАТ ДЛЯ КАЖДОГО ФАЙЛА ===
                        // 1. "УтвердилДата" - случайная дата от (datePeredachi - 20 дней) до datePeredachi
                        DateTime minUtverdilDate = datePeredachi.AddDays(-20);
                        DateTime maxUtverdilDate = datePeredachi;
                        string utverdilDateString = GetRandomWorkday(minUtverdilDate, maxUtverdilDate);
                        LogMessage($"Сгенерирована 'УтвердилДата': {utverdilDateString}");

                        // 2. "Нач.БюроДата" - случайная дата от (utverdilDate - 10 дней) до utverdilDate
                        DateTime utverdilDate = DateTime.ParseExact(utverdilDateString, "dd.MM.yyyy", null);
                        DateTime minNachBuroDate = utverdilDate.AddDays(-10);
                        DateTime maxNachBuroDate = utverdilDate;
                        string nachBuroDateString = GetRandomWorkday(minNachBuroDate, maxNachBuroDate);
                        LogMessage($"Сгенерирована 'Нач.БюроДата': {nachBuroDateString}");

                        // 3. "РазработалДата" - случайная дата от (nachBuroDate - 50 дней) до nachBuroDate
                        DateTime nachBuroDate = DateTime.ParseExact(nachBuroDateString, "dd.MM.yyyy", null);
                        DateTime minRazrabotalDate = nachBuroDate.AddDays(-50);
                        DateTime maxRazrabotalDate = nachBuroDate;
                        string razrabotalDateString = GetRandomWorkday(minRazrabotalDate, maxRazrabotalDate);
                        LogMessage($"Сгенерирована 'РазработалДата': {razrabotalDateString}");

                        LogMessage($"Обработка файла: {System.IO.Path.GetFileName(inputPdfPath)}...");
                        // Передаем сгенерированные даты в метод обработки
                        if (ProcessSinglePdf(inputPdfPath, fontPath, nachBuroDateString, utverdilDateString, razrabotalDateString))
                        {
                            successCount++;
                            LogMessage($"Файл {System.IO.Path.GetFileName(inputPdfPath)} успешно обработан.");
                        }
                        else
                        {
                            errorCount++;
                            LogMessage($"Ошибка при обработке файла {System.IO.Path.GetFileName(inputPdfPath)}.");
                        }
                    }
                    catch (Exception fileEx)
                    {
                        errorCount++;
                        LogMessage($"Критическая ошибка при обработке файла {System.IO.Path.GetFileName(inputPdfPath)}: {fileEx.Message}");
                    }
                }

                // 4. Итоги
                LogMessage($"Обработка завершена. Успешно: {successCount}, Ошибок: {errorCount}.");
                MessageBox.Show($"Обработка завершена.\nУспешно обработано: {successCount}\nОшибок: {errorCount}", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                LogMessage($"Ошибка в процессе обработки: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Всегда включаем кнопку обратно
                buttonProcessAll.Enabled = true;
                LogMessage("Обработка закончена.");
            }
        }

        /// <summary>
        /// Генерирует случайную рабочую дату (не суббота и не воскресенье) в заданном диапазоне
        /// </summary>
        /// <param name="startDate">Начальная дата диапазона (включительно)</param>
        /// <param name="endDate">Конечная дата диапазона (включительно)</param>
        /// <returns>Строка с датой в формате dd.MM.yyyy</returns>
        private string GetRandomWorkday(DateTime startDate, DateTime endDate)
        {
            Random random = new Random();
            List<DateTime> workdays = new List<DateTime>();

            // Создаем список рабочих дней в диапазоне
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workdays.Add(date);
                }
            }

            // Если рабочих дней нет, возвращаем любую дату из диапазона
            if (workdays.Count == 0)
            {
                TimeSpan diff = endDate - startDate;
                DateTime randomDate = startDate.AddDays(random.Next((int)diff.TotalDays + 1));
                return randomDate.ToString("dd.MM.yyyy");
            }

            // Выбираем случайную рабочую дату
            int randomIndex = random.Next(workdays.Count);
            return workdays[randomIndex].ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Обрабатывает один PDF файл
        /// </summary>
        /// <param name="inputPdfPath">Путь к входному PDF файлу</param>
        /// <param name="fontPath">Путь к файлу шрифта</param>
        /// <param name="dateForNachBuro">Дата для "Нач.бюро" (Нач.БюроДата)</param>
        /// <param name="dateForUtverdil">Дата для "Утвердил" (УтвердилДата)</param>
        /// <param name="dateForRazrabotal">Дата для "Разработал" (РазработалДата)</param>
        /// <returns>True, если успешно, иначе False</returns>
        private bool ProcessSinglePdf(string inputPdfPath, string fontPath, string dateForNachBuro, string dateForUtverdil, string dateForRazrabotal)
        {
            try
            {
                // 1. Определяем путь для выходного файла
                string outputPdfPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(inputPdfPath),
                    System.IO.Path.GetFileNameWithoutExtension(inputPdfPath) + "_edited.pdf"
                );

                // 2. Открываем PDF для редактирования
                using (var reader = new PdfReader(inputPdfPath))
                using (var writer = new PdfWriter(outputPdfPath))
                using (var pdfDoc = new PdfDocument(reader, writer))
                {
                    // Работаем с первой страницей
                    var page = pdfDoc.GetFirstPage();
                    if (page == null)
                    {
                        LogMessage($"  Ошибка: Не удалось получить первую страницу PDF '{System.IO.Path.GetFileName(inputPdfPath)}'.");
                        return false;
                    }

                    // 3. === Определяем формат страницы (A4 или A3) ===
                    var pageSize = page.GetPageSize();
                    float pageWidth = pageSize.GetWidth();
                    float pageHeight = pageSize.GetHeight();

                    bool isA3 = pageWidth > 600;
                    string format = isA3 ? "A3" : "A4";
                    LogMessage($"  Формат страницы: {format} ({pageWidth:F1} x {pageHeight:F1} точек)");

                    // 4. === Рассчитываем координаты в зависимости от формата ===

                    // Фиксированные смещения для перехода от A4 к A3
                    const float offsetX_A3 = 767.5f;
                    const float offsetY_A3 = -266.0f;

                    // Определяем смещения в зависимости от формата
                    float offsetX = isA3 ? offsetX_A3 : 0.0f;
                    float offsetY = isA3 ? offsetY_A3 : 0.0f;

                    // Рассчитываем координаты путем прибавления смещения
                    // Для текста "Утвердил"
                    float textX_Pdf = TextX_A4 + offsetX;
                    float textY_Pdf = TextY_A4 + offsetY;

                    // Для даты "Утвердил" (УтвердилДата)
                    float secondTextX_Pdf = SecondTextX_A4 + offsetX;
                    float secondTextY_Pdf = SecondTextY_A4 + offsetY;

                    // Для текста "Нач.бюро"
                    float thirdTextX_Pdf = ThirdTextX_A4 + offsetX;
                    float thirdTextY_Pdf = ThirdTextY_A4 + offsetY;

                    // Для даты "Нач.бюро" (Нач.БюроДата)
                    float fourthTextX_Pdf = FourthTextX_A4 + offsetX;
                    float fourthTextY_Pdf = FourthTextY_A4 + offsetY;

                    // Для текста "Разработал" (если нужен)
                    // float fifthTextX_Pdf = FifthTextX_A4 + offsetX;
                    // float fifthTextY_Pdf = FifthTextY_A4 + offsetY;

                    // Для даты "Разработал" (РазработалДата)
                    float sixthTextX_Pdf = SixthTextX_A4 + offsetX;
                    float sixthTextY_Pdf = SixthTextY_A4 + offsetY;

                    // Для изображения 1
                    float image1X_Pdf = Image1X_A4 + offsetX;
                    float image1Y_Pdf = Image1Y_A4 + offsetY;

                    // Для изображения 2
                    float image2X_Pdf = Image2X_A4 + offsetX;
                    float image2Y_Pdf = Image2Y_A4 + offsetY;

                    // Для изображения 3
                    float image3X_Pdf = Image3X_A4 + offsetX;
                    float image3Y_Pdf = Image3Y_A4 + offsetY;

                    // Масштабируем размеры изображений
                    float imageScaleX = isA3 ? A4_TO_A3_SCALE_X : 1.0f;
                    float imageScaleY = isA3 ? A4_TO_A3_SCALE_Y : 1.0f;

                    float image1TargetWidth = Image1TargetWidth_A4 * imageScaleX;
                    float image1TargetHeight = Image1TargetHeight_A4 * imageScaleY;
                    float image2TargetWidth = Image2TargetWidth_A4 * imageScaleX;
                    float image2TargetHeight = Image2TargetHeight_A4 * imageScaleY;
                    float image3TargetWidth = Image3TargetWidth_A4 * imageScaleX;
                    float image3TargetHeight = Image3TargetHeight_A4 * imageScaleY;

                    // 5. === Создаем PdfCanvas для рисования прямоугольников ===
                    PdfCanvas canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // === НАЧАЛО: Отрисовка прямоугольников ===
                    // Используем ваши точные координаты и размеры

                    // Прямоугольник 1 
                    float rect1X = -254f + offsetX;
                    float rect1Y = -437f + offsetY;
                    float rect1Width = 27f;
                    float rect1Height = 13f;

                    // Прямоугольник 2 
                    float rect2X = -225.6f + offsetX;
                    float rect2Y = -437f + offsetY;
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

                    LogMessage("  Белые прямоугольники нарисованы.");
                    // === КОНЕЦ: Отрисовка прямоугольников ===

                    // === НОВОЕ: Считывание текста из области ===
                    string readText = "";
                    try
                    {
                        // Определяем координаты области считывания
                        float readAreaX = -360f + offsetX;
                        float readAreaY = -436f + offsetY;
                        float readAreaWidth = 60f;
                        float readAreaHeight = 11f;

                        LogMessage($"  Попытка считывания текста из области: X={readAreaX:F1}, Y={readAreaY:F1}, W={readAreaWidth:F1}, H={readAreaHeight:F1}");

                        // Создаем прямоугольник для области интереса
                        Rectangle region = new Rectangle(readAreaX, readAreaY, readAreaWidth, readAreaHeight);

                        // === ИСПРАВЛЕНО: CS1503 - используем LocationTextExtractionStrategy без FilteredEventListener ===
                        // Используем LocationTextExtractionStrategy для извлечения текста
                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();

                        // Извлекаем текст из страницы
                        string allText = PdfTextExtractor.GetTextFromPage(page, strategy);

                        // Теперь нужно вручную отфильтровать текст, который попадает в нашу область
                        // LocationTextExtractionStrategy возвращает весь текст, но мы можем попробовать 
                        // получить текст, который находится в определенной области, проанализировав результат.
                        // Однако, более точный способ - это использовать RegionTextRenderInfo, если он доступен.
                        // Для простоты, попробуем получить текст, который начинается с определенных координат.
                        // Но это может быть неточно. Лучше использовать стратегию, которая поддерживает регионы.

                        // Альтернативный подход: попробуем использовать простую стратегию и вручную проверить координаты
                        // Но это сложно без доступа к координатам каждого символа.

                        // Попробуем использовать RegionTextRenderInfo, если он доступен в вашей версии.
                        // Если нет, используем LocationTextExtractionStrategy и примитивную фильтрацию.

                        // Пока что просто извлекаем весь текст и пытаемся найти нужное слово.
                        // Это не идеально, но может сработать для вашего случая.
                        if (!string.IsNullOrEmpty(allText))
                        {
                            // Разбиваем текст на строки и ищем нужное слово
                            string[] lines = allText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in lines)
                            {
                                // Простая проверка наличия слов
                                if (line.Contains("Старцев") || line.Contains("Максимов") ||
                                    line.Contains("Тихомиров") || line.Contains("Седюк") ||
                                    line.Contains("Русских"))
                                {
                                    // Берем первое найденное слово
                                    if (line.Contains("Старцев")) readText = "Старцев";
                                    else if (line.Contains("Максимов")) readText = "Максимов";
                                    else if (line.Contains("Тихомиров")) readText = "Тихомиров";
                                    else if (line.Contains("Седюк")) readText = "Седюк";
                                    else if (line.Contains("Русских")) readText = "Русских";
                                    break;
                                }
                            }
                        }

                        readText = readText.Trim();
                        LogMessage($"  Считанный текст (примитивный поиск): '{readText}'");

                        // Более точный способ (если RegionTextRenderInfo доступен):
                        // Попробуем его использовать, обернув в try-catch
                        /*
                        try 
                        {
                            // Это может не скомпилироваться, если RegionTextRenderInfo недоступен
                            // RegionTextRenderInfo regionInfo = new RegionTextRenderInfo(region);
                            // string regionText = PdfTextExtractor.GetTextFromPage(page, regionInfo);
                            // readText = regionText?.Trim() ?? "";
                        }
                        catch (Exception regionEx)
                        {
                            LogMessage($"  Не удалось использовать RegionTextRenderInfo: {regionEx.Message}");
                            // Продолжаем с примитивным поиском
                        }
                        */
                    }
                    catch (Exception readEx)
                    {
                        LogMessage($"  Ошибка считывания текста: {readEx.Message}");
                        readText = ""; // В случае ошибки считываем пустую строку
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
                            imagePath3_Selected = ""; // Не вставляем изображение
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
                    // Важно: используем новый content stream, чтобы текст рисовался поверх прямоугольников
                    PdfCanvas textCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // 7. === Загружаем шрифт ===
                    PdfFont font;
                    try
                    {
                        if (!string.IsNullOrEmpty(fontPath) && System.IO.File.Exists(fontPath))
                        {
                            byte[] fontBytes = System.IO.File.ReadAllBytes(fontPath);
                            font = PdfFontFactory.CreateFont(fontBytes, "Identity-H");
                            LogMessage($"  Шрифт загружен: {System.IO.Path.GetFileName(fontPath)}");
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
                    textCanvas.SetFontAndSize(font, FontSize_A4);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(textX_Pdf, textY_Pdf);
                    textCanvas.ShowText(TextToInsert_A4);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // УтвердилДата
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SecondFontSize_A4); // Используем размер для дат
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(secondTextX_Pdf, secondTextY_Pdf);
                    textCanvas.ShowText(dateForUtverdil); // Используем сгенерированную дату
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // Нач.бюро
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, ThirdFontSize_A4);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(thirdTextX_Pdf, thirdTextY_Pdf);
                    textCanvas.ShowText(ThirdTextToInsert_A4);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // Нач.БюроДата
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FourthFontSize_A4); // Используем размер для дат
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(fourthTextX_Pdf, fourthTextY_Pdf);
                    textCanvas.ShowText(dateForNachBuro); // Используем сгенерированную дату
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // === НОВОЕ: РазработалДата ===
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SixthFontSize_A4); // Используем размер для дат
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(sixthTextX_Pdf, sixthTextY_Pdf);
                    textCanvas.ShowText(dateForRazrabotal); // Используем сгенерированную дату
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    LogMessage("  Текстовые элементы вставлены.");

                    // 9. === Вставляем изображения ===
                    // Изображение 1
                    try
                    {
                        if (System.IO.File.Exists(ImagePath1_A4))
                        {
                            ImageData imageData1 = ImageDataFactory.Create(ImagePath1_A4);
                            if (imageData1 != null)
                            {
                                Image imageElement1 = new Image(imageData1);
                                imageElement1.SetFixedPosition(image1X_Pdf, image1Y_Pdf);
                                imageElement1.SetWidth(image1TargetWidth);
                                imageElement1.SetHeight(image1TargetHeight);

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
                            LogMessage($"  Файл изображения 1 не найден: {System.IO.Path.GetFileName(ImagePath1_A4)}");
                        }
                    }
                    catch (Exception imgEx)
                    {
                        LogMessage($"  Ошибка вставки изображения 1: {imgEx.Message}");
                    }

                    // Изображение 2
                    try
                    {
                        if (System.IO.File.Exists(ImagePath2_A4))
                        {
                            ImageData imageData2 = ImageDataFactory.Create(ImagePath2_A4);
                            if (imageData2 != null)
                            {
                                Image imageElement2 = new Image(imageData2);
                                imageElement2.SetFixedPosition(image2X_Pdf, image2Y_Pdf);
                                imageElement2.SetWidth(image2TargetWidth);
                                imageElement2.SetHeight(image2TargetHeight);

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
                            LogMessage($"  Файл изображения 2 не найден: {System.IO.Path.GetFileName(ImagePath2_A4)}");
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
                            ImageData imageData3 = ImageDataFactory.Create(imagePath3_Selected);
                            if (imageData3 != null)
                            {
                                Image imageElement3 = new Image(imageData3);
                                imageElement3.SetFixedPosition(image3X_Pdf, image3Y_Pdf);
                                imageElement3.SetWidth(image3TargetWidth);
                                imageElement3.SetHeight(image3TargetHeight);

                                Canvas layoutCanvas3 = new Canvas(page, page.GetPageSize());
                                layoutCanvas3.Add(imageElement3);
                                layoutCanvas3.Close();

                                LogMessage($"  Изображение 3 вставлено: {System.IO.Path.GetFileName(imagePath3_Selected)}");
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
                        LogMessage($"  Файл изображения 3 не найден: {System.IO.Path.GetFileName(imagePath3_Selected)}");
                    }
                    // Если imagePath3_Selected пустая строка, ничего не делаем - изображение не вставляется

                    LogMessage("  Обработка изображений завершена.");
                }

                LogMessage($"  Файл сохранен как: {System.IO.Path.GetFileName(outputPdfPath)}");
                return true;

            }
            catch (Exception ex)
            {
                LogMessage($"  Ошибка обработки файла '{System.IO.Path.GetFileName(inputPdfPath)}': {ex.Message}");
                return false;
            }
        }
    }
}