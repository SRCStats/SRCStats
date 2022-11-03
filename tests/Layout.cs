using System.Text.RegularExpressions;

using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SRCStats.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LayoutTests : PageTest
    {
        // all pages that we should test the layout on
        readonly string[] UrlPaths = { "/", "/users", "/home/disclaimer", "/home/faq", "/webhooks" };
        readonly string Root = @"https://localhost:7251";

        [SetUp]
        public async Task SetUp()
        {
            await Page.CloseAsync();
            foreach (var path in UrlPaths)
            {
                var page = await Context.NewPageAsync();
                await page.GotoAsync(Root + path);
            }
        }

        [Test]
        public async Task NavbarExistsAndHasLogo()
        {
            foreach (var Page in Context.Pages)
            {
                try
                {
                    await Expect(Page.Locator("header")).ToHaveCountAsync(1);
                    var headerLogo = Page.Locator("header > div > .site-logo");
                    await Expect(headerLogo).ToHaveAttributeAsync("aria-label", "Return home");
                    await headerLogo.ClickAsync();
                    await Expect(Page).ToHaveURLAsync(new Regex($"^{Root}"));
                }
                catch
                {
                    Console.Write($"Error occured on {Page.Url}:");
                    throw;
                }
            }
        }

        [Test]
        public async Task NavbarHasUsers()
        {
            foreach (var Page in Context.Pages)
            {
                try
                {
                    var usersNavItem = Page.Locator(".site-nav-desktop > a:has-text(\"Users\")");
                    await usersNavItem.ClickAsync();
                    await Expect(Page).ToHaveURLAsync(new Regex("\\/users$"));
                }
                catch
                {
                    Console.Write($"Error occured on {Page.Url}:");
                    throw;
                }
            }
        }

        [Test]
        public async Task NavbarHasRuns()
        {
            foreach (var Page in Context.Pages)
            {
                try
                {
                    var runsNavItem = Page.Locator(".site-nav-desktop > a:has-text(\"Runs\")");
                    await runsNavItem.ClickAsync();
                    /* todo: make this test pass. see issue #12
                    await Expect(Page).ToHaveURLAsync(new Regex("\\/runs$")); */
                }
                catch
                {
                    Console.Write($"Error occured on {Page.Url}:");
                    throw;
                }
            }
        }

        [Test]
        public async Task NavbarHasTools()
        {
            foreach (var Page in Context.Pages)
            {
                try
                {
                    var toolsNavItem = Page.Locator(".site-nav-desktop > a:has-text(\"Tools\")");
                    await toolsNavItem.ClickAsync();
                    await Expect(Page).ToHaveURLAsync(new Regex("\\/webhooks$"));
                }
                catch
                {
                    Console.Write($"Error occured on {Page.Url}:");
                    throw;
                }
            }
        }

        [Test]
        public async Task NavbarDropdownOpensAndCloses()
        {
            foreach (var Page in Context.Pages)
            {
                try
                {
                    var dropdownWindow = Page.Locator("#navbarToggleExternalContent");
                    await Expect(dropdownWindow).ToBeHiddenAsync();
                    var dropdownNav = Page.Locator(".site-nav-mobile");
                    await Expect(dropdownNav).ToBeHiddenAsync();
                    var dropdownButton = Page.Locator(".navbar-toggler");
                    await dropdownButton.ClickAsync();
                    await Expect(dropdownWindow).ToBeVisibleAsync();
                    await Expect(dropdownNav).ToBeVisibleAsync();
                    await Expect(Page.Locator(".site-nav-mobile > a:visible")).ToHaveCountAsync(3);
                    await dropdownButton.ClickAsync();
                    await Expect(dropdownWindow).ToBeHiddenAsync();
                    await Expect(dropdownNav).ToBeHiddenAsync();
                    await Expect(Page.Locator(".site-nav-mobile > a:visible")).ToHaveCountAsync(0);
                }
                catch
                {
                    Console.Write($"Error occured on {Page.Url}:");
                    throw;
                }
            }
        }

        [Test]
        public async Task NavbarDropdownTogglerBorderToggles()
        {
            foreach (var Page in Context.Pages)
            {
                try
                {
                    var dropdownButton = Page.Locator(".navbar-toggler");
                    await Expect(dropdownButton).ToHaveCSSAsync("color", "rgba(0, 0, 0, 0.55)");
                    await dropdownButton.ClickAsync();
                    await Expect(dropdownButton).ToHaveCSSAsync("color", "rgb(249, 208, 64)");
                    await dropdownButton.ClickAsync();
                    /* todo: make this test pass. see issue #32
                    await Expect(dropdownButton).ToHaveCSSAsync("color", "rgba(0, 0, 0, 0.55)"); */
                }
                catch
                {
                    Console.Write($"Error occured on {Page.Url}:");
                    throw;
                }
            }
        }

        [Test]
        public async Task NavbarDropdownDoesntBlockContent()
        {
            var Page = Context.Pages.Where(x => x.Url.EndsWith("/webhooks")).Single();
            var dropdownButton = Page.Locator(".navbar-toggler");
            await dropdownButton.ClickAsync();
            var webhookEntry = Page.Locator("#webhook-val");
            /* todo: make this test pass. see issue #32
            await webhookEntry.ClickAsync(new LocatorClickOptions() { Timeout = 1000 });
            await Expect(webhookEntry).ToBeFocusedAsync(new LocatorAssertionsToBeFocusedOptions() { Timeout = 1000 }); */
        }
    }
}