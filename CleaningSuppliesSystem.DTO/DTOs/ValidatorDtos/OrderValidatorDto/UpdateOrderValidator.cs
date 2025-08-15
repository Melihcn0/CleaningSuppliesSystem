using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using FluentValidation;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.OrderNote)
            .MaximumLength(100).WithMessage("Sipariş notunuz en fazla 100 karakter olmalıdır.");
    }
}