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

			// ... Run tests here

			testSiteManager.Stop();
		}
	}
}