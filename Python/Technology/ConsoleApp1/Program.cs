using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace WordImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = @"C:\PDF\1\Rename";

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine($"Папка {folderPath} не существует.");
                    Console.WriteLine("Нажмите любую клавищу для выхода...");
                    Console.ReadKey();
                    return;
                }

                // Получаем все файлы .docx с припиской "_Обработано"
                string[] files = Directory.GetFiles(folderPath, "*_Обработано*.docx");

                if (files.Length == 0)
                {
                    Console.WriteLine("Файлы с припиской '_Обработано' не найдены.");
                    Console.WriteLine("Нажмите любую клавищу для выхода...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"Найдено файлов для обработки: {files.Length}");

                foreach (string filePath in files)
                {
                    try
                    {
                        Console.WriteLine($"Обработка файла: {Path.GetFileName(filePath)}");
                        ProcessDocument(filePath);
                        Console.WriteLine($"Файл {Path.GetFileName(filePath)} успешно обработан.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке файла {filePath}: {ex.Message}");
                    }
                }

                Console.WriteLine("Обработка завершена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавищу для выхода...");
            Console.ReadKey();
        }

        static void ProcessDocument(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
            {
                var mainPart = doc.MainDocumentPart;
                var document = mainPart.Document;

                // Обрабатываем все рисунки в документе
                var drawingElements = document.Descendants<Drawing>();

                int imageCount = 0;

                foreach (var drawing in drawingElements)
                {
                    try
                    {
                        // Просто считаем изображения, не изменяя настройки обтекания
                        var anchors = drawing.Descendants<DW.Anchor>();
                        var inlines = drawing.Descendants<DW.Inline>();

                        imageCount += anchors.Count() + inlines.Count();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Ошибка при подсчете изображений: {ex.Message}");
                    }
                }

                Console.WriteLine($"  Найдено изображений: {imageCount}");

                // Сохраняем изменения (даже если не изменяли ничего, это помогает избежать ошибок)
                document.Save();
            }
        }
    }
}