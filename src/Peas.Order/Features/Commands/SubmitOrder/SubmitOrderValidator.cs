using FluentValidation;

namespace Peas.Order.Features.Commands.SubmitOrder
{
    public class SubmitOrderValidator : AbstractValidator<SubmitOrderCommand>
    {
        public SubmitOrderValidator() 
        {
            RuleFor(x => x.UserId).NotNull().NotEmpty().Length(1, 100);
            //...Other Rules
        }
    }
}
