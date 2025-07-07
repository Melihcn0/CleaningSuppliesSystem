using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class UILayoutController : Controller
    {
        public IActionResult Layout()
        {
            return View();
        }
    }
}
