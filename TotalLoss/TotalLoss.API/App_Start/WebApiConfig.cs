using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using TotalLoss.API.App_Start;
using TotalLoss.API.Http.Formatting;
using TotalLoss.API.Http.Log;
using TotalLoss.Repository;
using TotalLoss.Repository.Interface;
using TotalLoss.Service;
using Unity;
using Unity.Lifetime;

namespace TotalLoss.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();
            
            string connectionString = ConfigurationManager.ConnectionStrings["strConn"].ToString();
            string mailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            string smtpAddress = ConfigurationManager.AppSettings["SmtpAddress"].ToString();
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"].ToString());
            string url = ConfigurationManager.AppSettings["url"].ToString();
            string zEnviaUser = ConfigurationManager.AppSettings["zEnviaUser"].ToString();
            string zEnviaPassword = ConfigurationManager.AppSettings["zEnviaPassword"].ToString();

            var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            config.EnableCors(cors);

            // Injeção do Serviços
            container.RegisterInstance<IWorkRepository>(new WorkRepository(connectionString));

            //container.RegisterInstance(new HistoryService(container.Resolve<IHistoryRepository>()));
            container.RegisterInstance(new ConfigurationService(container.Resolve<IWorkRepository>()));
            container.RegisterInstance(new EmailService(mailFrom, smtpAddress, smtpPort));
            container.RegisterInstance(new TowingCompanyService(container.Resolve<IWorkRepository>(),                                                                
                                                                container.Resolve<EmailService>()));
            container.RegisterInstance(new IncidentAssessmentService(container.Resolve<IWorkRepository>(),
                                                                     zEnviaUser,
                                                                     zEnviaPassword,
                                                                     url));

            config.DependencyResolver = new UnityResolver(container);
            
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new LoggerExceptionLogger());            

            config.Formatters.Add(
                new FormMultipartEncodedMediaTypeFormatter(() =>
                new MultipartReducedMemoryStreamProvider(() =>
                {
                    var memoryStreamManager = container.Resolve<RecyclableMemoryStreamManager>("UploadMemory");
                    return memoryStreamManager.GetStream();
                })));

            // Mapeamento de rotas das controllers
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
