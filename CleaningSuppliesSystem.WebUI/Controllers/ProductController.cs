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

        public async Task<IActionResult> Index(int? categoryId, int page = 1, int pageSize = 10)
        {
            // Kategorileri al
            var categoriesResponse = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("HomeProduct/home-categories");
            var shownCategories = categoriesResponse?
                .Where(x => x.IsShown && !x.IsDeleted)
                .ToList() ?? new List<ResultCategoryDto>();

            // Ürünleri API üzerinden al (liste)
            string url = "HomeProduct/home-products";
            var products = await _client.GetFromJsonAsync<List<ResultProductDto>>(url)
                           ?? new List<ResultProductDto>();

            if (categoryId.HasValue)
                products = products.Where(x => x.CategoryId == categoryId.Value).ToList();

            // Sayfalama işlemini burada yap
            var pagedProducts = new PagedResponse<ResultProductDto>
            {
                Data = products.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = products.Count,
                TotalPages = (int)Math.Ceiling(products.Count / (double)pageSize)
            };

            // ViewModel
            var viewModel = new ProductListViewModel
            {
                Categories = shownCategories,
                PagedProducts = pagedProducts,
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }



    }
}
