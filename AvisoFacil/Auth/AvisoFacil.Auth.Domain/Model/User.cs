using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvisoFacil.Auth.Model
{
    public class User
    {
        /// <summary>
        /// Chave de Identificação do usuário
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do Usuário
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Login do usuário
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// senha do usuário
        /// </summary>
        public string Password { get; set; }
    }
}
