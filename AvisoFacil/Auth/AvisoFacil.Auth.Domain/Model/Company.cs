using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvisoFacil.Auth.Model
{
    public class Company
    {
        /// <summary>
        /// Chave de indentificação da empresa
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da empresa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// cnpj da empresa
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Tempo de expiração para um Token
        /// </summary>
        public int TokenExpirationHour { get; set; }

        /// <summary>
        /// Lista de usuários da empresa
        /// </summary>
        public IList<UserSystem> Users { get; set; }
    }
}
