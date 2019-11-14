using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class LoginController : ApiController
    {
        #region | Objects

        private readonly LoginService _loginService;

        #endregion

        #region | Constructor 

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        #endregion

        #region | Methods

        [HttpPost]
        [Route("api/login/auth")]
        public async Task<IHttpActionResult> Authenticate([FromBody] User userLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (userLogin == null)
                return BadRequest();

            // Busca usuário na base de dados através do request informado
            User _user = await Task.Run(() => _loginService.GetUserByLogin(userLogin));

            // Usuário logado não autorizado 
            if (_user == null)
                return Unauthorized();

            // Busca um token válido baseado no usuário informado
            var tokenJwt = await Task.Run(() => _loginService.GetTokenJWT(_user));

            // Retorna response com Token válido
            return Ok(new
            {
                status = "success",
                message = "User existent",
                data = new { token = tokenJwt }
            });
        }

        #endregion
    }
}