using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SRCStats.Tests
{
    [TestFixture]
    public class UsersTests : PageTest
    {
        readonly string Root = @"https://localhost:7251/users";

        [SetUp]
        public async Task SetUp()
        {
            await Page.GotoAsync(Root);
        }

        [Test]
        public async Task InvalidSearchReturns()
        {
            var submitButton = Page.Locator("#submit");
            await submitButton.ClickAsync();
            await Expect(Page.Locator("#prog-info")).ToHaveTextAsync("");
        }

        [Test]
        public async Task SearchIsSuccessful()
        {
            await Page.Locator("#userName").FillAsync("Camcorder");
            var submitButton = Page.Locator("#submit");
            await submitButton.ClickAsync();
            await Expect(Page.Locator("#prog-info")).ToHaveTextAsync(new Regex(".*"));
            await Expect(Page.Locator("#prog-info")).ToHaveTextAsync(new Regex("^Fetching user's examined runs"));
            await Expect(Page).ToHaveURLAsync(Root + "/Camcorder", new PageAssertionsToHaveURLOptions() { Timeout = 10000 });
        }

        [Test]
        public async Task SearchCanHappenAfterInvalidSearch()
        {
            var submitButton = Page.Locator("#submit");
            await submitButton.ClickAsync();
            await Expect(Page.Locator("#prog-info")).ToHaveTextAsync("");
            await Page.Locator("#userName").FillAsync("Boys");
            await submitButton.ClickAsync();
            /* todo: make this test pass. see issue #39
            await Expect(Page.Locator("#prog-info")).ToHaveTextAsync(new Regex(".*"));
            await Expect(Page).ToHaveURLAsync(Root + "/Boys", new PageAssertionsToHaveURLOptions() { Timeout = 10000 }); */
        }

        [Test]
        public async Task SearchHasExpectedResults()
        {
            // todo: open a new page on src and make sure the data lines up
            await Page.GotoAsync(Root + "/Camcorder");
            await Expect(Page.Locator(".username")).ToHaveTextAsync("Camcorder");
            await Expect(Page.GetByText("Staff Test Enjoyer")).ToHaveCountAsync(1);
            await Expect(Page.GetByText(new Regex("^Elo Staff$"))).ToHaveCountAsync(1);
        }
    }
}
