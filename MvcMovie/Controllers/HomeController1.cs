using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
