using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
// Импортируем пространство имен для парсера
using iTextSharp.text.pdf.parser;
// Для устранения конфликта имен Path
using Path = System.IO.Path;

namespace PDFSquareDrawer
{
    public partial class Form1 : Form
    {
        private Button drawButton;
        private Label statusLabel;
        private ComboBox fontComboBox;
        private TextBox logTextBox; // Новое поле для лога
        private bool isRussian = true; // true - русский, false - английский
        private const float A3_CORRECTION_X = -11.3f; // Постоянная поправка для формата А3

        // Константы для изображений (от правого нижнего края)
        private const float IMAGE1_OFFSET_X = 425f; // отступ от правого края влево
        private const float IMAGE1_OFFSET_Y = 80f; // отступ от нижнего края вверх
        private const float IMAGE2_OFFSET_X = 425f;
        private const float IMAGE2_OFFSET_Y = 40f;
        private const float IMAGE3_OFFSET_X = 425f;
        private const float IMAGE3_OFFSET_Y = 8f;
        private const float IMAGE_WIDTH = 80f;
        private const float IMAGE_HEIGHT = 30f;

        // Константы для текста (от правого нижнего края)
        private const float CHIEF_TEXT_OFFSET_X = 490f;
        private const float CHIEF_TEXT_OFFSET_Y = 45f;
        private const float APPROVED_TEXT_OFFSET_X = 490f;
        private const float APPROVED_TEXT_OFFSET_Y = 17f;

        // Константы для дат (от правого нижнего края)
        private const float DATE_DEVELOPED_OFFSET_X = 385f;
        private const float DATE_DEVELOPED_OFFSET_Y = 90f;
        private const float DATE_CHIEF_OFFSET_X = 385f;
        private const float DATE_CHIEF_OFFSET_Y = 47f;
        private const float DATE_APPROVED_OFFSET_X = 385f;
        private const float DATE_APPROVED_OFFSET_Y = 19f;

        // Значения дат
        private const string DATE_DEVELOPED_VALUE = "01.03.2019";
        private const string DATE_CHIEF_VALUE = "05.03.2019";
        private const string DATE_APPROVED_VALUE = "01.03.2019";

        // Константы для прямоугольников (координаты от правого нижнего края)
        private const float RECT1_OFFSET_X = 382.1f; // отступ от правого края влево
        private const float RECT1_OFFSET_Y = 85.7f;  // отступ от нижнего края вверх
        private const float RECT1_WIDTH = 27f;
        private const float RECT1_HEIGHT = 13f;
        private const float RECT2_OFFSET_X = 353.8f;
        private const float RECT2_OFFSET_Y = 85.7f;
        private const float RECT2_WIDTH = 5f;
        private const float RECT2_HEIGHT = 13f;

        // Константы для зоны распознавания (где раньше было поле "Разработал")
        private const float RECOGNITION_ZONE_OFFSET_X = 487f; // отступ от правого края влево
        private const float RECOGNITION_ZONE_OFFSET_Y = 86f;  // отступ от нижнего края вверх
        private const float RECOGNITION_ZONE_WIDTH = 61f;    // ширина зоны
        private const float RECOGNITION_ZONE_HEIGHT = 13f;    // высота зоны

