using SmallRepair.Api.Model;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Extensions
{
    public static class AssessmentExtension
    {
        public static Assessment ToModel(this AssessmentViewModel view)
        {
            Assessment assessment = new Assessment()
            {
                IdAssessment = view.Id,
                Mileage = view.Mileage,
                BodyType = view.BodyType,
                State = view.State,                
                Model = view.Model,
                Plate = view.Plate,
                CustomerName = view.CustomerName,
                IdCompany = ""
            };

            if (view.ServiceValues != null)
            {
                assessment.ServicesValues = view
                                                        .ServiceValues
                                                        .Select(a => new AssessmentServiceValue()
                                                        {
                                                            ServiceType = a.ServiceType,
                                                            Value = a.Value
                                                        }).ToList();
            }

            return assessment;
        }

        public static AssessmentViewModel ToView(this Assessment assessment)
        {
            AssessmentViewModel assessmentViewModel = new AssessmentViewModel()
            {
                Id = assessment.IdAssessment,
                BodyType = assessment.BodyType,
                Model = assessment.Model,
                Plate = assessment.Plate,
                CustomerName = assessment.CustomerName
            };

            if (assessment.ServicesValues != null)
            {
                assessmentViewModel.ServiceValues= assessment
                                                        .ServicesValues
                                                        .Select(a => new ServiceValueViewModel()
                                                        {
                                                            ServiceType = a.ServiceType,
                                                            Value = a.Value
                                                        })
                                                        .ToList();
            }

            return assessmentViewModel;
        }
    }
}
