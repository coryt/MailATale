using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MAT.Core.Models.Site;
using NLog;
using Raven.Client;
using Raven.Client.Document;

namespace MAT.Web.Infrastructure.IoC
{
    public class RavenDBInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IDocumentStore>()
                    .Instance(CreateDocumentStore())
                    .LifeStyle.Singleton,
                Component.For<IDocumentSession>()
                    .UsingFactoryMethod(GetSession)
                    .LifeStyle.PerWebRequest);


            container.Register(Component.For<SiteConfig>().UsingFactoryMethod(GetSiteConfig).LifeStyle.Transient);
        }

        private static SiteConfig GetSiteConfig(IKernel kernel)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var documentStore = kernel.Resolve<IDocumentStore>();

            SiteConfig config;
            using (var session = documentStore.OpenSession())
            {
                config = session.Load<SiteConfig>("Site/Config");
                if (config == null)
                {
                    logger.Fatal("SiteConfig not in db");
                    throw new Exception("SiteConfig is not found.");
                }
            }

            return config;
        }

        private static IDocumentStore CreateDocumentStore()
        {
            IDocumentStore documentStore = new DocumentStore
                {
                    ConnectionStringName = "RavenDB"
                }.Initialize();

            return documentStore;
        }

        private static IDocumentSession GetSession(IKernel kernel)
        {
            var documentStore = kernel.Resolve<IDocumentStore>();
            return documentStore.OpenSession();
        }
    }
}