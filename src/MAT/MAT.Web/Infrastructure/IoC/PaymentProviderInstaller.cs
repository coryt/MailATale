using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MAT.Core.PaymentProcessor;
using MAT.Core.Services;

namespace MAT.Web.Infrastructure.IoC
{
    public class PaymentProviderInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                AllTypes
                    .FromAssemblyNamed("MAT.Core")
                    .BasedOn<IPaymentProcessor>()
                    .LifestyleTransient()
                    .WithService.Base());

            container.Register(Component.For<ShippingCalculator>().LifeStyle.Transient);
        }
    }
}