using System;
using System.Threading.Tasks;

class Program
{
    public static async Task Main()
    {
        var lastDownloadedJonc = Directory.EnumerateFiles("./", "*.pdf")
            .Where(f => f.Length == 11 && int.TryParse(f.Substring(2, 5), out int num))
            .OrderByDescending(f => f)
            .FirstOrDefault();

        int lastNumeroJonc;
        if (lastDownloadedJonc != null) {
            lastNumeroJonc = int.Parse(lastDownloadedJonc.Substring(2, 5));
        }
        else {
            throw new FileNotFoundException();
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://juridoc.gouv.nc/");

        await page.FrameLocator("frame[name=\"Mframe\"]").GetByRole(AriaRole.Link, new() { NameString = "Edition courante et numéros spéciaux" }).ClickAsync();

        await page.FrameLocator("frame[name=\"Mframe\"]").FrameLocator("frame[name=\"Lframe\"]").Locator("#sliderDis1").SelectOptionAsync(new[] { "2022" });


        bool joncNotFound = false;
        while (!joncNotFound)
        {
            lastNumeroJonc++;

            try
            {
                var download = await page.RunAndWaitForDownloadAsync(async () =>
                {
                    await page.FrameLocator("frame[name=\"Mframe\"]").FrameLocator("frame[name=\"Lframe\"]").GetByRole(AriaRole.Link, new() { NameString = $"Consulter la version complète Pdf du n° {lastNumeroJonc} (HTTP)" }).ClickAsync(new LocatorClickOptions
                    {
                        Modifiers = new[] { KeyboardModifier.Alt },
                    });
                }, new PageRunAndWaitForDownloadOptions { Timeout = 5000});

                await download.SaveAsAsync($"./{lastNumeroJonc}.pdf");
            }
            catch (System.TimeoutException)
            {
                // Next Jonc not found
                joncNotFound = true;
            }
        }
    }
}
