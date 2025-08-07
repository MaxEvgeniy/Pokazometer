using System;
using System.IO;
using System.Collections.Generic;
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

        // Вторая дата (из textBox2) будет подставлена вместо этой константы
        // private const string SecondTextToInsert_A4 = "08.08.2025"; 
        private const float SecondTextX_A4 = -257f;
        private const float SecondTextY_A4 = -504f;
        private const int SecondFontSize_A4 = 8;

        private const string ThirdTextToInsert_A4 = "Нач.бюро"; // Исправлено на "Нач.бюро"
        private const float ThirdTextX_A4 = -361f;
        private const float ThirdTextY_A4 = -478f;
        private const int ThirdFontSize_A4 = 16;

        // Четвертая дата (из textBox1) будет подставлена вместо этой константы
        // private const string FourthTextToInsert_A4 = "05.08.2025";
        private const float FourthTextX_A4 = -257f;
        private const float FourthTextY_A4 = -476f;
        private const int FourthFontSize_A4 = 8;

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

        public Form1()
        {
            InitializeComponent();
            // Инициализируем поля ввода значениями по умолчанию
            textBoxPdfFolderPath.Text = DefaultInputPdfFolderPath;
            textBoxFontPath.Text = DefaultFontPath;

            // Инициализируем textBox1 и textBox2 значениями по умолчанию
            textBox1.Text = "08.08.2025"; // Дата Нач.Бюро
            textBox2.Text = "05.08.2025"; // Дата Утвердил

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

                // === Получаем даты из новых текстовых полей ===
                string dateForNachBuro = textBox1.Text.Trim(); // Дата Нач.Бюро
                string dateForUtverdil = textBox2.Text.Trim(); // Дата Утвердил

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
                        LogMessage($"Обработка файла: {System.IO.Path.GetFileName(inputPdfPath)}...");
                        // Передаем даты в метод обработки
                        if (ProcessSinglePdf(inputPdfPath, fontPath, dateForNachBuro, dateForUtverdil))
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
        /// Обрабатывает один PDF файл
        /// </summary>
        /// <param name="inputPdfPath">Путь к входному PDF файлу</param>
        /// <param name="fontPath">Путь к файлу шрифта</param>
        /// <param name="dateForNachBuro">Дата для "Нач.бюро" (из textBox1)</param>
        /// <param name="dateForUtverdil">Дата для "Утвердил" (из textBox2)</param>
        /// <returns>True, если успешно, иначе False</returns>
        private bool ProcessSinglePdf(string inputPdfPath, string fontPath, string dateForNachBuro, string dateForUtverdil)
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
                    const float offsetX_A3 = 767.0f;
                    const float offsetY_A3 = -267.0f;

                    // Определяем смещения в зависимости от формата
                    float offsetX = isA3 ? offsetX_A3 : 0.0f;
                    float offsetY = isA3 ? offsetY_A3 : 0.0f;

                    // Рассчитываем координаты путем прибавления смещения
                    // Для текста "Утвердил"
                    float textX_Pdf = TextX_A4 + offsetX;
                    float textY_Pdf = TextY_A4 + offsetY;

                    // Для даты "Утвердил" (из textBox2)
                    float secondTextX_Pdf = SecondTextX_A4 + offsetX;
                    float secondTextY_Pdf = SecondTextY_A4 + offsetY;

                    // Для текста "Нач.бюро"
                    float thirdTextX_Pdf = ThirdTextX_A4 + offsetX;
                    float thirdTextY_Pdf = ThirdTextY_A4 + offsetY;

                    // Для даты "Нач.бюро" (из textBox1)
                    float fourthTextX_Pdf = FourthTextX_A4 + offsetX;
                    float fourthTextY_Pdf = FourthTextY_A4 + offsetY;

                    // Для изображения 1
                    float image1X_Pdf = Image1X_A4 + offsetX;
                    float image1Y_Pdf = Image1Y_A4 + offsetY;

                    // Для изображения 2
                    float image2X_Pdf = Image2X_A4 + offsetX;
                    float image2Y_Pdf = Image2Y_A4 + offsetY;

                    // Масштабируем размеры изображений
                    float imageScaleX = isA3 ? A4_TO_A3_SCALE_X : 1.0f;
                    float imageScaleY = isA3 ? A4_TO_A3_SCALE_Y : 1.0f;

                    float image1TargetWidth = Image1TargetWidth_A4 * imageScaleX;
                    float image1TargetHeight = Image1TargetHeight_A4 * imageScaleY;
                    float image2TargetWidth = Image2TargetWidth_A4 * imageScaleX;
                    float image2TargetHeight = Image2TargetHeight_A4 * imageScaleY;

                    // 5. Создаем PdfCanvas для рисования текста
                    PdfCanvas textCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // 6. === Загружаем шрифт ===
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

                    // 7. === Вставляем текстовые элементы ===
                    // Утвердил
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FontSize_A4);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(textX_Pdf, textY_Pdf);
                    textCanvas.ShowText(TextToInsert_A4);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // Дата Утвердил (из textBox2)
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SecondFontSize_A4);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(secondTextX_Pdf, secondTextY_Pdf);
                    textCanvas.ShowText(dateForUtverdil); // Используем дату из параметра
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

                    // Дата Нач.бюро (из textBox1)
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FourthFontSize_A4);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(fourthTextX_Pdf, fourthTextY_Pdf);
                    textCanvas.ShowText(dateForNachBuro); // Используем дату из параметра
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    LogMessage("  Текстовые элементы вставлены.");

                    // 8. === Вставляем изображения ===
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

                    LogMessage("  Изображения вставлены.");
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