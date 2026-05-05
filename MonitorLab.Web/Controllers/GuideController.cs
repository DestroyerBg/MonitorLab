using Microsoft.AspNetCore.Mvc;

namespace MonitorLab.Web.Controllers
{
    public class GuideController : Controller
    {
        public IActionResult Crt() => View();
        public IActionResult Lcd() => View();
        public IActionResult Led() => View();
        public IActionResult Oled() => View();
        public IActionResult Qled() => View();
        public IActionResult MiniLed() => View();
        public IActionResult MicroLed() => View();
        public IActionResult Vga() => View();
        public IActionResult Dvi() => View();
        public IActionResult Hdmi() => View();
        public IActionResult DisplayPort() => View();
        public IActionResult UsbC() => View();
        public IActionResult Thunderbolt() => View();
    }
}
