using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Filters;
using TotalLoss.API.Http.Formatting;
using TotalLoss.API.Models;
using TotalLoss.Repository;
using TotalLoss.Repository.Interface;
using TotalLoss.Service;
using Unity;
using Unity.Injection;

namespace TotalLoss.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();

            // Cria conexão de base dados 
            DbConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["strConn"].ToString());
            string zEnviaUser = ConfigurationManager.AppSettings["zEnviaUser"].ToString();
            string zEnviaPassword =  ConfigurationManager.AppSettings["zEnviaPassword"].ToString();
            string url = ConfigurationManager.AppSettings["url"].ToString();

            var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            config.EnableCors(cors);

            // Injeção do Serviços
            container.RegisterInstance<IDbConnection>(conexao);
            container.RegisterType<IIncidentAssessmentRepository, IncidentAssessmentRepository>();
            container.RegisterType<IConfigurationRepository, ConfigurationRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IQuestionRepository, QuestionRepository>();
            container.RegisterType<ISalvageCompanyRepository, SalvageCompanyRepository>();

            container.RegisterInstance(new IncidentAssessmentService(container.Resolve<IIncidentAssessmentRepository>(), zEnviaUser, zEnviaPassword, url));

            config.DependencyResolver = new UnityResolver(container);

            // Permite que a classe BasicAuthenticationAttribute utilize os serviços injetados
            var providers = config.Services.GetFilterProviders().ToList();
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
            
            config.Formatters.Add(
                new FormMultipartEncodedMediaTypeFormatter(() =>
                new MultipartReducedMemoryStreamProvider(() =>
                {
                    var memoryStreamManager = container.Resolve<RecyclableMemoryStreamManager>("UploadMemory");
                    return memoryStreamManager.GetStream();
                })));


            // Mapeamento de rotas das controllers
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
