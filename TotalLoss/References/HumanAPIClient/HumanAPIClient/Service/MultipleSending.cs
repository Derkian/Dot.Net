using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Enum;
using HumanAPIClient.Model;
using HumanAPIClient.Service.Base;
using HumanAPIClient.Service.Interface;
using HumanAPIClient.Util.Exceptions;
using System.Text.RegularExpressions;

namespace HumanAPIClient.Service
{
    public class MultipleSending : BaseServiceSending, IMultipleBaseServiceSending
    {
        private const String SEND_MULTIPLE = "sendMultiple";
        private const String CHECK_MULTIPLE = "checkMultiple";

        public const String PARAM_FILE = "file";
	    public const String PARAM_LIST = "list";
	    public const String PARAM_TYPE = "type";
        public const String PARAM_ID_LIST = "idList";

        public MultipleSending(String account, String password)
            : base(account, password)
        {
            base.Host = "https://api-http.zenvia.com/GatewayIntegration/msgSms.do";
        }

        public List<String> send(Message msg)
        {
            this.validateAccountAndPassword();

            MultipleMessage message = (MultipleMessage)msg;

            this.validateSend(message);

            StringBuilder parameters = new StringBuilder();

            parameters.Append(MultipleSending.PARAM_DISPATCH).Append("=").Append(MultipleSending.SEND_MULTIPLE).Append("&");
            parameters.Append(MultipleSending.PARAM_ACCOUNT).Append("=").Append(this.Account).Append("&");
            parameters.Append(MultipleSending.PARAM_CODE).Append("=").Append(this.Password).Append("&");
            parameters.Append(MultipleSending.PARAM_TYPE).Append("=").Append((char)message.Type).Append("&");
            parameters.Append(MultipleSending.PARAM_CALLBACK_OPTION).Append("=").Append((int)message.Callback).Append("&");
            parameters.Append(MultipleSending.PARAM_LIST).Append("=").Append(message.Content);

            base.ContentType = BaseServiceSending.CONTENT_TYPE_APP_URLENCODED;
            return base.send(parameters.ToString());
        }

        public List<String> query(String[] ids)
        {
            this.validateAccountAndPassword();

            if (ids == null || ids.Length.Equals(0)) {
                throw new Exception("id is empty.");
            }
            else if (ids.Length.Equals(1))
            {
                SimpleSending simple = new SimpleSending(this.Account, this.Password);
                simple.query(ids[0]);
            }

            String paramsId = "";
            
            int length = ids.Length;
            for (int i = 0; i < length; i++)
            {
                paramsId += ids[i];
                if (i < (length - 1))
                {
                    paramsId += ";";
                }
            }

            StringBuilder parameters = new StringBuilder();

            parameters.Append(MultipleSending.PARAM_DISPATCH).Append("=").Append(MultipleSending.CHECK_MULTIPLE).Append("&");
            parameters.Append(MultipleSending.PARAM_ACCOUNT).Append("=").Append(this.Account).Append("&");
            parameters.Append(MultipleSending.PARAM_CODE).Append("=").Append(this.Password).Append("&");
            parameters.Append(MultipleSending.PARAM_ID_LIST).Append("=").Append(paramsId);

            base.ContentType = BaseServiceSending.CONTENT_TYPE_APP_URLENCODED;
            return base.send(parameters.ToString());
        }

        /// <summary>
        /// Valida os dados para envio de mensagem multipla
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
	    private bool validateSend(MultipleMessage message)
        {
		    String content = message.Content;
		    if (String.IsNullOrEmpty(content)) {
			    throw new ClientHumanException("Was not informed to send a list of messages.");
		    }

            String[] line = null;
            if (content.Contains("\r\n"))
            {
                line = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                line = content.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }

		    for (int i = 0; i < line.Length; i++) {
			    this.validateMessage(message.Type, line[i]);
		    }
		    return true;
	    }

        /// <summary>
        /// Valida parametros gerais da mensagem (campos obrigatorios, tamanho do texto, etc).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="linha"></param>
	    private void validateMessage(LayoutTypeEnum type, String linha) 
        {
            String[] fields = linha.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (LayoutTypeEnum.TYPE_A.Equals(type))
            {
                if (fields.Length != 2)
                {
				    throw new ClientHumanException("File format invalid.");
			    }

                base.validateMessage(fields[0], fields[1]);
            }
            else if (LayoutTypeEnum.TYPE_B.Equals(type))
            {
                if (fields.Length != 3)
                {
				    throw new ClientHumanException("File format invalid.");
			    }

                base.validateMessage(fields[0], fields[1], fields[2]);
            }
            else if (LayoutTypeEnum.TYPE_C.Equals(type))
            {
                if (fields.Length != 3)
                {
				    throw new ClientHumanException("File format invalid.");
			    }
    			
			    base.validateMessage(fields[0], fields[1], fields[2], null);
            }
            else if (LayoutTypeEnum.TYPE_D.Equals(type))
            {
                if (fields.Length != 4)
                {
				    throw new ClientHumanException("File format invalid.");
			    }

                base.validateMessage(fields[0], fields[1], fields[2], fields[3]);
            }
            else if (LayoutTypeEnum.TYPE_E.Equals(type))
            {
                if (fields.Length != 5)
                {
				    throw new ClientHumanException("File format invalid.");
			    }

                base.validateMessage(fields[0], fields[1], fields[2], fields[3], fields[4]);
		    } else {
			    throw new ClientHumanException("Type of file format invalid.");
		    }
	    }
    }
}
