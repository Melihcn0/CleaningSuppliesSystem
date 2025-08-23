using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LocationValidatorDto
{
    public class CreateLocationCityValidator : AbstractValidator<CreateLocationCityDto>
    {
        public CreateLocationCityValidator()
        {
            RuleFor(x => x.CityName)
                .NotEmpty().WithMessage("Şehir adı boş bırakılamaz.")
                .MaximumLength(15).WithMessage("Şehir adı en fazla 15 karakter olmalıdır.");
        }
    }
}
