using AutoMapper;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.RegisterDtos;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;

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
            CreateMap<UpdateProductDto, Product>().ReverseMap();
            CreateMap<Product, ResultProductDto>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Brand.Category.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Brand.Category.Name))
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.Id))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.Name))
                .ForMember(dest => dest.TopCategoryId, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.TopCategory.Id))
                .ForMember(dest => dest.TopCategoryName, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.TopCategory.Name));


            // Finance
            CreateMap<CreateFinanceDto, Finance>().ReverseMap();
            CreateMap<ResultFinanceDto, Finance>().ReverseMap();
            CreateMap<UpdateFinanceDto, Finance>().ReverseMap();

            // Invoice
            CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id'yi mapleme
                .ReverseMap();

            // Order
            CreateMap<CreateOrderDto, Order>().ReverseMap();
            CreateMap<Order, ResultOrderDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice));


            CreateMap<UpdateOrderDto, Order>().ReverseMap();
            CreateMap<OrderStatusUpdateDto, Order>().ReverseMap();

            // OrderItem
            CreateMap<CreateOrderItemDto, OrderItem>().ReverseMap();
            CreateMap<UpdateOrderItemDto, OrderItem>().ReverseMap();
            CreateMap<OrderItem, ResultOrderItemDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            CreateMap<AppUser, RegisterDto>().ReverseMap();
            CreateMap<AppRole, CreateRoleDto>().ReverseMap(); 
            CreateMap<AppRole, ResultRoleDto>().ReverseMap();

            CreateMap<UpdateDiscountDto, Order>().ReverseMap();

            CreateMap<CreateBrandDto, Brand>().ReverseMap();
            CreateMap<UpdateBrandDto, Brand>().ReverseMap();
            CreateMap<ResultBrandDto, Brand>().ReverseMap();

            CreateMap<CreateSubCategoryDto, SubCategory>().ReverseMap();
            CreateMap<UpdateSubCategoryDto, SubCategory>().ReverseMap();
            CreateMap<ResultSubCategoryDto, SubCategory>().ReverseMap();

            CreateMap<CreateTopCategoryDto, TopCategory>().ReverseMap();
            CreateMap<UpdateTopCategoryDto, TopCategory>().ReverseMap();
            CreateMap<ResultTopCategoryDto, TopCategory>().ReverseMap();


            CreateMap<Product, ResultStockOperationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity ?? 0))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Brand.Category.Name))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.Name))
                .ForMember(dest => dest.TopCategoryName, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.TopCategory.Name));

            CreateMap<CreateBannerDto, Banner>().ReverseMap();
            CreateMap<UpdateBannerDto, Banner>().ReverseMap();
            CreateMap<ResultBannerDto, Banner>().ReverseMap();

            CreateMap<CreateSecondaryBannerDto, SecondaryBanner>().ReverseMap();
            CreateMap<UpdateSecondaryBannerDto, SecondaryBanner>().ReverseMap();
            CreateMap<ResultSecondaryBannerDto, SecondaryBanner>().ReverseMap();

            CreateMap<CreateServiceIconDto, ServiceIcon>().ReverseMap();
            CreateMap<UpdateServiceIconDto, ServiceIcon>().ReverseMap();
            CreateMap<ResultServiceIconDto, ServiceIcon>().ReverseMap();

            CreateMap<CreateServiceDto, Service>().ReverseMap();
            CreateMap<UpdateServiceDto, Service>().ReverseMap();
            CreateMap<Service, ResultServiceDto>()
                .ForMember(dest => dest.IconName, opt => opt.MapFrom(src => src.ServiceIcon.IconName))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.ServiceIcon.IconUrl));

            CreateMap<AppUser, CustomerProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
            .ForMember(dest => dest.LastLogoutAt, opt => opt.MapFrom(src => src.LastLogoutAt))
            .AfterMap((src, dest) =>
            {
                if (src.CustomerAddresses != null)
                {
                    var defaultAddress = src.CustomerAddresses.FirstOrDefault(a => a.IsDefault);
                    if (defaultAddress != null)
                    {
                        dest.Address = defaultAddress.Address;
                        dest.AddressTitle = defaultAddress.AddressTitle;
                    }
                }
            });

            CreateMap<UpdateCustomerProfileDto, AppUser>().ReverseMap();

            CreateMap<CreateCustomerAddressDto, CustomerAddress>();
            CreateMap<UpdateCustomerAddressDto, CustomerAddress>();
            CreateMap<CustomerAddress, CustomerAddressDto>();


        }
    }
}
