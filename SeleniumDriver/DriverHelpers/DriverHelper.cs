using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumDriver.Classes;
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
        private readonly Browser browser;
        private readonly WindowsElement appWindow;
        private const int CommandTimeoutSeconds = 180;
        Process testApp;
        private bool IsWindowsDriver = false;
        private readonly Process driverProcess;

        public DriverHelper(Browser browserType, string appNameInput, string appPathInput, bool startApp)
        {
            Debug.WriteLine($"Running '{browserType}' test...");

            switch (browserType)
            {
                case Browser.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    chromeOptions.AddArgument("--start-maximized");
                    chromeOptions.AddArgument("no-sandbox");
                    chromeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
                    Driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromSeconds(CommandTimeoutSeconds));
                    browser = Browser.Chrome;
                    break;

                case Browser.Windows:

                    IsWindowsDriver = true;
                    var projectPath = Path.GetDirectoryName(Path.GetPathRoot(Directory.GetCurrentDirectory()));
                    driverProcess = new Process();
                    driverProcess.StartInfo.CreateNoWindow = true;
                    driverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    driverProcess.StartInfo.FileName = projectPath + "\\Driver\\WinAppDriver.exe";
                    driverProcess.Start();
                    var appCapabilities = new DesiredCapabilities();
                    var appName = appNameInput;
                    var appUnderTestPath = appPathInput;
                    var windowsApplicationDriverUrl = ConfigurationManager.AppSettings["WindowsApplicationDriverUrl"];

                    if (startApp)
                    {
                        testApp = Process.Start(appPathInput);

                        Thread.Sleep(1000);

                    }

                    appCapabilities.SetCapability("app", "Root");

                    var testAppWindow = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), appCapabilities).FindElementByName(appName);
                    appWindow = testAppWindow;
                    var testAppTopLevelWindowHandle = testAppWindow.GetAttribute("NativeWindowHandle");
                    testAppTopLevelWindowHandle = (int.Parse(testAppTopLevelWindowHandle)).ToString("x");

                    var testAppCapabilities = new DesiredCapabilities();
                    testAppCapabilities.SetCapability("appTopLevelWindow", testAppTopLevelWindowHandle);
                    Driver = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), testAppCapabilities);
                    browser = Browser.Windows;

                    break;
                default:
                    throw new ArgumentException(" Browser is not supported");


            }
            if (!browser.Equals(Browser.Windows))
            {
                Driver.Manage().Window.Maximize();
            }

        }

        public IWebElement FindElement(PageObject pageObject, int timeout)
        {
            try
            {
                return Wait(timeout).Until(ExpectedConditions.ElementExists(pageObject.Selector));
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"PageElement {pageObject.Name} could not be found", exc);
            }
        }

        public List<IWebElement> FindElements(PageObject pageObject, int timeout)
        {
            try
            {
                Wait(timeout).Until(ExpectedConditions.ElementExists(pageObject.Selector));
                return Driver.FindElements(pageObject.Selector).ToList();
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"PageElement {pageObject.Name} could not be found", exc);
            }
        }

        public IWebElement GetClickableElement(PageObject pageObject, int timeout)
        {
            Debug.WriteLine($"Waiting element to be clickable '{pageObject.Name}', {timeout}ms");
            try
            {
                return Wait(timeout).Until(ExpectedConditions.ElementToBeClickable(pageObject.Selector));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"PageElement <b>{pageObject.Name}</b> was not found", e);
            }
        }

        public bool IsVisibleInViewport(IWebElement element)
        {
            var elementy = element.Location.Y;

            var viewporty = (bool)((IJavaScriptExecutor)Driver).ExecuteScript(
                "return window.innerHeight > " + elementy + " && window.pageYOffset < " + elementy
                );

            return viewporty;
        }

        public void ClickElement(PageObject pageObject, int timeout, bool staleElementRepeatClick = true)
        {
            Debug.WriteLine($"Searching '{pageObject.Name}' ...");

            var element = FindElement(pageObject, timeout);
            WaitElementUntilVisible(pageObject, timeout);

            try
            {
                if (!element.Displayed && !element.Enabled)
                {
                    Debug.WriteLine($"Element '{pageObject.Name}' not in viewport, focusing...");
                    FocusOnElement(element);
                }

                else if (Driver.GetType() == typeof(InternetExplorerDriver) && !IsVisibleInViewport(FindElement(pageObject, timeout)))
                {
                    FocusOnElement(element);
                }

                Debug.WriteLine($"Clicking '{pageObject.Name}' ...");

                new Actions(Driver)
                    .MoveToElement(element) // click starts failing if removed
                    .Click(element)
                    .Build()    // click starts failing if removed
                    .Perform();
            }
            catch (StaleElementReferenceException) when (staleElementRepeatClick)
            {
                Debug.WriteLine($"Re-searching, focusing and clicking '{pageObject.Name}'");

                ClickElement(pageObject, timeout, false);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"PageElement {pageObject.Name} could not be clicked",
                    exc);
            }
        }

        public void ContextClickElement(PageObject pageObject, int timeout, bool staleElementRepeatClick = true)
        {
            Debug.WriteLine($"Searching '{pageObject.Name}' ...");

            var element = FindElement(pageObject, timeout);
            WaitElementUntilVisible(pageObject, timeout);

            try
            {
                if (!element.Displayed && !element.Enabled)
                {
                    Debug.WriteLine($"Element '{pageObject.Name}' not in viewport, focusing...");
                    FocusOnElement(element);
                }

                else if (Driver.GetType() == typeof(InternetExplorerDriver) && !IsVisibleInViewport(FindElement(pageObject, timeout)))
                {
                    FocusOnElement(element);
                }

                Debug.WriteLine($"Right Clicking '{pageObject.Name}' ...");

                new Actions(Driver)
                    .MoveToElement(element) // click starts failing if removed
                    .ContextClick(element)
                    .Build()    // click starts failing if removed
                    .Perform();
            }
            catch (StaleElementReferenceException) when (staleElementRepeatClick)
            {
                Debug.WriteLine($"Re-searching, focusing and right clicking '{pageObject.Name}'");

                ClickElement(pageObject, timeout, false);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"PageElement {pageObject.Name} could not be right clicked",
                    exc);
            }
        }

        public void SendInputToElement(PageObject pageObject, string input, int timeout)
        {
            var element = GetClickableElement(pageObject, timeout);

            try
            {
                element.SendKeys(input);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"Could not send input: {input}; to element: {pageObject.Name}", exc);
            }
        }

        public void ClearInputElement(PageObject pageObject, int timeout)
        {
            var element = GetClickableElement(pageObject, timeout);

            try
            {
                element.Clear();
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"Could not clear {pageObject.Name} elements input", exc);
            }
        }

        public string GetScreenshot()
        {
            var screenshotDriver = Driver as ITakesScreenshot;

            var screenShot = screenshotDriver?.GetScreenshot();

            return screenShot?.AsBase64EncodedString;
        }

        public bool IsElementPresent(PageObject pageObject, int timeout)
        {
            try
            {
                return Wait(timeout).Until(d =>
                {
                    var element = d.FindElement(pageObject.Selector);
                    return element.Enabled && element.Displayed;
                });
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void SelectOptionByText(PageObject pageObject, string text, int timeout)
        {
            try
            {
                var selectElement = GetSelectElement(pageObject, timeout);

                selectElement.SelectByText(text, false);
            }
            catch (Exception e)
            {
                throw new UnexpectedTagNameException("Could not select value in element.", e);
            }
        }

        public void SelectOptionByValue(PageObject pageObject, string value, int timeout)
        {
            try
            {
                var element = FindElement(pageObject, timeout);

                FocusOnElement(element);

                System.Threading.Thread.Sleep(500);    // Under Firefox will fail if removed

                var selectElement = GetSelectElement(pageObject, timeout);

                selectElement.SelectByValue(value);
            }

            catch (Exception e)
            {
                throw new UnexpectedTagNameException("Could not select value in element", e);
            }
        }

        public void NavigateToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public IWait<IWebDriver> Wait(int timeout)
        {
            return new WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeout));
        }

        public void WaitElementUntilVisible(PageObject pageObject, int timeout)
        {
            try
            {
                Wait(timeout).Until(ExpectedConditions.ElementIsVisible(pageObject.Selector));
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"PageElement {pageObject.Name} could not be found", exc);
            }
        }

        public void FocusOnElement(IWebElement element)
        {
            try
            {
                const string scrollElementIntoMiddle = "var viewPortHeight = Math.max("
                                                        + "document.documentElement.clientHeight, window.innerHeight || 0);"
                                                        + "var elementTop = arguments[0].getBoundingClientRect().top;"
                                                        + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

                ((IJavaScriptExecutor)Driver).ExecuteScript(scrollElementIntoMiddle, element);
            }
            catch (Exception e)
            {
                throw new UnexpectedTagNameException("Failed to perform focus over specific element", e);
            }
        }

        private SelectElement GetSelectElement(PageObject pageObject, int timeout)
        {
            try
            {
                var element = Wait(timeout).Until(d => d.FindElement(pageObject.Selector));

                return new SelectElement(element);
            }
            catch (UnexpectedTagNameException exc)
            {
                throw new UnexpectedTagNameException(
                    "Could not select value in element - element does not support selecting", exc);
            }
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
            driverProcess.Kill();
        }

        public void SwitchToNextTab()
        {
            Driver.SwitchTo().Window(
                Driver.WindowHandles.FirstOrDefault(
                    wh => wh != Driver.CurrentWindowHandle));
        }

        public void SwitchToPreviousTab()
        {
            Driver.SwitchTo().Window(
                Driver.WindowHandles.FirstOrDefault());
        }
        public void RetargetWindow(string appName)
        {
            for (int i = 0; i < 30; i++)
            {
                Thread.Sleep(2000);
                if (testApp.Responding)
                {
                    break;
                }

            }
            Thread.Sleep(40000);
            DesiredCapabilities appCapabilities = new DesiredCapabilities();
            string windowsApplicationDriverUrl = ConfigurationManager.AppSettings["WindowsApplicationDriverUrl"];
            appCapabilities.SetCapability("app", "Root");

            var TestAppWindow = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), appCapabilities).FindElementByAccessibilityId(appName);
            var TestAppTopLevelWindowHandle = TestAppWindow.GetAttribute("NativeWindowHandle");
            TestAppTopLevelWindowHandle = (int.Parse(TestAppTopLevelWindowHandle)).ToString("x");

            DesiredCapabilities testAppCapabilities = new DesiredCapabilities();
            testAppCapabilities.SetCapability("appTopLevelWindow", TestAppTopLevelWindowHandle);

            Driver = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), testAppCapabilities);
        }

        public WindowsElement GetAppWindow()
        {
            return appWindow;
        }
    }
}


