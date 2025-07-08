using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Diagnostics;
using DoneIt.Models;

namespace GeneradorQRWeb.Controllers
{
    public class QrController : Controller
    {
        private readonly ILogger<QrController> _logger;

        public QrController(ILogger<QrController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Qr(string texto)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            string model = Convert.ToBase64String(qrCodeImage);
            return View("~/Views/Home/Qr.cshtml", model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}