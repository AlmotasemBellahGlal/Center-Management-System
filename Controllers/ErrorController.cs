using Microsoft.AspNetCore.Mvc;

namespace Center_Management.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/Unauthorized
        public new IActionResult Unauthorized()
        {
            return View();
        }

        // GET: Error/NotFound
        public new IActionResult NotFound()
        {
            return View();
        }

        // GET: Error/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}
