using FluentValidation;
using MAT.Core.InputModels;

namespace MAT.Core.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterAccountModel>
    {
        public RegisterModelValidator() //IUserService userService
        {
            this.RuleFor(x => x.Email)
                .NotEmpty()
                //.Must(x => !userService.DoesUserExistWithUsername(x)).WithMessage("This user already exists")
                .Length(6, 20).WithMessage("Username must be between 6 and 20 characters long")
                .Matches("^[A-z0-9]+$").WithMessage("Username can only contain alpha numeric characters");
            this.RuleFor(x => x.Password)
                .NotEmpty()
                .Length(6, 10000).WithMessage("Password must be at least 6 characters long");
        }
    }
}