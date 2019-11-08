using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using SeleniumDriver;

namespace XPathCheck
{
    class Highlight : IHighlight
    {
        private Window overlayWindow;
        private Canvas overlay;
        private Polygon poly;
        public void Init(int x, int y, int w, int h)
        {
            overlayWindow = new Window
            {
                Left = x,
                Top = y,
                Width = w,
                Height = h,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                Background = null,
                Topmost = true
            };
            overlayWindow.Show();
            overlay = new Canvas {Visibility = 0};
            overlayWindow.Content = overlay;
        }

        public void DrawRect(int x, int y, int w, int h)
        {
            overlay.Children.Clear();
            var a = new Point(x, y + h);
            var b = new Point(x + w, y + h);
            var c = new Point(x + w, y);
            var d = new Point(x, y);
            poly = new Polygon();
            poly.Points.Add(a);
            poly.Points.Add(b);
            poly.Points.Add(c);
            poly.Points.Add(d);
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
