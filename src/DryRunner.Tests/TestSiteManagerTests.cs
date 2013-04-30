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
			var testSiteManager = new TestSiteManager(9000, "DryRunner.TestWebsite");
			testSiteManager.Start();

			try
			{
				using (var webClient = new WebClient())
				{
					var html = webClient.DownloadString("http://localhost:9000");
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