using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Model
{
    /// <summary>
    /// Classe responsavel por armazenar as informacoes de um sms.
    /// </summary>
    public class SimpleMessage : Message {
    	/// <summary>
    	/// Id da mensagem
    	/// </summary>
	    private String id;
        /// <summary>
        /// numero que sera enviado a mensagem
        /// </summary>
	    private String to;
        /// <summary>
        /// de quem e enviado a mensagem
        /// </summary>
	    private String from;
        /// <summary>
        /// mensagem a ser enviada
        /// </summary>
	    private String message;
        /// <summary>
        /// data de agendamento
        /// </summary>
        private String schedule;

        public String Id
        {
            get { return id; }
            set { id = value; }
        }

        public String To
        {
            get { return to; }
            set { to = value; }
        }

        public String From
        {
            get { return from; }
            set { from = value; }
        }

        public String Message
        {
            get { return message; }
            set { message = value; }
        }

        public String Schedule
        {
            get { return schedule; }
            set { schedule = value; }
        }

        public SimpleMessage()
            : base()
        {
	    }
    }
}
