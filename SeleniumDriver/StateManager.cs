using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Appium.Windows;

namespace SeleniumDriver
{
    public class StateManager<TOverlay> : IDisposable
        where TOverlay : class, IHighlight, new()
    {
        private WindowsElement _app;
        private WindowsDriver<WindowsElement> _driver;
        private Process _driverProcess;
        private Process _appProcess;
        private TOverlay _overlay;

        public WindowsElement GetApp()
        {
            return _app ?? throw new Exception("Bad workflow, find app first!");
        }

        public WindowsDriver<WindowsElement> GetDriver()
        {
            return _driver ?? throw new Exception("Bad workflow, find app first!");
        }

        public TOverlay GetOverlay()
        {
            return _overlay ?? throw new Exception("Bad workflow, find app first!");
        }

        public void InitApp(string appName, string appPathInput, bool startApp = false)
        {
            Dispose();
            var projectPath = Path.GetDirectoryName(Path.GetPathRoot(Directory.GetCurrentDirectory()));
            _driverProcess = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = projectPath + "\\Driver\\WinAppDriver.exe"
                }
            };
            _driverProcess.Start();

            if (startApp)
            {
                _appProcess = Process.Start(appPathInput);

                Thread.Sleep(1000);

            }
            var factory = new WindowsAppFactory();
            _app = factory.CreateApp(appName);
            _driver = factory.CreateDriver(_app);
            _overlay = new TOverlay();
            _overlay.Init(_app.Coordinates.LocationInViewport.X, _app.Coordinates.LocationInViewport.Y, _app.Size.Width, _app.Size.Height);
        }

        public void SnapToApp()
        {
            var highlight = GetOverlay();
            var appWindow = GetApp();
            highlight.SnapToApp(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _driver?.Dispose();
            _driverProcess?.Dispose();
            _overlay?.Close();
            //We do not dispose appProces to do nto close that app when this application is closed
        }
    }
}
