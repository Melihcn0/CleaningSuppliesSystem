using CleaningSuppliesSystem.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _client;

        public ProductController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var categoriesResponse = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("HomeProduct/home-categories");
            var shownCategories = categoriesResponse?
                .Where(x => x.IsShown && !x.IsDeleted)
                .ToList() ?? new List<ResultCategoryDto>();

            var productResponse = await _client.GetAsync("HomeProduct/home-products");
            if (!productResponse.IsSuccessStatusCode)
                return View(new ProductListViewModel());

            var productJson = await productResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ResultProductDto>>(productJson)
                           ?? new List<ResultProductDto>();

            if (categoryId.HasValue)
                products = products.Where(x => x.CategoryId == categoryId.Value).ToList();

            var viewModel = new ProductListViewModel
            {
                Categories = shownCategories,
                Products = products,
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }


    }
}
