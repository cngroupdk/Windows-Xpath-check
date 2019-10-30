using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace XPathConsole
{
    class Highlight
    {
        private Window overlayWindow;
        private Canvas overlay;
        private Polygon poly;
        public Highlight(int x, int y, int w, int h)
        {
            overlayWindow = new Window();
            overlayWindow.Left = x;
            overlayWindow.Top = y;
            overlayWindow.Width = w;
            overlayWindow.Height = h;
            overlayWindow.AllowsTransparency = true;
            overlayWindow.WindowStyle = WindowStyle.None;
            overlayWindow.Background = null;
            overlayWindow.Topmost = true;
            overlayWindow.Show();
            overlay = new Canvas();
            overlay.Visibility = 0;
            overlayWindow.Content = overlay;
        }

        public void drawRect(int x, int y, int w, int h)
        {
            overlay.Children.Clear();
            var A = new Point(x, y + h);
            var B = new Point(x + w, y + h);
            var C = new Point(x + w, y);
            var D = new Point(x, y);
            poly = new Polygon();
            poly.Points.Add(A);
            poly.Points.Add(B);
            poly.Points.Add(C);
            poly.Points.Add(D);
            poly.Stroke = new SolidColorBrush(Colors.Red);
            poly.StrokeThickness = 3;
            overlay.Children.Add(poly);
        }

        public void Close()
        {
            overlayWindow?.Close();
        }

        public void Hide()
        {
            overlayWindow.Hide();
        }

        public void Show()
        {
            overlayWindow.Show();
        }

        public void SnapToApp(int x, int y, int w, int h)
        {
            overlayWindow.Left = x;
            overlayWindow.Top = y;
            overlayWindow.Width = w;
            overlayWindow.Height = h;
        }
    }
}
