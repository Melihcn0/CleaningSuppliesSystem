using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LocationValidatorDto
{
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
}
