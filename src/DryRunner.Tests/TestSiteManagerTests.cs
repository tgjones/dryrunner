using System;
using System.Net;
using NUnit.Framework;

namespace DryRunner.Tests
{
    [TestFixture]
    public class TestSiteManagerTests
    {
        [Test]
        public void CanDeploySite()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite");
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySiteWithHiddenIisExpressWindow()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", showIisExpressWindow: false);
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySite_CustomPortAndApplicationPath()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", port: 9000, applicationPath: "/blub");
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