using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Enum;

namespace HumanAPIClient.Model
{
    /// <summary>
    /// Classe abstrata que é utilizada como modelos para 
    /// os tipos de mensagens multiplas possiveis.
    /// </summary>
    abstract public class MultipleMessage : Message
    {
        /// <summary>
        /// Tipo de layout da lista ou arquivo a ser enviado
        /// </summary>
	    private LayoutTypeEnum type;

        /// <summary>
        /// Seta e retorna o tipo de layout do arquivo ou lista
        /// </summary>
        public LayoutTypeEnum Type 
        {
            get { return type; }
            set { type = value; }
        }
    	
        /// <summary>
        /// Retorna o conteudo a ser enviado
        /// </summary>
        public abstract String Content { get; }

        /// <summary>
        /// Contrutor da classe informando o tipo de layout do arquivo ou lista
        /// </summary>
        /// <param name="type"></param>
        public MultipleMessage(LayoutTypeEnum type)
            : base()
        {
            this.type = type;
        }
    }
}