        // Список возможных файлов изображений
        private string[] possibleImageFiles = {
            "Подп001.tif", "Подп002.tif", "Подп003.tif",
            "Подп004.tif", "Подп005.tif", "Подп006.tif",
            "Подп_Не_Распознано.tif"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "PDF Square Drawer";
            this.Size = new Size(600, 500); // Увеличиваем размер окна
            this.StartPosition = FormStartPosition.CenterScreen;

            int currentY = 20;

            drawButton = new Button();
            drawButton.Text = "Добавить текст, изображения и прямоугольники (Русский)";
            drawButton.Size = new Size(350, 40);
            drawButton.Location = new Point(50, currentY);
            drawButton.Click += DrawButton_Click;
            currentY += 50;

            // Кнопка для переключения языка
            Button toggleLanguageButton = new Button();
            toggleLanguageButton.Text = "Переключить на English";
            toggleLanguageButton.Size = new Size(250, 30);
            toggleLanguageButton.Location = new Point(50, currentY);
            toggleLanguageButton.Click += ToggleLanguageButton_Click;
            currentY += 40;

            // Выбор шрифта
            Label fontLabel = new Label();
            fontLabel.Text = "Выберите шрифт:";
            fontLabel.Size = new Size(150, 20);
            fontLabel.Location = new Point(50, currentY);
            currentY += 25;

            fontComboBox = new ComboBox();
            fontComboBox.Size = new Size(300, 25);
            fontComboBox.Location = new Point(50, currentY);
            fontComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Заполняем список всеми доступными шрифтами системы
            PopulateFontList();

            currentY += 40;

            statusLabel = new Label();
            statusLabel.Size = new Size(400, 30);
            statusLabel.Location = new Point(50, currentY);
            statusLabel.Text = "Готово к работе";
            currentY += 35;

            // Добавляем текстовое поле для лога
            Label logLabel = new Label();
            logLabel.Text = "Лог:";
            logLabel.Size = new Size(100, 20);
            logLabel.Location = new Point(50, currentY);
            currentY += 20;

            logTextBox = new TextBox();
            logTextBox.Size = new Size(450, 100);
            logTextBox.Location = new Point(50, currentY);
            logTextBox.Multiline = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.ReadOnly = true;

            this.Controls.Add(drawButton);
            this.Controls.Add(toggleLanguageButton);
            this.Controls.Add(fontLabel);
            this.Controls.Add(fontComboBox);
            this.Controls.Add(statusLabel);
            this.Controls.Add(logLabel);
            this.Controls.Add(logTextBox);
        }

        private void PopulateFontList()
        {
            // Очищаем список
            fontComboBox.Items.Clear();

            // Получаем все установленные шрифты в системе
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = fontsCollection.Families;

                foreach (FontFamily fontFamily in fontFamilies)
                {
                    fontComboBox.Items.Add(fontFamily.Name);
                }
            }

            // Пытаемся выбрать шрифт "GOST type A Italic" по умолчанию
            bool foundGostFont = false;
            for (int i = 0; i < fontComboBox.Items.Count; i++)
            {
                if (fontComboBox.Items[i].ToString().Contains("GOST") &&
                    fontComboBox.Items[i].ToString().Contains("Italic"))
                {
                    fontComboBox.SelectedIndex = i;
                    foundGostFont = true;
                    break;
                }
            }

            // Если шрифт GOST не найден, выбираем первый доступный
            if (!foundGostFont && fontComboBox.Items.Count > 0)
            {
                fontComboBox.SelectedIndex = 0;
            }
        }

