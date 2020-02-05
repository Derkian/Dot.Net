using AvisoFacil.Auth.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AvisoFacil.Auth.API.Controllers
{

    public class TestController : ApiController
    {

        [HttpGet]
        [Route("api/test/admin")]
        [Authorize(Roles = Profiles.Administrator)]
        public async Task<IHttpActionResult> Authenticate()
        {
            return Ok(new { profile = Profiles.Administrator });
        }

        [HttpGet]
        [Route("api/test/onlyreading")]
        [Authorize(Roles = Profiles.OnlyReading)]
        public async Task<IHttpActionResult> OnlyReading()
        {
            return Ok(new { profile = Profiles.OnlyReading });
        }

        [HttpGet]
        [Route("api/test/operator")]
        [Authorize(Roles = Profiles.Operator)]
        public async Task<IHttpActionResult> Operator()
        {
            return Ok(new { profile = Profiles.Operator });
        }

        [HttpGet]
        [Route("api/test/other")]
        [Authorize(Roles = Profiles.Others)]
        public async Task<IHttpActionResult> Others()
        {
            return Ok(new { profile = Profiles.Others });
        }

        [HttpGet]
        [Route("api/test/all")]
        [Authorize]
        public async Task<IHttpActionResult> All()
        {
            return Ok(new { profile = "All" });
        }
    }
}
