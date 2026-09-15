using System.Diagnostics;
using KrisanthiumCurrency.Models;
using Microsoft.AspNetCore.Mvc;

namespace KrisanthiumCurrency.Controllers
{
    public class HomeController : Controller {
        /// <summary>Halaman "Exchange Rate" - kurs terbaru + tombol Get Latest Rate + Sync</summary>
        public IActionResult Index() => View();

        /// <summary>Halaman "Exchange Rate History" - filter currency & date</summary>
        public IActionResult History() => View();

        public IActionResult Error() => View();
    }
}
