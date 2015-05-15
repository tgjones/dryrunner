DryRunner
==========

DryRunner provides isolated integration testing for ASP.NET. A common problem when doing automated integration testing against ASP.NET websites
is that tests can inadvertently modify your local development database, or other files within your development website. DryRunner solves that problem
by:

* deploying a test version of your website to a temporary location,
* hosting the test version of your website using IIS Express, and
* cleaning up afterwards by deleting the test version.

DryRunner requires you to create a `Test` build configuration (alongside the usual `Debug`, `Release` and any other build configurations you may already have).
You can use `Web.Test.config` to configure test-specific database connection strings, and other test-specific settings.

Installation
------------

Install the [DryRunner](https://nuget.org/packages/DryRunner) package using NuGet.

Configuration
-------------

You'll need to setup a couple of things before DryRunner will work:

1. Install IIS Express on your local development machine (and build server, if you want to use DryRunner there too).
2. Create a `Test` [build configuration](http://msdn.microsoft.com/en-us/library/kwybya3w.aspx) for your website project.
   Note that you don't need to create a new solution configuration, only a project-level configuration for the website
   project(s) that you want to test.

Usage
-----

```csharp
// You'd normally want to do this in a test fixture setup.
string websiteProjectName = "DryRunner.TestWebsite";
TestSiteManager testSiteManager = new TestSiteManager(websiteProjectName, new TestSiteOptions
{
	Port = 9000, // Any port that won't conflict with other services running on your computer (optional; default is 8888).
	ApplicationPath = "/blub" // Application path (optional; default is the server root "/").
});
testSiteManager.Start();

// Run your tests and point them to: http://localhost:9000/blub

// You'd normally want to do this in a test fixture teardown.
testSiteManager.Stop();
```

If you don't need to customise the port or application path, you can just do:

```csharp
string websiteProjectName = "DryRunner.TestWebsite";
TestSiteManager testSiteManager = new TestSiteManager(websiteProjectName);
testSiteManager.Start();
```
Additionally, if you are using MSBuild macros in your web application project, you can utilize additional parameters to configure the $(SolutionDir) and $(ProjectDir).  Just add the paths to your **TestSiteOptions** object using the *SolutionDir* and *ProjectDir*, respectively.

License
-------

DryRunner is released under the [MIT License](http://www.opensource.org/licenses/MIT).
