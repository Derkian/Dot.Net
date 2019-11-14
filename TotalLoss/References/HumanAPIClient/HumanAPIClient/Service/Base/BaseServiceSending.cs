using System;
using System.Net;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Service.Interface;
using HumanAPIClient.Util;
using HumanAPIClient.Util.Exceptions;

namespace HumanAPIClient.Service.Base
{
    /// <summary>
    /// Classe abstrata que serve como base para todo o 
    /// tipo de envio de requisicao para o gateway de sms.
    /// </summary>
    public abstract class BaseServiceSending
    {
        /// <summary>
        /// Tamanho maximo da mensagem considerando "from"
        /// </summary>
        public const int BODY_MAX_LENGTH = 150;
        /// <summary>
        /// Pattern para formatacao de data
        /// </summary>
        public const int ID_MAX_LENGTH = 20;

        public const String PARAM_TO = "to";
	    public const String PARAM_MSG = "msg";
	    public const String PARAM_ID = "id";
	    public const String PARAM_FROM = "from";
	    public const String PARAM_SCHEDULE = "schedule";
	    public const String PARAM_CALLBACK_OPTION = "callbackOption";
    	
	    public const String PARAM_CODE = "code";
	    public const String PARAM_ACCOUNT = "account";
        public const String PARAM_DISPATCH = "dispatch";

        protected const String CONTENT_TYPE_APP_URLENCODED = "application/x-www-form-urlencoded";

        protected const String METHOD_POST = "POST";

        public const String DATE_FORMAT = "dd/MM/yyyy HH:mm:ss";

        private String account;
        private String password;

        private String contentType;
        private String method;

        private String host;

        private IWebProxy proxy;

        public String Account
        {
            get
            {
                return account;
            }
        }

        public String Password
        {
            get
            {
                return password;
            }
        }

        public String Host
        {
            get
            {
                return host;
            }

            set
            {
                host = value;
            }
        }

        public IWebProxy Proxy
        {
            get
            {
                return proxy;
            }

            set
            {
                proxy = value;
            }
        }

        protected String ContentType
        {
            get
            {
                return contentType;
            }

            set
            {
                contentType = value;
            }
        }

        protected String Method
        {
            get
            {
                return method;
            }

            set
            {
                method = value;
            }
        }


        public BaseServiceSending(String account, String password) 
        {
            this.account     = account;
            this.password    = password;

            this.ContentType = BaseServiceSending.CONTENT_TYPE_APP_URLENCODED;
            this.Method = METHOD_POST;
        }

        /// <summary>
        /// Envia uma requisicao para um sevidor
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<String> send(String parameters) 
        {
            HttpClientHelper http = new HttpClientHelper();

            HttpWebRequest webRequest = http.configureConection(this.Host, this.ContentType, this.Method, this.Proxy);

            http.sendRequest(parameters, webRequest);

            return http.getResponse(webRequest);
        }

        /// <summary>
        /// Valida parametros gerais da mensagem (campos obrigatorios, tamanho do texto, etc).
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <param name="from"></param>
        /// <param name="schedule"></param>
	    protected void validateMessage(String to, String message, String id, String from, String schedule) 
        {
            DateTime dt = new DateTime();
		    try 
            {
			    if (schedule != null) 
                {
                    dt = DateTime.ParseExact(schedule, BaseServiceSending.DATE_FORMAT, new CultureInfo("pt-BR"), DateTimeStyles.NoCurrentDateDefault);
			    }
		    }
            catch (ArgumentNullException e) 
            {
                throw new ClientHumanException("Date invalid.");
            }
            catch (ArgumentException e) 
            {
                throw new ClientHumanException("Date invalid.");
            }
            catch (FormatException e)
            {
			    throw new ClientHumanException("Date invalid.");
		    }
    		
		    this.validateMessage(to, message, id, from);
	    }
    	
        /// <summary>
        /// Valida parametros gerais da mensagem (campos obrigatorios, tamanho do texto, etc).
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <param name="from"></param>
	    protected void validateMessage(String to, String message, String id, String from) 
        {
            if (id != null && id.Length > BaseServiceSending.ID_MAX_LENGTH) 
            {
                throw new ClientHumanException("Field \"" + PARAM_ID + "\" can not have more than " + BaseServiceSending.ID_MAX_LENGTH + " characters.");
		    }
    		
		    this.validateMessage(to, message, from);
	    }
    	
        /// <summary>
        /// Valida parametros gerais da mensagem (campos obrigatorios, tamanho do texto, etc).
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="from"></param>
	    protected void validateMessage(String to, String message, String from) 
        {
		    int length = BaseServiceSending.BODY_MAX_LENGTH;
		    if (from != null) 
            {
                length -= from.Length;
		    }
    		
		    this.validateMessage(to, message, length);
	    }
    	
        /// <summary>
        /// Valida parametros gerais da mensagem (campos obrigatorios, tamanho do texto, etc).
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
	    protected void validateMessage(String to, String message) 
        {
            int length = BaseServiceSending.BODY_MAX_LENGTH;
		    this.validateMessage(to, message, length);
	    }

	    private void validateMessage(String to, String message, int length) 
        {
		    if (to == null || to.Trim().Length.Equals(0)) 
            {
			    throw new ClientHumanException("Field \""+PARAM_TO+"\" is required.");
		    }

            if (message == null || message.Trim().Length.Equals(0)) 
            {
			    throw new ClientHumanException("Field \""+PARAM_MSG+"\" is required.");
		    } 
            else 
            {
			    if (length < message.Length) 
                {
                    throw new ClientHumanException("Fields \"" + PARAM_MSG + "\" + \"" + PARAM_FROM + "\" can not exceed " + BaseServiceSending.BODY_MAX_LENGTH + " characters.");
			    }
		    }
	    }

        /// <summary>
        /// Valida a conta e a senha.
        /// </summary>
	    protected void validateAccountAndPassword() 
        {
            if (String.IsNullOrEmpty(this.Account))
            {
			    throw new ClientHumanException("Field \"" + PARAM_ACCOUNT + "\" is required.");
		    }
            if (String.IsNullOrEmpty(this.Password))
            {
			    throw new ClientHumanException("Field \"" + PARAM_CODE + "\" is required.");
		    }
	    }
    }
}
