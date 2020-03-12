using Microsoft.EntityFrameworkCore;
using SmallRepair.Business.Model;
using SmallRepair.Management.Context;
using SmallRepair.Management.Enum;
using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallRepair.Business
{
    public class AssessmentBusiness : BaseBussiness
    {
        public AssessmentBusiness(RepositoryEntity repository)
            : base(repository)
        {
        }

        public ResponseMessage<Assessment> Add(Assessment assessment)
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
                    _repository.Add(assessment);

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

        public ResponseMessage<AssessmentReport> AssessmentReport(Assessment assessment)
        {
            ResponseMessage<AssessmentReport> responseMessage = null;

            try
            {
                var findResult = this.Find(assessment);

                if (findResult.Sucess)
                {
                    var assessmentDb = findResult.Object;
                    var service = assessmentDb.Parts.SelectMany(x => x.Services, (part, servico) => new Service()
                    {
                        ServiceType = servico.ServiceType,
                        Time = servico.Time,
                        ValuePerHour = servico.ValuePerHour,
                        MaterialValue = servico.MaterialValue,
                        Value = servico.Value,
                        Total = servico.Total
                    })
                    .GroupBy(x => x.ServiceType)
                    .Select(x => new AssessmentSummary
                    {
                        ServiceType = x.Key,
                        Description = Enum.GetName(typeof(EnmServiceType), x.Key),
                        AmountHours = x.Sum(a => a.Time),
                        TotalService = x.Sum(a => a.Value),
                        TotalMaterial = x.Sum(a => a.MaterialValue),
                        Total = x.Sum(a => a.Total),
                        ValuePerHour = x.Sum(a => a.ValuePerHour)
                    })
                    .ToList();

                    var parts = assessmentDb.Parts.Sum(a => a.TotalPrice);
                    var additionalParts = assessmentDb.AssessmentAdditionalServices.Sum(a => a.Value);

                    service.Add(new AssessmentSummary() { Description = "Parts", Total = parts });
                    service.Add(new AssessmentSummary() { Description = "AdditionalService", Total = additionalParts });

                    var assessmentReport = new AssessmentReport()
                    {
                        Assessment = assessmentDb,
                        Summaries = service
                    };

                    responseMessage = ResponseMessage<AssessmentReport>.Ok(assessmentReport);

                }
                else
                    return responseMessage = ResponseMessage<AssessmentReport>.Fault(findResult.Error.ToArray());
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<AssessmentReport>.Fault(ex.Message);
            }

            return responseMessage;
        }

        //public ResponseMessage<Assessment> Update(Assessment assessment)
        //{
        //    ResponseMessage<Assessment> assessmentResult = null;

        //    try
        //    {
        //        //consulta o cliente
        //        Assessment assessmentDb = _repository.Find<Assessment>(assessment.IdAssessment);                

        //        if (assessmentDb != null)
        //        {
        //            //cria o orçamento
        //            _repository.Update(assessment);

        //            //commit
        //            _repository.SaveChanges();

        //            //objeto de retorno
        //            assessmentResult = ResponseMessage<Assessment>.Ok(assessment);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        assessmentResult = ResponseMessage<Assessment>.Fault(new string[] { ex.Message });
        //    }

        //    return assessmentResult;
        //}

        public ResponseMessage<Pagination<Assessment>> List(Pagination<Assessment> page, Customer customer)
        {
            ResponseMessage<Pagination<Assessment>> pageResult = null;

            try
            {
                var assessmentsList = _repository.Context
                                                 .Assessments
                                                 .Include(a => a.Parts)
                                                 .Where(a => a.IdCustomer == customer.IdCustomer)
                                                 .OrderByDescending(a => a.IdAssessment)
                                                 .ToList();

                page.Page = assessmentsList.Skip((page.PageNumber - 1) * page.PageSize)
                                           .Take(page.PageSize)
                                           .ToList();

                page.TotalRecords = assessmentsList.Count();

                page.TotalPages = page.TotalRecords;

                pageResult = ResponseMessage<Pagination<Assessment>>.Ok(page);
            }
            catch (Exception ex)
            {
                pageResult = ResponseMessage<Pagination<Assessment>>.Fault(new string[] { ex.Message });
            }

            return pageResult;
        }

        public ResponseMessage<Assessment> Find(Assessment assessment)
        {
            ResponseMessage<Assessment> responseMessage = null;

            try
            {
                Assessment assessmentDb = _repository.Context
                                                .Assessments
                                                .Include(a => a.AssessmentAdditionalServices)
                                                .Include(a => a.Customer)
                                                .Include(a => a.AssessmentServicesValues)
                                                .FirstOrDefault(a => a.IdAssessment == assessment.IdAssessment && a.IdCustomer == assessment.IdCustomer);

                if (assessmentDb == null)
                    return ResponseMessage<Assessment>.Fault("Orçamento não encontrado.");

                assessmentDb.Parts = _repository.Context
                                                .Parts
                                                .Include(a => a.Services)
                                                .Where(a => a.IdAssessment == assessmentDb.IdAssessment)
                                                .ToList();

                responseMessage = ResponseMessage<Assessment>.Ok(assessmentDb);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Assessment>.Fault(ex.Message);
            }

            return responseMessage;
        }

        public ResponseMessage<Part> AddPart(Assessment assessment, Part part)
        {
            ResponseMessage<Part> responseMessage;

            try
            {
                Catalog catalog = null;

                assessment = _repository.Find<Assessment>(assessment.IdAssessment);

                //lista o valor por serviço
                IList<AssessmentServiceValue> assessmentServiceValues =
                                                                assessment.AssessmentServicesValues ??
                                                                _repository.All<AssessmentServiceValue>(a => a.IdAssessment == assessment.IdAssessment).ToList();

                //consulta o tempo por baremo
                IList<BaremoTime> baremoTimes = _repository
                                        .All<BaremoTime>(
                                                            bar => bar.Catalog.Code == part.Code &&
                                                            bar.Baremo.MalfunctionType == part.MalfunctionType &&
                                                            bar.Baremo.IntensityType == part.IntensityType
                                                        ).ToList();

                //se não encontrar o baremo
                if (baremoTimes == null || baremoTimes.Count == 0)
                    catalog = _repository.All<Catalog>(a => a.Code == part.Code).FirstOrDefault();
                else
                    catalog = baremoTimes.First()?.Catalog ?? _repository.All<Catalog>(a => a.IdCatalog == baremoTimes.First().IdCatalog).FirstOrDefault();

                if (catalog == null)
                    return ResponseMessage<Part>.Fault("Peça não encontrada no catalogo");

                //verifica se existe a peça
                bool addPart = true;
                Part partAdd = _repository.All<Part>(a => a.IdAssessment == assessment.IdAssessment && a.Code == part.Code).FirstOrDefault();

                if (partAdd != null)
                    addPart = false;
                else
                    partAdd = part;

                partAdd.Description = catalog.Description;

                partAdd.TotalPrice = part.Quantity * part.UnitaryValue;

                partAdd.IdAssessment = assessment.IdAssessment;

                partAdd.MalfunctionType = part.MalfunctionType;

                partAdd.IntensityType = part.IntensityType;

                if (partAdd.Services == null)
                    partAdd.Services = new List<Service>();

                foreach (BaremoTime baremo in baremoTimes)
                {
                    double _valuePerHour = assessmentServiceValues.FirstOrDefault(a => a.ServiceType == baremo.ServiceType).Value;

                    Service service = new Service()
                    {
                        Time = baremo.ServiceTime,
                        ServiceType = baremo.ServiceType,
                        ValuePerHour = _valuePerHour,
                        Value = baremo.ServiceTime * _valuePerHour,
                        MaterialValue = baremo.MaterialValue,
                    };

                    //Valor total servico
                    service.Total = service.Value + service.MaterialValue;

                    partAdd.Services.Add(service);
                }

                //Total Material
                partAdd.TotalMaterial = partAdd.Services.Sum(a => a.MaterialValue);

                //Total Servico
                partAdd.TotalService = partAdd.Services.Sum(a => a.Value);

                //total da peça
                partAdd.Total = partAdd.TotalService + partAdd.TotalMaterial;


                if (addPart)
                    _repository.Add<Part>(partAdd);
                else
                {
                    //remove todos os serviços anteriores
                    var partServices = _repository.All<Service>(serv => serv.IdPart == partAdd.IdPart).ToArray();
                    _repository.Delete<Service>(partServices);

                    //atualiza a peça
                    _repository.Update<Part>(partAdd);
                }

                _repository.SaveChanges();

                //calcular orcamento
                this.Calculate(new Assessment() { IdAssessment = assessment.IdAssessment });


                responseMessage = ResponseMessage<Part>.Ok(partAdd);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Part>.Fault(ex.Message);
            }

            return responseMessage;
        }

        public ResponseMessage<bool> DeletePart(Assessment assessment, Part part)
        {
            ResponseMessage<bool> responseMessage = null;

            try
            {
                Part partRemove = _repository.All<Part>(pa => pa.IdPart == part.IdPart && pa.IdAssessment == assessment.IdAssessment).FirstOrDefault();

                if (partRemove == null)
                    return ResponseMessage<bool>.Fault("Peça não encontrada!");

                //apaga a peça
                var services = _repository.All<Service>(serv => serv.IdPart == partRemove.IdPart).ToArray();

                //deletar todos os serviços da peça
                _repository.Delete<Service>(services);

                //apaga a peça
                _repository.Delete<Part>(partRemove);

                //commit
                _repository.SaveChanges();

                //calcular orcamento
                this.Calculate(new Assessment() { IdAssessment = assessment.IdAssessment });

                responseMessage = ResponseMessage<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<bool>.Fault(ex.Message);
            }

            return responseMessage;
        }

        public ResponseMessage<AssessmentAdditionalService> AddAdditionalService(Assessment assessment, AdditionalService additionalService)
        {
            ResponseMessage<AssessmentAdditionalService> responseMessage = null;

            try
            {
                Assessment assessmentDb = Find(assessment).Object;

                if (assessmentDb != null)
                {
                    AssessmentAdditionalService assessmentAdditionalService = new AssessmentAdditionalService()
                    {
                        IdAssessment = assessmentDb.IdAssessment,
                        Value = additionalService.Value,
                        Description = additionalService.Description
                    };

                    //add addtional service
                    assessmentDb.AssessmentAdditionalServices.Add(assessmentAdditionalService);

                    //update assessment
                    _repository.Add<AssessmentAdditionalService>(assessmentAdditionalService);

                    //commit
                    _repository.SaveChanges();

                    //calcular orcamento
                    this.Calculate(assessmentDb);

                    responseMessage = ResponseMessage<AssessmentAdditionalService>.Ok(assessmentAdditionalService);
                }
                else
                    responseMessage = ResponseMessage<AssessmentAdditionalService>.Ok(null);

            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<AssessmentAdditionalService>.Fault(ex.Message);
            }

            return responseMessage;
        }

        public ResponseMessage<bool> DeleteAdditionalService(Assessment assessment, AssessmentAdditionalService additionalService)
        {
            ResponseMessage<bool> responseMessage = null;

            try
            {
                AssessmentAdditionalService additionaltRemove = _repository
                    .All<AssessmentAdditionalService>(
                                                        pa => pa.IdAssessment == assessment.IdAssessment &&
                                                        pa.IdAssessmentAdditionalService == additionalService.IdAssessmentAdditionalService
                                                     ).FirstOrDefault();

                if (additionaltRemove == null)
                    return ResponseMessage<bool>.Fault("Serviço não encontrada!");

                //deletar o serviço
                _repository.Delete<AssessmentAdditionalService>(additionaltRemove);

                //commit
                _repository.SaveChanges();

                //calcular orcamento
                this.Calculate(new Assessment() { IdAssessment = assessment.IdAssessment });

                responseMessage = ResponseMessage<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<bool>.Fault(ex.Message);
            }

            return responseMessage;
        }

        public ResponseMessage<Part> AddManualPart(Assessment assessment, Part part)
        {
            ResponseMessage<Part> responseMessage = null;

            try
            {
                Assessment assessmentDb = Find(assessment).Object;

                if (assessmentDb != null)
                {   
                    part.IdAssessment = assessmentDb.IdAssessment;
                        
                    part.TotalPrice = part.UnitaryValue * part.Quantity;

                    _repository.Add<Part>(part);

                    _repository.SaveChanges();

                    assessmentDb = this.Calculate(assessmentDb).Object;
                }

                responseMessage = ResponseMessage<Part>.Ok(part);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Part>.Fault(ex.Message);
            }

            return responseMessage;
        }

        private ResponseMessage<Assessment> Calculate(Assessment assessment)
        {
            ResponseMessage<Assessment> responseMessage;

            try
            {
                Assessment assessmentDb = _repository.Context
                                                .Assessments
                                                .Include(a => a.AssessmentAdditionalServices)
                                                .Include(a => a.AssessmentServicesValues)
                                                .FirstOrDefault(a => a.IdAssessment == assessment.IdAssessment);

                if (assessmentDb == null)
                    return ResponseMessage<Assessment>.Fault("Orçamento não encontrado.");

                assessmentDb.Parts = _repository.Context
                                                .Parts
                                                .Include(a => a.Services)
                                                .Where(a => a.IdAssessment == assessmentDb.IdAssessment)
                                                .ToList();

                assessmentDb.Total = assessmentDb.Parts.Sum(a => a.TotalPrice + a.TotalMaterial + a.TotalService) +
                                     assessmentDb.AssessmentAdditionalServices.Sum(a => a.Value);

                //update
                _repository.Update(assessmentDb);

                //COMMIT
                _repository.SaveChanges();

                responseMessage = ResponseMessage<Assessment>.Ok(assessmentDb);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Assessment>.Fault(ex.Message);
            }

            return responseMessage;

        }
    }
}
