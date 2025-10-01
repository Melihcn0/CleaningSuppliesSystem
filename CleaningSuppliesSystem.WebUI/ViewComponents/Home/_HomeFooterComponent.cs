using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeFooterComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

}
