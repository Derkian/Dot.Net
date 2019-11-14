using AudatexWeb.NucleoCompartilhado;
using System;
using System.IO;
using System.Web.Http.ExceptionHandling;

namespace TotalLoss.API.Http.Log
{
    public class LoggerExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            try
            {
                Stream requestBodyStream = context.Request.Content.ReadAsStreamAsync().Result;
                requestBodyStream.Position = 0;
                string requestBody = string.Empty;

                using (StreamReader sr = new StreamReader(requestBodyStream))
                {
                    requestBody = sr.ReadToEnd();
                }


                LogErro.LogarErro(context.Exception,
                                  $" Method : {context.Request.Method.ToString()}, Body : {requestBody}",
                                  context.Request.RequestUri.AbsoluteUri);
            }
            catch
            {
            }
        }
    }
}