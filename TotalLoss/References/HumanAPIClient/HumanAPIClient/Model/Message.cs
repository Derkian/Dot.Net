using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Enum;

namespace HumanAPIClient.Model
{
    /// <summary>
    /// Classe abstrata utilizada como molde para construcao de todo o tipo de classe
    /// para envio de sms.
    /// </summary>
    abstract public class Message
    {
        /// <summary>
        /// O callback que a mensagem (ou a lista de mensagens) irá ter. 
        /// </summary>
        private CallbackTypeEnum callback;

        /// <summary>
        /// O callback que a mensagem (ou a lista de mensagens) irá ter.
        /// </summary>
        public CallbackTypeEnum Callback
        {
            get { return this.callback; }
            set { this.callback = value; }
        }

        /// <summary>
        /// Construtor da classe setando o callback default (Inativo).
        /// </summary>
        public Message()
        {
            this.Callback = CallbackTypeEnum.INACTIVE;
        }
    }
}
