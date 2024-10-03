using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Flvt.Scraper.Extensions;

public static class WebDriverExtensions
{
    private const int timeout = 10;
    public static WebDriverWait Wait(this IWebDriver driver) => new(driver, TimeSpan.FromSeconds(timeout));
}