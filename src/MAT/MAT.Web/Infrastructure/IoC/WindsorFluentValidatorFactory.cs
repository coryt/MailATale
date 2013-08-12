using System;
using Castle.MicroKernel;
using FluentValidation;

namespace MAT.Web.Infrastructure.IoC
{
    public class WindsorFluentValidatorFactory : ValidatorFactoryBase
    {
        private readonly IKernel _kernel;

        public WindsorFluentValidatorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _kernel.HasComponent(validatorType)
                       ? _kernel.Resolve<IValidator>(validatorType)
                       : null;
        }
    }
}