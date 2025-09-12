using AutoMapper;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
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
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;

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
                .ForMember(dest => dest.TopCategoryName, opt => opt.MapFrom(src => src.Brand.Category.SubCategory.TopCategory.Name))

                // KDV dahil fiyat ve indirimli fiyat mapping
                .ForMember(dest => dest.PriceWithVat, opt => opt.MapFrom(src => src.UnitPrice * (1 + src.VatRate / 100)))
                .ForMember(dest => dest.DiscountedPriceWithVat, opt => opt.MapFrom(src =>
                    (src.UnitPrice * (1 + src.VatRate / 100)) * (1 - src.DiscountRate / 100)));



            // Finance
            CreateMap<CreateFinanceDto, Finance>().ReverseMap();
            CreateMap<ResultFinanceDto, Finance>().ReverseMap();
            CreateMap<UpdateFinanceDto, Finance>().ReverseMap();

            // Invoice
            // Invoice → InvoiceDto
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.GeneratedAt, opt => opt.MapFrom(src => src.GeneratedAt))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => src.InvoiceType))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.CustomerLastName))
                .ForMember(dest => dest.CustomerNationalId, opt => opt.MapFrom(src => src.CustomerNationalId))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src => src.CustomerPhoneNumber))
                .ForMember(dest => dest.CustomerCompanyName, opt => opt.MapFrom(src => src.CustomerCompanyName))
                .ForMember(dest => dest.CustomerTaxOffice, opt => opt.MapFrom(src => src.CustomerTaxOffice))
                .ForMember(dest => dest.CustomerTaxNumber, opt => opt.MapFrom(src => src.CustomerTaxNumber))
                .ForMember(dest => dest.CustomerAddressTitle, opt => opt.MapFrom(src => src.CustomerAddressTitle))
                .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.CustomerAddress))
                .ForMember(dest => dest.CustomerCityName, opt => opt.MapFrom(src => src.CustomerCityName))
                .ForMember(dest => dest.CustomerDistrictName, opt => opt.MapFrom(src => src.CustomerDistrictName))
                .ForMember(dest => dest.AdminFirstName, opt => opt.MapFrom(src => src.AdminFirstName))
                .ForMember(dest => dest.AdminLastName, opt => opt.MapFrom(src => src.AdminLastName))
                .ForMember(dest => dest.AdminPhoneNumber, opt => opt.MapFrom(src => src.AdminPhoneNumber))
                .ForMember(dest => dest.InvoiceCompanyName, opt => opt.MapFrom(src => src.InvoiceCompanyName))
                .ForMember(dest => dest.InvoiceCompanyTaxOffice, opt => opt.MapFrom(src => src.InvoiceCompanyTaxOffice))
                .ForMember(dest => dest.InvoiceCompanyTaxNumber, opt => opt.MapFrom(src => src.InvoiceCompanyTaxNumber))
                .ForMember(dest => dest.InvoiceCompanyAddress, opt => opt.MapFrom(src => src.InvoiceCompanyAddress))
                .ForMember(dest => dest.InvoiceCompanyCityName, opt => opt.MapFrom(src => src.InvoiceCompanyCityName))
                .ForMember(dest => dest.InvoiceCompanyDistrictName, opt => opt.MapFrom(src => src.InvoiceCompanyDistrictName))
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems));

            // InvoiceItem → InvoiceItemDto
            CreateMap<InvoiceItem, InvoiceItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.VatRate))
                .ForMember(dest => dest.VatAmount, opt => opt.MapFrom(src => src.VatAmount))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.DiscountRate, opt => opt.MapFrom(src => src.DiscountRate ?? 0))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
                .ForMember(dest => dest.DiscountedUnitPrice, opt => opt.MapFrom(src => src.DiscountedUnitPrice ?? 0));


            // Bireysel adres → Invoice
            CreateMap<CustomerIndividualAddress, Invoice>()
                .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => "Individual"))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.CustomerNationalId, opt => opt.MapFrom(src => src.AppUser.NationalId))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.CustomerCompanyName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerTaxOffice, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerTaxNumber, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceItems, opt => opt.Ignore()); // ürünleri manuel ekle

            // Kurumsal adres → Invoice
            CreateMap<CustomerCorporateAddress, Invoice>()
                .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => "Corporate"))
                .ForMember(dest => dest.CustomerCompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.CustomerTaxOffice, opt => opt.MapFrom(src => src.TaxOffice))
                .ForMember(dest => dest.CustomerTaxNumber, opt => opt.MapFrom(src => src.TaxNumber))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.CustomerNationalId, opt => opt.MapFrom(src => src.AppUser.NationalId))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.InvoiceItems, opt => opt.Ignore()); 


            // Order
            CreateMap<CreateOrderDto, Order>().ReverseMap();



            // Admin siparişleri için mapping (AdminBank ignore ediliyor)
            CreateMap<Order, AdminResultOrderDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems ?? new List<OrderItem>()))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice));

            // Müşteri siparişleri için mapping (AdminBank context ile geliyor)
            CreateMap<Order, CustomerResultOrderDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(dest => dest.AdminBank, opt => opt.MapFrom((src, dest, _, context) =>
                    context?.Items != null && context.Items.ContainsKey("AdminBank")
                        ? context.Items["AdminBank"] as CompanyBankDto
                        : null));



            CreateMap<UpdateOrderDto, Order>().ReverseMap();
            CreateMap<OrderStatusUpdateDto, Order>().ReverseMap();

            // OrderItem
            CreateMap<CreateOrderItemDto, OrderItem>().ReverseMap();
            CreateMap<UpdateOrderItemDto, OrderItem>().ReverseMap();
            CreateMap<OrderItem, ResultOrderItemDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.DiscountRate, opt => opt.MapFrom(src => src.DiscountRate))
                .ForMember(dest => dest.DiscountedUnitPrice, opt => opt.MapFrom(src => src.DiscountedUnitPrice))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Product.VatRate));



            CreateMap<AppUser, RegisterDto>().ReverseMap();
            CreateMap<AppRole, CreateRoleDto>().ReverseMap(); 
            CreateMap<AppRole, ResultRoleDto>().ReverseMap();

            CreateMap<UpdateDiscountDto, Order>().ReverseMap();

            CreateMap<CreateBrandDto, Brand>().ReverseMap();
            CreateMap<UpdateBrandDto, Brand>().ReverseMap();
            CreateMap<Brand, ResultBrandDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.Category.SubCategory.Name))
                .ForMember(dest => dest.TopCategoryName, opt => opt.MapFrom(src => src.Category.SubCategory.TopCategory.Name))
                .ReverseMap();

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
                     var defaultIndividual = src.CustomerIndividualAddresses?.FirstOrDefault(a => a.IsDefault);
                     if (defaultIndividual != null)
                     {
                         dest.Address = defaultIndividual.Address;
                         dest.AddressTitle = defaultIndividual.AddressTitle;
                         return;
                     }
                     var defaultCorporate = src.CustomerCorporateAddresses?.FirstOrDefault(a => a.IsDefault);
                     if (defaultCorporate != null)
                     {
                         dest.Address = defaultCorporate.Address;
                         dest.AddressTitle = defaultCorporate.AddressTitle;
                     }
                 });




            CreateMap<UpdateCustomerProfileDto, AppUser>().ReverseMap();

            CreateMap<CreateCustomerIndividualAddressDto, CustomerIndividualAddress>().ReverseMap();
            CreateMap<UpdateCustomerIndividualAddressDto, CustomerIndividualAddress>().ReverseMap();
            CreateMap<CustomerIndividualAddress, CustomerIndividualAddressDto>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CityName))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName));


            CreateMap<CreateCustomerCorporateAddressDto, CustomerCorporateAddress>().ReverseMap();
            CreateMap<UpdateCustomerCorporateAddressDto, CustomerCorporateAddress>().ReverseMap();
            CreateMap<CustomerCorporateAddress, CustomerCorporateAddressDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.TaxOffice, opt => opt.MapFrom(src => src.TaxOffice))
                .ForMember(dest => dest.TaxNumber, opt => opt.MapFrom(src => src.TaxNumber))
                .ForMember(dest => dest.AddressTitle, opt => opt.MapFrom(src => src.AddressTitle))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CityName))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName));

            CreateMap<CreateLocationCityDto, LocationCity>().ReverseMap();
            CreateMap<CreateLocationDistrictDto, LocationDistrict>().ReverseMap();

            CreateMap<LocationDistrict, ResultLocationDistrictDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.DeletedDate, opt => opt.MapFrom(src => src.DeletedDate));

            CreateMap<LocationCity, ResultLocationCityDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CityName))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.DeletedDate, opt => opt.MapFrom(src => src.DeletedDate));

            CreateMap<LocationCity, ResultLocationCityWithLocationDistrictDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CityName))
                .ForMember(dest => dest.Districts, opt => opt.MapFrom(src =>
                    src.Districts != null
                        ? src.Districts.Select(d => d.DistrictName).ToList()
                        : new List<string>()));

            CreateMap<AppUser, AdminProfileDto>()
                    .ForMember(dest => dest.CompanyBank, opt => opt.MapFrom(src => src.CompanyBank))
                    .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.CompanyAddress));

            CreateMap<AppUser, UpdateAdminProfileDto>().ReverseMap();

            CreateMap<CompanyAddress, CompanyAddressDto>().ReverseMap();
            CreateMap<CompanyAddress, UpdateCompanyAddressDto>().ReverseMap();
            //CreateMap<AppUser, UpdateCompanyAddressDto>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CompanyAddress.Id))
            //    .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyAddress.CompanyName))
            //    .ForMember(dest => dest.TaxOffice, opt => opt.MapFrom(src => src.CompanyAddress.TaxOffice))
            //    .ForMember(dest => dest.TaxNumber, opt => opt.MapFrom(src => src.CompanyAddress.TaxNumber))
            //    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.CompanyAddress.Address))
            //    .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CompanyAddress.CityName))
            //    .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.CompanyAddress.DistrictName));


            CreateMap<CompanyBank, CompanyBankDto>().ReverseMap();
            CreateMap<CompanyBank, UpdateCompanyBankDto>().ReverseMap();
            //CreateMap<AppUser, UpdateCompanyBankDto>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CompanyBank.Id))
            //    .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.CompanyBank.BankName))
            //    .ForMember(dest => dest.AccountHolder, opt => opt.MapFrom(src => src.CompanyBank.AccountHolder))
            //    .ForMember(dest => dest.IBAN, opt => opt.MapFrom(src => src.CompanyBank.IBAN));





        }
    }
}
