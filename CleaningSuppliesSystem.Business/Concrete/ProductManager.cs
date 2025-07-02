using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class ProductManager : GenericManager<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductManager(IRepository<Product> repository, IProductRepository productRepository)
            : base(repository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<Product>> TGetProductsWithCategoriesAsync()
        {
            return await _productRepository.GetProductsWithCategoriesAsync();
        }       
        public async Task<Product> TGetByIdAsyncWithCategory(int id)
        {
            return await _productRepository.GetByIdAsyncWithCategory(id);
        }
    }
}