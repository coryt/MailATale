using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using MAT.Core.Models.Site;
using MAT.Core.Tasks;
using MAT.Web.App_Start;
using MAT.Web.Infrastructure.Indexes;
using MAT.Web.Infrastructure.Tasks;
using NLog;
using Raven.Client;
using Raven.Client.Indexes;

namespace MAT.Web
{
    public class MvcApplication : HttpApplication
    {
        public static IWindsorContainer Container;

        public MvcApplication()
        {
            EndRequest += (sender, args) => TaskExecutor.StartExecuting();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configuration.Services.RemoveAll(typeof (System.Web.Http.Validation.ModelValidatorProvider), v => v is InvalidModelValidatorProvider);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Container = WindsorConfig.ConfigureWindsor(GlobalConfiguration.Configuration);

            var logger = LogManager.GetLogger(GetType().Name);

            var docStore = Container.Resolve<IDocumentStore>();
            
            TryCreatingIndexesOrRedirectToErrorPage(docStore);

            // In case the versioning bundle is installed, make sure it will version
            // only what we opt-in to version
            using (var session = docStore.OpenSession())
            {
                session.Store(new
                {
                    Exclude = true,
                    Id = "Raven/Versioning/DefaultConfiguration",
                });
                session.SaveChanges();
            }
            TaskExecutor.DocumentStore = docStore;

            var emailerTask = Container.Resolve<GiftEmailerTask>();
            TaskExecutor.ExecuteTask(emailerTask);

            logger.Info(Container.Resolve<SiteConfig>().ToString());
            logger.Info("Started MAT Website");
        }

        private static void TryCreatingIndexesOrRedirectToErrorPage(IDocumentStore docStore)
        {
            try
            {
                IndexCreation.CreateIndexes(typeof(Users_ByUserCredentialsEmail).Assembly, docStore);
                IndexCreation.CreateIndexes(typeof(UserSubscription_ByUserId).Assembly, docStore);
                IndexCreation.CreateIndexes(typeof(GiftPurchaseNotice_DueToday).Assembly, docStore);
            }
            catch (WebException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null)
                    throw;

                switch (socketException.SocketErrorCode)
                {
                    case SocketError.AddressNotAvailable:
                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                    case SocketError.TimedOut:
                    case SocketError.ConnectionRefused:
                    case SocketError.HostDown:
                    case SocketError.HostUnreachable:
                    case SocketError.HostNotFound:
                        HttpContext.Current.Response.Redirect("~/Views/Error/500.htm");
                        break;
                    default:
                        throw;
                }
            }
        }
    }
}