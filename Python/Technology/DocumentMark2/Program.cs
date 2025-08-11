using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFSquareDrawer
{
    public partial class Form1 : Form
    {
        private Button drawButton;
        private Label statusLabel;
        private ComboBox fontComboBox;
        private bool isRussian = true; // true - русский, false - английский

        // Константы для изображений (от правого нижнего края)
        private const float IMAGE1_OFFSET_X = 425f; // отступ от правого края влево
        private const float IMAGE1_OFFSET_Y = 80f; // отступ от нижнего края вверх
        private const float IMAGE2_OFFSET_X = 425f;
        private const float IMAGE2_OFFSET_Y = 40f;
        private const float IMAGE3_OFFSET_X = 425f;
        private const float IMAGE3_OFFSET_Y = 8f;

        private const float IMAGE_WIDTH = 90f;
        private const float IMAGE_HEIGHT = 40f;

        // Константы для текста (от правого нижнего края)
        private const float TEXT_OFFSET_X = 490f; // отступ от правого края влево
        private const float TEXT_OFFSET_Y = 87f;  // отступ от нижнего края вверх

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

        private string[] imageFiles = { "Подп001.tif", "Подп002.tif", "Подп003.tif" };

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "PDF Square Drawer";
            this.Size = new Size(500, 400);
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
            statusLabel.Size = new Size(400, 60);
            statusLabel.Location = new Point(50, currentY);
            statusLabel.Text = "Готово к работе";

            this.Controls.Add(drawButton);
            this.Controls.Add(toggleLanguageButton);
            this.Controls.Add(fontLabel);
            this.Controls.Add(fontComboBox);
            this.Controls.Add(statusLabel);
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

            // Проверяем, существуют ли файлы изображений
            bool imagesExist = true;
            string missingImages = "";
            foreach (string imageFile in imageFiles)
            {
                string imagePath = Path.Combine(folderPath, imageFile);
                if (!File.Exists(imagePath))
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
                        ProcessPdfFile(filePath);
                        processedCount++;
                        statusLabel.Text = $"Обработано: {processedCount}/{pdfFiles.Length}";
                        this.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка обработки файла {Path.GetFileName(filePath)}: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                statusLabel.Text = $"Готово! Обработано: {processedCount}";
                MessageBox.Show($"Обработка завершена!\nОбработано файлов: {processedCount}",
                    "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Ошибка!";
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

                        // Рисуем прямоугольники
                        DrawRectangles(canvas, pageSize);

                        // Выбираем текст в зависимости от языка
                        string mainText = isRussian ? "Разработал" : "Developed by";
                        string chiefText = isRussian ? "Нач.Бюро" : "Chief";
                        string approvedText = isRussian ? "Утвердил" : "Approved";

                        AddMainText(canvas, pageSize, mainText, TEXT_OFFSET_X, TEXT_OFFSET_Y);
                        AddChiefText(canvas, pageSize, chiefText, 200f, TEXT_OFFSET_Y);
                        AddApprovedText(canvas, pageSize, approvedText, 100f, TEXT_OFFSET_Y);

                        // Добавляем даты
                        AddDateText(canvas, pageSize, DATE_DEVELOPED_VALUE, DATE_DEVELOPED_OFFSET_X, DATE_DEVELOPED_OFFSET_Y);
                        AddDateText(canvas, pageSize, DATE_CHIEF_VALUE, DATE_CHIEF_OFFSET_X, DATE_CHIEF_OFFSET_Y);
                        AddDateText(canvas, pageSize, DATE_APPROVED_VALUE, DATE_APPROVED_OFFSET_X, DATE_APPROVED_OFFSET_Y);

                        // Добавляем изображения
                        string folderPath = Path.GetDirectoryName(filePath);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, imageFiles[0]), IMAGE1_OFFSET_X, IMAGE1_OFFSET_Y);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, imageFiles[1]), IMAGE2_OFFSET_X, IMAGE2_OFFSET_Y);
                        AddImageToPdf(canvas, pageSize, Path.Combine(folderPath, imageFiles[2]), IMAGE3_OFFSET_X, IMAGE3_OFFSET_Y);
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

        private void DrawRectangles(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize)
        {
            // Устанавливаем красный цвет для прямоугольников
            canvas.SetColorFill(BaseColor.RED);
            canvas.SetColorStroke(BaseColor.RED);

            // Рисуем первый прямоугольник
            float rect1X = pageSize.Right - RECT1_OFFSET_X;
            float rect1Y = pageSize.Bottom + RECT1_OFFSET_Y;
            canvas.Rectangle(rect1X, rect1Y, RECT1_WIDTH, RECT1_HEIGHT);
            canvas.Fill();

            // Рисуем второй прямоугольник
            float rect2X = pageSize.Right - RECT2_OFFSET_X;
            float rect2Y = pageSize.Bottom + RECT2_OFFSET_Y;
            canvas.Rectangle(rect2X, rect2Y, RECT2_WIDTH, RECT2_HEIGHT);
            canvas.Fill();
        }

        private void AddMainText(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string text, float offsetX, float offsetY)
        {
            AddTextWithFontFromBottomRight(canvas, pageSize, text, offsetX, offsetY, 14);
        }

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
            }
            catch (Exception ex)
            {
                // Просто игнорируем ошибки с изображениями, чтобы не прерывать обработку PDF
                Console.WriteLine($"Ошибка при добавлении изображения {imagePath}: {ex.Message}");
            }
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