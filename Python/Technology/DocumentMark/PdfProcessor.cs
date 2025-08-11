using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
// === iText 9 ===
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
// === ��� ������ � �������� ���������� ��������� ===
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace DocumentMark
{
    public class PdfProcessor
    {
        // === ��������� ������������ ������� ����� "������." ===
        private const float Text_OffsetFromReferenceX = 55f;
        private const float Text_OffsetFromReferenceY = 0f;
        private const int FontSize_Main = 16;

        private const float SecondText_OffsetFromReferenceX = 159f;
        private const float SecondText_OffsetFromReferenceY = -2f;
        private const int SecondFontSize_Date = 8;

        private const float ThirdText_OffsetFromReferenceX = 55f;
        private const float ThirdText_OffsetFromReferenceY = 28f;
        private const int ThirdFontSize_Main = 16;

        private const float FourthText_OffsetFromReferenceX = 159f;
        private const float FourthText_OffsetFromReferenceY = 26f;
        private const int FourthFontSize_Date = 8;

        private const float SixthText_OffsetFromReferenceX = 159f;
        private const float SixthText_OffsetFromReferenceY = 73f;
        private const int SixthFontSize_Date = 8;

        private const float Image1_OffsetFromReferenceX = 90f;
        private const float Image1_OffsetFromReferenceY = -15f;
        private const float Image1_TargetWidth = 90f;
        private const float Image1_TargetHeight = 40f;
        private const string ImagePath1 = @"C:\PDF\1\Rename\����001.tif";

        private const float Image2_OffsetFromReferenceX = 105f;
        private const float Image2_OffsetFromReferenceY = 15f;
        private const float Image2_TargetWidth = 80f;
        private const float Image2_TargetHeight = 50f;
        private const string ImagePath2 = @"C:\PDF\1\Rename\����002.tif";

        private const float Image3_OffsetFromReferenceX = 105f; // ������, ���������
        private const float Image3_OffsetFromReferenceY = 62f;  // ������, ���������
        private const float Image3_TargetWidth = 80f;          // ������, ���������
        private const float Image3_TargetHeight = 50f;         // ������, ���������
        private const string ImagePath3_Default = @"C:\PDF\1\Rename\����003.tif";
        private const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\����002.tif";
        private const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\����006.tif";
        private const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\����005.tif";
        private const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\����004.tif";

        // === ��������� ��� ���� ���������� ������ ===
        private const float ReadArea_OffsetFromReferenceX = 55f;    // ������, ��������� (������������ ������� ����� "������.")
        private const float ReadArea_OffsetFromReferenceY = 73f;   // ������, ��������� (������������ ������� ����� "������.")
        private const float ReadArea_Width = 100f;                 // ������, ���������
        private const float ReadArea_Height = 15f;                // ������, ���������

        /// <summary>
        /// ���������� ��������� ������� ���� (�� ������� � �� �����������) � �������� ���������
        /// </summary>
        public string GetRandomWorkday(DateTime startDate, DateTime endDate)
        {
            Random random = new Random();
            List<DateTime> workdays = new List<DateTime>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workdays.Add(date);
                }
            }

            if (workdays.Count == 0)
            {
                TimeSpan diff = endDate - startDate;
                DateTime randomDate = startDate.AddDays(random.Next((int)diff.TotalDays + 1));
                return randomDate.ToString("dd.MM.yyyy");
            }

            int randomIndex = random.Next(workdays.Count);
            return workdays[randomIndex].ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// ������������ ���� PDF ����
        /// </summary>
        public bool ProcessSinglePdf(string inputPdfPath, string fontPath, string dateForNachBuro, string dateForUtverdil, string dateForRazrabotal)
        {
            try
            {
                string outputPdfPath = Path.Combine(
                    Path.GetDirectoryName(inputPdfPath),
                    Path.GetFileNameWithoutExtension(inputPdfPath) + "_edited.pdf"
                );

                using (var reader = new PdfReader(inputPdfPath))
                using (var writer = new PdfWriter(outputPdfPath))
                using (var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader, writer)) // ���������� ������ ���
                {
                    var page = pdfDoc.GetFirstPage();
                    if (page == null)
                    {
                        // LogMessage($"  ������: �� ������� �������� ������ �������� PDF '{Path.GetFileName(inputPdfPath)}'.");
                        return false;
                    }

                    var pageSize = page.GetPageSize();
                    float pageWidth = pageSize.GetWidth();
                    float pageHeight = pageSize.GetHeight();
                    // LogMessage($"  ������ ��������: {pageWidth:F1} x {pageHeight:F1} �����");

                    // === ����� ������� ����� "������." ===
                    float referenceX = 0;
                    float referenceY = 0;
                    bool referenceFound = false;

                    try
                    {
                        // LogMessage("  ����� ������� ����� '������.'...");

                        var listener = new CustomTextEventListener();
                        var parser = new PdfCanvasProcessor(listener);
                        parser.ProcessContent(page.GetContentBytes(), page.GetResources());

                        var foundTextItems = listener.FindAllText("������.");
                        if (foundTextItems != null && foundTextItems.Count > 0)
                        {
                            var foundText = foundTextItems[0];
                            referenceX = foundText.Rect.GetX();
                            referenceY = foundText.Rect.GetY();
                            referenceFound = true;
                            // LogMessage($"  ������� ����� '������.' �������: X={referenceX:F2}, Y={referenceY:F2}");
                        }
                        else
                        {
                            // LogMessage("  ������� ����� '������.' �� �������. ������������ ���������� (0, 0) �� ���������.");
                        }
                    }
                    catch (Exception /*refEx*/)
                    {
                        // LogMessage($"  ������ ������ ������� ����� '������.': {refEx.Message}. ������������ ���������� (0, 0) �� ���������.");
                    }
                    // === �����: ����� ������� ����� ===

                    // === ��������� ������������ ������� ����� "������." ===
                    // === ����������: �������� �� ������� ����� (referenceX, referenceY) ===

                    // --- "��������" ---
                    float textX_Pdf = referenceX + Text_OffsetFromReferenceX;
                    float textY_Pdf = referenceY + Text_OffsetFromReferenceY;
                    const int FontSize_Main = 16;

                    // --- "������������" ---
                    float secondTextX_Pdf = referenceX + SecondText_OffsetFromReferenceX;
                    float secondTextY_Pdf = referenceY + SecondText_OffsetFromReferenceY;
                    const int SecondFontSize_Date = 8;

                    // --- "���.����" ---
                    float thirdTextX_Pdf = referenceX + ThirdText_OffsetFromReferenceX;
                    float thirdTextY_Pdf = referenceY + ThirdText_OffsetFromReferenceY;
                    const int ThirdFontSize_Main = 16;

                    // --- "���.��������" ---
                    float fourthTextX_Pdf = referenceX + FourthText_OffsetFromReferenceX;
                    float fourthTextY_Pdf = referenceY + FourthText_OffsetFromReferenceY;
                    const int FourthFontSize_Date = 8;

                    // --- "��������������" ---
                    float sixthTextX_Pdf = referenceX + SixthText_OffsetFromReferenceX;
                    float sixthTextY_Pdf = referenceY + SixthText_OffsetFromReferenceY;
                    const int SixthFontSize_Date = 8;

                    // --- ����������� 1 (����001.tif) ---
                    float image1X_Pdf = referenceX + Image1_OffsetFromReferenceX;
                    float image1Y_Pdf = referenceY + Image1_OffsetFromReferenceY;
                    const float Image1_TargetWidth = 90f;
                    const float Image1_TargetHeight = 40f;
                    const string ImagePath1 = @"C:\PDF\1\Rename\����001.tif";

                    // --- ����������� 2 (����002.tif) ---
                    float image2X_Pdf = referenceX + Image2_OffsetFromReferenceX;
                    float image2Y_Pdf = referenceY + Image2_OffsetFromReferenceY;
                    const float Image2_TargetWidth = 80f;
                    const float Image2_TargetHeight = 50f;
                    const string ImagePath2 = @"C:\PDF\1\Rename\����002.tif";

                    // --- ����������� 3 (����003.tif � ������������) ---
                    float image3X_Pdf = referenceX + Image3_OffsetFromReferenceX;
                    float image3Y_Pdf = referenceY + Image3_OffsetFromReferenceY;
                    const float Image3_TargetWidth = 80f;
                    const float Image3_TargetHeight = 50f;
                    const string ImagePath3_Default = @"C:\PDF\1\Rename\����003.tif";
                    const string ImagePath3_Alt1 = @"C:\PDF\1\Rename\����002.tif";
                    const string ImagePath3_Alt2 = @"C:\PDF\1\Rename\����006.tif";
                    const string ImagePath3_Alt3 = @"C:\PDF\1\Rename\����005.tif";
                    const string ImagePath3_Alt4 = @"C:\PDF\1\Rename\����004.tif";

                    // --- ���� ���������� ������ ---
                    float readAreaX_Pdf = referenceX + ReadArea_OffsetFromReferenceX;
                    float readAreaY_Pdf = referenceY + ReadArea_OffsetFromReferenceY;
                    const float ReadArea_Width = 100f;
                    const float ReadArea_Height = 15f;

                    // === ������ ���������� ��������� ������������ ������� ����� ===

                    // --- ������ ���������� ��������� X (�� ������ ����) ---
                    // ��� ���������� ����

                    // --- ������ ���������� ��������� Y (�� ������� ����) ---
                    // � PDF (0,0) � ������ ����� ����, Y ������ �����
                    // ��� ���������� ����

                    // --- ������� ��� ���� ���������� ---
                    // ��� ���������� ����

                    // 5. === ������� PdfCanvas ��� ��������� ��������������� ===
                    PdfCanvas canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // === ������: ��������� ��������������� ===
                    // ������������� 1 
                    float rect1X = referenceX + 106f;
                    float rect1Y = referenceY + 13f;
                    float rect1Width = 27f;
                    float rect1Height = 13f;

                    // ������������� 2 
                    float rect2X = referenceX + 135.4f;
                    float rect2Y = referenceY + 13f;
                    float rect2Width = 5f;
                    float rect2Height = 13f;

                    // ������ ������ ������������� (�����, �����������)
                    canvas.SaveState();
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);
                    canvas.Rectangle(rect1X, rect1Y, rect1Width, rect1Height);
                    canvas.Fill();
                    canvas.RestoreState();

                    // ������ ������ ������������� (�����, �����������)
                    canvas.SaveState();
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);
                    canvas.Rectangle(rect2X, rect2Y, rect2Width, rect2Height);
                    canvas.Fill();
                    canvas.RestoreState();

                    // === �����: ������� ������������� ��� ���� ���������� ===
                    canvas.SaveState();
                    canvas.SetStrokeColorRgb(1.0f, 0.0f, 0.0f);
                    canvas.SetFillColorRgb(1.0f, 1.0f, 1.0f);
                    canvas.SetLineWidth(1);
                    canvas.Rectangle(readAreaX_Pdf, readAreaY_Pdf, ReadArea_Width, ReadArea_Height);
                    canvas.FillStroke();
                    canvas.RestoreState();

                    // LogMessage("  �������������� ���������� (������� ������� ��� ���� ����������).");
                    // === �����: ��������� ��������������� ===

                    // === �����: ���������� ������ �� ������� ===
                    string readText = "";
                    try
                    {
                        // LogMessage($"  ������� ���������� ������ �� �������: X={readAreaX_Pdf:F1}, Y={readAreaY_Pdf:F1}, W={ReadArea_Width:F1}, H={ReadArea_Height:F1}");

                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                        string allText = PdfTextExtractor.GetTextFromPage(page, strategy);

                        if (!string.IsNullOrEmpty(allText))
                        {
                            string[] lines = allText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in lines)
                            {
                                if (line.IndexOf("�������", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("��������", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("���������", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("�����", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    line.IndexOf("�������", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    if (line.IndexOf("�������", StringComparison.OrdinalIgnoreCase) >= 0) readText = "�������";
                                    else if (line.IndexOf("��������", StringComparison.OrdinalIgnoreCase) >= 0) readText = "��������";
                                    else if (line.IndexOf("���������", StringComparison.OrdinalIgnoreCase) >= 0) readText = "���������";
                                    else if (line.IndexOf("�����", StringComparison.OrdinalIgnoreCase) >= 0) readText = "�����";
                                    else if (line.IndexOf("�������", StringComparison.OrdinalIgnoreCase) >= 0) readText = "�������";
                                    break;
                                }
                            }
                        }

                        readText = readText.Trim();
                        // LogMessage($"  ��������� ����� (����������� �����): '{readText}'");
                    }
                    catch (Exception /*readEx*/)
                    {
                        // LogMessage($"  ������ ���������� ������: {readEx.Message}");
                        readText = "";
                    }
                    // === �����: ���������� ������ �� ������� ===

                    // === �����: ����������� ���� � ����������� 3 �� ������ ���������� ������ ===
                    string imagePath3_Selected = "";
                    try
                    {
                        if (readText.Equals("�������", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt1;
                            // LogMessage($"  ���������� ����������� ��� �������: ����002.tif (�� ����� '�������')");
                        }
                        else if (readText.Equals("��������", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Default;
                            // LogMessage($"  ���������� ����������� ��� �������: ����003.tif (�� ����� '��������')");
                        }
                        else if (readText.Equals("���������", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt2;
                            // LogMessage($"  ���������� ����������� ��� �������: ����006.tif (�� ����� '���������')");
                        }
                        else if (readText.Equals("�����", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt3;
                            // LogMessage($"  ���������� ����������� ��� �������: ����005.tif (�� ����� '�����')");
                        }
                        else if (readText.Equals("�������", StringComparison.OrdinalIgnoreCase))
                        {
                            imagePath3_Selected = ImagePath3_Alt4;
                            // LogMessage($"  ���������� ����������� ��� �������: ����004.tif (�� ����� '�������')");
                        }
                        else
                        {
                            imagePath3_Selected = "";
                            if (string.IsNullOrEmpty(readText))
                            {
                                // LogMessage($"  ����� �� ������ ��� ����. ����������� 3 ��������� �� �����.");
                            }
                            else
                            {
                                // LogMessage($"  ������� ����������� ����� '{readText}'. ����������� 3 ��������� �� �����.");
                            }
                        }
                    }
                    catch (Exception /*selectEx*/)
                    {
                        // LogMessage($"  ������ ����������� �����������: {selectEx.Message}");
                        imagePath3_Selected = "";
                    }
                    // === �����: ����������� ���� � ����������� 3 ===

                    // 6. === ������� ����� PdfCanvas ��� ��������� ������ ===
                    PdfCanvas textCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

                    // 7. === ��������� ����� ===
                    PdfFont font;
                    try
                    {
                        if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
                        {
                            byte[] fontBytes = File.ReadAllBytes(fontPath);
                            font = PdfFontFactory.CreateFont(fontBytes, "Identity-H");
                            // LogMessage($"  ����� ��������: {Path.GetFileName(fontPath)}");
                        }
                        else
                        {
                            font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
                            // LogMessage($"  ����� �� ������. ������������ Helvetica-Oblique.");
                        }
                    }
                    catch (Exception /*fontEx*/)
                    {
                        font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
                        // LogMessage($"  ������ �������� ������: {fontEx.Message}. ������������ Helvetica-Oblique.");
                    }

                    // 8. === ��������� ��������� �������� ===
                    // ��������
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FontSize_Main);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(textX_Pdf, textY_Pdf);
                    textCanvas.ShowText("��������");
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // ������������
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SecondFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(secondTextX_Pdf, secondTextY_Pdf);
                    textCanvas.ShowText(dateForUtverdil);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // ���.����
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, ThirdFontSize_Main);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(thirdTextX_Pdf, thirdTextY_Pdf);
                    textCanvas.ShowText("���.����");
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // ���.��������
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, FourthFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(fourthTextX_Pdf, fourthTextY_Pdf);
                    textCanvas.ShowText(dateForNachBuro);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // ��������������
                    textCanvas.SaveState();
                    textCanvas.BeginText();
                    textCanvas.SetFontAndSize(font, SixthFontSize_Date);
                    textCanvas.SetFillColor(ColorConstants.BLACK);
                    textCanvas.MoveText(sixthTextX_Pdf, sixthTextY_Pdf);
                    textCanvas.ShowText(dateForRazrabotal);
                    textCanvas.EndText();
                    textCanvas.RestoreState();

                    // LogMessage("  ��������� �������� ���������.");

                    // 9. === ��������� ����������� ===
                    // ����������� 1
                    try
                    {
                        if (File.Exists(ImagePath1))
                        {
                            ImageData imageData1 = ImageDataFactory.Create(ImagePath1);
                            if (imageData1 != null)
                            {
                                Image imageElement1 = new Image(imageData1); // ���������� ���������
                                imageElement1.SetFixedPosition(image1X_Pdf, image1Y_Pdf);
                                imageElement1.SetWidth(Image1_TargetWidth);
                                imageElement1.SetHeight(Image1_TargetHeight);

                                Canvas layoutCanvas1 = new Canvas(page, page.GetPageSize());
                                layoutCanvas1.Add(imageElement1);
                                layoutCanvas1.Close();
                            }
                            else
                            {
                                // LogMessage("  ������: �� ������� ��������� ������ ����������� 1.");
                            }
                        }
                        else
                        {
                            // LogMessage($"  ���� ����������� 1 �� ������: {Path.GetFileName(ImagePath1)}");
                        }
                    }
                    catch (Exception /*imgEx*/)
                    {
                        // LogMessage($"  ������ ������� ����������� 1: {imgEx.Message}");
                    }

                    // ����������� 2
                    try
                    {
                        if (File.Exists(ImagePath2))
                        {
                            ImageData imageData2 = ImageDataFactory.Create(ImagePath2);
                            if (imageData2 != null)
                            {
                                Image imageElement2 = new Image(imageData2); // ���������� ���������
                                imageElement2.SetFixedPosition(image2X_Pdf, image2Y_Pdf);
                                imageElement2.SetWidth(Image2_TargetWidth);
                                imageElement2.SetHeight(Image2_TargetHeight);

                                Canvas layoutCanvas2 = new Canvas(page, page.GetPageSize());
                                layoutCanvas2.Add(imageElement2);
                                layoutCanvas2.Close();
                            }
                            else
                            {
                                // LogMessage("  ������: �� ������� ��������� ������ ����������� 2.");
                            }
                        }
                        else
                        {
                            // LogMessage($"  ���� ����������� 2 �� ������: {Path.GetFileName(ImagePath2)}");
                        }
                    }
                    catch (Exception /*imgEx*/)
                    {
                        // LogMessage($"  ������ ������� ����������� 2: {imgEx.Message}");
                    }

                    // === �����: ����������� 3 (� �������� �������) ===
                    if (!string.IsNullOrEmpty(imagePath3_Selected) && File.Exists(imagePath3_Selected))
                    {
                        try
                        {
                            ImageData imageData3 = ImageDataFactory.Create(imagePath3_Selected);
                            if (imageData3 != null)
                            {
                                Image imageElement3 = new Image(imageData3); // ���������� ���������
                                imageElement3.SetFixedPosition(image3X_Pdf, image3Y_Pdf);
                                imageElement3.SetWidth(Image3_TargetWidth);
                                imageElement3.SetHeight(Image3_TargetHeight);

                                Canvas layoutCanvas3 = new Canvas(page, page.GetPageSize());
                                layoutCanvas3.Add(imageElement3);
                                layoutCanvas3.Close();

                                // LogMessage($"  ����������� 3 ���������: {Path.GetFileName(imagePath3_Selected)}");
                            }
                            else
                            {
                                // LogMessage("  ������: �� ������� ��������� ������ ����������� 3.");
                            }
                        }
                        catch (Exception /*imgEx*/)
                        {
                            // LogMessage($"  ������ ������� ����������� 3: {imgEx.Message}");
                        }
                    }
                    else if (!string.IsNullOrEmpty(imagePath3_Selected))
                    {
                        // LogMessage($"  ���� ����������� 3 �� ������: {Path.GetFileName(imagePath3_Selected)}");
                    }
                    // ���� imagePath3_Selected ������ ������, ������ �� ������ - ����������� �� �����������

                    // LogMessage("  ��������� ����������� ���������.");
                }

                // LogMessage($"  ���� �������� ���: {Path.GetFileName(outputPdfPath)}");
                return true;

            }
            catch (Exception ex)
            {
                // LogMessage($"  ������ ��������� ����� '{Path.GetFileName(inputPdfPath)}': {ex.Message}");
                return false;
            }
        }
    }
}