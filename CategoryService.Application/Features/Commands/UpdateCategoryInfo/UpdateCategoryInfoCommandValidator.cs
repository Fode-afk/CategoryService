using FluentValidation;

namespace CategoryService.Application.Features.Commands.UpdateCategoryInfo;

public sealed class UpdateCategoryInfoCommandValidator : AbstractValidator<UpdateCategoryInfoCommand>
{
    public UpdateCategoryInfoCommandValidator()
    {
    
    }
}