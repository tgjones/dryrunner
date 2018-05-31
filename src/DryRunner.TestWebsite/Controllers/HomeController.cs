using System.Web.Mvc;

namespace DryRunner.TestWebsite.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}