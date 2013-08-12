using System.Web.Http;
using System.Web.Mvc;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FluentValidation.Mvc;
using MAT.Core.Tasks;
using MAT.Web.Infrastructure.IoC;
using Raven.Client;

namespace MAT.Web.App_Start
{
    public class WindsorConfig
    {
        private static IWindsorContainer _container;

        public static IWindsorContainer ConfigureWindsor(HttpConfiguration configuration)
        {
            // Create / Initialize the container  
            _container = new WindsorContainer();
            // Find our IWindsorInstallers from this Assembly and optionally from our DI assembly which is in abother project.  

            _container.Install(FromAssembly.This());
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            //Documentation http://docs.castleproject.org/Windsor.Resolvers.ashx  
            //To support typed factories add this:  
            _container.AddFacility<TypedFactoryFacility>();
            //Documentation http://docs.castleproject.org/Windsor.Typed-Factory-Facility.ashx  
            // Set the WebAPI DependencyResolver to our new WindsorDependencyResolver  
            var dependencyResolver = new WindsorDependencyResolver(_container);
            configuration.DependencyResolver = dependencyResolver;

            var controllerFactory = new WindsorControllerFactory(_container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(_container);

            //setup fluent validation
            var fluentValidationModelValidatorProvider =
                new FluentValidationModelValidatorProvider(new WindsorFluentValidatorFactory(_container.Kernel));

            //add fluent validator to the ModelValidatorProviders collection
            ModelValidatorProviders.Providers.Add(fluentValidationModelValidatorProvider);

            TaskExecutor.DocumentStore = _container.Resolve<IDocumentStore>();
            return _container;
        }
    }
}