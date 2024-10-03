using Flvt.Domain.Advertisements;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Flvt.Scraper;

internal abstract class WebScraper : IDisposable
{
    protected readonly Filter Filter;
    protected readonly string BaseUrl;
    protected string QueryUrl;
    protected readonly IWebDriver Browser;
    private readonly ChromeOptions _options = new()
    {
        BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
        PageLoadStrategy = PageLoadStrategy.Normal,
    };

    protected WebScraper(Filter filter, string baseUrl)
    {
        Filter = filter;
        BaseUrl = baseUrl;
        QueryUrl = baseUrl;
        Browser = new ChromeDriver();
        // options.AddArgument("--headless"); TODO implement headless mode
    }

    public abstract Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync();

    protected abstract void BuildQueryUrl();

    public void Dispose()
    {
        Browser.Dispose();
    }
}