using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Util.Exceptions
{
    /// <summary>
    /// Excessao personalizada da API.
    /// </summary>
    public class ClientHumanException : Exception
    {
        public ClientHumanException() : base()
        {
        }

        public ClientHumanException(string message)
            : base(message)
        {
            
        }
    }
}
