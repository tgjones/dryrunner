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
int port = 9000; // Any port that won't conflict with other services running on your computer.
string websiteProjectName = "DryRunner.TestWebsite";
TestSiteManager testSiteManager = new TestSiteManager(port, websiteProjectName);
testSiteManager.Start();

// ... Run tests here

// You'd normally want to do this in a test fixture teardown.
testSiteManager.Stop();
```

License
-------

DryRunner  is released under the [MIT License](http://www.opensource.org/licenses/MIT).