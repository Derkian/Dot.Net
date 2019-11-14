using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HumanAPIClient.Model;
using HumanAPIClient.Service.Base;
using HumanAPIClient.Util;

namespace HumanAPIClient.Service
{
    public class QueryMessage : BaseServiceSending
    {
        private const String DISPATCH_RECEIVED = "listReceived";
        private const String GATEWAY_URI = "https://api-http.zenvia.com/GatewayIntegration/msgSms.do";

        public QueryMessage(String account, String password)
            : base(account, password)          
        {
            base.Host =  GATEWAY_URI;
        }

        public QueryMessage(String account, String password, String host)
            : base(account, password)
        {
            base.Host = host;
        }

        public List<SimpleMessage> listReceivedSMS(){
            this.validateAccountAndPassword();
            base.ContentType = BaseServiceSending.CONTENT_TYPE_APP_URLENCODED;
            StringBuilder parameters = getParameters();
            Stream stream = sendMoRequest(parameters.ToString());
            List<SimpleMessage> messages = ResponseParser.getMessagesFromStream(stream);            
            return messages;
        }

        private StringBuilder getParameters()
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append(BaseServiceSending.PARAM_DISPATCH).Append("=").Append(DISPATCH_RECEIVED).Append("&");
            parameters.Append(BaseServiceSending.PARAM_ACCOUNT).Append("=").Append(this.Account).Append("&");
            parameters.Append(BaseServiceSending.PARAM_CODE).Append("=").Append(this.Password).Append("&");
            return parameters;
        }

        /// <summary>
        /// Envia uma requisicao para um sevidor e retorna a resposta como uma stream
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Stream sendMoRequest(String parameters)
        {
            HttpClientHelper http = new HttpClientHelper();
            HttpWebRequest webRequest = http.configureConection(this.Host, this.ContentType, this.Method, this.Proxy);
            http.sendRequest(parameters, webRequest);
            return http.getResponseAsStream(webRequest);
        }


        
    }
}
