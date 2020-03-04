using Microsoft.EntityFrameworkCore;
using SmallRepair.Management.Model;
using System;

namespace SmallRepair.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new DbContextOptionsBuilder<Management.Context.SmallRepairDbContext>();

            builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Baremo;Trusted_Connection=True;MultipleActiveResultSets=true");

            Management.Context.SmallRepairDbContext context =
                new Management.Context.SmallRepairDbContext(builder.Options);

            Management.Repository.RepositoryEntity repository =
                new Management.Repository.RepositoryEntity(context);            

            Business.AssessmentBusiness assessmentBusiness = new Business.AssessmentBusiness(repository);

            var assessment = new Assessment()
            {
                IdCustomer = 1,
                BodyType = "SEDAN",
                Mileage = "10000",
                Plate = "ABC-1234",
                Model = "VW - VOYAGE"
            };

            assessmentBusiness.Create(assessment);

            //Console.WriteLine("Hello World!");
        }
    }
}
