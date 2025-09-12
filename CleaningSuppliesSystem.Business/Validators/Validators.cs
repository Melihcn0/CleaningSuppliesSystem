using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.ForgotPasswordDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.LoginDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.RegisterDtos;
using CleaningSuppliesSystem.DTO.DTOs.ResetPasswordDtos;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ToggleDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using FluentValidation;

namespace CleaningSuppliesSystem.Business.Validators
{
    public class Validators
    {
        public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
        {
            public CreateCategoryValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Ürün Grubu / Kategori adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Ürün Grubu / Kategori adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.SubCategoryId)
                    .NotNull().WithMessage("Alt kategori seçmelisiniz.");

                RuleFor(x => x.TopCategoryId)
                    .NotNull().WithMessage("Üst kategori seçmelisiniz.");

                RuleFor(x => x.ImageFile)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl))
                    .WithMessage("Ürün Grubu fotoğrafı gereklidir.");
            }
        }
        public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
        {
            public UpdateCategoryValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Ürün Grubu / Kategori adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Ürün Grubu / Kategori adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.SubCategoryId)
                    .NotNull().WithMessage("Alt kategori seçmelisiniz.");

                RuleFor(x => x.TopCategoryId)
                    .NotNull().WithMessage("Üst kategori seçmelisiniz.");

                RuleFor(x => x.ImageFile)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl))
                    .WithMessage("Ürün Grubu fotoğrafı gereklidir.");
            }
        }

        public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
        {
            public CreateOrderValidator()
            {
                RuleFor(x => x.OrderNote)
                    .MaximumLength(100).WithMessage("Sipariş notunuz en fazla 100 karakter olmalıdır.");
            }
        }
        public class UpdateOrderValidator : AbstractValidator<UpdateOrderDto>
        {
            public UpdateOrderValidator()
            {
                RuleFor(x => x.OrderNote)
                    .MaximumLength(100).WithMessage("Sipariş notunuz en fazla 100 karakter olmalıdır.");
            }
        }

        public class CreateProductValidator : AbstractValidator<CreateProductDto>
        {
            public CreateProductValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                    .MaximumLength(70).WithMessage("Ürün adı en fazla 70 karakter olmalıdır.");

                RuleFor(x => x.CategoryId)
                    .GreaterThan(0).WithMessage("Ürün Grubu / Kategori seçimi zorunludur.");

                RuleFor(x => x.BrandId)
                    .GreaterThan(0).WithMessage("Marka seçimi zorunludur.");

                RuleFor(x => x.UnitPrice)
                    .GreaterThan(0).WithMessage("Ürün birim fiyatı 0'dan büyük olmalı.")
                    .LessThanOrEqualTo(5000).WithMessage("Ürün birim fiyatı 5.000₺ geçemez.");

                RuleFor(x => x.VatRate)
                    .NotNull().WithMessage("KDV oranı boş bırakılamaz.")
                    .GreaterThanOrEqualTo(0).WithMessage("KDV oranı 0 veya daha büyük olmalı.")
                    .LessThanOrEqualTo(100).WithMessage("KDV 100’ü geçemez.");

                RuleFor(x => x.ImageFile)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl))
                    .WithMessage("Ürün fotoğrafı gereklidir.");

            }
        }
        public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
        {
            public UpdateProductValidator()
            {

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                    .MaximumLength(70).WithMessage("Ürün adı en fazla 70 karakter olmalıdır.");

                RuleFor(x => x.CategoryId)
                    .GreaterThan(0).WithMessage("Ürün Grubu / Kategori seçimi zorunludur.");

                RuleFor(x => x.BrandId)
                    .GreaterThan(0).WithMessage("Marka seçimi zorunludur.");

                RuleFor(x => x.UnitPrice)
                    .GreaterThan(0).WithMessage("Ürün birim fiyatı 0'dan büyük olmalı.")
                    .LessThanOrEqualTo(5000).WithMessage("Ürün birim fiyatı 5.000₺ geçemez.");

                RuleFor(x => x.VatRate)
                    .GreaterThanOrEqualTo(0).WithMessage("KDV oranı 0 veya daha büyük olmalı.")
                    .LessThanOrEqualTo(100).WithMessage("KDV 100’ü geçemez.");

                RuleFor(x => x.ImageFile)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl))
                    .WithMessage("Ürün fotoğrafı gereklidir.");
            }
        }
        public class UpdateDiscountValidator : AbstractValidator<UpdateDiscountDto>
        {
            public UpdateDiscountValidator()
            {
                RuleFor(x => x.DiscountRate)
                    .GreaterThanOrEqualTo(0).WithMessage("İndirim oranı 0 veya daha büyük olmalı.")
                    .LessThanOrEqualTo(100).WithMessage("İndirim %100’ü geçemez.");
            }
        }
        public class CreateFinanceValidator : AbstractValidator<CreateFinanceDto>
        {
            public CreateFinanceValidator()
            {
                RuleFor(x => x.Type)
                    .NotEmpty().WithMessage("Finans tipi seçimi zorunludur.");

                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Finans adı boş bırakılamaz.")
                    .MaximumLength(40).WithMessage("Finans adı en fazla 40 karakter olmalıdır.");

                RuleFor(x => x.Amount)
                    .NotEmpty().WithMessage("Finans miktarı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Finans miktarı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(50000).WithMessage("Finans miktarı 50000 TL'den fazla olamaz.");
            }
        }
        public class UpdateFinanceValidator : AbstractValidator<UpdateFinanceDto>
        {
            public UpdateFinanceValidator()
            {
                RuleFor(x => x.Type)
                    .NotEmpty().WithMessage("Finans tipi seçimi zorunludur.");

                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Finans adı boş bırakılamaz.")
                    .MaximumLength(40).WithMessage("Finans adı en fazla 40 karakter olmalıdır.");

                RuleFor(x => x.Amount)
                    .NotEmpty().WithMessage("Finans miktarı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Finans miktarı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(50000).WithMessage("Finans miktarı 50000 TL'den fazla olamaz.");
            }
        }
        public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
        {
            public UserRegisterValidator()
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                    .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Ad en fazla 30 karakter olabilir.");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                    .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Soyad en fazla 30 karakter olabilir.");

                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                    .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
                    .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olabilir.");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                    .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                    .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olabilir.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Şifre tekrar alanı boş bırakılamaz.")
                    .Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyumlu değil.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            }
        }
        public class UserLoginValidator : AbstractValidator<UserLoginDto>
        {
            public UserLoginValidator()
            {
                RuleFor(x => x.Identifier)
                    .NotEmpty().WithMessage("Kullanıcı adı veya email boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Kullanıcı adı veya email en fazla 50 karakter olabilir.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Şifre boş bırakılamaz")
                    .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                    .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                    .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            }
        }
        public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
        {
            public CreateRoleValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Rol tipi seçimi zorunludur.");
            }
        }
        public class UserForgotPasswordValidator : AbstractValidator<UserForgotPasswordDto>
        {
            public UserForgotPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                    .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");
            }
        } // WebUI için forgotPassword
        public class UserResetPasswordValidator : AbstractValidator<UserResetPasswordDto>
        {
            public UserResetPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                    .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                    .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olabilir.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Şifre tekrar alanı boş bırakılamaz.")
                    .Equal(x => x.NewPassword).WithMessage("Şifreler birbiriyle uyumlu değil.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            }
        } // WebUI için resetPassword
        public class RegisterValidator : AbstractValidator<RegisterDto>
        {
            public RegisterValidator()
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                    .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Ad en fazla 30 karakter olabilir.");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                    .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Soyad en fazla 30 karakter olabilir.");

                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                    .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
                    .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olabilir.");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                    .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                    .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olabilir.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Şifre tekrar alanı boş bırakılamaz.")
                    .Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyumlu değil.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            }
        } // API için registerValidator
        public class LoginValidator : AbstractValidator<LoginDto>
        {
            public LoginValidator()
            {
                RuleFor(x => x.Identifier)
                    .NotEmpty().WithMessage("Kullanıcı adı veya email boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Kullanıcı adı veya email en fazla 50 karakter olabilir.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Şifre boş bırakılamaz")
                    .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                    .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                    .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            }
        } // API için loginValidator
        public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
        {
            public ForgotPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                    .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");
            }
        } // API için forgotPasswordValidator
        public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
        {
            public ResetPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                    .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                    .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                    .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olabilir.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Şifre tekrar alanı boş bırakılamaz.")
                    .Equal(x => x.NewPassword).WithMessage("Şifreler birbiriyle uyumlu değil.")
                    .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            }
        } // API için resetPasswordValidator
        public class UserAssignRoleValidator : AbstractValidator<UserAssignRoleDto>
        {
            public UserAssignRoleValidator()
            {
                RuleFor(x => x.UserId)
                    .GreaterThan(0).WithMessage("User ID geçersiz.");

                RuleFor(x => x.Roles)
                    .NotEmpty().WithMessage("En az bir rol atanmalı.")
                    .Must(roles => roles.Any(r => r.RoleExist))
                    .WithMessage("En az bir rol seçilmelidir.");
            }
        }
        public class ToggleStatusValidator : AbstractValidator<ToggleStatusDto>
        {
            public ToggleStatusValidator()
            {
                RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Geçersiz ID.");
            }
        }
        public class CreateTopCategoryValidator : AbstractValidator<CreateTopCategoryDto>
        {
            public CreateTopCategoryValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Üst kategori adı boş olamaz.")
                    .MinimumLength(2).WithMessage("Üst kategori adı en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("AÜstlt kategori adı en fazla 50 karakter olabilir.");
            }
        }
        public class UpdateTopCategoryValidator : AbstractValidator<UpdateTopCategoryDto>
        {
            public UpdateTopCategoryValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Üst kategori adı boş olamaz.")
                    .MinimumLength(2).WithMessage("Üst kategori adı en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("AÜstlt kategori adı en fazla 50 karakter olabilir.");
            }
        }
        public class CreateSubCategoryValidator : AbstractValidator<CreateSubCategoryDto>
        {
            public CreateSubCategoryValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Alt kategori adı boş olamaz.")
                    .MinimumLength(2).WithMessage("Alt kategori adı en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("Alt kategori adı en fazla 50 karakter olabilir.");

                RuleFor(x => x.TopCategoryId).NotNull().WithMessage("Üst kategori seçmelisiniz.");
            }
        }
        public class UpdateSubCategoryValidator : AbstractValidator<UpdateSubCategoryDto>
        {
            public UpdateSubCategoryValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Alt kategori adı boş olamaz.")
                    .MinimumLength(2).WithMessage("Alt kategori adı en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("Alt kategori adı en fazla 50 karakter olabilir.");

                RuleFor(x => x.TopCategoryId)
                    .NotNull().WithMessage("Üst kategori seçmelisiniz.");
            }
        }

        public class CreateBrandValidator : AbstractValidator<CreateBrandDto>
        {
            public CreateBrandValidator()
            {
                RuleFor(x => x.CategoryId)
                    .NotNull().WithMessage("Ürün Grubu seçmelisiniz.");

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Marka adı boş olamaz.")
                    .MinimumLength(2).WithMessage("Marka adı en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("Marka adı en fazla 50 karakter olabilir.");
            }
        }
        public class UpdateBrandValidator : AbstractValidator<UpdateBrandDto>
        {
            public UpdateBrandValidator()
            {
                RuleFor(x => x.CategoryId)
                    .NotNull().WithMessage("Ürün Grubu seçmelisiniz.");

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Marka adı boş olamaz.")
                    .MinimumLength(2).WithMessage("Marka adı en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("Marka adı en fazla 50 karakter olabilir.");
            }
        }
        public class CreateStockOperationValidator : AbstractValidator<CreateStockOperationDto>
        {
            public CreateStockOperationValidator()
            {
                RuleFor(x => x.TopCategoryId)
                    .GreaterThan(0).WithMessage("Üst kategori seçmelisiniz.");

                RuleFor(x => x.SubCategoryId)
                    .GreaterThan(0).WithMessage("Alt kategori seçmelisiniz.");

                RuleFor(x => x.CategoryId)
                    .GreaterThan(0).WithMessage("Kategori seçmelisiniz.");

                RuleFor(x => x.BrandId)
                    .GreaterThan(0).WithMessage("Marka seçmelisiniz.");

                RuleFor(x => x.ProductId)
                    .GreaterThan(0).WithMessage("Ürün seçmelisiniz.");

                RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır.")
                    .LessThanOrEqualTo(100).WithMessage("Miktar 100'e eşit veya 100'den küçük olmalıdır.");
            }
        }

        public class CreateBannerValidator : AbstractValidator<CreateBannerDto>
        {
            public CreateBannerValidator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Başlık boş olamaz.")
                    .MaximumLength(100).WithMessage("Başlık en fazla 40 karakter olabilir.");

                RuleFor(x => x.Subtitle)
                    .NotEmpty().WithMessage("Alt başlık boş olamaz.")
                    .MaximumLength(150).WithMessage("Alt başlık en fazla 50 karakter olabilir.");

                RuleFor(x => x.Statistic1Title)
                    .NotEmpty().WithMessage("İstatistik 1 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 1 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic1)
                    .NotEmpty().WithMessage("İstatistik 1 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 1 en fazla 7 karakter olabilir.");

                RuleFor(x => x.Statistic2Title)
                    .NotEmpty().WithMessage("İstatistik 2 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 2 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic2)
                    .NotEmpty().WithMessage("İstatistik 2 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 2 en fazla 7 karakter olabilir.");

                RuleFor(x => x.Statistic3Title)
                    .NotEmpty().WithMessage("İstatistik 3 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 3 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic3)
                    .NotEmpty().WithMessage("İstatistik 3 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 3 en fazla 7 karakter olabilir.");

                RuleFor(x => x.Statistic4Title)
                    .NotEmpty().WithMessage("İstatistik 4 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 4 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic4)
                    .NotEmpty().WithMessage("İstatistik 4 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 4 en fazla 7 karakter olabilir.");

                RuleFor(x => x.ImageFile)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl))
                    .WithMessage("İkincil banner fotoğrafı gereklidir.");
            }

        }

        public class UpdateBannerValidator : AbstractValidator<UpdateBannerDto>
        {
            public UpdateBannerValidator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Banner başlığı boş olamaz.")
                    .MaximumLength(100).WithMessage("Başlık en fazla 40 karakter olabilir.");

                RuleFor(x => x.Subtitle)
                    .NotEmpty().WithMessage("Alt başlık boş olamaz.")
                    .MaximumLength(150).WithMessage("Alt başlık en fazla 50 karakter olabilir.");

                RuleFor(x => x.Statistic1Title)
                    .NotEmpty().WithMessage("İstatistik 1 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 1 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic1)
                    .NotEmpty().WithMessage("İstatistik 1 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 1 en fazla 7 karakter olabilir.");

                RuleFor(x => x.Statistic2Title)
                    .NotEmpty().WithMessage("İstatistik 2 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 2 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic2)
                    .NotEmpty().WithMessage("İstatistik 2 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 2 en fazla 7 karakter olabilir.");

                RuleFor(x => x.Statistic3Title)
                    .NotEmpty().WithMessage("İstatistik 3 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 3 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic3)
                    .NotEmpty().WithMessage("İstatistik 3 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 3 en fazla 7 karakter olabilir.");

                RuleFor(x => x.Statistic4Title)
                    .NotEmpty().WithMessage("İstatistik 4 adı boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 4 en fazla 15 karakter olabilir.");

                RuleFor(x => x.Statistic4)
                    .NotEmpty().WithMessage("İstatistik 4 boş olamaz.")
                    .MaximumLength(50).WithMessage("İstatistik 4 en fazla 7 karakter olabilir.");
            }

        }

        public class CreateSecondaryBannerValidator : AbstractValidator<CreateSecondaryBannerDto>
        {
            public CreateSecondaryBannerValidator()
            {
                RuleFor(x => x.Title1)
                    .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                    .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

                RuleFor(x => x.Title2)
                    .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                    .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

                RuleFor(x => x.Title3)
                    .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                    .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

                RuleFor(x => x.Description1)
                    .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                    .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

                RuleFor(x => x.Description2)
                    .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                    .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

                RuleFor(x => x.Description3)
                    .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                    .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

                RuleFor(x => x.ButtonTitle1)
                    .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                    .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

                RuleFor(x => x.ButtonTitle2)
                    .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                    .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

                RuleFor(x => x.ButtonTitle3)
                    .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                    .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

                RuleFor(x => x.ImageFile1)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl1))
                    .WithMessage("İkincil banner fotoğrafı gereklidir.");

                RuleFor(x => x.ImageFile2)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl2))
                    .WithMessage("İkincil banner fotoğrafı gereklidir.");

                RuleFor(x => x.ImageFile3)
                    .Must((dto, file) => file != null || !string.IsNullOrWhiteSpace(dto.ImageUrl3))
                    .WithMessage("İkincil banner fotoğrafı gereklidir.");
            }
        }

        public class UpdateSecondaryBannerValidator : AbstractValidator<UpdateSecondaryBannerDto>
        {
            public UpdateSecondaryBannerValidator()
            {
                RuleFor(x => x.Title1)
                    .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                    .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

                RuleFor(x => x.Title2)
                    .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                    .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

                RuleFor(x => x.Title3)
                    .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                    .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

                RuleFor(x => x.Description1)
                    .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                    .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

                RuleFor(x => x.Description2)
                    .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                    .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

                RuleFor(x => x.Description3)
                    .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                    .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

                RuleFor(x => x.ButtonTitle1)
                    .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                    .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

                RuleFor(x => x.ButtonTitle2)
                    .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                    .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

                RuleFor(x => x.ButtonTitle3)
                    .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                    .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");
            }
        }
        public class CreateServiceIconValidator : AbstractValidator<CreateServiceIconDto>
        {
            public CreateServiceIconValidator()
            {
                RuleFor(x => x.IconName)
                    .NotEmpty().WithMessage("İkon adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("İkon adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.IconUrl)
                    .NotNull().WithMessage("İkon Url boş bırakılamaz.")
                    .MaximumLength(70).WithMessage("İkon Url en fazla 70 karakter olmalıdır.");

            }
        }
        public class UpdateServiceIconValidator : AbstractValidator<UpdateServiceIconDto>
        {
            public UpdateServiceIconValidator()
            {
                RuleFor(x => x.IconName)
                    .NotEmpty().WithMessage("İkon adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("İkon adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.IconUrl)
                    .NotNull().WithMessage("İkon Url boş bırakılamaz.")
                    .MaximumLength(70).WithMessage("İkon Url en fazla 70 karakter olmalıdır.");
            }
        }

        public class CreateServiceValidator : AbstractValidator<CreateServiceDto>
        {
            public CreateServiceValidator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Hizmet adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Hizmet adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Hizmet açıklaması boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Hizmet açıklaması en fazla 100 karakter olmalıdır.");

                RuleFor(x => x.ServiceIconId)
                    .NotNull().WithMessage("Hizmet iconu seçmelisiniz.");
            }
        }

        public class UpdateServiceValidator : AbstractValidator<UpdateServiceDto>
        {
            public UpdateServiceValidator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Hizmet adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Hizmet adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Hizmet açıklaması boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Hizmet açıklaması en fazla 100 karakter olmalıdır.");

                RuleFor(x => x.ServiceIconId)
                    .NotNull().WithMessage("Hizmet iconu seçmelisiniz.");
            }
        }
        public class UpdateCustomerProfileValidator : AbstractValidator<UpdateCustomerProfileDto>
        {
            public UpdateCustomerProfileValidator()
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Ad boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Soyad boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                    .MaximumLength(30).WithMessage("Kullanıcı adı en fazla 30 karakter olabilir.");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("E-posta boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir e-posta giriniz.")
                    .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir.");

                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                    .MinimumLength(17).WithMessage("Telefon numarası en az 17 karakter olmalıdır.")
                    .MaximumLength(17).WithMessage("Telefon numarası en fazla 17 karakter olmalıdır.");

                RuleFor(x => x.NationalId)
                    .NotEmpty().WithMessage("Kimlik numarası boş bırakılamaz.")
                    .MaximumLength(11).WithMessage("Kimlik numarası en fazla 11 karakter olmalıdır.");
            }
        }

        public class CreateIndividualAddressValidator : AbstractValidator<CreateCustomerIndividualAddressDto>
        {
            public CreateIndividualAddressValidator()
            {
                RuleFor(x => x.CityId)
                    .NotNull().WithMessage("Şehir seçmelisiniz.");

                RuleFor(x => x.DistrictId)
                    .NotEmpty().WithMessage("İlçe seçmelisiniz.");

                RuleFor(x => x.AddressTitle)
                    .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                    .MaximumLength(35).WithMessage("Adres başlığı en fazla 35 karakter olmalıdır.");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                    .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");
            }
        }

        public class CreateCorporateAddressValidator : AbstractValidator<CreateCustomerCorporateAddressDto>
        {
            public CreateCorporateAddressValidator()
            {
                RuleFor(x => x.CityId)
                    .NotNull().WithMessage("Şehir seçmelisiniz.");

                RuleFor(x => x.DistrictId)
                    .NotEmpty().WithMessage("İlçe seçmelisiniz.");

                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olmalıdır.");

                RuleFor(x => x.TaxOffice)
                    .NotEmpty().WithMessage("Vergi dairesi boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Vergi dairesi 100 karakter olmalıdır.");

                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage("Vergi numarası boş bırakılamaz.")
                    .Length(11).WithMessage("Vergi numarası 11 karakter olmalıdır.");

                RuleFor(x => x.AddressTitle)
                    .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                    .MaximumLength(35).WithMessage("Adres başlığı en fazla 35 karakter olmalıdır.");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                    .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");

            }
        }

        public class UpdateIndividualAddressValidator : AbstractValidator<UpdateCustomerIndividualAddressDto>
        {
            public UpdateIndividualAddressValidator()
            {
                RuleFor(x => x.CityId)
                    .NotNull().WithMessage("Şehir seçmelisiniz.");

                RuleFor(x => x.DistrictId)
                    .NotEmpty().WithMessage("İlçe seçmelisiniz.");

                RuleFor(x => x.AddressTitle)
                    .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                    .MaximumLength(35).WithMessage("Adres başlığı en fazla 35 karakter olmalıdır.");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                    .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");

            }
        }

        public class UpdateCorporateAddressValidator : AbstractValidator<UpdateCustomerCorporateAddressDto>
        {
            public UpdateCorporateAddressValidator()
            {
                RuleFor(x => x.CityId)
                    .NotNull().WithMessage("Şehir seçmelisiniz.");

                RuleFor(x => x.DistrictId)
                    .NotEmpty().WithMessage("İlçe seçmelisiniz.");

                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olmalıdır.");

                RuleFor(x => x.TaxOffice)
                    .NotEmpty().WithMessage("Vergi dairesi boş bırakılamaz.");

                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage("Vergi numarası boş bırakılamaz.")
                    .Length(11).WithMessage("Vergi numarası 11 karakter olmalıdır.");

                RuleFor(x => x.AddressTitle)
                    .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                    .MaximumLength(35).WithMessage("Adres başlığı en fazla 35 karakter olmalıdır.");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                    .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");

            }
        }

        public class CreateLocationCityValidator : AbstractValidator<CreateLocationCityDto>
        {
            public CreateLocationCityValidator()
            {
                RuleFor(x => x.CityName)
                    .NotEmpty().WithMessage("Şehir adı boş bırakılamaz.")
                    .MaximumLength(15).WithMessage("Şehir adı en fazla 15 karakter olmalıdır.");
            }
        }
        public class CreateLocationDistrictValidator : AbstractValidator<CreateLocationDistrictDto>
        {
            public CreateLocationDistrictValidator()
            {
                RuleFor(x => x.CityId)
                    .NotNull().WithMessage("Şehir seçmelisiniz.");

                RuleFor(x => x.DistrictName)
                    .NotEmpty().WithMessage("İlçe adı boş bırakılamaz.")
                    .MaximumLength(25).WithMessage("İlçe adı en fazla 25 karakter olmalıdır.");
            }
        }
        public class UpdateCompanyAddressValidator : AbstractValidator<UpdateCompanyAddressDto>
        {
            public UpdateCompanyAddressValidator()
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olmalıdır.");

                RuleFor(x => x.TaxOffice)
                    .NotEmpty().WithMessage("Vergi dairesi boş bırakılamaz.");

                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage("Vergi numarası boş bırakılamaz.")
                    .Length(11).WithMessage("Vergi numarası 11 karakter olmalıdır.");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                    .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");

                RuleFor(x => x.CityName)
                    .NotEmpty().WithMessage("Şehir adı boş bırakılamaz.")
                    .MaximumLength(15).WithMessage("Şehir adı en fazla 15 karakter olmalıdır.");

                RuleFor(x => x.DistrictName)
                    .NotEmpty().WithMessage("İlçe adı boş bırakılamaz.")
                    .MaximumLength(25).WithMessage("İlçe adı en fazla 25 karakter olmalıdır.");
            }
        }

        public class UpdateAdminProfileValidator : AbstractValidator<UpdateCustomerProfileDto>
        {
            public UpdateAdminProfileValidator()
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Ad boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Soyad boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                    .MaximumLength(30).WithMessage("Kullanıcı adı en fazla 30 karakter olabilir.");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("E-posta boş bırakılamaz.")
                    .EmailAddress().WithMessage("Geçerli bir e-posta giriniz.")
                    .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir.");

                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                    .MinimumLength(17).WithMessage("Telefon numarası en az 17 karakter olmalıdır.")
                    .MaximumLength(17).WithMessage("Telefon numarası en fazla 17 karakter olmalıdır.");

                RuleFor(x => x.NationalId)
                    .NotEmpty().WithMessage("Kimlik numarası boş bırakılamaz.")
                    .MaximumLength(11).WithMessage("Kimlik numarası en fazla 11 karakter olmalıdır.");
            }
        }

        public class UpdateCompanyBankValidator : AbstractValidator<UpdateCompanyBankDto>
        {
            public UpdateCompanyBankValidator()
            {
                RuleFor(x => x.BankName)
                    .NotEmpty().WithMessage("Banka adı boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Banka adı en fazla 100 karakter olabilir.");

                RuleFor(x => x.AccountHolder)
                    .NotEmpty().WithMessage("Hesap sahibi boş bırakılamaz.")
                    .MaximumLength(100).WithMessage("Hesap sahibi en fazla 100 karakter olabilir.");

                RuleFor(x => x.IBAN)
                    .NotEmpty().WithMessage("IBAN boş bırakılamaz.")
                    .Length(26).WithMessage("Türkiye IBAN'ı 26 karakter olmalı.");
            }
        }
    }
}
