using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFProcessor
{
    class Program
    {
        // Настройки путей
        private static readonly string InputFolder = @"C:\PDF\1\Rename";
        private static readonly string OutputFolder = @"C:\PDF\1\Rename";
        private static readonly string ImagesFolder = @"C:\PDF\1\Rename"; // Папка с изображениями

        // Координаты для считывания текста (x, y, width, height)
        private static readonly float TextX = 100f;
        private static readonly float TextY = 700f;
        private static readonly float TextWidth = 200f;
        private static readonly float TextHeight = 50f;

        // Координаты и размеры для вставки изображений
        private static readonly float SignatureX = 150f;
        private static readonly float SignatureY = 50f;
        private static readonly float SignatureWidth = 100f;
        private static readonly float SignatureHeight = 30f;

        private static readonly float Stamp1X = 50f;
        private static readonly float Stamp1Y = 50f;
        private static readonly float Stamp1Width = 80f;
        private static readonly float Stamp1Height = 25f;

        private static readonly float Stamp2X = 300f;
        private static readonly float Stamp2Y = 50f;
        private static readonly float Stamp2Width = 80f;
        private static readonly float Stamp2Height = 25f;

        static void Main(string[] args)
        {
            try
            {
                // Создаем папку для обработанных файлов
                if (!Directory.Exists(OutputFolder))
                {
                    Directory.CreateDirectory(OutputFolder);
                }

                // Получаем все PDF файлы с нужной припиской
                string[] pdfFiles = Directory.GetFiles(InputFolder, "*_(Технология)_Обработано.pdf");

                Console.WriteLine($"Найдено файлов для обработки: {pdfFiles.Length}");

                foreach (string pdfFile in pdfFiles)
                {
                    ProcessPdfFile(pdfFile);
                }

                Console.WriteLine("Обработка завершена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static void ProcessPdfFile(string inputPath)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(inputPath);
                string outputPath = Path.Combine(OutputFolder, $"{fileName}_Подписано.pdf");

                Console.WriteLine($"Обработка файла: {fileName}.pdf");

                using (PdfReader reader = new PdfReader(inputPath))
                {
                    using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, fs))
                        {
                            // Получаем текст из первой страницы
                            string recognizedText = ExtractTextFromRegion(reader, 1, TextX, TextY, TextWidth, TextHeight);
                            Console.WriteLine($"Распознанный текст: {recognizedText}");

                            // Рисуем красную рамку (для настройки позиции)
                            DrawRedFrame(stamper, reader, TextX, TextY, TextWidth, TextHeight);

                            // Определяем изображение подписи по распознанному тексту
                            string signatureImage = GetSignatureImageByKeyword(recognizedText);

                            // Обрабатываем каждую страницу
                            for (int page = 1; page <= reader.NumberOfPages; page++)
                            {
                                // Вставляем подпись по ключевому слову
                                if (!string.IsNullOrEmpty(signatureImage))
                                {
                                    InsertImage(stamper, page, signatureImage, SignatureX, SignatureY, SignatureWidth, SignatureHeight);
                                }

                                // Вставляем фиксированное изображение 1
                                string stamp1Path = Path.Combine(ImagesFolder, "Подп002.tif");
                                if (File.Exists(stamp1Path))
                                {
                                    InsertImage(stamper, page, stamp1Path, Stamp1X, Stamp1Y, Stamp1Width, Stamp1Height);
                                }

                                // Вставляем фиксированное изображение 2
                                string stamp2Path = Path.Combine(ImagesFolder, "Подп001.tif");
                                if (File.Exists(stamp2Path))
                                {
                                    InsertImage(stamper, page, stamp2Path, Stamp2X, Stamp2Y, Stamp2Width, Stamp2Height);
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Файл сохранен: {Path.GetFileName(outputPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке файла {inputPath}: {ex.Message}");
            }
        }

        private static string ExtractTextFromRegion(PdfReader reader, int pageNumber, float x, float y, float width, float height)
        {
            try
            {
                Rectangle rect = new Rectangle(x, y, x + width, y + height);
                RenderFilter[] renderFilters = { new RegionTextRenderFilter(rect) };

                ITextExtractionStrategy strategy = new FilteredTextRenderListener(
                    new LocationTextExtractionStrategy(), renderFilters);

                string text = PdfTextExtractor.GetTextFromPage(reader, pageNumber, strategy);
                return text?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка извлечения текста: {ex.Message}");
                return string.Empty;
            }
        }

        private static void DrawRedFrame(PdfStamper stamper, PdfReader reader, float x, float y, float width, float height)
        {
            try
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    PdfContentByte canvas = stamper.GetOverContent(page);
                    canvas.SetColorStroke(BaseColor.RED);
                    canvas.SetLineWidth(1f);
                    canvas.Rectangle(x, y, width, height);
                    canvas.Stroke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка рисования рамки: {ex.Message}");
            }
        }

        private static string GetSignatureImageByKeyword(string text)
        {
            var keywordMap = new Dictionary<string, string>
            {
                { "Максимов", "Подп003.tif" },
                { "Старцев", "Подп002.tif" },
                { "Тихомиров", "Подп006.tif" },
                { "Седюк", "Подп005.tif" },
                { "Русских", "Подп004.tif" },
                { "Самылов", "Подп007.tif" }
            };

            foreach (var kvp in keywordMap)
            {
                if (text.Contains(kvp.Key))
                {
                    string imagePath = Path.Combine(ImagesFolder, kvp.Value);
                    return File.Exists(imagePath) ? imagePath : null;
                }
            }

            return null;
        }

        private static void InsertImage(PdfStamper stamper, int pageNumber, string imagePath, float x, float y, float width, float height)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"Изображение не найдено: {imagePath}");
                    return;
                }

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagePath);
                img.SetAbsolutePosition(x, y);
                img.ScaleToFit(width, height);

                PdfContentByte canvas = stamper.GetOverContent(pageNumber);
                canvas.AddImage(img);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка вставки изображения {imagePath}: {ex.Message}");
            }
        }
    }
}