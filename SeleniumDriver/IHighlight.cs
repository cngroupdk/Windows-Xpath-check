namespace SeleniumDriver
{
    public interface IHighlight
    {
        void Init(int x, int y, int w, int h);
        void DrawRect(int x, int y, int w, int h);
        void Close();
        void Hide();
        void Show();
        void SnapToApp(int x, int y, int w, int h);
        void Clear();
    }
}