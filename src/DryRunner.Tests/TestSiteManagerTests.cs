using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml.XPath;
using DryRunner.Options;
using DryRunner.Util;
using NUnit.Framework;

namespace DryRunner.Tests
{
  [TestFixture]
  public class TestSiteManagerTests
  {
    [TestFixtureSetUp]
    public void SetUp()
    {
      TestSiteDeployerOptions.DefaultMsBuildToolsVersion = MsBuildToolsVersion.v15_0;
    }

    [Test]
    public void CanDeploySite()
    {
      var manager = new TestSiteManager("DryRunner.TestWebsite");
      StartAndCheckSite(manager, "http://localhost:8888");
    }

    [Test]
    public void CanDeploySiteWithHiddenIisExpressWindow()
    {
      var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions { ShowIisExpressWindow = false });
      StartAndCheckSite(manager, "http://localhost:8888");
    }

    [Test]
    public void CanDeploySiteWithExplicitProjectFolderAndFileName()
    {
      var manager =
          new TestSiteManager(new TestSiteDeployerOptions("DryRunner.TestWebsite", "DryRunner.TestWebsite.csproj"));
      StartAndCheckSite(manager, "http://localhost:8888");
    }

    [Test]
    public void CanDeploySiteWithEnableWindowsAuthentication()
    {
      var manager = new TestSiteManager(
          "DryRunner.TestWebsite",
          new TestSiteServerOptions
          {
              EnableWindowsAuthentication = true
          });
      StartAndCheckSite(manager, "http://localhost:8888");
    }

    [Test]
    public void CanDeploySite_CustomPortAndApplicationPath()
    {
      var manager = new TestSiteManager(
          "DryRunner.TestWebsite",
          new TestSiteServerOptions
          {
              Port = 9000,
              ApplicationPath = "/blub"
          });
      StartAndCheckSite(manager, "http://localhost:9000/blub");
    }

    [Test]
    public void CanDeploySite_WithCustomDeployDirectory()
    {
      var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

      var manager = new TestSiteManager(new TestSiteDeployerOptions("DryRunner.TestWebsite") { DeployDirectory = tempPath });

      StartAndCheckSite(manager, "http://localhost:8888");

      var webConfigPath = Path.Combine(tempPath, "Web.config");
      Assert.That(File.Exists(webConfigPath), Is.True);

      Directory.Delete(tempPath, true);
    }

    [Test]
    public void CanDeploySite_WithHttps()
    {
      var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions { UseHttps = true });
      StartAndCheckSite(manager, "https://localhost:44333");
    }

    [Test]
    public void CanDeploySite_With64BitIis()
    {
      var manager = new TestSiteManager("DryRunner.TestWebsite", new TestSiteServerOptions { Use64BitIisExpress = true });
      StartAndCheckSite(manager, "http://localhost:8888");
    }

    [Test]
    public void CanChangeConfigAndRestart ()
    {
      var manager = new TestSiteManager("DryRunner.TestWebsite");

      manager.Start();

      // ACT
      manager.ChangeConfigurationAndRestart(
          x =>
          {
            var appSetting = x.XPathSelectElement("//appSettings/add[@key='Greeting']");
            Trace.Assert(appSetting != null, "appSetting != null");
            appSetting.SetAttributeValue("value", "Ola");
            return x;
          });

      // ASSERT
      CheckSite("http://localhost:8888", greeting: "Ola");
    }

    private static void StartAndCheckSite(TestSiteManager testSiteManager, string surfToUrl, string greeting = "Hello")
    {
      testSiteManager.Start();

      try
      {
        CheckSite(surfToUrl, greeting);
      }
      finally
      {
        testSiteManager.Stop();
      }
    }

    private static void CheckSite(string surfToUrl, string greeting)
    {
      using (var webClient = new WebClient())
      {
        var html = webClient.DownloadString(surfToUrl);
        Assert.That(html, Contains.Substring($"<h1>{greeting} World</h1>"));
      }
    }
  }
}