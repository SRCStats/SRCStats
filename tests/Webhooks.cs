using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SRCStats.Tests
{
    [TestFixture]
    public class WebhooksTests : PageTest
    {
        readonly string Root = @"https://localhost:7251/webhooks";

        [SetUp]
        public async Task SetUp()
        {
            await Page.GotoAsync(Root);
            await Page.Locator("#webhook-val").FillAsync(@"https://discord.com/api/webhooks/xxxxxxxxxxxxxxxx/xxxxxxxxxxxxxx");
        }

        [Test]
        public async Task HoverTooltipsAppear()
        {
            await Page.Locator("#records-tooltip").HoverAsync();
            await Expect(Page.Locator(".tooltip-inner:has-text(\"Events for when a run is verified.\")")).ToBeVisibleAsync();
            await Page.Locator("#verification-tooltip").HoverAsync();
            await Expect(Page.Locator(".tooltip-inner:has-text(\"Events for when a run's verification status changes.\")")).ToBeVisibleAsync();
            await Page.Locator("#context-tooltip").HoverAsync();
            await Expect(Page.Locator(".tooltip-inner:has-text(\"Game: Specific games. User: All games moderated by that user.\")")).ToBeVisibleAsync();
        }

        [Test]
        public async Task WebhookRecordsAddAndRemoveGame()
        {
            var input = Page.Locator("#rgames-input");
            await input.FillAsync("sm64");
            await Page.Keyboard.PressAsync("Enter");
            var entry = Page.Locator("#rgames-list > a");
            await Expect(entry).ToHaveTextAsync("Super Mario 64");
            await Page.GetByText("Select categories...").ClickAsync();
            await Expect(Page.GetByText("Super Mario 64 - Main")).ToHaveCountAsync(1);
            await Expect(Page.GetByText("120 Star")).ToHaveCountAsync(2);
            await entry.ClickAsync();
            await Expect(entry).ToHaveCountAsync(0);
            await Page.GetByText("Select categories...").ClickAsync();
            await Expect(Page.GetByText("Super Mario 64 - Main")).ToHaveCountAsync(0);
            await Expect(Page.GetByText("120 Star")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task WebhookRecordsAddAndRemoveUser()
        {
            var input = Page.Locator("#rusers-input");
            await input.FillAsync("meta");
            await Page.Keyboard.PressAsync("Enter");
            var entry = Page.Locator("#rusers-list > a");
            await Expect(entry).ToHaveTextAsync("Meta");
            await entry.ClickAsync();
            await Expect(entry).ToHaveCountAsync(0);
        }

        [Test]
        public async Task WebhookRecordsHasEvents()
        {
            var dropdown = Page.GetByText("Select events...").First;
            await dropdown.ClickAsync();
            var wrOnly = Page.Locator("span:has-text(\"World records only\")");
            var wrAndPb = Page.Locator("span:has-text(\"World records and personal bests\")");
            var allRuns = Page.Locator("span:has-text(\"All submitted runs\")");
            await Expect(wrOnly).ToHaveCountAsync(1);
            await Expect(wrAndPb).ToHaveCountAsync(1);
            await Expect(allRuns).ToHaveCountAsync(1);
            await wrOnly.ClickAsync();
            dropdown = Page.Locator("[title='World records only']");
            await Expect(dropdown).ToHaveCountAsync(1);
            await dropdown.ClickAsync();
            await wrAndPb.ClickAsync();
            dropdown = Page.Locator("[title='World records and personal bests']");
            await Expect(dropdown).ToHaveCountAsync(1);
            await dropdown.ClickAsync();
            await allRuns.ClickAsync();
            dropdown = Page.Locator("[title='All submitted runs']");
            await Expect(dropdown).ToHaveCountAsync(1);
        }
    }
}
