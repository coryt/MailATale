using FluentValidation;
using MAT.Core.InputModels;

namespace MAT.Core.Validators
{
    public class LogOnModelValidator : AbstractValidator<LogOnModel>
    {
        public LogOnModelValidator() //IUserService userService
        {
            this.RuleFor(x => x.Login)
                .NotEmpty()
                //.Must((model, property) => userService.DoesUserExistWithUsernameAndPassword(model.Username, model.Password))
                .WithMessage("User/password combination does not exist in our system");

            this.RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}