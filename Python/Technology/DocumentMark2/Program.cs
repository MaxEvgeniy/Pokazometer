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
        private TextBox transferDateTextBox; // Текстовое поле для даты приема-передачи
        private GroupBox profileGroupBox; // Группа для радиокнопок профилей
        private RadioButton[] profileRadioButtons; // Массив радиокнопок профилей
        private int selectedProfile = 0; // Выбранный профиль (0-3)
        private bool isRussian = true; // true - русский, false - английский
        private const float A3_CORRECTION_X = -11.3f; // Постоянная поправка для формата А3
        private const float ROTATED_PAGE_CORRECTION_X = 348.3f; // Дополнительная коррекция для перевернутых страниц А3

        // Класс для хранения координат профиля
        public class ProfileCoordinates
        {
            // Константы для изображений (от правого нижнего края)
            public float IMAGE1_OFFSET_X { get; set; }
            public float IMAGE1_OFFSET_Y { get; set; }
            public float IMAGE2_OFFSET_X { get; set; }
            public float IMAGE2_OFFSET_Y { get; set; }
            public float IMAGE3_OFFSET_X { get; set; }
            public float IMAGE3_OFFSET_Y { get; set; }
            public float IMAGE_WIDTH { get; set; }
            public float IMAGE_HEIGHT { get; set; }

            // Константы для текста (от правого нижнего края)
            public float CHIEF_TEXT_OFFSET_X { get; set; }
            public float CHIEF_TEXT_OFFSET_Y { get; set; }
            public float APPROVED_TEXT_OFFSET_X { get; set; }
            public float APPROVED_TEXT_OFFSET_Y { get; set; }

            // Константы для дат (от правого нижнего края)
            public float DATE_DEVELOPED_OFFSET_X { get; set; }
            public float DATE_DEVELOPED_OFFSET_Y { get; set; }
            public float DATE_CHIEF_OFFSET_X { get; set; }
            public float DATE_CHIEF_OFFSET_Y { get; set; }
            public float DATE_APPROVED_OFFSET_X { get; set; }
            public float DATE_APPROVED_OFFSET_Y { get; set; }

            // Константы для прямоугольников (от правого нижнего края)
            public float RECT1_OFFSET_X { get; set; }
            public float RECT1_OFFSET_Y { get; set; }
            public float RECT1_WIDTH { get; set; }
            public float RECT1_HEIGHT { get; set; }
            public float RECT2_OFFSET_X { get; set; }
            public float RECT2_OFFSET_Y { get; set; }
            public float RECT2_WIDTH { get; set; }
            public float RECT2_HEIGHT { get; set; }

            // Константы для зоны распознавания (где раньше было поле "Разработал")
            public float RECOGNITION_ZONE_OFFSET_X { get; set; }
            public float RECOGNITION_ZONE_OFFSET_Y { get; set; }
            public float RECOGNITION_ZONE_WIDTH { get; set; }
            public float RECOGNITION_ZONE_HEIGHT { get; set; }
        }

        // Массив профилей координат
        private ProfileCoordinates[] profiles = new ProfileCoordinates[4];

        // Константы для размеров шрифтов
        private const float MAIN_TEXT_FONT_SIZE = 14f; // Размер шрифта для основных надписей
        private const float DATE_FONT_SIZE = 8f; // Размер шрифта для дат

        // Значения дат по умолчанию
        private const string DEFAULT_TRANSFER_DATE = "01.07.2019";

        // Список возможных файлов изображений
        private string[] possibleImageFiles = {
            "Подп001.tif", "Подп002.tif", "Подп003.tif",
            "Подп004.tif", "Подп005.tif", "Подп006.tif", "Подп007.tif",
            "Подп_Не_Распознано.tif"
        };

        public Form1()
        {
            InitializeComponent();
            InitializeProfiles();
        }

        private void InitializeComponent()
        {
            this.Text = "PDF Square Drawer";
            this.Size = new Size(650, 600); // Увеличиваем размер окна
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

            // Текстовое поле для даты приема-передачи
            Label transferDateLabel = new Label();
            transferDateLabel.Text = "Дата приема-передачи:";
            transferDateLabel.Size = new Size(150, 20);
            transferDateLabel.Location = new Point(50, currentY);

            transferDateTextBox = new TextBox();
            transferDateTextBox.Size = new Size(100, 25);
            transferDateTextBox.Location = new Point(210, currentY);
            transferDateTextBox.Text = DEFAULT_TRANSFER_DATE;
            currentY += 40;

            // Группа радиокнопок для выбора профиля
            profileGroupBox = new GroupBox();
            profileGroupBox.Text = "Выбор профиля координат";
            profileGroupBox.Size = new Size(300, 120);
            profileGroupBox.Location = new Point(50, currentY);

            profileRadioButtons = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                profileRadioButtons[i] = new RadioButton();
                profileRadioButtons[i].Text = $"Профиль {i + 1}";
                profileRadioButtons[i].Location = new Point(20, 25 + i * 25);
                profileRadioButtons[i].Size = new Size(100, 20);
                profileRadioButtons[i].Tag = i;
                profileRadioButtons[i].CheckedChanged += ProfileRadioButton_CheckedChanged;
                profileGroupBox.Controls.Add(profileRadioButtons[i]);
            }

            // По умолчанию выбираем первый профиль
            profileRadioButtons[0].Checked = true;

            currentY += 130;

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
            logTextBox.Size = new Size(500, 100);
            logTextBox.Location = new Point(50, currentY);
            logTextBox.Multiline = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.ReadOnly = true;

            this.Controls.Add(drawButton);
            this.Controls.Add(toggleLanguageButton);
            this.Controls.Add(fontLabel);
            this.Controls.Add(fontComboBox);
            this.Controls.Add(transferDateLabel);
            this.Controls.Add(transferDateTextBox);
            this.Controls.Add(profileGroupBox);
            this.Controls.Add(statusLabel);
            this.Controls.Add(logLabel);
            this.Controls.Add(logTextBox);
        }

        // Инициализация профилей координат
        private void InitializeProfiles()
        {
            // Профиль 1 (по умолчанию) - текущие значения
            profiles[0] = new ProfileCoordinates
            {
                IMAGE1_OFFSET_X = 425f,
                IMAGE1_OFFSET_Y = 80f,
                IMAGE2_OFFSET_X = 425f,
                IMAGE2_OFFSET_Y = 40f,
                IMAGE3_OFFSET_X = 425f,
                IMAGE3_OFFSET_Y = 8f,
                IMAGE_WIDTH = 80f,
                IMAGE_HEIGHT = 30f,

                CHIEF_TEXT_OFFSET_X = 490f,
                CHIEF_TEXT_OFFSET_Y = 45f,
                APPROVED_TEXT_OFFSET_X = 490f,
                APPROVED_TEXT_OFFSET_Y = 17f,

                DATE_DEVELOPED_OFFSET_X = 385f,
                DATE_DEVELOPED_OFFSET_Y = 90f,
                DATE_CHIEF_OFFSET_X = 385f,
                DATE_CHIEF_OFFSET_Y = 47f,
                DATE_APPROVED_OFFSET_X = 385f,
                DATE_APPROVED_OFFSET_Y = 19f,

                RECT1_OFFSET_X = 382.1f,
                RECT1_OFFSET_Y = 86.8f,  // Обновлено
                RECT1_WIDTH = 27f,
                RECT1_HEIGHT = 12f,      // Обновлено
                RECT2_OFFSET_X = 353.8f,
                RECT2_OFFSET_Y = 86.8f,  // Обновлено
                RECT2_WIDTH = 5f,
                RECT2_HEIGHT = 12f,      // Обновлено

                RECOGNITION_ZONE_OFFSET_X = 488f,
                RECOGNITION_ZONE_OFFSET_Y = 86f,
                RECOGNITION_ZONE_WIDTH = 63f,
                RECOGNITION_ZONE_HEIGHT = 13f
            };

            // Профиль 2 - значения по умолчанию (вы можете изменить их)
            profiles[1] = new ProfileCoordinates
            {
                IMAGE1_OFFSET_X = 425f,
                IMAGE1_OFFSET_Y = 80f,
                IMAGE2_OFFSET_X = 425f,
                IMAGE2_OFFSET_Y = 40f,
                IMAGE3_OFFSET_X = 425f,
                IMAGE3_OFFSET_Y = 8f,
                IMAGE_WIDTH = 80f,
                IMAGE_HEIGHT = 30f,

                CHIEF_TEXT_OFFSET_X = 490f,
                CHIEF_TEXT_OFFSET_Y = 45f,
                APPROVED_TEXT_OFFSET_X = 490f,
                APPROVED_TEXT_OFFSET_Y = 17f,

                DATE_DEVELOPED_OFFSET_X = 385f,
                DATE_DEVELOPED_OFFSET_Y = 90f,
                DATE_CHIEF_OFFSET_X = 385f,
                DATE_CHIEF_OFFSET_Y = 47f,
                DATE_APPROVED_OFFSET_X = 385f,
                DATE_APPROVED_OFFSET_Y = 19f,

                RECT1_OFFSET_X = 382.1f,
                RECT1_OFFSET_Y = 86.8f,  // Обновлено
                RECT1_WIDTH = 27f,
                RECT1_HEIGHT = 12f,      // Обновлено
                RECT2_OFFSET_X = 353.8f,
                RECT2_OFFSET_Y = 86.8f,  // Обновлено
                RECT2_WIDTH = 5f,
                RECT2_HEIGHT = 12f,      // Обновлено

                RECOGNITION_ZONE_OFFSET_X = 487f,
                RECOGNITION_ZONE_OFFSET_Y = 86f,
                RECOGNITION_ZONE_WIDTH = 63f,
                RECOGNITION_ZONE_HEIGHT = 13f
            };

            // Профиль 3 - значения по умолчанию (вы можете изменить их)
            profiles[2] = new ProfileCoordinates
            {
                IMAGE1_OFFSET_X = 425f,
                IMAGE1_OFFSET_Y = 80f,
                IMAGE2_OFFSET_X = 425f,
                IMAGE2_OFFSET_Y = 40f,
                IMAGE3_OFFSET_X = 425f,
                IMAGE3_OFFSET_Y = 8f,
                IMAGE_WIDTH = 80f,
                IMAGE_HEIGHT = 30f,

                CHIEF_TEXT_OFFSET_X = 490f,
                CHIEF_TEXT_OFFSET_Y = 45f,
                APPROVED_TEXT_OFFSET_X = 490f,
                APPROVED_TEXT_OFFSET_Y = 17f,

                DATE_DEVELOPED_OFFSET_X = 385f,
                DATE_DEVELOPED_OFFSET_Y = 90f,
                DATE_CHIEF_OFFSET_X = 385f,
                DATE_CHIEF_OFFSET_Y = 47f,
                DATE_APPROVED_OFFSET_X = 385f,
                DATE_APPROVED_OFFSET_Y = 19f,

                RECT1_OFFSET_X = 382.1f,
                RECT1_OFFSET_Y = 86.8f,  // Обновлено
                RECT1_WIDTH = 27f,
                RECT1_HEIGHT = 12f,      // Обновлено
                RECT2_OFFSET_X = 353.8f,
                RECT2_OFFSET_Y = 86.8f,  // Обновлено
                RECT2_WIDTH = 5f,
                RECT2_HEIGHT = 12f,      // Обновлено

                RECOGNITION_ZONE_OFFSET_X = 487f,
                RECOGNITION_ZONE_OFFSET_Y = 86f,
                RECOGNITION_ZONE_WIDTH = 63f,
                RECOGNITION_ZONE_HEIGHT = 13f
            };

            // Профиль 4 - значения по умолчанию (вы можете изменить их)
            profiles[3] = new ProfileCoordinates
            {
                IMAGE1_OFFSET_X = 425f,
                IMAGE1_OFFSET_Y = 80f,
                IMAGE2_OFFSET_X = 425f,
                IMAGE2_OFFSET_Y = 40f,
                IMAGE3_OFFSET_X = 425f,
                IMAGE3_OFFSET_Y = 8f,
                IMAGE_WIDTH = 80f,
                IMAGE_HEIGHT = 30f,

                CHIEF_TEXT_OFFSET_X = 490f,
                CHIEF_TEXT_OFFSET_Y = 45f,
                APPROVED_TEXT_OFFSET_X = 490f,
                APPROVED_TEXT_OFFSET_Y = 17f,

                DATE_DEVELOPED_OFFSET_X = 385f,
                DATE_DEVELOPED_OFFSET_Y = 90f,
                DATE_CHIEF_OFFSET_X = 385f,
                DATE_CHIEF_OFFSET_Y = 47f,
                DATE_APPROVED_OFFSET_X = 385f,
                DATE_APPROVED_OFFSET_Y = 19f,

                RECT1_OFFSET_X = 382.1f,
                RECT1_OFFSET_Y = 86.8f,  // Обновлено
                RECT1_WIDTH = 27f,
                RECT1_HEIGHT = 12f,      // Обновлено
                RECT2_OFFSET_X = 353.8f,
                RECT2_OFFSET_Y = 86.8f,  // Обновлено
                RECT2_WIDTH = 5f,
                RECT2_HEIGHT = 12f,      // Обновлено

                RECOGNITION_ZONE_OFFSET_X = 487f,
                RECOGNITION_ZONE_OFFSET_Y = 86f,
                RECOGNITION_ZONE_WIDTH = 63f,
                RECOGNITION_ZONE_HEIGHT = 13f
            };
        }

        private void ProfileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Checked)
            {
                selectedProfile = (int)radioButton.Tag;
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"Выбран профиль: {selectedProfile + 1}\r\n");
                }
            }
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

            // Пытаемся выбрать шрифт "GOST Type A" по умолчанию
            bool foundGostFont = false;
            for (int i = 0; i < fontComboBox.Items.Count; i++)
            {
                if (fontComboBox.Items[i].ToString().Contains("GOST") &&
                    fontComboBox.Items[i].ToString().Contains("Type A"))
                {
                    fontComboBox.SelectedIndex = i;
                    foundGostFont = true;
                    break;
                }
            }

            // Если шрифт GOST Type A не найден, ищем просто "Gost type A"
            if (!foundGostFont)
            {
                for (int i = 0; i < fontComboBox.Items.Count; i++)
                {
                    if (fontComboBox.Items[i].ToString().ToLower().Contains("gost type a"))
                    {
                        fontComboBox.SelectedIndex = i;
                        foundGostFont = true;
                        break;
                    }
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
            if (logTextBox != null)
            {
                logTextBox.Text = $"Начало обработки... (Профиль: {selectedProfile + 1})\r\n";
            }

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
            if (statusLabel != null)
            {
                statusLabel.Text = "Обработка...";
            }
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
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"Обработка файла: {Path.GetFileName(filePath)}\r\n");
                        }
                        ProcessPdfFile(filePath);
                        processedCount++;
                        if (statusLabel != null)
                        {
                            statusLabel.Text = $"Обработано: {processedCount}/{pdfFiles.Length}";
                        }
                        this.Refresh();
                    }
                    catch (Exception ex)
                    {
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"Ошибка обработки файла {Path.GetFileName(filePath)}: {ex.Message}\r\n");
                        }
                        MessageBox.Show($"Ошибка обработки файла {Path.GetFileName(filePath)}: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (statusLabel != null)
                {
                    statusLabel.Text = $"Готово! Обработано: {processedCount}";
                }
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"Обработка завершена! Обработано файлов: {processedCount}\r\n");
                }
                MessageBox.Show($"Обработка завершена!\nОбработано файлов: {processedCount}",
                    "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = "Ошибка!";
                }
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
                }
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

                        // Определяем, является ли страница форматом А3
                        bool isA3Format = IsA3Format(pageSize);

                        // Определяем, является ли страница А3 И перевернутой
                        // ВАЖНО: Проверка на "перевернутость" применяется только к А3!
                        bool isPageRotated = false; // По умолчанию считаем, что не перевернута
                        if (isA3Format) // Проверяем перевернутость ТОЛЬКО для А3
                        {
                            isPageRotated = pageSize.Height > pageSize.Width;
                            // Для А4 и других форматов isPageRotated останется false
                        }

                        // Применяем поправку для формата А3
                        float correctionX = 0f;
                        if (isA3Format)
                        {
                            correctionX = A3_CORRECTION_X;
                            // Применяем дополнительную коррекцию только для А3
                            if (isPageRotated)
                            {
                                correctionX += ROTATED_PAGE_CORRECTION_X;
                            }
                        }
                        // Для А4 и других форматов дополнительная коррекция не применяется

                        // Логируем информацию для отладки
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"  Формат страницы: {(isA3Format ? "A3" : "не A3")}\r\n");
                            if (isA3Format) // Логируем перевернутость только для А3
                            {
                                logTextBox.AppendText($"  A3 перевернут: {isPageRotated}\r\n");
                            }
                            else
                            {
                                logTextBox.AppendText($"  Страница перевернута (проверяется только для A3): {pageSize.Height > pageSize.Width}\r\n");
                            }
                            logTextBox.AppendText($"  CorrectionX: {correctionX:F2}\r\n");
                        }

                        // Извлекаем текст из зоны распознавания
                        string recognizedText = "";
                        try
                        {
                            recognizedText = ExtractTextFromRecognitionZoneWithSameCoords(reader, 1, pageSize, correctionX);
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"  Извлеченный текст: '{recognizedText}'\r\n");
                            }
                            if (statusLabel != null)
                            {
                                statusLabel.Text = $"Извлечен текст: '{recognizedText}'";
                            }
                            this.Refresh();
                        }
                        catch (Exception ex)
                        {
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"  Ошибка извлечения текста: {ex.Message}\r\n");
                            }
                            MessageBox.Show($"Ошибка извлечения текста: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // Рисуем прямоугольники (теперь белые)
                        DrawRectangles(canvas, pageSize, correctionX);

                        // Генерируем зависимые даты
                        DateTime transferDate;
                        if (!DateTime.TryParseExact(transferDateTextBox.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out transferDate))
                        {
                            transferDate = DateTime.ParseExact(DEFAULT_TRANSFER_DATE, "dd.MM.yyyy", null);
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"  Неверный формат даты, используется значение по умолчанию: {DEFAULT_TRANSFER_DATE}\r\n");
                            }
                        }

                        // Генерируем даты с учетом выходных дней
                        string approvedDate = GenerateWorkingDay(transferDate.AddMonths(-5), transferDate);
                        string chiefDate = GenerateWorkingDay(DateTime.ParseExact(approvedDate, "dd.MM.yyyy", null).AddMonths(-3), DateTime.ParseExact(approvedDate, "dd.MM.yyyy", null));
                        string developedDate = GenerateWorkingDay(DateTime.ParseExact(chiefDate, "dd.MM.yyyy", null).AddMonths(-2), DateTime.ParseExact(chiefDate, "dd.MM.yyyy", null));

                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"  Дата приема-передачи: {transferDateTextBox.Text}\r\n");
                            logTextBox.AppendText($"  УтвердилДата: {approvedDate}\r\n");
                            logTextBox.AppendText($"  Нач.БюроДата: {chiefDate}\r\n");
                            logTextBox.AppendText($"  РазработалДата: {developedDate}\r\n");
                        }

                        // Получаем текущий профиль координат
                        ProfileCoordinates currentProfile = profiles[selectedProfile];

                        // Добавляем текстовые поля в кодировке UTF-8, курсивом
                        AddChiefText(canvas, pageSize, "Старцев", currentProfile.CHIEF_TEXT_OFFSET_X - correctionX, currentProfile.CHIEF_TEXT_OFFSET_Y);
                        AddApprovedText(canvas, pageSize, "Афанасьев", currentProfile.APPROVED_TEXT_OFFSET_X - correctionX, currentProfile.APPROVED_TEXT_OFFSET_Y);

                        // Добавляем даты в кодировке UTF-8, курсивом (вместо надписей "РазработалДата" и т.д.)
                        AddDateValueText(canvas, pageSize, developedDate, currentProfile.DATE_DEVELOPED_OFFSET_X - correctionX, currentProfile.DATE_DEVELOPED_OFFSET_Y);
                        AddDateValueText(canvas, pageSize, chiefDate, currentProfile.DATE_CHIEF_OFFSET_X - correctionX, currentProfile.DATE_CHIEF_OFFSET_Y);
                        AddDateValueText(canvas, pageSize, approvedDate, currentProfile.DATE_APPROVED_OFFSET_X - correctionX, currentProfile.DATE_APPROVED_OFFSET_Y);

                        // Добавляем изображения
                        string folderPath = Path.GetDirectoryName(filePath);

                        // Выбираем изображение подписи на основе распознанного текста
                        string imagePath1 = GetSignatureImagePath(folderPath, recognizedText);
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"  Выбрано изображение: {Path.GetFileName(imagePath1)}\r\n");
                        }

                        AddImageToPdf(canvas, pageSize, imagePath1, currentProfile.IMAGE1_OFFSET_X - correctionX, currentProfile.IMAGE1_OFFSET_Y, currentProfile.IMAGE_WIDTH, currentProfile.IMAGE_HEIGHT);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, "Подп002.tif"), currentProfile.IMAGE2_OFFSET_X - correctionX, currentProfile.IMAGE2_OFFSET_Y, currentProfile.IMAGE_WIDTH, currentProfile.IMAGE_HEIGHT);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, "Подп001.tif"), currentProfile.IMAGE3_OFFSET_X - correctionX, currentProfile.IMAGE3_OFFSET_Y, currentProfile.IMAGE_WIDTH, currentProfile.IMAGE_HEIGHT);

                        // Рисуем временную красную рамку для зоны распознавания (для отладки)
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
            // Нормализуем размеры: ширина должна быть больше высоты
            float width = pageSize.Width;
            float height = pageSize.Height;

            // Если высота больше ширины, меняем их местами
            if (height > width)
            {
                float temp = width;
                width = height;
                height = temp;
            }

            // A3 размер: 297 x 420 мм или 842 x 1191 точек
            // Проверяем размеры страницы (с небольшим допуском)
            // A3 в точках (72 DPI): 841.89 x 1190.55
            return (Math.Abs(width - 842) < 15 && Math.Abs(height - 1191) < 15) ||
                   (Math.Abs(width - 1191) < 15 && Math.Abs(height - 842) < 15);
        }

        private void DrawRectangles(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, float correctionX)
        {
            // Получаем текущий профиль координат
            ProfileCoordinates currentProfile = profiles[selectedProfile];

            // Устанавливаем БЕЛЫЙ цвет для прямоугольников
            canvas.SetColorFill(BaseColor.WHITE);
            canvas.SetColorStroke(BaseColor.WHITE);

            // Рисуем первый прямоугольник
            float rect1X = pageSize.Right - currentProfile.RECT1_OFFSET_X + correctionX;
            float rect1Y = pageSize.Bottom + currentProfile.RECT1_OFFSET_Y;
            canvas.Rectangle(rect1X, rect1Y, currentProfile.RECT1_WIDTH, currentProfile.RECT1_HEIGHT);
            canvas.Fill();

            // Рисуем второй прямоугольник
            float rect2X = pageSize.Right - currentProfile.RECT2_OFFSET_X + correctionX;
            float rect2Y = pageSize.Bottom + currentProfile.RECT2_OFFSET_Y;
            canvas.Rectangle(rect2X, rect2Y, currentProfile.RECT2_WIDTH, currentProfile.RECT2_HEIGHT);
            canvas.Fill();
        }
