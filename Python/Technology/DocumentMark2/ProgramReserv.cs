using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
// Импортируем пространство имен для парсера
using iTextSharp.text.pdf.parser;
// Для устранения конфликта имен Path
using Path = System.IO.Path;
// Добавляем ссылки на Microsoft Word Object Library
using Word = Microsoft.Office.Interop.Word;

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

        // Константы для размеров изображений (одинаковые для всех)
        private const float IMAGE_UNIFORM_WIDTH = 80f;
        private const float IMAGE_UNIFORM_HEIGHT = 30f;

        // Константы для координат вставки изображений (от правого нижнего края)
        private const float IMAGE1_INSERT_X = 425f;
        private const float IMAGE1_INSERT_Y = 80f;
        private const float IMAGE2_INSERT_X = 425f;
        private const float IMAGE2_INSERT_Y = 40f;
        private const float IMAGE3_INSERT_X = 425f;
        private const float IMAGE3_INSERT_Y = 8f;

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
        private const string DATE_APPROVED_VALUE = "01.03.2019"; // Добавляем эту константу
        private const string DATE_CHIEF_VALUE = "05.03.2019";
        private const string DATE_DEVELOPED_VALUE = "01.03.2019";

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

                RECOGNITION_ZONE_OFFSET_X = 487f,
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
                // Добавляем проверку на null для избежания NullReferenceException
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
            string outputDocxPath = filePath.Replace(".pdf", "_Обработано.docx");
            // Извлекаем текст из зоны распознавания
            string recognizedText = "";

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

                        // Определяем, является ли страница А3 И перевернутой одновременно
                        // ВАЖНО: Проверка на "перевернутость" применяется только к А3!
                        bool isPageRotated = isA3Format && (pageSize.Height > pageSize.Width);

                        // Применяем поправку для формата А3
                        float correctionX = 0f;
                        if (IsA3Format(pageSize))
                        {
                            correctionX = A3_CORRECTION_X;
                            // Применяем дополнительную коррекцию только для А3
                            if (isPageRotated)
                            {
                                correctionX += ROTATED_PAGE_CORRECTION_X;
                            }
                        }
                        // Для А4 и других форматов дополнительная коррекция не применяется


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

                        // Добавляем изображения поверх текста с указанными координатами
                        string folderPath = Path.GetDirectoryName(filePath);

                        // Выбираем изображение подписи на основе распознанного текста
                        string imagePath1 = GetSignatureImagePath(folderPath, recognizedText);
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"  Выбрано изображение: {Path.GetFileName(imagePath1)}\r\n");
                        }

                        // Вставляем изображения поверх текста с одинаковыми размерами и разными координатами
                        AddImageToPdf(canvas, pageSize, imagePath1, IMAGE1_INSERT_X - correctionX, IMAGE1_INSERT_Y, IMAGE_UNIFORM_WIDTH, IMAGE_UNIFORM_HEIGHT);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, "Подп002.tif"), IMAGE2_INSERT_X - correctionX, IMAGE2_INSERT_Y, IMAGE_UNIFORM_WIDTH, IMAGE_UNIFORM_HEIGHT);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, "Подп001.tif"), IMAGE3_INSERT_X - correctionX, IMAGE3_INSERT_Y, IMAGE_UNIFORM_WIDTH, IMAGE_UNIFORM_HEIGHT);

                        // Рисование рамки закомментировано
                        // DrawRecognitionZone(canvas, pageSize, correctionX);
                    }

                    stamper.Close();
                    reader.Close();
                }

                // Обрабатываем соответствующий .docx файл
                ProcessWordDocument(filePath, recognizedText);
            }
            catch (Exception ex)
            {
                // Если файл уже существует, удаляем его
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
                if (File.Exists(outputDocxPath))
                    File.Delete(outputDocxPath);

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

        // Метод рисования рамки закомментирован
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

                    // Масштабируем изображение до заданных размеров (одинаковые для всех)
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
        /// Извлекает текст из заданной области на странице PDF.
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

                // 1. Определяем координаты зоны распознавания
                float zoneX = pageSize.Right - currentProfile.RECOGNITION_ZONE_OFFSET_X + correctionX;
                float zoneY = pageSize.Bottom + currentProfile.RECOGNITION_ZONE_OFFSET_Y;
                // Создаем прямоугольник для зоны
                iTextSharp.text.Rectangle recognitionRect = new iTextSharp.text.Rectangle(
                    zoneX, zoneY, zoneX + currentProfile.RECOGNITION_ZONE_WIDTH, zoneY + currentProfile.RECOGNITION_ZONE_HEIGHT
                );

                // Логируем координаты для отладки
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    PageSize: W={pageSize.Width:F2}, H={pageSize.Height:F2}\r\n");
                    logTextBox.AppendText($"    Формат A3: {isA3Format}\r\n");
                    logTextBox.AppendText($"    Страница перевернута (только для A3): {isPageRotated}\r\n");
                    logTextBox.AppendText($"    Оригинальные координаты зоны распознавания: X={zoneX:F2}, Y={zoneY:F2}, W={currentProfile.RECOGNITION_ZONE_WIDTH:F2}, H={currentProfile.RECOGNITION_ZONE_HEIGHT:F2}\r\n");
                }

                // Для инверсных страниц (А3 и перевернутых) корректируем координаты и размеры
                if (isPageRotated)
                {
                    // Попробуем поменять местами ширину и высоту зоны распознавания
                    float correctedWidth = currentProfile.RECOGNITION_ZONE_HEIGHT;
                    float correctedHeight = currentProfile.RECOGNITION_ZONE_WIDTH;

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

                // 2. Используем RegionTextRenderFilter и FilteredTextRenderListener для извлечения текста с позициями
                RegionTextRenderFilter filter = new RegionTextRenderFilter(recognitionRect);
                FilteredTextRenderListener listener = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filter);

                // 3. Извлекаем текст с применением фильтра
                extractedText = PdfTextExtractor.GetTextFromPage(reader, pageNumber, listener);

                // 4. Очищаем текст от лишних пробелов и символов новой строки
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
        /// <summary>
        /// Определяет путь к изображению подписи на основе извлеченного текста
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

        /// <summary>
        /// Обрабатывает соответствующий Word документ (.docx)
        /// </summary>
        /// <summary>
        /// Обрабатывает соответствующий Word документ (.docx)
        /// </summary>
        /// <summary>
        /// Обрабатывает соответствующий Word документ (.docx)
        /// </summary>
        private void ProcessWordDocument(string pdfFilePath, string recognizedText)
        {
            object missing = System.Type.Missing; // Локальная переменная для ref параметров
            try
            {
                // Формируем путь к .docx файлу
                string docxPath = pdfFilePath.Replace(".pdf", ".docx");
                string outputDocxPath = pdfFilePath.Replace(".pdf", "_Обработано.docx");

                if (!File.Exists(docxPath))
                {
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"  Предупреждение: файл Word не найден: {Path.GetFileName(docxPath)}\r\n");
                    }
                    return;
                }

                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Обработка Word документа: {Path.GetFileName(docxPath)}\r\n");
                }

                // Создаем экземпляр Word приложения
                Word.Application wordApp = new Word.Application();
                wordApp.Visible = false; // Не показываем Word
                Word.Document doc = null;

                try
                {
                    // Открываем документ
                    object fileName = docxPath;
                    object readOnly = false;
                    object isVisible = false;


                    doc = wordApp.Documents.Open(
                        ref fileName,
                        ref missing, // ref missing
                        ref readOnly,
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref isVisible,
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing, // ref missing
                        ref missing  // ref missing
                    );

                    // Обрабатываем КАЖДУЮ страницу документа ТОЛЬКО ОДИН РАЗ
                    int totalPages = doc.ActiveWindow.Panes[1].Pages.Count;

                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"    Всего страниц в документе: {totalPages}\r\n");
                    }

                    // Обрабатываем каждую страницу документа
                    for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
                    {
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"    Обработка страницы Word: {pageNumber}\r\n");
                        }

                        try
                        {
                            // Обрабатываем КАЖДУЮ таблицу на странице
                            for (int tableIndex = 1; tableIndex <= doc.Tables.Count; tableIndex++)
                            {
                                Word.Table table = doc.Tables[tableIndex];

                                // Ищем ячейку со словом "Разраб." в текущей таблице
                                Word.Cell targetCell = FindCellWithText(table, "Разраб.");

                                if (targetCell != null)
                                {
                                    if (logTextBox != null)
                                    {
                                        logTextBox.AppendText($"      Найдена ячейка 'Разраб.' в таблице {tableIndex}\r\n");
                                    }

                                    // Получаем координаты ячейки
                                    int rowIdx = targetCell.RowIndex;
                                    int colIdx = targetCell.ColumnIndex;

                                    // 1. Открываем файл .docx, имя и адрес которого совпадает с именем и адресом редактируемого файла .pdf
                                    // Уже открыли выше

                                    // 2. В таблице на первой странице ищем ячейку со словом "Разраб."
                                    // Уже нашли выше

                                    // 3. Удаляем данные в двух следующих по строке ячейках слева направо
                                    ClearNextTwoCells(table, rowIdx, colIdx);

                                    // 4. В следующей ячейке справа от "Разраб." записываем распознанное в программе выше слово
                                    string developerName = GetNameFromRecognizedText(recognizedText);
                                    SetCellValue(table, rowIdx, colIdx + 3, developerName); // Пропускаем 2 очищенных ячейки + 1

                                    // 5. В следующую ячейку строки вставляем ту же картинку, что и в файл .pdf
                                    string folderPath = Path.GetDirectoryName(pdfFilePath);
                                    string imagePath1 = GetSignatureImagePath(folderPath, recognizedText);
                                    // Вставляем картинку поверх текста на страницу (не в ячейку)
                                    InsertImageOverText(doc, pageNumber, imagePath1, IMAGE1_INSERT_X, IMAGE1_INSERT_Y, IMAGE_UNIFORM_WIDTH, IMAGE_UNIFORM_HEIGHT);

                                    // 6. В следующую ячейку строки вставляем дату, которая выбирается случайным образом в диапазоне от даты "УтвердилДата" до "УтвердилДата"+1месяц
                                    DateTime approvalDate = DateTime.ParseExact(DATE_APPROVED_VALUE, "dd.MM.yyyy", null);
                                    DateTime endDate = approvalDate.AddMonths(1);
                                    Random random = new Random();
                                    TimeSpan timeSpan = endDate - approvalDate;
                                    DateTime randomDate = approvalDate.AddDays(random.Next(0, (int)timeSpan.TotalDays + 1));
                                    string randomDateString = randomDate.ToString("dd.MM.yyyy");
                                    SetCellValue(table, rowIdx, colIdx + 5, randomDateString); // Пропускаем 4 ячейки + 1

                                    // 7. Переходим на строчку ниже, в ячейку под словом "Разраб." и заменяем ее содержимое на "Нач. Бюро"
                                    SetCellValue(table, rowIdx + 1, colIdx, "Нач. Бюро");

                                    // 8. В следующей ячейке справа записываем слово "Старцев"
                                    SetCellValue(table, rowIdx + 1, colIdx + 1, "Старцев");

                                    // 9. В следующей ячейке справа вставляем картинку "Подп002.tif"
                                    string imagePath2 = Path.Combine(folderPath, "Подп002.tif");
                                    // Вставляем картинку поверх текста на страницу (не в ячейку)
                                    InsertImageOverText(doc, pageNumber, imagePath2, IMAGE2_INSERT_X, IMAGE2_INSERT_Y, IMAGE_UNIFORM_WIDTH, IMAGE_UNIFORM_HEIGHT);

                                    // 10. В следующей ячейке справа вставляем дату, которая на неделю позже даты, которую вставили в пункте 6
                                    DateTime datePlusWeek = randomDate.AddDays(7);
                                    string datePlusWeekString = datePlusWeek.ToString("dd.MM.yyyy");
                                    SetCellValue(table, rowIdx + 1, colIdx + 3, datePlusWeekString); // Пропускаем 2 ячейки + 1

                                    // 11. Спускаемся еще на строку ниже под словом "Разраб." и заменяем ее содержимое на "Утвердил"
                                    SetCellValue(table, rowIdx + 2, colIdx, "Утвердил");

                                    // 12. В следующей ячейке справа записываем слово "Афанасьев"
                                    SetCellValue(table, rowIdx + 2, colIdx + 1, "Афанасьев");

                                    // 13. В следующей ячейке справа вставляем картинку "Подп001.tif"
                                    string imagePath3 = Path.Combine(folderPath, "Подп001.tif");
                                    // Вставляем картинку поверх текста на страницу (не в ячейку)
                                    InsertImageOverText(doc, pageNumber, imagePath3, IMAGE3_INSERT_X, IMAGE3_INSERT_Y, IMAGE_UNIFORM_WIDTH, IMAGE_UNIFORM_HEIGHT);

                                    // 14. В следующей ячейке справа вставляем дату, которая на неделю позже даты из пункта 10
                                    DateTime finalDate = datePlusWeek.AddDays(7);
                                    string finalDateString = finalDate.ToString("dd.MM.yyyy");
                                    SetCellValue(table, rowIdx + 2, colIdx + 3, finalDateString); // Пропускаем 2 ячейки + 1

                                    // 15. Повторяем пункты от 2 до 14 для каждой страницы документа .docx
                                    // Уже обрабатываем каждую страницу в цикле

                                    // Устанавливаем шрифт для всех измененных ячеек
                                    ApplyGostFontToCells(table, rowIdx, colIdx);

                                    if (logTextBox != null)
                                    {
                                        logTextBox.AppendText($"      Обработана таблица {tableIndex} на странице {pageNumber}\r\n");
                                    }

                                    // Прерываем обработку таблиц, чтобы не обрабатывать одну и ту же ячейку несколько раз
                                    break;
                                }
                                else
                                {
                                    if (logTextBox != null)
                                    {
                                        logTextBox.AppendText($"      Ячейка 'Разраб.' не найдена в таблице {tableIndex}\r\n");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"    Ошибка обработки страницы {pageNumber}: {ex.Message}\r\n");
                            }
                            MessageBox.Show($"Ошибка обработки страницы {pageNumber}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Сохраняем документ с новым именем
                    object saveAsFile = outputDocxPath;
                    doc.SaveAs2(ref saveAsFile, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing);

                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"  Word документ сохранен как: {Path.GetFileName(outputDocxPath)}\r\n");
                    }
                }
                finally
                {
                    // Закрываем документ и приложение Word
                    if (doc != null)
                    {
                        object saveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
                        object originalFormat = Word.WdOriginalFormat.wdOriginalDocumentFormat;
                        object routeDocument = false;
                        doc.Close(ref saveChanges, ref originalFormat, ref routeDocument);
                    }

                    wordApp.Quit(ref missing, ref missing, ref missing);
                }
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Ошибка обработки Word документа: {ex.Message}\r\n");
                }
                MessageBox.Show($"Ошибка обработки Word документа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Ищет ячейку с заданным текстом в таблице
        /// </summary>
        private Word.Cell FindCellWithText(Word.Table table, string searchText)
        {
            try
            {
                for (int row = 1; row <= table.Rows.Count; row++)
                {
                    for (int col = 1; col <= table.Columns.Count; col++)
                    {
                        try
                        {
                            Word.Cell cell = table.Cell(row, col);
                            if (cell != null && cell.Range != null &&
                                cell.Range.Text != null &&
                                cell.Range.Text.Contains(searchText))
                            {
                                return cell;
                            }
                        }
                        catch
                        {
                            // Игнорируем ошибки доступа к ячейкам
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Ошибка поиска ячейки с текстом '{searchText}': {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка поиска ячейки с текстом '{searchText}': {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Очищает две следующие ячейки справа от заданной
        /// </summary>
        private void ClearNextTwoCells(Word.Table table, int rowIdx, int colIdx)
        {
            try
            {
                // Очищаем первую ячейку справа
                if (colIdx + 1 <= table.Columns.Count)
                {
                    try
                    {
                        Word.Cell cell1 = table.Cell(rowIdx, colIdx + 1);
                        if (cell1 != null && cell1.Range != null)
                        {
                            cell1.Range.Text = "";
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"      Очищена ячейка [{rowIdx},{colIdx + 1}]\r\n");
                            }
                        }
                    }
                    catch
                    {
                        // Игнорируем ошибки
                    }
                }

                // Очищаем вторую ячейку справа
                if (colIdx + 2 <= table.Columns.Count)
                {
                    try
                    {
                        Word.Cell cell2 = table.Cell(rowIdx, colIdx + 2);
                        if (cell2 != null && cell2.Range != null)
                        {
                            cell2.Range.Text = "";
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"      Очищена ячейка [{rowIdx},{colIdx + 2}]\r\n");
                            }
                        }
                    }
                    catch
                    {
                        // Игнорируем ошибки
                    }
                }
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Ошибка очистки ячеек: {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка очистки ячеек: {ex.Message}");
            }
        }

        /// <summary>
        /// Устанавливает значение ячейки
        /// </summary>
        private void SetCellValue(Word.Table table, int rowIdx, int colIdx, string value)
        {
            try
            {
                if (rowIdx <= table.Rows.Count && colIdx <= table.Columns.Count)
                {
                    Word.Cell cell = table.Cell(rowIdx, colIdx);
                    if (cell != null && cell.Range != null)
                    {
                        cell.Range.Text = value;

                        // Устанавливаем шрифт GOST 2.304 A, размер 11
                        cell.Range.Font.Name = "GOST 2.304 A";
                        cell.Range.Font.Size = 11;

                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"      Установлено значение '{value}' в ячейку [{rowIdx},{colIdx}]\r\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Ошибка установки значения '{value}' в ячейку [{rowIdx},{colIdx}]: {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка установки значения '{value}' в ячейку [{rowIdx},{colIdx}]: {ex.Message}");
            }
        }

        /// <summary>
        /// Вставляет изображение поверх текста на страницу Word документа
        /// </summary>
        /// <summary>
        /// Вставляет изображение поверх текста на страницу Word документа
        /// </summary>
        /// <summary>
        /// Вставляет изображение поверх текста на страницу Word документа
        /// </summary>
        private void InsertImageOverText(Word.Document doc, int pageNumber, string imagePath, float offsetX, float offsetY, float width, float height)
        {
            try
            {
                // Проверяем существование файла изображения
                if (!File.Exists(imagePath))
                {
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"    Ошибка: файл изображения не найден: {imagePath}\r\n");
                    }
                    return;
                }

                // Проверяем формат файла изображения
                string fileExtension = Path.GetExtension(imagePath).ToLower();
                string finalImagePath = imagePath;

                // Если это .tif файл, конвертируем его в .png для лучшей совместимости с Word
                if (fileExtension == ".tif" || fileExtension == ".tiff")
                {
                    string convertedImagePath = ConvertTiffToPng(imagePath);
                    if (!string.IsNullOrEmpty(convertedImagePath) && File.Exists(convertedImagePath))
                    {
                        finalImagePath = convertedImagePath;
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"    Преобразован .tif в .png: {Path.GetFileName(convertedImagePath)}\r\n");
                        }
                    }
                    else
                    {
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"    Ошибка преобразования .tif файла: {imagePath}\r\n");
                        }
                        return;
                    }
                }

                if (doc != null && doc.Application != null)
                {
                    try
                    {
                        // Получаем активное окно и выделение
                        Word.Window activeWindow = doc.ActiveWindow;
                        if (activeWindow != null)
                        {
                            Word.Selection selection = activeWindow.Selection;
                            if (selection != null)
                            {
                                // Устанавливаем позицию курсора в конец документа
                                object unit = Word.WdUnits.wdStory;
                                object extend = Word.WdMovementType.wdMove;
                                selection.EndKey(ref unit, ref extend);

                                // Получаем текущую позицию курсора
                                Word.Range selectionRange = selection.Range;

                                // Подготавливаем параметры для метода AddPicture (БЕЗ ref!)
                                // ВАЖНО: Первый параметр должен быть string, а не object!
                                string picturePath = finalImagePath; // Используем string вместо object
                                object linkToFile = false;
                                object saveWithDocument = true;
                                object range = selectionRange;

                                // Вставляем изображение (параметры БЕЗ ref!)
                                Word.InlineShape inlineShape = selection.InlineShapes.AddPicture(
                                    picturePath, ref linkToFile, ref saveWithDocument, ref range);

                                // Масштабируем изображение до заданных размеров
                                inlineShape.Width = width;
                                inlineShape.Height = height;

                                // Преобразуем InlineShape в Shape для возможности позиционирования поверх текста
                                Word.Shape shape = inlineShape.ConvertToShape();

                                // Устанавливаем позиционирование поверх текста
                                shape.WrapFormat.Type = Word.WdWrapType.wdWrapNone; // Нет обтекания
                                shape.RelativeHorizontalPosition = Word.WdRelativeHorizontalPosition.wdRelativeHorizontalPositionPage;
                                shape.RelativeVerticalPosition = Word.WdRelativeVerticalPosition.wdRelativeVerticalPositionPage;

                                // Устанавливаем координаты (от правого нижнего края страницы)
                                // Word использует другую систему координат: (0,0) - левый верхний угол
                                float pageWidth = doc.Sections[1].PageSetup.PageWidth;
                                float pageHeight = doc.Sections[1].PageSetup.PageHeight;

                                // Преобразуем координаты из правого нижнего угла в левый верхний
                                shape.Left = pageWidth - offsetX;
                                shape.Top = pageHeight - offsetY - height; // Учитываем высоту изображения

                                if (logTextBox != null)
                                {
                                    logTextBox.AppendText($"    Вставлена картинка '{Path.GetFileName(finalImagePath)}' поверх текста на странице {pageNumber}\r\n");
                                    logTextBox.AppendText($"    Координаты: X={shape.Left:F2}, Y={shape.Top:F2}, W={width:F2}, H={height:F2}\r\n");
                                }
                            }
                            else
                            {
                                if (logTextBox != null)
                                {
                                    logTextBox.AppendText($"    Ошибка: невозможно получить выделение в документе\r\n");
                                }
                            }
                        }
                        else
                        {
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"    Ошибка: невозможно получить активное окно документа\r\n");
                            }
                        }
                    }
                    catch (Exception innerEx)
                    {
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText($"    Ошибка вставки картинки поверх текста {finalImagePath}: {innerEx.Message}\r\n");
                        }
                        Console.WriteLine($"Ошибка вставки картинки поверх текста {finalImagePath}: {innerEx.Message}");

                        // Попробуем альтернативный метод вставки
                        try
                        {
                            // Альтернативный метод: вставка в конец документа без специального позиционирования
                            string picturePath = finalImagePath; // Используем string вместо object
                            object linkToFile = false;
                            object saveWithDocument = true;
                            object missing = System.Type.Missing;

                            Word.InlineShape inlineShape = doc.InlineShapes.AddPicture(
                                picturePath, ref linkToFile, ref saveWithDocument, ref missing);

                            // Масштабируем изображение
                            inlineShape.Width = width;
                            inlineShape.Height = height;

                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"    Вставлена картинка '{Path.GetFileName(finalImagePath)}' (альтернативный метод)\r\n");
                            }
                        }
                        catch (Exception altEx)
                        {
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText($"    Альтернативный метод также не сработал: {altEx.Message}\r\n");
                            }
                        }
                    }
                }
                else
                {
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText($"    Ошибка: Word документ или приложение не инициализированы\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Общая ошибка вставки картинки поверх текста {imagePath}: {ex.Message}\r\n");
                }
                Console.WriteLine($"Общая ошибка вставки картинки поверх текста {imagePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Преобразует .tif файл в .png для совместимости с Word
        /// </summary>
        private string ConvertTiffToPng(string tiffPath)
        {
            try
            {
                string pngPath = tiffPath.Replace(".tif", ".png").Replace(".TIF", ".png");

                // Если .png файл уже существует, используем его
                if (File.Exists(pngPath))
                {
                    return pngPath;
                }

                // Пытаемся преобразовать .tif в .png
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(tiffPath))
                {
                    image.Save(pngPath, System.Drawing.Imaging.ImageFormat.Png);
                }

                return pngPath;
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Ошибка преобразования .tif в .png: {ex.Message}\r\n");
                }
                return "";
            }
        }

        /// <summary>
        /// Применяет шрифт GOST 2.304 A к ячейкам таблицы
        /// </summary>
        private void ApplyGostFontToCells(Word.Table table, int startRow, int startCol)
        {
            try
            {
                // Применяем шрифт к измененным ячейкам
                for (int row = startRow; row <= Math.Min(startRow + 2, table.Rows.Count); row++)
                {
                    for (int col = startCol; col <= Math.Min(startCol + 5, table.Columns.Count); col++)
                    {
                        try
                        {
                            Word.Cell cell = table.Cell(row, col);
                            if (cell != null && cell.Range != null)
                            {
                                // Устанавливаем шрифт GOST 2.304 A, размер 11
                                cell.Range.Font.Name = "GOST 2.304 A";
                                cell.Range.Font.Size = 11;
                            }
                        }
                        catch
                        {
                            // Игнорируем ошибки
                        }
                    }
                }

                if (logTextBox != null)
                {
                    logTextBox.AppendText($"      Применен шрифт GOST 2.304 A, размер 11 к таблице\r\n");
                }
            }
            catch (Exception ex)
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"    Ошибка применения шрифта к таблице: {ex.Message}\r\n");
                }
                Console.WriteLine($"Ошибка применения шрифта к таблице: {ex.Message}");
            }
        }

        /// <summary>
        /// Определяет имя на основе распознанного текста
        /// </summary>
        /// <summary>
        /// Определяет имя на основе распознанного текста
        /// </summary>
        private string GetNameFromRecognizedText(string recognizedText)
        {
            // Приводим текст к нижнему регистру для нечувствительного сравнения и убираем пробелы
            string normalizedText = recognizedText.ToLower().Trim();
            if (logTextBox != null)
            {
                logTextBox.AppendText($"  Анализ текста для имени: '{normalizedText}'\r\n");
            }

            // Проверяем специальное условие для "Самылов"
            if (normalizedText.Contains("самылов"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Самылов -> Подп007.tif\r\n");
                }
                return "Самылов";
            }
            // Логика выбора имени - ищем вхождение ключевых слов
            else if (normalizedText.Contains("максимов"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Максимов -> Подп003.tif\r\n");
                }
                return "Максимов";
            }
            else if (normalizedText.Contains("старцев"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Старцев -> Подп002.tif\r\n");
                }
                return "Старцев";
            }
            else if (normalizedText.Contains("русских"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Русских -> Подп004.tif\r\n");
                }
                return "Русских";
            }
            else if (normalizedText.Contains("седюк"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Седюк -> Подп005.tif\r\n");
                }
                return "Седюк";
            }
            else if (normalizedText.Contains("тихомиров"))
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Найдено совпадение: Тихомиров -> Подп006.tif\r\n");
                }
                return "Тихомиров";
            }
            else
            {
                if (logTextBox != null)
                {
                    logTextBox.AppendText($"  Совпадений не найдено, используем имя по умолчанию\r\n");
                }
                // Если текст не распознан или не соответствует ключевым словам
                // Возвращаем имя "Не распознано" если оно существует
                return "Не распознано";
            }
        }


        // ... (остальные методы остаются без изменений) ...
    }

    // ... (остальные классы остаются без изменений) ...

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