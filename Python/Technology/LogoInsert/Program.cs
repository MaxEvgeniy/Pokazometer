using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace LogoInsert
{
    class Program
    {
        // Настройки путей
        private static readonly string InputFolder = @"C:\PDF\1\Rename";
        private static readonly string OutputFolder = @"C:\PDF\1\Rename\Stamped";
        private static readonly string ImagesFolder = @"C:\PDF\1\Rename"; // Папка с изображениями

        // Координаты и размеры для вставки изображений
        private static readonly float SignatureX = 720f;
        private static readonly float SignatureY = 115f;
        private static readonly float SignatureWidth = 100f;
        private static readonly float SignatureHeight = 100f;

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
                string[] pdfFiles = Directory.GetFiles(InputFolder, "*.pdf");

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
                string outputPath = System.IO.Path.Combine(OutputFolder, $"{fileName}.pdf");

                Console.WriteLine($"Обработка файла: {fileName}.pdf");

                using (PdfReader reader = new PdfReader(inputPath))
                {
                    using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, fs))
                        {
                            // Обрабатываем каждую страницу
                            for (int page = 1; page <= reader.NumberOfPages; page++)
                            {
                                // Вставляем фиксированное изображение 1
                                string stamp1Path = System.IO.Path.Combine(ImagesFolder, "Печать.tif");
                                if (File.Exists(stamp1Path))
                                {
                                    InsertImage(stamper, page, stamp1Path, SignatureX, SignatureY, SignatureWidth, SignatureHeight);
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