        private void ToggleLanguageButton_Click(object sender, EventArgs e)
        {
            isRussian = !isRussian;
            if (isRussian)
            {
                drawButton.Text = "Добавить текст, изображения и прямоугольники (Русский)";
                ((Button)sender).Text = "Переключить на English";
            }
            else
            {
                drawButton.Text = "Добавить текст, изображения и прямоугольники (English)";
                ((Button)sender).Text = "Переключить на Русский";
            }
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\PDF\1\Rename";

            // Очищаем лог перед началом обработки
            logTextBox.Text = "Начало обработки...\r\n";

            // Проверяем, существуют ли файлы изображений
            bool imagesExist = true;
            string missingImages = "";
            foreach (string imageFile in possibleImageFiles)
            {
                string imagePath = Path.Combine(folderPath, imageFile);
                // Не проверяем "Подп_Не_Распознано.tif" как обязательный
                if (!imageFile.Equals("Подп_Не_Распознано.tif") && !File.Exists(imagePath))
                {
                    imagesExist = false;
                    missingImages += imageFile + ", ";
                }
            }

            if (!imagesExist)
            {
                missingImages = missingImages.TrimEnd(' ', ',');
                DialogResult result = MessageBox.Show(
                    $"Следующие файлы изображений не найдены в папке {folderPath}:\n{missingImages}\n\nПродолжить без изображений?",
                    "Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            drawButton.Enabled = false;
            statusLabel.Text = "Обработка...";
            this.Refresh();

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    MessageBox.Show($"Папка {folderPath} не найдена!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    drawButton.Enabled = true;
                    return;
                }

                string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf");

                if (pdfFiles.Length == 0)
                {
                    MessageBox.Show("PDF файлы не найдены в папке!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    drawButton.Enabled = true;
                    return;
                }

                int processedCount = 0;

                foreach (string filePath in pdfFiles)
                {
                    try
                    {
                        logTextBox.AppendText($"Обработка файла: {Path.GetFileName(filePath)}\r\n");
                        ProcessPdfFile(filePath);
                        processedCount++;
                        statusLabel.Text = $"Обработано: {processedCount}/{pdfFiles.Length}";
                        this.Refresh();
                    }
                    catch (Exception ex)
                    {
                        logTextBox.AppendText($"Ошибка обработки файла {Path.GetFileName(filePath)}: {ex.Message}\r\n");
                        MessageBox.Show($"Ошибка обработки файла {Path.GetFileName(filePath)}: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                statusLabel.Text = $"Готово! Обработано: {processedCount}";
                logTextBox.AppendText($"Обработка завершена! Обработано файлов: {processedCount}\r\n");
                MessageBox.Show($"Обработка завершена!\nОбработано файлов: {processedCount}",
                    "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Ошибка!";
                logTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                drawButton.Enabled = true;
            }
        }

        private void ProcessPdfFile(string filePath)
        {
            // Создаем имя для нового файла с суффиксом "_Обработано"
            string outputPath = filePath.Replace(".pdf", "_Обработано.pdf");

            try
            {
                using (FileStream inputPdfStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (FileStream outputPdfStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    PdfReader reader = new PdfReader(inputPdfStream);
                    PdfStamper stamper = new PdfStamper(reader, outputPdfStream);

                    // Обрабатываем только первую страницу
                    if (reader.NumberOfPages >= 1)
                    {
                        PdfContentByte canvas = stamper.GetOverContent(1);
                        iTextSharp.text.Rectangle pageSize = reader.GetPageSize(1);

                        // Применяем поправку для формата А3
                        float correctionX = 0f;
                        if (IsA3Format(pageSize))
                        {
                            correctionX = A3_CORRECTION_X;
                        }

                        // Извлекаем текст из зоны распознавания
                        string recognizedText = "";
                        try
                        {
                            recognizedText = ExtractTextFromRecognitionZone(reader, 1, pageSize, correctionX);
                            logTextBox.AppendText($"  Извлеченный текст: '{recognizedText}'\r\n");
                            statusLabel.Text = $"Извлечен текст: '{recognizedText}'";
                            this.Refresh();
                        }
                        catch (Exception ex)
                        {
                            logTextBox.AppendText($"  Ошибка извлечения текста: {ex.Message}\r\n");
                            MessageBox.Show($"Ошибка извлечения текста: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // Рисуем прямоугольники (теперь белые)
                        DrawRectangles(canvas, pageSize, correctionX);

                        // Выбираем текст в зависимости от языка
                        string chiefText = isRussian ? "Нач.Бюро" : "Chief";
                        string approvedText = isRussian ? "Утвердил" : "Approved";

                        AddChiefText(canvas, pageSize, chiefText, CHIEF_TEXT_OFFSET_X - correctionX, CHIEF_TEXT_OFFSET_Y);
                        AddApprovedText(canvas, pageSize, approvedText, APPROVED_TEXT_OFFSET_X - correctionX, APPROVED_TEXT_OFFSET_Y);

                        // Добавляем даты
                        AddDateText(canvas, pageSize, DATE_DEVELOPED_VALUE, DATE_DEVELOPED_OFFSET_X - correctionX, DATE_DEVELOPED_OFFSET_Y);
                        AddDateText(canvas, pageSize, DATE_CHIEF_VALUE, DATE_CHIEF_OFFSET_X - correctionX, DATE_CHIEF_OFFSET_Y);
                        AddDateText(canvas, pageSize, DATE_APPROVED_VALUE, DATE_APPROVED_OFFSET_X - correctionX, DATE_APPROVED_OFFSET_Y);

                        // Добавляем изображения
                        string folderPath = Path.GetDirectoryName(filePath);

                        // Выбираем изображение подписи на основе распознанного текста
                        string imagePath1 = GetSignatureImagePath(folderPath, recognizedText);
                        logTextBox.AppendText($"  Выбрано изображение: {Path.GetFileName(imagePath1)}\r\n");

                        AddImageToPdf(canvas, pageSize, imagePath1, IMAGE1_OFFSET_X - correctionX, IMAGE1_OFFSET_Y);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, "Подп002.tif"), IMAGE2_OFFSET_X - correctionX, IMAGE2_OFFSET_Y);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, "Подп003.tif"), IMAGE3_OFFSET_X - correctionX, IMAGE3_OFFSET_Y);

                        // Рисование рамки закомментировано
                        // DrawRecognitionZone(canvas, pageSize, correctionX);
                    }

                    stamper.Close();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Если файл уже существует, удаляем его
                if (File.Exists(outputPath))
                    File.Delete(outputPath);

                throw new Exception($"Ошибка: {ex.Message}");
            }
        }

        private bool IsA3Format(iTextSharp.text.Rectangle pageSize)
        {
            // A3 размер: 297 x 420 мм или 842 x 1191 точек
            // Проверяем размеры страницы (с небольшим допуском)
            float width = pageSize.Width;
            float height = pageSize.Height;

            // A3 в точках (72 DPI): 841.89 x 1190.55
            return (Math.Abs(width - 842) < 10 && Math.Abs(height - 1191) < 10) ||
                   (Math.Abs(width - 1191) < 10 && Math.Abs(height - 842) < 10);
        }

        private void DrawRectangles(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, float correctionX)
        {
            // Устанавливаем БЕЛЫЙ цвет для прямоугольников
            canvas.SetColorFill(BaseColor.WHITE);
            canvas.SetColorStroke(BaseColor.WHITE);

            // Рисуем первый прямоугольник
            float rect1X = pageSize.Right - RECT1_OFFSET_X + correctionX;
            float rect1Y = pageSize.Bottom + RECT1_OFFSET_Y;
            canvas.Rectangle(rect1X, rect1Y, RECT1_WIDTH, RECT1_HEIGHT);
            canvas.Fill();

            // Рисуем второй прямоугольник
            float rect2X = pageSize.Right - RECT2_OFFSET_X + correctionX;
            float rect2Y = pageSize.Bottom + RECT2_OFFSET_Y;
            canvas.Rectangle(rect2X, rect2Y, RECT2_WIDTH, RECT2_HEIGHT);
            canvas.Fill();
        }

        // Метод рисования рамки закомментирован
        /*
        private void DrawRecognitionZone(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, float correctionX)
        {
            // Рисуем временную красную рамку для зоны распознавания
            canvas.SetColorStroke(BaseColor.RED);
            canvas.SetLineWidth(1.0f);

            float zoneX = pageSize.Right - RECOGNITION_ZONE_OFFSET_X + correctionX;
            float zoneY = pageSize.Bottom + RECOGNITION_ZONE_OFFSET_Y;

            canvas.Rectangle(zoneX, zoneY, RECOGNITION_ZONE_WIDTH, RECOGNITION_ZONE_HEIGHT);
            canvas.Stroke();
        }
        */

        private void AddChiefText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, 14);
        }

        private void AddApprovedText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, 14);
        }

        private void AddDateText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, 7);
        }

