using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class LoginService
    {
        private IWorkRepository _workRepository;

        public LoginService(IWorkRepository workRepository)
        {
            this._workRepository = workRepository;
        }

        /// <summary>
        /// Cria conta de Usuário a ser utilizado para logar na aplicação   
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User Create(User user)
        {
            try
            {
                _workRepository.BeginTransaction();

                // Insere Usuário na base de dados
                _workRepository.UserRepository.Create(user);

                // Comita a transação
                _workRepository.Commit();

                //retorna Usuário criado
                return user;
            }
            catch (Exception ex)
            {
                _workRepository.RollBack();

                throw ex;
            }
        }

        /// <summary>
        ///  Busca um usuário válido na base a partir do login informado 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User GetUserByLogin(User user)
        {
            User _user = null;
            try
            {
                _user = _workRepository.UserRepository.Find(user.Login, user.Password);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _user;
        }

        /// <summary>
        /// Retorna um Token JWT válido a partir do login informado
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public string GetTokenJWT(User user)
        {
            string tokenJwt = null;
            try
            {
                DateTime issuedAt = DateTime.UtcNow;
                DateTime expires = DateTime.UtcNow.AddHours(double.Parse(ConfigurationManager.AppSettings["Token_TimeExpirationHours"]));

                var tokenHandler = new JwtSecurityTokenHandler();

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, user.Company.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Company.Name),
                    new Claim(ClaimTypes.GroupSid, user.Company.TypeCompany.GetHashCode().ToString()),                    
                    new Claim(ClaimTypes.UserData, user.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, user.Login)
                });

                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(ConfigurationManager.AppSettings["Token_SecurityKey"]));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                var token = tokenHandler.CreateJwtSecurityToken
                (
                    issuer: ConfigurationManager.AppSettings["Token_Issuer"],
                    audience: ConfigurationManager.AppSettings["Token_Audience"],
                    subject: claimsIdentity,
                    notBefore: issuedAt,
                    expires: expires,
                    signingCredentials: signingCredentials
                 );

                tokenJwt = tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tokenJwt;
        }
    }
}
