using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System.Configuration;
using System.IO;
using System.Threading;

namespace SeleniumDriver.DriverHelpers
{
    public class DriverHelper : IDisposable
    {
        private IWebDriver Driver { get; set; }
        private readonly WindowsElement appWindow;
        readonly Process testApp;
        private readonly Process driverProcess;

        public DriverHelper( string appPathInput, bool startApp)
        {
            var projectPath = Path.GetDirectoryName(Path.GetPathRoot(Directory.GetCurrentDirectory()));
            driverProcess = new Process();
            driverProcess.StartInfo.CreateNoWindow = true;
            driverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            driverProcess.StartInfo.FileName = projectPath + "\\Driver\\WinAppDriver.exe";
            driverProcess.Start();
            
            if (startApp)
            {
                testApp = Process.Start(appPathInput);

                Thread.Sleep(1000);

            }

            //appCapabilities.SetCapability("app", "Root");

            //appWindow = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), appCapabilities).FindElementByName(appName);
           
            //var testAppTopLevelWindowHandle = appWindow.GetAttribute("NativeWindowHandle");
            //testAppTopLevelWindowHandle = (int.Parse(testAppTopLevelWindowHandle)).ToString("x");

            //var testAppCapabilities = new DesiredCapabilities();
            //testAppCapabilities.SetCapability("appTopLevelWindow", testAppTopLevelWindowHandle);
            //Driver = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), testAppCapabilities);
        }

        public IWebElement FindElement(By selector, int timeout)
        {
            try
            {
                return Wait(timeout).Until(ExpectedConditions.ElementExists(selector));
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException("Element could not be found", exc);
            }
        }

        public List<IWebElement> FindElements(By selector, int timeout)
        {
            try
            {
                Wait(timeout).Until(ExpectedConditions.ElementExists(selector));
                return Driver.FindElements(selector).ToList();
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException("Element could not be found", exc);
            }
        }

        public IWait<IWebDriver> Wait(int timeout)
        {
            return new WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeout));
        }


        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
            driverProcess.Kill();
        }
    }
}


