using AvisoFacil.Auth.Model;
using AvisoFacil.Auth.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AvisoFacil.Auth.API.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("api/login/auth")]
        public async Task<IHttpActionResult> Authenticate([FromBody] User user)
        {
            UserService _userService = new UserService();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (user == null)
                return BadRequest();

            // Busca um token válido baseado no usuário informado
            var tokenJwt = await Task.Run(() => _userService.Login(user));

            // Retorna response com Token válido
            return Ok(new
            {
                status = "success",
                message = "User existent",
                data = new { token = tokenJwt }
            });
        }
    }
}
