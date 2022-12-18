using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    

    // [SetUp]
    // public async Task SetUp()
    // {

    // }

    [Test]
    public async Task HaveLinkToLastMissingJoncs()
    {
        await Page.GotoAsync("https://juridoc.gouv.nc/");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Documentation juridique NC"));

        // create a locator
        var joncIndexLink = Page.FrameLocator("frame[name=\"Mframe\"]").GetByRole(AriaRole.Link, new() { NameString = "Edition courante et numéros spéciaux" });

        // Click the get started link.
        await joncIndexLink.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*openpage"));

        await Page.FrameLocator("frame[name=\"Mframe\"]").FrameLocator("frame[name=\"Lframe\"]").Locator("#sliderDis1").SelectOptionAsync(new[] { "2022" });

        var joncDownloadLink = Page.FrameLocator("frame[name=\"Mframe\"]").FrameLocator("frame[name=\"Lframe\"]").GetByRole(AriaRole.Link, new() { NameString = "Consulter la version complète Pdf du n° 10488 (HTTP)" });
        
        await Expect(joncDownloadLink).ToBeEnabledAsync();
    }

    // [TearDown]
    // public async Task TearDown()
    // {

    // }
}