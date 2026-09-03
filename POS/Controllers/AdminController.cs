using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}