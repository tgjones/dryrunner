using System.Web.Mvc;
using JetBrains.Annotations;

namespace DryRunner.TestWebsite
{
    [UsedImplicitly]
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}