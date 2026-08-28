using Microsoft.AspNetCore.Mvc;

namespace Center_Management.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index - Welcome page
        public IActionResult Index()
        {
            // If user is logged in, redirect based on role
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("MyExams", "Exam");
                }
                else if (User.IsInRole("Teacher") || User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            // Show welcome page for non-authenticated users
            return View();
        }
    }
}
