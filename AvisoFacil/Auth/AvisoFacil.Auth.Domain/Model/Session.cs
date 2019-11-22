using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvisoFacil.Auth.Model
{
    public class Session
    {
        /// <summary>
        /// Data da Sessão
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Token gerado
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Estpa ativo?
        /// </summary>
        public bool Active { get; set; }
    }
}
