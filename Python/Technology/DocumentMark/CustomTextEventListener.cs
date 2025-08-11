using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Geom; // Используем полное имя

namespace DocumentMark
{
    /// <summary>
    /// Пользовательский слушатель событий для извлечения текста с координатами
    /// ИСПРАВЛЕНО: Для работы с iText 9 и LineSegment
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
                        iText.Kernel.Geom.Rectangle textRect = null; // Используем полное имя

                        try
                        {
                            // === ИСПРАВЛЕНО: CS1061 - правильный способ получения ограничивающего прямоугольника в iText 9 ===
                            // Способ 1: Используем GetDescentLine().GetBoundingRectangle()
                            // Это стандартный способ в iText 7/9
                            var descentLine = renderInfo.GetDescentLine();
                            if (descentLine != null)
                            {
                                textRect = descentLine.GetBoundingRectangle(); // <-- ПРАВИЛЬНЫЙ МЕТОД
                            }
                        }
                        catch (MissingMethodException)
                        {
                            // Если GetBoundingRectangle() недоступен, попробуем другие методы
                            try
                            {
                                // Способ 2: Попробуем получить координаты из StartPoint и EndPoint
                                var descentLine = renderInfo.GetDescentLine();
                                if (descentLine != null)
                                {
                                    var startPoint = descentLine.GetStartPoint();
                                    var endPoint = descentLine.GetEndPoint();

                                    if (startPoint != null && endPoint != null)
                                    {
                                        // Получаем координаты точек
                                        float x1 = startPoint.Get(0);
                                        float y1 = startPoint.Get(1);
                                        float x2 = endPoint.Get(0);
                                        float y2 = endPoint.Get(1);

                                        // Вычисляем bounding box
                                        float minX = Math.Min(x1, x2);
                                        float maxX = Math.Max(x1, x2);
                                        float minY = Math.Min(y1, y2);
                                        float maxY = Math.Max(y1, y2);

                                        // Простая оценка ширины и высоты
                                        float width = maxX - minX;
                                        float height = maxY - minY;

                                        // Если высота слишком маленькая, используем эвристическое значение
                                        if (height <= 0)
                                        {
                                            // Примерная высота символа (очень грубая оценка)
                                            height = 10f;
                                            minY = y1 - height / 2;
                                            maxY = y1 + height / 2;
                                        }

                                        textRect = new iText.Kernel.Geom.Rectangle(minX, minY, width, height); // Используем полное имя
                                    }
                                }
                            }
                            catch
                            {
                                // Игнорируем ошибки второго способа
                            }
                        }
                        catch
                        {
                            // Игнорируем ошибки первого способа
                        }

                        // Если все еще не получили валидный прямоугольник, создаем минимальный для отладки
                        if (textRect == null || (textRect.GetWidth() <= 0 && textRect.GetHeight() <= 0))
                        {
                            // Последний резорт: очень грубая оценка
                            var descentLine = renderInfo.GetDescentLine();
                            if (descentLine != null)
                            {
                                var startPoint = descentLine.GetStartPoint();
                                if (startPoint != null)
                                {
                                    float x = startPoint.Get(0);
                                    float y = startPoint.Get(1);
                                    // Примерная ширина и высота (очень грубая оценка)
                                    float estimatedWidth = text.Length * 6f; // ~6 точек на символ
                                    float estimatedHeight = 10f; // Примерная высота символа
                                    textRect = new iText.Kernel.Geom.Rectangle(x, y - estimatedHeight/2, estimatedWidth, estimatedHeight); // Используем полное имя
                                }
                            }
                        }

                        if (textRect != null && (textRect.GetWidth() > 0 || textRect.GetHeight() > 0))
                        {
                            _textItems.Add(new TextItem(text, textRect));
                        }
                        else
                        {
                            // Даже если прямоугольник не валиден, добавляем текст без координат для отладки
                            // _textItems.Add(new TextItem(text, new iText.Kernel.Geom.Rectangle(0, 0, 0, 0))); // Используем полное имя
                            // Или просто игнорируем
                        }
                    }
                    catch (Exception /*ex*/) // Переменная ex убрана, так как не используется напрямую
                    {
                        // Игнорируем ошибки для отдельных элементов текста
                        // LogMessage($"Предупреждение при обработке текста '{text}': {ex.Message}");
                    }
                }
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return new HashSet<EventType> { EventType.RENDER_TEXT };
        }

        /// <summary>
        /// Находит ВСЕ вхождения указанного текста
        /// </summary>
        public List<TextItem> FindAllText(string searchText)
        {
            List<TextItem> results = new List<TextItem>();
            foreach (var item in _textItems)
            {
                // ИСПРАВЛЕНО: CS1501 - правильное использование IndexOf с StringComparison
                if (item.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// Находит первое вхождение указанного текста
        /// </summary>
        public TextItem FindText(string searchText)
        {
            var allMatches = FindAllText(searchText);
            return allMatches.Count > 0 ? allMatches[0] : null;
        }

        /// <summary>
        /// Возвращает все найденные текстовые элементы
        /// </summary>
        public List<TextItem> GetAllTextItems()
        {
            return new List<TextItem>(_textItems);
        }
    }
}