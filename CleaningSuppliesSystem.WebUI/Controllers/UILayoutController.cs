using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class UILayoutController : Controller
    {
        [HttpGet]
        public IActionResult Layout()
        {
            return View();
        }
    }
}
