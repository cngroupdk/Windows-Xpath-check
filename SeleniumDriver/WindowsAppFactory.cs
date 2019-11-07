using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace SeleniumDriver
{
    public class WindowsAppFactory
    {
        private readonly string _windowsApplicationDriverUrl;
        public WindowsAppFactory(string windowsApplicationDriverUrl = null)
        {
            _windowsApplicationDriverUrl = string.IsNullOrWhiteSpace(windowsApplicationDriverUrl)
                ? ConfigurationManager.AppSettings["WindowsApplicationDriverUrl"]
                : windowsApplicationDriverUrl;
        }

        public WindowsElement CreateApp(string appName)
        {
            var uri = new Uri(_windowsApplicationDriverUrl);

            var capabilities = new DesiredCapabilities();
            capabilities.SetCapability("app", "Root");
            return new WindowsDriver<WindowsElement>(uri, capabilities).FindElementByName(appName);
        }

        public WindowsDriver<WindowsElement> CreateDriver(WindowsElement app)
        {
            var handle = app.GetAttribute("NativeWindowHandle");
            handle = (int.Parse(handle)).ToString("x");

            var capabilities = new DesiredCapabilities();
            capabilities.SetCapability("appTopLevelWindow", handle);

            var uri = new Uri(_windowsApplicationDriverUrl);
            return new WindowsDriver<WindowsElement>(uri, capabilities);
        }
    }
}
