using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using OpenQA.Selenium;
using SeleniumDriver.DriverHelpers;
using SeleniumExtras.WaitHelpers;

namespace SeleniumDriver.Classes
{
    public abstract class PageObjectAction<T> where T : PageObjectElements, new()
    {
        protected readonly DriverHelper Driver;
        protected readonly int TimeoutAction;
        protected readonly int TimeoutPageLoad;

        public T Elements { get; }

        protected PageObjectAction(DriverHelper driver)
        {
            TimeoutAction = Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutAction"]);
            TimeoutPageLoad = Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutPageLoad"]);
            Driver = driver;
            Elements = new T();
        }

        public virtual void LeftClick(PageObject pageObject)
        {
            Driver.ClickElement(pageObject, TimeoutAction);
        }

        public virtual void RightClick(PageObject pageObject)
        {
            Driver.ContextClickElement(pageObject, TimeoutAction);
        }

        public virtual void SendInput(PageObject pageObject, string input)
        {
            Driver.SendInputToElement(pageObject, input, TimeoutAction);
        }

        public virtual void ClearInputElement(PageObject pageObject)
        {
            Driver.ClearInputElement(pageObject, TimeoutAction);
        }

        public virtual string GetElementText(PageObject pageObject)
        {
            return Driver.FindElement(pageObject, TimeoutAction).Text;
        }

        public virtual void SelectOptionByText(PageObject pageObject, string text)
        {
            Driver.SelectOptionByText(pageObject, text, TimeoutAction);
        }

        public virtual void SelectOptionByValue(PageObject pageObject, string value)
        {
            Driver.SelectOptionByValue(pageObject, value, TimeoutAction);
        }

        public virtual void NavigateToUrl(string url)
        {
            Driver.NavigateToUrl(url);
        }

        public virtual void WaitForElement(PageObject pageObject)
        {
            Driver.FindElement(pageObject, TimeoutAction);
        }

        public virtual void SwitchToNextTab()
        {
            Driver.SwitchToNextTab();
        }

        public virtual void SwitchToPreviousTab()
        {
            Driver.SwitchToPreviousTab();
        }

        public virtual void WaitForElementToBeVisible(PageObject pageObject)
        {
            Driver.Wait(TimeoutAction).Until(ExpectedConditions.ElementIsVisible(pageObject.Selector));
        }

        public virtual void WaitForElementToBeClickable(PageObject pageObject)
        {
            Driver.Wait(TimeoutAction).Until(ExpectedConditions.ElementToBeClickable(pageObject.Selector));
        }

        public virtual void WaitForElementInvisibility(PageObject pageObject)
        {
            Driver.Wait(TimeoutAction).Until(ExpectedConditions.InvisibilityOfElementLocated(pageObject.Selector));
        }

        public virtual void LongWaitForElementInvisibility(PageObject pageObject)
        {
            Driver.Wait(TimeoutPageLoad).Until(ExpectedConditions.InvisibilityOfElementLocated(pageObject.Selector));
        }

        public virtual void WaitForElementToNotExist(PageObject pageObject)
        {
            Driver.Wait(TimeoutAction).Until(d => d.FindElements(pageObject.Selector).Any() == false);
        }

        public virtual void FocusOnElement(PageObject pageObject)
        {
            Driver.FocusOnElement(FindElement(pageObject));
        }

        public string GetElementValue(PageObject pageObject, string attributeName)
        {
            return FindElement(pageObject).GetAttribute(attributeName);
        }

        protected IWebElement FindElement(PageObject pageObject)
        {
            return Driver.FindElement(pageObject, TimeoutAction);
        }

        protected List<IWebElement> FindElements(PageObject pageObject)
        {
            return Driver.FindElements(pageObject, TimeoutAction);
        }
    }
}
