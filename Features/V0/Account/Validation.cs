using FluentValidation;

namespace MoExpenseTracker.Features.V0.Account;


class AccountUpdateValidation : AbstractValidator<UpdateProfileDto>
{
    public AccountUpdateValidation()
    {
        RuleFor(dto => dto.Name)
        .NotNull()
        .NotEmpty()
        .MinimumLength(5)
        .MaximumLength(255);

        RuleFor(dto => dto.Email)
        .NotNull()
        .NotEmpty()
        .EmailAddress();
    }
}