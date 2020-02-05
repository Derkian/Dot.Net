using AvisoFacil.Auth.Const;
using AvisoFacil.Auth.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AvisoFacil.Auth.Service
{
    public class UserService
    {
        private Company _company = new Company();

        public UserService()
        {
            this._company.TokenExpirationHour = 12;
            this._company.Name = "Allianz";
            this._company.Id = 1;

            this._company.Users = new List<UserSystem>();


            this._company.Users.Add(new UserSystem()
            {
                Profile = Profiles.Administrator,
                Login = "rodrigo.derkian@audatex.com.br",
                Password = "1234"
            });

            this._company.Users.Add(new UserSystem()
            {
                Profile = Profiles.Operator,
                Login = "flavio.ota@audatex.com.br",
                Password = "1234"
            });

            this._company.Users.Add(new UserSystem()
            {
                Profile = Profiles.OnlyReading,
                Login = "allan@audatex.com.br",
                Password = "1234"
            });

            this._company.Users.Add(new UserSystem()
            {
                Profile = Profiles.Others,
                Login = "diego@audatex.com.br",
                Password = "1234"
            });
        }

        public string Login(User user)
        {
            UserSystem _user = this._company.Users.FirstOrDefault(a => a.Login == user.Login && a.Password == user.Password);

            if (_user != null)
            {
                return this.GetToken(_user, this._company);
            }
            else
                return "";
        }

        public string GetToken(UserSystem user, Company company)
        {
            string tokenJwt = null;
            try
            {
                DateTime issuedAt = DateTime.UtcNow;
                DateTime expires = DateTime.UtcNow.AddHours(company.TokenExpirationHour);

                var tokenHandler = new JwtSecurityTokenHandler();

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
                 {
                    new Claim(ClaimTypes.Sid, company.Id.ToString()),
                    new Claim(ClaimTypes.Name, company.Name),
                    new Claim(ClaimTypes.UserData, user.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, user.Login),
                    new Claim(ClaimTypes.Role, user.Profile)
                });

                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(ConfigurationManager.AppSettings["Token_SecurityKey"]));
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
