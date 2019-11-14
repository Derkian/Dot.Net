using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Enum
{
    /// <summary>
    /// Enumerador para os tipos de callback.
    /// </summary>
    public enum CallbackTypeEnum
    {
        /// <summary>
        /// Nao realiza callback (padrao)
        /// </summary>
	    INACTIVE = 0,
	    /// <summary>
        /// Envia apenas o status final da mensagem
	    /// </summary>
	    FINAL = 1,
	    /// <summary>
        /// Envia os status intermediarios e final da mensagem.
	    /// </summary>
	    FULL = 2,
    }
}
