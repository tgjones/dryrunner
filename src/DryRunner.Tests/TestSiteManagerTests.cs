using System.Net;
using NUnit.Framework;

namespace DryRunner.Tests
{
    [TestFixture]
    public class TestSiteManagerTests
    {
        //[Test]
        public void CanDeploySite()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite");
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySiteWithHiddenIisExpressWindow()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteOptions
            {
                ShowIisExpressWindow = false
            });
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySiteWithExplicitProjectFileName()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteOptions
            {
                ProjectFileName = "DryRunner.TestWebsite.csproj"
            });
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySiteWithEnableWindowsAuthentication()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteOptions
            {
                EnableWindowsAuthentication = true
            });
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySite_CustomPortAndApplicationPath()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteOptions
            {
                Port = 9000,
                ApplicationPath = "/blub"
            });
            CheckSite(manager, "http://localhost:9000/blub");
        }

        private static void CheckSite(TestSiteManager testSiteManager, string surfToUrl)
        {
            testSiteManager.Start();

            try
            {
                using (var webClient = new WebClient())
                {
                    var html = webClient.DownloadString(surfToUrl);
                    Assert.That(html, Contains.Substring("<h1>Hello World</h1>"));
                }
            }
            finally
            {
                testSiteManager.Stop();
            }
        }
    }
}