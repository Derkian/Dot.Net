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

            //builder.UseLazyLoadingProxies();

            Management.Context.SmallRepairDbContext context =
                new Management.Context.SmallRepairDbContext(builder.Options);

            Management.Repository.RepositoryEntity repository =
                new Management.Repository.RepositoryEntity(context);

            Business.AssessmentBusiness assessmentBusiness = new Business.AssessmentBusiness(repository);

            var assessment = new Assessment()
            {
                IdCompany = "",
                BodyType = "SEDAN",
                Mileage = "10000",
                Plate = "ABC-1234",
                Model = "VW - VOYAGE",
                AssessmentServicesValues = new AssessmentServiceValue[] {
                    new AssessmentServiceValue() {
                        ServiceType = Management.Enum.EnmServiceType.Painting,
                        Value = 2.2
                    },
                    new AssessmentServiceValue() {
                        ServiceType = Management.Enum.EnmServiceType.Repair,
                        Value = 3.2
                    },
                }
            };

            //assessmentBusiness.Create(assessment);

            assessmentBusiness.AddPart(new Assessment() { IdAssessment = 1 }, new Part() { Code = "2", MalfunctionType = Management.Enum.EnmMalfunctionType.Risk, IntensityType = Management.Enum.EnmIntensityType.Light });

            //Console.WriteLine("Hello World!");
        }
    }
}
