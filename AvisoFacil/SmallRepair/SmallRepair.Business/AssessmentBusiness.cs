using SmallRepair.Business.Model;
using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallRepair.Business
{
    public class AssessmentBusiness
    {
        private readonly RepositoryEntity _repository;

        public AssessmentBusiness(RepositoryEntity repository)
        {
            _repository = repository;
        }

        public ResponseMessage<Assessment> Create(Assessment assessment)
        {
            ResponseMessage<Assessment> assessmentResult = null;

            try
            {
                //consulta o cliente
                Customer customer = _repository.Find<Customer>(assessment.IdCustomer);

                if (customer != null)
                {
                    //adiciona o valor por serviço do cliente do orçamento
                    if (assessment.AssessmentServicesValues == null || assessment.AssessmentServicesValues.Count() == 0)
                    {
                        assessment.AssessmentServicesValues =
                            _repository.All<ServiceValue>(a => a.IdCustomer == customer.IdCustomer)
                            .Select(a => new AssessmentServiceValue()
                            {
                                ServiceType = a.ServiceType,
                                Value = a.Value
                            }).ToList();
                    }

                    //cria o orçamento
                    _repository.Add<Assessment>(assessment);

                    //commit
                    _repository.SaveChanges();

                    //objeto de retorno
                    assessmentResult = ResponseMessage<Assessment>.Ok(assessment);
                }
            }
            catch (Exception ex)
            {
                assessmentResult = ResponseMessage<Assessment>.Fault(new string[] { ex.Message });                
            }

            return assessmentResult;
        }
    }
}
