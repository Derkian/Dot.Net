using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TotalLoss.Service;
using TotalLoss.Repository;
using System.Data.SqlClient;
using TotalLoss.Domain.Model;

namespace TotalLoss.Test
{
    [TestClass]
    public class EmailServiceTest
    {
        [TestMethod]
        public void SendEmail()
        {
            //var sql = new SqlConnection("Data Source=10.33.170.65;Persist Security Info=True;Initial Catalog=UNIFAudaFNOL;Password=msWh62sQ;User=AxSystem");
            //var repository = new ConfigurationRepository(sql);
            //ConfigurationService configuration = new ConfigurationService(repository);


            //InsuranceCompany company = configuration.GetConfiguration(1, Domain.Enums.TypeCompany.InsuranceCompany);

            //EmailService emailService = new EmailService("no-reply@solera.com", "10.66.97.2", 25);


            //emailService.Send(new System.Collections.Generic.List<string>() { "rodrigo.santos@audatex.com.br" },
            //    null, "Pontuação Guincho", company.Image, company.Image, System.IO.File.ReadAllText(@"C:/Users/rodrigo.santos/Desktop/email.html"));

        }
    }
}