/*
        private void DrawRecognitionZone(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, float correctionX)
        {
            // Получаем текущий профиль координат
            ProfileCoordinates currentProfile = profiles[selectedProfile];

            // Рисуем временную красную рамку для зоны распознавания
            canvas.SetColorStroke(BaseColor.RED);
            canvas.SetLineWidth(1.0f);

            float zoneX = pageSize.Right - currentProfile.RECOGNITION_ZONE_OFFSET_X + correctionX;
            float zoneY = pageSize.Bottom + currentProfile.RECOGNITION_ZONE_OFFSET_Y;

            // Логируем координаты для отладки
            if (logTextBox != null)
            {
                bool isPageRotated = pageSize.Height > pageSize.Width;
                logTextBox.AppendText($"    Рисование рамки: X={zoneX:F2}, Y={zoneY:F2}, W={currentProfile.RECOGNITION_ZONE_WIDTH:F2}, H={currentProfile.RECOGNITION_ZONE_HEIGHT:F2} (перевернута: {isPageRotated})\r\n");
            }
            canvas.Rectangle(zoneX, zoneY, currentProfile.RECOGNITION_ZONE_WIDTH, currentProfile.RECOGNITION_ZONE_HEIGHT);
            canvas.Stroke();
        }
*/
        private void AddChiefText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, MAIN_TEXT_FONT_SIZE, true); // курсив
        }

        private void AddApprovedText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, MAIN_TEXT_FONT_SIZE, true); // курсив
        }

        private void AddDateValueText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, DATE_FONT_SIZE, true); // курсив
        }

        private void AddTextWithFontFromBottomRight(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY, float fontSize, bool isItalic = false)
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

                    // Проверяем наличие курсивной версии
                    string fontPathItalic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                                                       selectedFont + " Italic.ttf");
                    if (!File.Exists(fontPathItalic))
                    {
                        fontPathItalic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                                                    selectedFont + "-Italic.ttf");
                    }
                    if (!File.Exists(fontPathItalic))
                    {
                        fontPathItalic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                                                    selectedFont + "Oblique.ttf");
                    }

                    if (isItalic && File.Exists(fontPathItalic))
                    {
                        // Используем курсивную версию шрифта
                        baseFont = BaseFont.CreateFont(fontPathItalic, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    }
                    else if (File.Exists(fontPath))
                    {
                        // Используем обычную версию шрифта
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
                                if (isItalic)
                                {
                                    // Пытаемся найти курсивную версию
                                    string italicPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                                                                   selectedFont + " Italic" + ext);
                                    if (File.Exists(italicPath))
                                    {
                                        baseFont = BaseFont.CreateFont(italicPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                        fontFound = true;
                                        break;
                                    }
                                    else
                                    {
                                        // Если курсив не найден, используем обычный шрифт
                                        baseFont = BaseFont.CreateFont(fullPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                        fontFound = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    baseFont = BaseFont.CreateFont(fullPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                    fontFound = true;
                                    break;
                                }
                            }
                        }

                        if (!fontFound)
                        {
                            // Если системный шрифт не найден, используем стандартный курсив или обычный
                            if (isItalic)
                            {
                                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                            }
                            else
                            {
                                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                            }
                        }
                    }
                }
                catch
                {
                    // Если не удалось создать шрифт из файла, используем стандартный курсив или обычный
                    if (isItalic)
                    {
                        baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    }
                    else
                    {
                        baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    }
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
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Ошибка при добавлении текста '{text}': {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка при добавлении текста: {ex.Message}");
            }
        }

        private void AddImageToPdf(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string imagePath, float offsetX, float offsetY, float width, float height)
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
                    image.ScaleToFit(width, height);

                    canvas.AddImage(image);
                }
                else
                {
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"  Предупреждение: файл изображения не найден: {imagePath}\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                // Просто игнорируем ошибки с изображениями, чтобы не прерывать обработку PDF
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Ошибка при добавлении изображения {imagePath}: {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка при добавлении изображения {imagePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Извлекает текст из заданной области на странице PDF, используя те же координаты, что и для рисования рамки.
        /// </summary>
        /// <summary>
        /// Извлекает текст из заданной области на странице PDF, используя те же координаты, что и для рисования рамки.
        /// </summary>
        private string ExtractTextFromRecognitionZoneWithSameCoords(PdfReader reader, int pageNumber, iTextSharp.text.Rectangle pageSize, float correctionX)
        {
            string extractedText = "";

            try
            {
                // Получаем текущий профиль координат
                ProfileCoordinates currentProfile = profiles[selectedProfile];

                // Определяем, является ли страница форматом А3
                bool isA3Format = IsA3Format(pageSize);

                // Определяем, является ли страница А3 И перевернутой одновременно
                // ВАЖНО: Проверка на "перевернутость" применяется только к А3!
                bool isPageRotated = isA3Format && (pageSize.Height > pageSize.Width);

                // Используем точно такие же координаты, как в DrawRecognitionZone
                float zoneX = pageSize.Right - currentProfile.RECOGNITION_ZONE_OFFSET_X + correctionX;
                float zoneY = pageSize.Bottom + currentProfile.RECOGNITION_ZONE_OFFSET_Y;
                float zoneWidth = currentProfile.RECOGNITION_ZONE_WIDTH;
                float zoneHeight = currentProfile.RECOGNITION_ZONE_HEIGHT;

                // Создаем прямоугольник для зоны распознавания
                iTextSharp.text.Rectangle recognitionRect = new iTextSharp.text.Rectangle(
                    zoneX, zoneY, zoneX + zoneWidth, zoneY + zoneHeight
                );

                // Логируем координаты для отладки
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    PageSize: W={pageSize.Width:F2}, H={pageSize.Height:F2}\r\n");
                    logTextBox.AppendText($"    Формат A3: {isA3Format}\r\n");
                    logTextBox.AppendText($"    Страница перевернута (только для A3): {isPageRotated}\r\n");
                    logTextBox.AppendText($"    Оригинальные координаты зоны распознавания: X={zoneX:F2}, Y={zoneY:F2}, W={zoneWidth:F2}, H={zoneHeight:F2}\r\n");
                }

                // Для инверсных страниц (А3 и перевернутых) корректируем координаты и размеры
                if (isPageRotated)
                {
                    // Попробуем поменять местами ширину и высоту зоны распознавания
                    float correctedWidth = zoneHeight;
                    float correctedHeight = zoneWidth;

                    // Пересчитываем координаты для инверсной страницы
                    // Возможно, потребуется смещение
                    float correctedZoneX = zoneX + 50; // Коррекция
                    float correctedZoneY = zoneY + 610; // Коррекция

                    // Создаем скорректированный прямоугольник
                    recognitionRect = new iTextSharp.text.Rectangle(
                        correctedZoneX, correctedZoneY, correctedZoneX + correctedWidth, correctedZoneY + correctedHeight
                    );

                    // Логируем коррекцию для отладки
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"    Инверсная страница A3: скорректированы координаты зоны распознавания\r\n");
                        logTextBox.AppendText($"    Скорректировано: X={correctedZoneX:F2}, Y={correctedZoneY:F2}, W={correctedWidth:F2}, H={correctedHeight:F2}\r\n");
                    }
                }

                // Используем RegionTextRenderFilter и FilteredTextRenderListener для извлечения текста с позициями
                RegionTextRenderFilter filter = new RegionTextRenderFilter(recognitionRect);
                FilteredTextRenderListener listener = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filter);

                // Извлекаем текст с применением фильтра
                extractedText = PdfTextExtractor.GetTextFromPage(reader, pageNumber, listener);

                // Очищаем текст от лишних пробелов и символов новой строки
                extractedText = extractedText.Trim();

                // Логируем результат для отладки
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Распознанный текст: '{extractedText}'\r\n");
                }

            }
            catch (Exception ex)
            {
                // Просто логируем ошибку и возвращаем пустую строку
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Ошибка при извлечении текста из зоны: {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка при извлечении текста из зоны: {ex.Message}");
                extractedText = "";
            }

            return extractedText;
        }

        /// <summary>
        /// Определяет путь к изображению подписи на основе извлеченного текста.
        /// </summary>
        private string GetSignatureImagePath(string folderPath, string recognizedText)
        {
            // Приводим текст к нижнему регистру для нечувствительного сравнения и убираем пробелы
            string normalizedText = recognizedText.ToLower().Trim();
            if (logTextBox != null)
            {
                logTextBox.AppendText($"  Анализ текста: '{normalizedText}'\r\n");
            }

            // Проверяем специальное условие для "Самылов"
            if (normalizedText.Contains("самылов"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Самылов -> Подп007.tif\r\n");
                }
                return Path.Combine(folderPath, "Подп007.tif");
            }
            // Логика выбора изображения - ищем вхождение ключевых слов
            else if (normalizedText.Contains("максимов"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Максимов -> Подп003.tif\r\n");
                }
                return Path.Combine(folderPath, "Подп003.tif");
            }
            else if (normalizedText.Contains("старцев"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Старцев -> Подп002.tif\r\n");
                }
                return Path.Combine(folderPath, "Подп002.tif");
            }
            else if (normalizedText.Contains("русских"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Русских -> Подп004.tif\r\n");
                }
                return Path.Combine(folderPath, "Подп004.tif");
            }
            else if (normalizedText.Contains("седюк"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Седюк -> Подп005.tif\r\n");
                }
                return Path.Combine(folderPath, "Подп005.tif");
            }
            else if (normalizedText.Contains("тихомиров"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Тихомиров -> Подп006.tif\r\n");
                }
                return Path.Combine(folderPath, "Подп006.tif");
            }
            else
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Совпадений не найдено, используем изображение по умолчанию\r\n");
                }
                // Если текст не распознан или не соответствует ключевым словам
                // Возвращаем изображение "Не распознано" если оно существует
                string unknownPath = Path.Combine(folderPath, "Подп_Не_Распознано.tif");
                if (File.Exists(unknownPath))
                {
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"  Используется изображение: Подп_Не_Распознано.tif\r\n");
                    }
                    return unknownPath;
                }
                else
                {
                    // Возвращаем первое изображение из списка по умолчанию как запасной вариант
                    string defaultPath = Path.Combine(folderPath, "Подп001.tif");
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"  Используется изображение по умолчанию: Подп001.tif\r\n");
                    }
                    return defaultPath;
                }
            }
        }

        /// <summary>
        /// Генерирует рабочий день в заданном диапазоне дат, исключая выходные
        /// </summary>
        /// <param name="startDate">Начальная дата диапазона</param>
        /// <param name="endDate">Конечная дата диапазона</param>
        /// <returns>Строка с датой в формате dd.MM.yyyy</returns>
        private string GenerateWorkingDay(DateTime startDate, DateTime endDate)
        {
            // Убедимся, что startDate <= endDate
            if (startDate > endDate)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            // Создаем список рабочих дней в диапазоне
            var workingDays = new System.Collections.Generic.List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Проверяем, что это не выходной (суббота = 6, воскресенье = 0)
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays.Add(date);
                }
            }

            // Если рабочих дней нет, возвращаем последний день диапазона
            if (workingDays.Count == 0)
            {
                return endDate.ToString("dd.MM.yyyy");
            }

            // Выбираем случайный рабочий день
            Random random = new Random();
            DateTime selectedDate = workingDays[random.Next(workingDays.Count)];
            return selectedDate.ToString("dd.MM.yyyy");
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