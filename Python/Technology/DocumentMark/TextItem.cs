using System;
using iText.Kernel.Geom; // Используем полное имя

namespace DocumentMark
{
    /// <summary>
    /// Представляет текстовый элемент с его координатами
    /// </summary>
    public class TextItem
    {
        public string Text { get; }
        public iText.Kernel.Geom.Rectangle Rect { get; } // Используем полное имя

        public TextItem(string text, iText.Kernel.Geom.Rectangle rect) // Используем полное имя
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Rect = rect ?? throw new ArgumentNullException(nameof(rect));
        }
    }
}