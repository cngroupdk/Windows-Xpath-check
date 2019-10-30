using OpenQA.Selenium;

namespace SeleniumDriver.Classes
{
    public class PageObject
    {
        public readonly By Selector;
        public readonly string Name;

        public PageObject(By by, string name)
        {
            Selector = by;
            Name = name;
        }
    }
}
