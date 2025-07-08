using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
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
                    .NotEmpty().WithMessage("Kategori adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Kategori adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.ImageUrl)
                    .NotEmpty().WithMessage("Kategori resmi boş bırakılamaz.");
            }
        }
        public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
        {
            public UpdateCategoryValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Kategori adı boş bırakılamaz.")
                    .MaximumLength(50).WithMessage("Kategori adı en fazla 50 karakter olmalıdır.");

                RuleFor(x => x.ImageUrl)
                    .NotEmpty().WithMessage("Kategori resmi boş bırakılamaz.");
            }
        }
        public class CreateProductValidator : AbstractValidator<CreateProductDto>
        {
            public CreateProductValidator()
            {
                RuleFor(x => x.ImageUrl)
                    .NotEmpty().WithMessage("Kategori resmi boş bırakılamaz.");

                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("ürün Kategori seçimi zorunludur.");

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                    .MaximumLength(70).WithMessage("Ürün adı en fazla 70 karakter olmalıdır.");

                RuleFor(x => x.UnitPrice)
                    .NotEmpty().WithMessage("Ürün birim fiyatı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Ürün birim fiyatı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(1000).WithMessage("Ürün birim fiyatı 1000 TL'den fazla olamaz.");
            }
        }
        public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
        {
            public UpdateProductValidator()
            {
                RuleFor(x => x.ImageUrl)
                    .NotEmpty().WithMessage("Kategori resmi boş bırakılamaz.");

                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("ürün Kategori seçimi zorunludur.");

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                    .MaximumLength(70).WithMessage("Ürün adı en fazla 70 karakter olmalıdır.");

                RuleFor(x => x.UnitPrice)
                    .NotEmpty().WithMessage("Ürün birim fiyatı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Ürün birim fiyatı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(1000).WithMessage("Ürün birim fiyatı 1000 TL'den fazla olamaz.");
            }
        }
        public class DiscountValidator : AbstractValidator<UpdateProductDto>
        {
            public DiscountValidator()
            {
                RuleFor(x => x.DiscountRate)
                    .NotEmpty().WithMessage("İndirim oranı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("İndirim sıfırdan büyük olmalı.")
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
                    .MaximumLength(40).WithMessage("Ürün adı en fazla 40 karakter olmalıdır.");

                RuleFor(x => x.Amount)
                    .NotEmpty().WithMessage("Finans miktarı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Finans miktarı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(30000).WithMessage("Finans miktarı 30000 TL'den fazla olamaz.");
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
                    .MaximumLength(40).WithMessage("Ürün adı en fazla 40 karakter olmalıdır.");

                RuleFor(x => x.Amount)
                    .NotEmpty().WithMessage("Finans miktarı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Finans miktarı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(10000).WithMessage("Finans miktarı 10000 TL'den fazla olamaz.");
            }
        }
        public class CreateStockEntryValidator : AbstractValidator<CreateStockEntryDto>
        {
            public CreateStockEntryValidator()
            {
                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Stok seçimi zorunludur.");

                RuleFor(x => x.Quantity)
                    .NotEmpty().WithMessage("Stok miktarı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Stok miktarı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(30000).WithMessage("Stok miktarı 30000 TL'den fazla olamaz.");

                RuleFor(x => x.Description)
                    .MaximumLength(100).WithMessage("Açıklama en fazla 100 karakter olmalıdır.");
            }
        }
        public class UpdateStockEntryValidator : AbstractValidator<UpdateStockEntryDto>
        {
            public UpdateStockEntryValidator()
            {
                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Stok tipi seçimi zorunludur.");

                RuleFor(x => x.Quantity)
                    .NotEmpty().WithMessage("Stok miktarı boş bırakılamaz.")
                    .GreaterThan(0).WithMessage("Stok miktarı sıfırdan büyük olmalıdır.")
                    .LessThanOrEqualTo(10000).WithMessage("Stok miktarı 10000 TL'den fazla olamaz.");

                RuleFor(x => x.Description)
                    .MaximumLength(100).WithMessage("Açıklama en fazla 100 karakter olmalıdır.");
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
                    .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olabilir.");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Şifre tekrar alanı boş bırakılamaz.")
                    .Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyumlu değil.");
            }

        }
    }
}
