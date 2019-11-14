using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Util
{
    /// <summary>
    /// Classe responsavel por possuir ferramentas http.
    /// </summary>
    class HttpClientHelper
    {
        static HttpClientHelper()
        {
            // Configura o TLS 1.2
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        /// <summary>
        /// Executa uma requicao.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="webRequest"></param>
        public void sendRequest(String parameters, HttpWebRequest webRequest)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(parameters);

            Stream os = null;

            // send request
            webRequest.ContentLength = bytes.Length;   //Count bytes to send
            os = webRequest.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);          //Send it
            if (os != null)
            {
                os.Close();
            }
        }

        /// <summary>
        /// Busca os dados de resposta e formata em uma lista.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public List<String> getResponse(HttpWebRequest webRequest)
        {
            List<String> retorno = new List<String>();

            // get response
            WebResponse webResponse = webRequest.GetResponse();
            if (webResponse == null)
            {
                return null;
            }
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());

            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                retorno.Add(line);
            }

            return retorno;
        }

        /// <summary>
        /// Busca os dados de resposta e retorna uma stream.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public Stream getResponseAsStream(HttpWebRequest webRequest)
        {
            // get response
            WebResponse webResponse = webRequest.GetResponse();
            if (webResponse == null)
            {
                return null;
            }

            Stream responseStream = webResponse.GetResponseStream();

            return responseStream;
        }

        /// <summary>
        /// Configura uma conexao com o host
        /// </summary>
        /// <param name="host"></param>
        /// <param name="contentType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public HttpWebRequest configureConection(String host, String contentType, String method)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(host);
            webRequest.KeepAlive = false;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.ContentType = contentType;
            webRequest.Method = method;

            return webRequest;
        }

        /// <summary>
        /// Configura uma conexao com o host
        /// </summary>
        /// <param name="host"></param>
        /// <param name="contentType"></param>
        /// <param name="method"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public HttpWebRequest configureConection(String host, String contentType, String method, IWebProxy proxy)
        {
            HttpWebRequest webRequest = configureConection(host, contentType, method);

            webRequest.Proxy = proxy;

            return webRequest;
        }
    }
}
