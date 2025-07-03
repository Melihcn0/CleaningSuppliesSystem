using AutoMapper;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;

namespace CleaningSuppliesSystem.API.Mapping
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            // Category
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<ResultCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            // Product
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<ResultProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();

            // StockEntry
            CreateMap<CreateStockEntryDto, StockEntry>().ReverseMap();
            CreateMap<UpdateStockEntryDto, StockEntry>().ReverseMap();
            CreateMap<ResultStockEntryDto, StockEntry>().ReverseMap();

            // Finance
            CreateMap<CreateFinanceDto, Finance>().ReverseMap();
            CreateMap<UpdateFinanceDto, Finance>().ReverseMap();
            CreateMap<ResultFinanceDto, Finance>().ReverseMap();

            // Invoice
            CreateMap<CreateInvoiceDto, Invoice>().ReverseMap();
            CreateMap<ResultInvoiceDto, Invoice>().ReverseMap();

            // Order
            CreateMap<CreateOrderDto, Order>().ReverseMap();
            CreateMap<ResultOrderDto, Order>().ReverseMap();
            CreateMap<UpdateOrderDto, Order>().ReverseMap();

            // OrderItem
            CreateMap<CreateOrderItemDto, OrderItem>().ReverseMap();
            CreateMap<UpdateOrderItemDto, OrderItem>().ReverseMap();
            CreateMap<ResultOrderItemDto, OrderItem>().ReverseMap();
        }
    }
}
