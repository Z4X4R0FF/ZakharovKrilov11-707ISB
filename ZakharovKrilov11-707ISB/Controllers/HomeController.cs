using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZakharovKrilov11_707ISB.Models;

namespace ZakharovKrilov11_707ISB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}