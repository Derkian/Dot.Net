using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Business.Model
{
    public class UserApiModel
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string CompanyId { get; set; }

        public string SystemId { get; set; }
    }

    public class UserApiResponse
    {
        public string Id { get; set; }
        public string code { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Mensagem { get; set; }
    }
}
