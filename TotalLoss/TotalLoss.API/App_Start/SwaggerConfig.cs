using System.Web.Http;
using WebActivatorEx;
using TotalLoss.API;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Collections.Generic;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace TotalLoss.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger("docs/{apiVersion}", c =>
                {
                    c.SingleApiVersion("v1", "TotalLoss.API");
                    c.PrettyPrint();

                    c.DocumentFilter<SwaggerAddEnumDescriptions>();
                });
        }
    }

    public class SwaggerAddEnumDescriptions : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            // add enum descriptions to result models
            foreach (KeyValuePair<string, Schema> schemaDictionaryItem in swaggerDoc.definitions)
            {
                Schema schema = schemaDictionaryItem.Value;
                foreach (KeyValuePair<string, Schema> propertyDictionaryItem in schema.properties)
                {
                    Schema property = propertyDictionaryItem.Value;
                    IList<object> propertyEnums = property.@enum;
                    if (propertyEnums != null && propertyEnums.Count > 0)
                    {
                        property.description += DescribeEnum(propertyEnums);
                    }
                }
            }

            // add enum descriptions to input parameters
            if (swaggerDoc.paths.Count > 0)
            {
                foreach (PathItem pathItem in swaggerDoc.paths.Values)
                {
                    DescribeEnumParameters(pathItem.parameters);

                    // head, patch, options, delete left out
                    List<Operation> possibleParameterisedOperations = new List<Operation> { pathItem.get, pathItem.post, pathItem.put };
                    possibleParameterisedOperations.FindAll(x => x != null).ForEach(x => DescribeEnumParameters(x.parameters));
                }
            }
        }

        private void DescribeEnumParameters(IList<Parameter> parameters)
        {
            if (parameters != null)
            {
                foreach (Parameter param in parameters)
                {
                    IList<object> paramEnums = param.@enum;
                    if (paramEnums != null && paramEnums.Count > 0)
                    {
                        param.description += DescribeEnum(paramEnums);
                    }
                }
            }
        }

        private string DescribeEnum(IList<object> enums)
        {
            List<string> enumDescriptions = new List<string>();
            foreach (object enumOption in enums)
            {
                enumDescriptions.Add(string.Format("{0} = {1}", (int)enumOption, Enum.GetName(enumOption.GetType(), enumOption)));
            }
            return string.Join(", ", enumDescriptions.ToArray());
        }

    }    
}