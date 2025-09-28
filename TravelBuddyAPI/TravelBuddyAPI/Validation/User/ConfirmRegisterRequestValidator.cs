using FluentValidation;
using TravelBuddyAPI.DTOs;
namespace TravelBuddyAPI.Validation.User

{
    public class ConfirmRegisterRequestValidator : AbstractValidator<ConfirmRegisterRequestDto>
    {
        public ConfirmRegisterRequestValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
            RuleFor(x => x.TokenType).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.ExpiresAt).NotEmpty();
            RuleFor(x => x.ExpiresIn).NotEmpty();
        }
    }
}
