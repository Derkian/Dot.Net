using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.API.Util
{
    public static class Helper
    {
        public static Company GetConfiguration()
        {
            int _idConfiguration;
            int _typeCompany;
            var user = ((ClaimsPrincipal)HttpContext.Current.User);

            string idConfiguration = user.Claims.First(a => a.Type == ClaimTypes.Sid).Value;
            string configName = user.Claims.First(a => a.Type == ClaimTypes.Name).Value;
            string typeCompany = user.Claims.First(a => a.Type == ClaimTypes.GroupSid).Value;

            int.TryParse(idConfiguration, out _idConfiguration);
            int.TryParse(typeCompany, out _typeCompany);

            return new Company() { Id = _idConfiguration, Name = configName, TypeCompany = (TypeCompany)_typeCompany };
        }

        public static User GetUser()
        {
            try
            {
                int _id;

                var user = ((ClaimsPrincipal)HttpContext.Current.User);
                string configName = user.Claims.First(a => a.Type == ClaimTypes.GivenName).Value;
                string userId = user.Claims.First(a => a.Type == ClaimTypes.UserData).Value;

                int.TryParse(userId, out _id);

                return new User() { Login = configName, Id = _id };
            }
            catch (Exception ex)
            {
                return new User();
            }
        }
    }
}