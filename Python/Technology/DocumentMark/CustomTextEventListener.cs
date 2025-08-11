using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Geom; // ���������� ������ ���

namespace DocumentMark
{
    /// <summary>
    /// ���������������� ��������� ������� ��� ���������� ������ � ������������
    /// ����������: ��� ������ � iText 9 � LineSegment
    /// </summary>
    public class CustomTextEventListener : IEventListener
    {
        private readonly List<TextItem> _textItems = new List<TextItem>();

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_TEXT)
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                string text = renderInfo.GetText();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        iText.Kernel.Geom.Rectangle textRect = null; // ���������� ������ ���

                        try
                        {
                            // === ����������: CS1061 - ���������� ������ ��������� ��������������� �������������� � iText 9 ===
                            // ������ 1: ���������� GetDescentLine().GetBoundingRectangle()
                            // ��� ����������� ������ � iText 7/9
                            var descentLine = renderInfo.GetDescentLine();
                            if (descentLine != null)
                            {
                                textRect = descentLine.GetBoundingRectangle(); // <-- ���������� �����
                            }
                        }
                        catch (MissingMethodException)
                        {
                            // ���� GetBoundingRectangle() ����������, ��������� ������ ������
                            try
                            {
                                // ������ 2: ��������� �������� ���������� �� StartPoint � EndPoint
                                var descentLine = renderInfo.GetDescentLine();
                                if (descentLine != null)
                                {
                                    var startPoint = descentLine.GetStartPoint();
                                    var endPoint = descentLine.GetEndPoint();

                                    if (startPoint != null && endPoint != null)
                                    {
                                        // �������� ���������� �����
                                        float x1 = startPoint.Get(0);
                                        float y1 = startPoint.Get(1);
                                        float x2 = endPoint.Get(0);
                                        float y2 = endPoint.Get(1);

                                        // ��������� bounding box
                                        float minX = Math.Min(x1, x2);
                                        float maxX = Math.Max(x1, x2);
                                        float minY = Math.Min(y1, y2);
                                        float maxY = Math.Max(y1, y2);

                                        // ������� ������ ������ � ������
                                        float width = maxX - minX;
                                        float height = maxY - minY;

                                        // ���� ������ ������� ���������, ���������� ������������� ��������
                                        if (height <= 0)
                                        {
                                            // ��������� ������ ������� (����� ������ ������)
                                            height = 10f;
                                            minY = y1 - height / 2;
                                            maxY = y1 + height / 2;
                                        }

                                        textRect = new iText.Kernel.Geom.Rectangle(minX, minY, width, height); // ���������� ������ ���
                                    }
                                }
                            }
                            catch
                            {
                                // ���������� ������ ������� �������
                            }
                        }
                        catch
                        {
                            // ���������� ������ ������� �������
                        }

                        // ���� ��� ��� �� �������� �������� �������������, ������� ����������� ��� �������
                        if (textRect == null || (textRect.GetWidth() <= 0 && textRect.GetHeight() <= 0))
                        {
                            // ��������� ������: ����� ������ ������
                            var descentLine = renderInfo.GetDescentLine();
                            if (descentLine != null)
                            {
                                var startPoint = descentLine.GetStartPoint();
                                if (startPoint != null)
                                {
                                    float x = startPoint.Get(0);
                                    float y = startPoint.Get(1);
                                    // ��������� ������ � ������ (����� ������ ������)
                                    float estimatedWidth = text.Length * 6f; // ~6 ����� �� ������
                                    float estimatedHeight = 10f; // ��������� ������ �������
                                    textRect = new iText.Kernel.Geom.Rectangle(x, y - estimatedHeight/2, estimatedWidth, estimatedHeight); // ���������� ������ ���
                                }
                            }
                        }

                        if (textRect != null && (textRect.GetWidth() > 0 || textRect.GetHeight() > 0))
                        {
                            _textItems.Add(new TextItem(text, textRect));
                        }
                        else
                        {
                            // ���� ���� ������������� �� �������, ��������� ����� ��� ��������� ��� �������
                            // _textItems.Add(new TextItem(text, new iText.Kernel.Geom.Rectangle(0, 0, 0, 0))); // ���������� ������ ���
                            // ��� ������ ����������
                        }
                    }
                    catch (Exception /*ex*/) // ���������� ex ������, ��� ��� �� ������������ ��������
                    {
                        // ���������� ������ ��� ��������� ��������� ������
                        // LogMessage($"�������������� ��� ��������� ������ '{text}': {ex.Message}");
                    }
                }
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return new HashSet<EventType> { EventType.RENDER_TEXT };
        }

        /// <summary>
        /// ������� ��� ��������� ���������� ������
        /// </summary>
        public List<TextItem> FindAllText(string searchText)
        {
            List<TextItem> results = new List<TextItem>();
            foreach (var item in _textItems)
            {
                // ����������: CS1501 - ���������� ������������� IndexOf � StringComparison
                if (item.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// ������� ������ ��������� ���������� ������
        /// </summary>
        public TextItem FindText(string searchText)
        {
            var allMatches = FindAllText(searchText);
            return allMatches.Count > 0 ? allMatches[0] : null;
        }

        /// <summary>
        /// ���������� ��� ��������� ��������� ��������
        /// </summary>
        public List<TextItem> GetAllTextItems()
        {
            return new List<TextItem>(_textItems);
        }
    }
}