using System;
using System.IO;
using System.Drawing;
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
        private static readonly string OutputFolder = @"C:\PDF\1\Rename\Processed";
        private static readonly string ImagesFolder = @"C:\PDF\1\Rename"; // Папка с изображениями

        // Координаты для считывания текста (x, y, width, height)
        private static readonly float TextX = 557f;
        private static readonly float TextY = 103f;
        private static readonly float TextWidth = 100f;
        private static readonly float TextHeight = 10f;

        // Координаты и размеры для вставки изображений
        private static readonly float SignatureX = 640f;
        private static readonly float SignatureY = 98f;
        private static readonly float SignatureWidth = 110f;
        private static readonly float SignatureHeight = 40f;

        private static readonly float Stamp1X = 640f;
        private static readonly float Stamp1Y = 83f;
        private static readonly float Stamp1Width = 110f;
        private static readonly float Stamp1Height = 40f;

        private static readonly float Stamp2X = 640f;
        private static readonly float Stamp2Y = 57f;
        private static readonly float Stamp2Width = 130f;
        private static readonly float Stamp2Height = 50f;

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
                string fileName = System.IO.Path.GetFileNameWithoutExtension(inputPath);
                string outputPath = System.IO.Path.Combine(OutputFolder, $"{fileName}_Подписано.pdf");

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

                            // Рисуем красную рамку (для настройки позиции) - только на первой странице
                            //DrawRedFrame(stamper, 1, TextX, TextY, TextWidth, TextHeight);

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
                                string stamp1Path = System.IO.Path.Combine(ImagesFolder, "Подп002.tif");
                                if (File.Exists(stamp1Path))
                                {
                                    InsertImage(stamper, page, stamp1Path, Stamp1X, Stamp1Y, Stamp1Width, Stamp1Height);
                                }

                                // Вставляем фиксированное изображение 2
                                string stamp2Path = System.IO.Path.Combine(ImagesFolder, "Подп001.tif");
                                if (File.Exists(stamp2Path))
                                {
                                    InsertImage(stamper, page, stamp2Path, Stamp2X, Stamp2Y, Stamp2Width, Stamp2Height);
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Файл сохранен: {System.IO.Path.GetFileName(outputPath)}");
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

        private static void DrawRedFrame(PdfStamper stamper, int pageNumber, float x, float y, float width, float height)
        {
            try
            {
                PdfContentByte canvas = stamper.GetOverContent(pageNumber);
                canvas.SetColorStroke(BaseColor.RED);
                canvas.SetLineWidth(2f);

                // Рисуем прямоугольник
                canvas.Rectangle(x, y, width, height);
                canvas.Stroke();

                Console.WriteLine($"Красная рамка нарисована на странице {pageNumber} в координатах ({x}, {y}, {width}, {height})");
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
                    string imagePath = System.IO.Path.Combine(ImagesFolder, kvp.Value);
                    if (File.Exists(imagePath))
                    {
                        Console.WriteLine($"Найдено ключевое слово: {kvp.Key}, изображение: {kvp.Value}");
                        return imagePath;
                    }
                    else
                    {
                        Console.WriteLine($"Изображение не найдено: {imagePath}");
                        return null;
                    }
                }
            }

            Console.WriteLine("Ключевые слова не найдены в тексте");
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

                // Устанавливаем позицию и размер
                img.SetAbsolutePosition(x, y);
                img.ScaleAbsolute(width, height);

                PdfContentByte canvas = stamper.GetOverContent(pageNumber);
                canvas.AddImage(img);

                Console.WriteLine($"Изображение {System.IO.Path.GetFileName(imagePath)} вставлено на страницу {pageNumber} в координатах ({x}, {y})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка вставки изображения {imagePath}: {ex.Message}");
            }
        }
    }
}