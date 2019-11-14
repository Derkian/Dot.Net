using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Enum;
using HumanAPIClient.Model;
using HumanAPIClient.Service.Base;
using HumanAPIClient.Service.Interface;

namespace HumanAPIClient.Service
{
    /// <summary>
    /// Classe responsavel pelos servicos destinados ao envio de 
    /// mensagens sms individuais para o gateway.
    /// </summary>
    public class SimpleSending : BaseServiceSending, ISimpleBaseServiceSending
    {
        private const String SEND = "send";
        private const String CHECK = "check";

        /// <summary>
        /// Construtor da classe que permite fornecer a conta e a senha
        /// para autenticacao no gateway.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public SimpleSending(String account, String password)
            : base(account, password)
        {
            this.Host = "https://api-http.zenvia.com/GatewayIntegration/msgSms.do";
            this.Method = SimpleSending.METHOD_POST;
            this.ContentType = BaseServiceSending.CONTENT_TYPE_APP_URLENCODED;
        }

        public List<String> send(Message msg)
        {
            this.validateAccountAndPassword();

            SimpleMessage message = (SimpleMessage)msg;

            this.validateMessage(message.To, message.Message, message.Id, message.From, message.Schedule);

            StringBuilder parameters = new StringBuilder();

            parameters.Append(SimpleSending.PARAM_DISPATCH).Append("=").Append(SimpleSending.SEND).Append("&");
            parameters.Append(SimpleSending.PARAM_ACCOUNT).Append("=").Append(this.Account).Append("&");
            parameters.Append(SimpleSending.PARAM_CODE).Append("=").Append(this.Password).Append("&");
            parameters.Append(SimpleSending.PARAM_TO).Append("=").Append(message.To).Append("&");
            parameters.Append(SimpleSending.PARAM_MSG).Append("=").Append(message.Message);

            if (message.From != null && !message.From.Trim().Length.Equals(0))
            {
                parameters.Append("&").Append(SimpleSending.PARAM_FROM).Append("=").Append(message.From);
            }

            if (message.Id != null && !message.Id.Trim().Length.Equals(0))
            {
                parameters.Append("&").Append(SimpleSending.PARAM_ID).Append("=").Append(message.Id);
            }
            if (message.Schedule != null)
            {
                DateTime dt = DateTime.ParseExact(message.Schedule, SimpleSending.DATE_FORMAT, new CultureInfo("pt-BR"), DateTimeStyles.NoCurrentDateDefault);
                parameters.Append("&").Append(SimpleSending.PARAM_SCHEDULE).Append("=").Append(dt.ToString(SimpleSending.DATE_FORMAT));
            }

            parameters.Append("&").Append(SimpleSending.PARAM_CALLBACK_OPTION).Append("=").Append((int)message.Callback);

            return base.send(parameters.ToString());
        }

        public List<String> query(String id)
        {
            this.validateAccountAndPassword();

            if (id == null || id.Trim().Length.Equals(0)) 
            {
                throw new Exception("id is empty.");
            }

            StringBuilder parameters = new StringBuilder();

            parameters.Append(SimpleSending.PARAM_DISPATCH).Append("=").Append(SimpleSending.CHECK).Append("&");
            parameters.Append(SimpleSending.PARAM_ACCOUNT).Append("=").Append(this.Account).Append("&");
            parameters.Append(SimpleSending.PARAM_CODE).Append("=").Append(this.Password).Append("&");
            parameters.Append(SimpleSending.PARAM_ID).Append("=").Append(id.Trim());

            return base.send(parameters.ToString());
        }
    }
}
