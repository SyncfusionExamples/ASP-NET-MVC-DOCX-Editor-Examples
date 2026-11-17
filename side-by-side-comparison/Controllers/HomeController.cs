using System.Web.Mvc;

namespace DocumentEditor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}