        private void AddTextWithFontFromBottomRight(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY, float fontSize)
        {
            try
            {
                BaseFont baseFont = null;

                // Выбираем шрифт в зависимости от выбора пользователя
                string selectedFont = fontComboBox.SelectedItem?.ToString() ?? "Helvetica";

                // Пытаемся создать шрифт из системного шрифта
                try
                {
                    // Путь к системным шрифтам
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                                                  selectedFont + ".ttf");

                    if (File.Exists(fontPath))
                    {
                        baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    }
                    else
                    {
                        // Если TTF не найден, пробуем другие расширения
                        string[] extensions = { ".ttf", ".otf", ".ttc" };
                        bool fontFound = false;

                        foreach (string ext in extensions)
                        {
                            string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                                                          selectedFont + ext);
                            if (File.Exists(fullPath))
                            {
                                baseFont = BaseFont.CreateFont(fullPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                fontFound = true;
                                break;
                            }
                        }

                        if (!fontFound)
                        {
                            // Если системный шрифт не найден, используем стандартный курсив
                            baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                        }
                    }
                }
                catch
                {
                    // Если не удалось создать шрифт из файла, используем стандартный курсив
                    baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                }

                // Пересчитываем координаты от правого нижнего края в абсолютные координаты
                float x = pageSize.Right - offsetX;
                float y = pageSize.Bottom + offsetY;

                canvas.SetFontAndSize(baseFont, fontSize);
                canvas.SetColorFill(BaseColor.BLACK);

                canvas.BeginText();
                canvas.SetTextMatrix(x, y);
                canvas.ShowText(text);
                canvas.EndText();

            }
            catch (Exception ex)
            {
                // Если не удалось добавить текст, просто игнорируем ошибку
                logTextBox.AppendText($"  Ошибка при добавлении текста '{text}': {ex.Message}\r\n");
                Console.WriteLine($"Ошибка при добавлении текста: {ex.Message}");
            }
        }

        private void AddImageToPdf(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string imagePath, float offsetX, float offsetY)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                    // Пересчитываем координаты от правого нижнего края в абсолютные координаты
                    // Для изображений позиция указывает на левый нижний угол
                    float x = pageSize.Right - offsetX;
                    float y = pageSize.Bottom + offsetY;

                    // Устанавливаем позицию изображения (x, y - координаты левого нижнего угла)
                    image.SetAbsolutePosition(x, y);

                    // Масштабируем изображение до заданных размеров
                    image.ScaleToFit(IMAGE_WIDTH, IMAGE_HEIGHT);

                    canvas.AddImage(image);
                }
                else
                {
                    logTextBox.AppendText($"  Предупреждение: файл изображения не найден: {imagePath}\r\n");
                }
            }
            catch (Exception ex)
            {
                // Просто игнорируем ошибки с изображениями, чтобы не прерывать обработку PDF
                logTextBox.AppendText($"  Ошибка при добавлении изображения {imagePath}: {ex.Message}\r\n");
                Console.WriteLine($"Ошибка при добавлении изображения {imagePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Извлекает текст из заданной области на странице PDF.
        /// </summary>
        private string ExtractTextFromRecognitionZone(PdfReader reader, int pageNumber, iTextSharp.text.Rectangle pageSize, float correctionX)
        {
            string extractedText = "";

            try
            {
                // 1. Определяем координаты зоны распознавания
                float zoneX = pageSize.Right - RECOGNITION_ZONE_OFFSET_X + correctionX;
                float zoneY = pageSize.Bottom + RECOGNITION_ZONE_OFFSET_Y;
                // Создаем прямоугольник для зоны
                iTextSharp.text.Rectangle recognitionRect = new iTextSharp.text.Rectangle(
                    zoneX, zoneY, zoneX + RECOGNITION_ZONE_WIDTH, zoneY + RECOGNITION_ZONE_HEIGHT
                );

                // 2. Используем RegionTextRenderFilter и FilteredTextRenderListener для извлечения текста с позициями
                RegionTextRenderFilter filter = new RegionTextRenderFilter(recognitionRect);
                FilteredTextRenderListener listener = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filter);

                // 3. Извлекаем текст с применением фильтра
                extractedText = PdfTextExtractor.GetTextFromPage(reader, pageNumber, listener);

                // 4. Очищаем текст от лишних пробелов и символов новой строки
                extractedText = extractedText.Trim();

            }
            catch (Exception ex)
            {
                // Просто логируем ошибку и возвращаем пустую строку
                logTextBox.AppendText($"  Ошибка при извлечении текста из зоны: {ex.Message}\r\n");
                Console.WriteLine($"Ошибка при извлечении текста из зоны: {ex.Message}");
                extractedText = "";
            }

            return extractedText;
        }

        /// <summary>
        /// Определяет путь к изображению подписи на основе извлеченного текста.
        /// </summary>
        /// <summary>
        /// Определяет путь к изображению подписи на основе извлеченного текста.
        /// </summary>
        private string GetSignatureImagePath(string folderPath, string recognizedText)
        {
            // Приводим текст к нижнему регистру для нечувствительного сравнения
            string normalizedText = recognizedText.ToLower().Trim();
            logTextBox.AppendText($"  Анализ текста: '{normalizedText}'\r\n");

            // Логика выбора изображения - ищем вхождение ключевых слов
            if (normalizedText.Contains("максимов"))
            {
                logTextBox.AppendText($"  Найдено совпадение: Максимов -> Подп003.tif\r\n");
                return Path.Combine(folderPath, "Подп003.tif");
            }
            else if (normalizedText.Contains("старцев"))
            {
                logTextBox.AppendText($"  Найдено совпадение: Старцев -> Подп002.tif\r\n");
                return Path.Combine(folderPath, "Подп002.tif");
            }
            else if (normalizedText.Contains("русских"))
            {
                logTextBox.AppendText($"  Найдено совпадение: Русских -> Подп004.tif\r\n");
                return Path.Combine(folderPath, "Подп004.tif");
            }
            else if (normalizedText.Contains("седюк"))
            {
                logTextBox.AppendText($"  Найдено совпадение: Седюк -> Подп005.tif\r\n");
                return Path.Combine(folderPath, "Подп005.tif");
            }
            else if (normalizedText.Contains("тихомиров"))
            {
                logTextBox.AppendText($"  Найдено совпадение: Тихомиров -> Подп006.tif\r\n");
                return Path.Combine(folderPath, "Подп006.tif");
            }
            else
            {
                logTextBox.AppendText($"  Совпадений не найдено, используем изображение по умолчанию\r\n");
                // Если текст не распознан или не соответствует ключевым словам
                // Возвращаем изображение "Не распознано" если оно существует
                string unknownPath = Path.Combine(folderPath, "Подп_Не_Распознано.tif");
                if (File.Exists(unknownPath))
                {
                    logTextBox.AppendText($"  Используется изображение: Подп_Не_Распознано.tif\r\n");
                    return unknownPath;
                }
                else
                {
                    // Возвращаем первое изображение из списка по умолчанию как запасной вариант
                    string defaultPath = Path.Combine(folderPath, "Подп001.tif");
                    logTextBox.AppendText($"  Используется изображение по умолчанию: Подп001.tif\r\n");
                    return defaultPath;
                }
            }
        }
    }

    // Дополнительный класс для фильтрации текста по области
    public class RegionTextRenderFilter : RenderFilter
    {
        private iTextSharp.text.Rectangle filterRect;

        public RegionTextRenderFilter(iTextSharp.text.Rectangle filterRect)
        {
            this.filterRect = filterRect;
        }

        public override bool AllowText(TextRenderInfo renderInfo)
        {
            // Получаем границы текста как LineSegment
            LineSegment descentLine = renderInfo.GetDescentLine();
            LineSegment ascentLine = renderInfo.GetAscentLine();

            // Получаем координаты углов текста
            Vector startPoint = descentLine.GetStartPoint();
            Vector endPoint = descentLine.GetEndPoint();
            Vector startPointAscent = ascentLine.GetStartPoint();
            Vector endPointAscent = ascentLine.GetEndPoint();

            // Создаем прямоугольник, описывающий текст
            float x1 = Math.Min(startPoint[Vector.I1], Math.Min(endPoint[Vector.I1], Math.Min(startPointAscent[Vector.I1], endPointAscent[Vector.I1])));
            float x2 = Math.Max(startPoint[Vector.I1], Math.Max(endPoint[Vector.I1], Math.Max(startPointAscent[Vector.I1], endPointAscent[Vector.I1])));
            float y1 = Math.Min(startPoint[Vector.I2], Math.Min(endPoint[Vector.I2], Math.Min(startPointAscent[Vector.I2], endPointAscent[Vector.I2])));
            float y2 = Math.Max(startPoint[Vector.I2], Math.Max(endPoint[Vector.I2], Math.Max(startPointAscent[Vector.I2], endPointAscent[Vector.I2])));

            iTextSharp.text.Rectangle textRect = new iTextSharp.text.Rectangle(x1, y1, x2, y2);

            // Проверяем пересечение прямоугольников вручную
            return DoRectanglesIntersect(textRect, filterRect);
        }

        // Метод для проверки пересечения двух прямоугольников
        private bool DoRectanglesIntersect(iTextSharp.text.Rectangle rect1, iTextSharp.text.Rectangle rect2)
        {
            return !(rect1.Right < rect2.Left ||
                     rect2.Right < rect1.Left ||
                     rect1.Top < rect2.Bottom ||
                     rect2.Top < rect1.Bottom);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}