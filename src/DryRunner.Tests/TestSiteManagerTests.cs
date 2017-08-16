using System;
using System.IO;
using System.Net;
using DryRunner.Options;
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
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions {ShowIisExpressWindow = false});
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySiteWithExplicitProjectFileName()
        {
            var manager =
                new TestSiteManager(new TestSiteDeployerOptions("DryRunner.TestWebsite")
                {
                  ProjectFileName = "DryRunner.TestWebsite.csproj"
                });
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySiteWithEnableWindowsAuthentication()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions
            {
                EnableWindowsAuthentication = true
            });
            CheckSite(manager, "http://localhost:8888");
        }

        [Test]
        public void CanDeploySite_CustomPortAndApplicationPath()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions
            {
                Port = 9000,
                ApplicationPath = "/blub"
            });
            CheckSite(manager, "http://localhost:9000/blub");
        }

        [Test]
        public void CanDeploySite_WithCustomDeployDirectory()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var manager = new TestSiteManager(new TestSiteDeployerOptions("DryRunner.TestWebsite") {DeployDirectory = tempPath});

            CheckSite(manager, "http://localhost:8888");

            var webConfigPath = Path.Combine(tempPath, "Web.config");
            Assert.That(File.Exists(webConfigPath), Is.True);

            Directory.Delete(tempPath, true);
        }

        [Test]
        public void CanDeploySite_WithHttps()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions {UseHttps = true});
            CheckSite(manager, "https://localhost:44333");
        }

        [Test]
        public void CanDeploySite_With64BitIis()
        {
            var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions {Use64BitIisExpress = true});
            CheckSite(manager, "http://localhost:8888");
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