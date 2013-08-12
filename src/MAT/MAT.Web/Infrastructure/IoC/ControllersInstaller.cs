using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MAT.Web.Controllers;

namespace MAT.Web.Infrastructure.IoC
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                   .BasedOn<MATController>()
                                   .If(t => t.Name.EndsWith("Controller"))
                                   .Configure(c => c.LifestyleTransient()));

            container.Register(Classes.FromAssemblyNamed("MAT.Core")
                                   .BasedOn<Controller>()
                                   .If(t => t.Name.EndsWith("Controller"))
                                   .Configure(c => c.LifestyleTransient()));
        }
    }
}