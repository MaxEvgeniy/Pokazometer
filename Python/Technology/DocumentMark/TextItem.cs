using System;
using iText.Kernel.Geom; // ���������� ������ ���

namespace DocumentMark
{
    /// <summary>
    /// ������������ ��������� ������� � ��� ������������
    /// </summary>
    public class TextItem
    {
        public string Text { get; }
        public iText.Kernel.Geom.Rectangle Rect { get; } // ���������� ������ ���

        public TextItem(string text, iText.Kernel.Geom.Rectangle rect) // ���������� ������ ���
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Rect = rect ?? throw new ArgumentNullException(nameof(rect));
        }
    }
}