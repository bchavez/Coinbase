using System;
using FluentValidation;

namespace Coinbase.ObjectModel
{
    public class ButtonValidator : AbstractValidator<ButtonRequest>
    {
        public ButtonValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0m);

            RuleFor(x => x.Currency)
                .Must(x => Enum.IsDefined(typeof(Currency), x))
                .WithMessage("A valid currency must be used.");

            RuleFor(x => x.Repeat)
                .NotEmpty()
                .When(b => b.Subscription);
        }
    }
}