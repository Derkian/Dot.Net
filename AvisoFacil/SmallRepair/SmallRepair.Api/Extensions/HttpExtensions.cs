using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SmallRepair.Api.Extensions
{
    public static class HttpExtensions
    {
        public static string GetClaim(this IPrincipal user, string claimName)
        {
            try
            {
                return ((ClaimsPrincipal)user)
                            .Claims
                            .FirstOrDefault(x => x.Type.ToUpper() == claimName.ToUpper())
                            .Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
