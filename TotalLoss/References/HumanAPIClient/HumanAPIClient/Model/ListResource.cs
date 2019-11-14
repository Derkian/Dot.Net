using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Enum;

namespace HumanAPIClient.Model
{
    /// <summary>
    /// Classe responsavel por armazenar uma lista de sms 
    /// para envio para o gateway de sms.
    /// </summary>
    public class ListResource : MultipleMessage
    {
        /// <summary>
        /// Lista de sms a ser enviada.
        /// </summary>
	    private String content;
    	
        /// <summary>
        /// Construtor da classe informando a lista de sms e o tipo de layout da mesma.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        public ListResource(String list, LayoutTypeEnum type)
            : base(type)
        {
		    content = list;
	    }

        override public String Content
        {
            get
            {
                return this.content;
            }
	    }
    }
}
