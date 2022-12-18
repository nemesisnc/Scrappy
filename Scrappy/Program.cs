using System;
using System.Threading.Tasks;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://juridoc.gouv.nc/");

        await page.FrameLocator("frame[name=\"Mframe\"]").GetByRole(AriaRole.Link, new() { NameString = "Edition courante et numéros spéciaux" }).ClickAsync();

        await page.FrameLocator("frame[name=\"Mframe\"]").FrameLocator("frame[name=\"Lframe\"]").Locator("#sliderDis1").SelectOptionAsync(new[] { "2022" });

        var download = await page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.FrameLocator("frame[name=\"Mframe\"]").FrameLocator("frame[name=\"Lframe\"]").GetByRole(AriaRole.Link, new() { NameString = "Consulter la version complète Pdf du n° 10488 (HTTP)" }).ClickAsync(new LocatorClickOptions
            {
                Modifiers = new[] { KeyboardModifier.Alt },
            });
        });

        await download.SaveAsAsync("./10488.pdf");
    }
}
