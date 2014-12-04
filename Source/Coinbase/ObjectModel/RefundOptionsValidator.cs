using FluentValidation;

namespace Coinbase.ObjectModel
{
    public class RefundOptionsValidator : AbstractValidator<RefundOptions>
    {
        public RefundOptionsValidator()
        {
            RuleFor(x => x.RefundIsoCurrency)
                .NotEmpty();
        }
    }
}