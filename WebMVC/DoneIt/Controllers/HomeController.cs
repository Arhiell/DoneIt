using System.Diagnostics;
using DoneIt.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoneIt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioLogueado")))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            return View();
        }

        public IActionResult Proyecto()
        {
            return RedirectToAction("Editor", "ProyectoEditor");
        }


        public IActionResult Perfil()
        {
            return View();
        }

        public IActionResult GenerarQR()
        {
            return View();
        }


        public IActionResult Cerrar()
        {
            return View();
        }



        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
       // public IActionResult Error()
        //
        //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
       // }
    }
}
