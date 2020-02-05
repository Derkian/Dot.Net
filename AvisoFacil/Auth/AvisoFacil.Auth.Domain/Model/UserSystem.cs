using AvisoFacil.Auth.Const;
using AvisoFacil.Auth.Enum;
using System.Collections.Generic;

namespace AvisoFacil.Auth.Model
{
    public class UserSystem : User
    {
        /// <summary>
        /// Sistema que o usuário tem acesso
        /// </summary>
        public Systems System { get; set; }

        /// <summary>
        /// Perfil do usuário nesse sistema
        /// </summary>
        public string Profile { get; set; } = Profiles.Others;

        /// <summary>
        /// Está ativo ou inativo?
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Sessões do usuário
        /// </summary>
        public Session Session { get; set; }
    }
}
