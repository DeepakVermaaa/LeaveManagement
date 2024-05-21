using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Welcome()
        {
            return View();
        }
    }
}
