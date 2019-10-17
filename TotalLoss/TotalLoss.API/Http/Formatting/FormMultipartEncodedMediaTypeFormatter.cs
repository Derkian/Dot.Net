using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using System.Web.Http.ValueProviders.Providers;

namespace TotalLoss.API.Http.Formatting
{
    public class FormMultipartEncodedMediaTypeFormatter : MediaTypeFormatter
    {
        public const string SupportedMediaType = "multipart/form-data";
        private Func<MultipartStreamProvider> _multipartStreamProviderFactory;

        /// <summary>
        /// Instances a new <seealso cref="FormMultipartEncodedMediaTypeFormatter"/>
        /// </summary>
        /// <param name="multipartStreamProviderFactory"><seealso cref="MultipartStreamProvider"/> factory so you can inject your own code</param>
        public FormMultipartEncodedMediaTypeFormatter(Func<MultipartStreamProvider> multipartStreamProviderFactory)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(SupportedMediaType));
            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            SupportedEncodings.Add(Encoding.GetEncoding("iso-8859-1"));
            _multipartStreamProviderFactory = multipartStreamProviderFactory;
        }

        public override bool CanReadType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return false;
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream,
            HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (readStream == null) throw new ArgumentNullException(nameof(readStream));

            try
            {
                // load multipart data into memory 
                var multipartProvider = await content.ReadAsMultipartAsync(_multipartStreamProviderFactory())
                    .ConfigureAwait(false);
                // fill parts into a ditionary for later binding to model
                var modelDictionary = await ToModelDictionaryAsync(multipartProvider)
                    .ConfigureAwait(false);
                // bind data to model 
                return BindToModel(modelDictionary, type, formatterLogger);
            }
            catch (IOException ex)
            {
                if (formatterLogger == null)
                    throw;

                var e = ex.InnerException as InvalidOperationException;
                if (e?.Message?.ToLowerInvariant()?.Contains("requested capacity is too large") == true)
                {
                    var validationException = new ValidationException("File is too big", ex);
                    formatterLogger.LogError("File", validationException);

                }
                else
                {
                    formatterLogger.LogError(string.Empty, e);
                }

                return GetDefaultValueForType(type);
            }
            catch (Exception e)
            {
                if (formatterLogger == null)
                    throw;

                formatterLogger.LogError(string.Empty, e);
                return GetDefaultValueForType(type);
            }
        }

        private async Task<IDictionary<string, object>> ToModelDictionaryAsync(
            MultipartStreamProvider multipartProvider)
        {
            var dictionary = new Dictionary<string, object>();

            // iterate all parts 
            foreach (var part in multipartProvider.Contents)
            {
                // unescape the name 
                var name = part.Headers.ContentDisposition.Name.Trim('"');

                // if we have a filename, we treat the part as file upload,
                // otherwise as simple string, model binder will convert strings to other types. 
                if (MultipartReducedMemoryStreamProvider.SuggestsFileContent(part.Headers))
                {
                    // set null if no content was submitted to have support for [Required]
                    if (part.Headers.ContentLength.GetValueOrDefault() > 0)
                    {
                        dictionary[name] = new HttpPostedFileMultipart(
                            part.Headers.ContentDisposition.FileName.Trim('"'),
                            part.Headers.ContentType?.MediaType,
                            await part.ReadAsStreamAsync()
                        );
                    }
                    else
                    {
                        dictionary[name] = null;
                    }
                }
                else
                {
                    dictionary[name] = await part.ReadAsStringAsync();
                }
            }

            return dictionary;
        }


        private object BindToModel(IDictionary<string, object> data, Type type, IFormatterLogger formatterLogger)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (type == null) throw new ArgumentNullException(nameof(type));

            using (var config = new HttpConfiguration())
            {
                // if there is a requiredMemberSelector set, use this one by replacing the validator provider
                var validateRequiredMembers = RequiredMemberSelector != null && formatterLogger != null;
                if (validateRequiredMembers)
                {
                    config.Services.Replace(typeof(ModelValidatorProvider),
                        new RequiredMemberModelValidatorProvider(RequiredMemberSelector));
                }

                // create a action context for model binding
                var actionContext = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Configuration = config,
                        ControllerDescriptor = new HttpControllerDescriptor
                        {
                            Configuration = config
                        }
                    }
                };

                // create model binder context 
                var valueProvider = new NameValuePairsValueProvider(data, CultureInfo.InvariantCulture);
                var metadataProvider = actionContext.ControllerContext.Configuration.Services.GetModelMetadataProvider();
                var metadata = metadataProvider.GetMetadataForType(null, type);
                var modelBindingContext = new ModelBindingContext
                {
                    ModelName = string.Empty,
                    FallbackToEmptyPrefix = false,
                    ModelMetadata = metadata,
                    ModelState = actionContext.ModelState,
                    ValueProvider = valueProvider
                };

                // bind model 
                var modelBinderProvider = new CompositeModelBinderProvider(config.Services.GetModelBinderProviders());
                var binder = modelBinderProvider.GetBinder(config, type);
                var haveResult = binder.BindModel(actionContext, modelBindingContext);

                // log validation errors 
                if (formatterLogger != null)
                {
                    foreach (var modelStatePair in actionContext.ModelState)
                    {
                        foreach (var modelError in modelStatePair.Value.Errors)
                        {
                            if (modelError.Exception != null)
                            {
                                formatterLogger.LogError(modelStatePair.Key, modelError.Exception);
                            }
                            else
                            {
                                formatterLogger.LogError(modelStatePair.Key, modelError.ErrorMessage);
                            }
                        }
                    }
                }

                return haveResult ? modelBindingContext.Model : GetDefaultValueForType(type);
            }
        }
    }
}