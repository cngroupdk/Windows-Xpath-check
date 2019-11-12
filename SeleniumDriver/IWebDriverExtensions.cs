using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace SeleniumDriver
{
    public static class IWebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, string userXPath, int timeout)
        {
            var selector = By.XPath(userXPath);
            try
            {
                return Wait(driver, timeout).Until(ExpectedConditions.ElementExists(selector));
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException("Element could not be found", exc);
            }
        }

        public static List<IWebElement> FindElements(this IWebDriver driver,string userXPath, int timeout)
        {
            var selector = By.XPath(userXPath);
            try
            {
                Wait(driver, timeout).Until(ExpectedConditions.ElementExists(selector));
                return driver.FindElements(selector).ToList();
            }
            catch (Exception exc) when (exc is NoSuchElementException || exc is WebDriverTimeoutException)
            {
                throw new NoSuchElementException("Element could not be found", exc);
            }
        }

        private static IWait<IWebDriver> Wait(IWebDriver driver, int timeout)
        {
            return new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
        }
    }
}
