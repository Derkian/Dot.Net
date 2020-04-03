using Microsoft.EntityFrameworkCore;
using RazorLight;
using SelectPdf;
using SmallRepair.Business.Model;
using SmallRepair.Business.Util;
using SmallRepair.Management.Enum;
using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SmallRepair.Business
{
    public class AssessmentBusiness : BaseBussiness
    {
        #region VARIAVEIS

        public readonly Email _email;

        #endregion

        #region CONSTRUTOR
        public AssessmentBusiness(RepositoryEntity repository, Email email)
            : base(repository)
        {
            _email = email;
        }
        #endregion

        #region PUBLICOS
        public ResponseMessage<Assessment> CreateAssessment(Assessment assessment)
        {
            ResponseMessage<Assessment> assessmentResult = null;

            try
            {
                //consulta o cliente
                Company customer = _repository.Find<Company>(assessment.IdCompany);

                if (customer != null)
                {
                    //adiciona o valor por serviço do cliente do orçamento
                    if (assessment.ServicesValues == null || assessment.ServicesValues.Count() == 0)
                    {
                        assessment.ServicesValues =
                            _repository.All<ServiceValue>(a => a.IdCompany == customer.IdCompany)
                            .Select(a => new AssessmentServiceValue()
                            {
                                ServiceType = a.ServiceType,
                                Value = a.Value
                            }).ToList();
                    }

                    assessment.State = EnmAssessmentState.InProgress;

                    //Data de Criação
                    assessment.Created = DateTime.Now;

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
                assessmentResult = ResponseMessage<Assessment>.Fault(ex.StackTrace);
            }

            return assessmentResult;
        }

        public ResponseMessage<AssessmentReport> GetAssessmentSummary(Assessment assessment)
        {
            ResponseMessage<AssessmentReport> responseMessage = null;

            try
            {
                var findResult = this.Find(assessment);

                if (findResult.Sucess)
                {
                    var assessmentDb = findResult.Object;

                    if (assessmentDb.Version != assessment.Version)
                    {
                        var resultVersion = this.GetVersion(assessment);

                        if (!resultVersion.Sucess)
                            return ResponseMessage<AssessmentReport>.Fault(resultVersion.Error.ToArray());

                        assessmentDb = resultVersion.Object;
                    }

                    var resultSummary = this.CreateSummary(assessmentDb);

                    if (!resultSummary.Sucess)
                        return ResponseMessage<AssessmentReport>.Fault(resultSummary.Error.ToArray());

                    responseMessage = ResponseMessage<AssessmentReport>.Ok(resultSummary.Object);

                }
                else
                    return responseMessage = ResponseMessage<AssessmentReport>.Fault(findResult.Error.ToArray());
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<AssessmentReport>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<bool> CreateVersion(Assessment assessment, User user)
        {
            ResponseMessage<bool> responseMessage = null;

            try
            {
                Assessment assessmentDb = Find(assessment).Object;

                if (assessmentDb == null || assessmentDb.State != EnmAssessmentState.Complete)
                    return ResponseMessage<bool>.Fault("Orçamento não encontrado.");

                assessmentDb.Version += 1;
                assessmentDb.State = EnmAssessmentState.InProgress;

                string assessmentJson = JsonConvert.SerializeObject(assessmentDb, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

                //criar o histórico
                var assessmentHistory = new AssessmentVersion()
                {
                    IdAssessment = assessmentDb.IdAssessment,
                    ChangeDate = DateTime.Now,
                    Type = EnmAssessmentVersion.NewVersion,
                    IdUser = user.IdUser,
                    Email = user.Email,
                    Total = assessmentDb.Total,
                    AssessmentData = assessmentJson,
                    Version = assessmentDb.Version
                };

                //adicionar histórico
                _repository.Add(assessmentHistory);

                _repository.Update(assessmentDb);

                _repository.SaveChanges();


                responseMessage = ResponseMessage<bool>.Ok(true);

            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<bool>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<Assessment> UpdateAssessment(Assessment assessment, User user)
        {
            ResponseMessage<Assessment> assessmentResult = null;

            try
            {
                //consulta o cliente
                Assessment assessmentDb = Find(assessment).Object;

                if (assessmentDb != null)
                {
                    assessmentDb.State = EnmAssessmentState.Complete;

                    //update assessent
                    _repository.Update(assessmentDb);

                    string assessmentJson = JsonConvert.SerializeObject(assessmentDb, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    //criar o histórico
                    var assessmentHistory = new AssessmentVersion()
                    {
                        IdAssessment = assessmentDb.IdAssessment,
                        ChangeDate = DateTime.Now,
                        Type = EnmAssessmentVersion.AssessmentComplete,
                        IdUser = user.IdUser,
                        Email = user.Email,
                        Total = assessmentDb.Total,
                        AssessmentData = assessmentJson,
                        Version = assessmentDb.Version
                    };

                    //adicionar histórico
                    _repository.Add(assessmentHistory);

                    //commit
                    _repository.SaveChanges();

                    //objeto de retorno
                    assessmentResult = ResponseMessage<Assessment>.Ok(assessment);
                }
            }
            catch (Exception ex)
            {
                assessmentResult = ResponseMessage<Assessment>.Fault(ex.StackTrace);
            }

            return assessmentResult;
        }

        public ResponseMessage<Pagination<Assessment>> List(Pagination<Assessment> page, Search search)
        {
            ResponseMessage<Pagination<Assessment>> pageResult = null;

            try
            {
                var conditions = search.GetFunc();

                var assessmentsList = _repository.Context
                                                 .Assessments
                                                 .Include(a => a.Parts)
                                                 .Include(a => a.ServicesValues)
                                                 .Where(conditions)
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
                pageResult = ResponseMessage<Pagination<Assessment>>.Fault(ex.StackTrace);
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
                                                .Include(a => a.AdditionalServices)
                                                .Include(a => a.Company)
                                                .Include(a => a.ServicesValues)
                                                .FirstOrDefault
                                                (
                                                    a => a.IdAssessment == assessment.IdAssessment
                                                    && a.IdCompany == assessment.IdCompany
                                                );

                if (assessmentDb == null)
                    return ResponseMessage<Assessment>.Fault("Orçamento não encontrado.");

                assessmentDb.Parts = _repository.Context
                                                .Parts
                                                .Include(a => a.Services)
                                                .Where(a => a.IdAssessment == assessmentDb.IdAssessment)
                                                .OrderByDescending(a => a.MalfunctionType)
                                                .ToList();

                responseMessage = ResponseMessage<Assessment>.Ok(assessmentDb);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Assessment>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<Part> AddPart(Assessment assessment, Part part)
        {
            ResponseMessage<Part> responseMessage;

            try
            {
                Catalog catalog = null;

                assessment = Find(assessment).Object;

                if (assessment == null)
                    return ResponseMessage<Part>.Fault("Orçamento não encontrado.");
                else if (assessment.State == EnmAssessmentState.Complete)
                    return ResponseMessage<Part>.Fault("Orçamento finalizado.");

                //lista o valor por serviço
                IList<AssessmentServiceValue> assessmentServiceValues =
                                                                assessment.ServicesValues ??
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

                //Tempo Total
                partAdd.TotalTime = partAdd.Services.Sum(a => a.Time);

                //Total Material
                partAdd.TotalMaterial = partAdd.Services.Sum(a => a.MaterialValue);

                //Total Servico
                partAdd.TotalService = partAdd.Services.Sum(a => a.Value);

                //total da peça
                partAdd.Total = partAdd.TotalPrice +
                                partAdd.TotalService +
                                partAdd.TotalMaterial;

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
                responseMessage = ResponseMessage<Part>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<bool> DeletePart(Assessment assessment, Part part)
        {
            ResponseMessage<bool> responseMessage = null;

            try
            {
                assessment = Find(assessment).Object;

                if (assessment == null)
                    return ResponseMessage<bool>.Fault("Orçamento não encontrado.");
                else if (assessment.State == EnmAssessmentState.Complete)
                    return ResponseMessage<bool>.Fault("Orçamento finalizado.");

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
                responseMessage = ResponseMessage<bool>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<AssessmentAdditionalService> AddAdditionalService(Assessment assessment, AdditionalService additionalService)
        {
            ResponseMessage<AssessmentAdditionalService> responseMessage = null;

            try
            {
                Assessment assessmentDb = Find(assessment).Object;

                if (assessmentDb == null)
                    return ResponseMessage<AssessmentAdditionalService>.Fault("Orçamento não encontrado.");
                else if (assessmentDb.State == EnmAssessmentState.Complete)
                    return ResponseMessage<AssessmentAdditionalService>.Fault("Orçamento finalizado.");


                AssessmentAdditionalService assessmentAdditionalService = new AssessmentAdditionalService()
                {
                    IdAssessment = assessmentDb.IdAssessment,
                    Value = additionalService.Value,
                    Description = additionalService.Description
                };

                //add addtional service
                assessmentDb.AdditionalServices.Add(assessmentAdditionalService);

                //update assessment
                _repository.Add<AssessmentAdditionalService>(assessmentAdditionalService);

                //commit
                _repository.SaveChanges();

                //calcular orcamento
                this.Calculate(assessmentDb);

                responseMessage = ResponseMessage<AssessmentAdditionalService>.Ok(assessmentAdditionalService);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<AssessmentAdditionalService>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<bool> DeleteAdditionalService(Assessment assessment, AssessmentAdditionalService additionalService)
        {
            ResponseMessage<bool> responseMessage = null;

            try
            {
                assessment = Find(assessment).Object;

                if (assessment == null)
                    return ResponseMessage<bool>.Fault("Orçamento não encontrado.");
                else if (assessment.State == EnmAssessmentState.Complete)
                    return ResponseMessage<bool>.Fault("Orçamento finalizado.");

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
                responseMessage = ResponseMessage<bool>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public ResponseMessage<Part> AddManualPart(Assessment assessment, Part part)
        {
            ResponseMessage<Part> responseMessage = null;

            try
            {
                Assessment assessmentDb = Find(assessment).Object;

                if (assessmentDb == null)
                    return ResponseMessage<Part>.Fault("Orçamento não encontrado.");
                else if (assessmentDb.State == EnmAssessmentState.Complete)
                    return ResponseMessage<Part>.Fault("Orçamento finalizado.");


                part.IdAssessment = assessmentDb.IdAssessment;

                part.TotalPrice = part.UnitaryValue * part.Quantity;

                part.Total = part.TotalPrice;

                _repository.Add<Part>(part);

                _repository.SaveChanges();

                assessmentDb = this.Calculate(assessmentDb).Object;

                responseMessage = ResponseMessage<Part>.Ok(part);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Part>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }
        #endregion

        #region PUBLICOS ASYNC
        public async Task<ResponseMessage<string>> ReportHTMLAsync(Assessment assessment, string IdCompany, string reportCode)
        {
            ResponseMessage<string> responseMessage;

            try
            {
                string _template = string.Empty;
                ReportTemplate reportTemplate;
                Assessment assessmentReport = null;

                var companyReportTemplate = _repository
                                        .All<CompanyReportTemplate>(a => a.IdCompany == IdCompany &&
                                                                         a.Code.ToUpper().Equals(reportCode.ToUpper()))
                                        .FirstOrDefault();


                if (companyReportTemplate == null)
                {
                    reportTemplate = _repository.All<ReportTemplate>(a => a.Code.ToUpper().Equals(reportCode)).FirstOrDefault();
                    _template = reportTemplate.Template ?? "";
                }
                else
                    _template = companyReportTemplate.Template ?? "";

                if (string.IsNullOrEmpty(_template))
                    return ResponseMessage<string>.Fault("Não existe template cadastrado.");

                //CONSULTA O ASSESSMENT NA BASE DE DADOS
                var assessmentDb = Find(assessment);

                if (!assessmentDb.Sucess)
                    return ResponseMessage<string>.Fault(assessmentDb.Error.ToArray());

                //CONSULTA A VERSÃO NO HISTÓRICO
                if (assessmentDb.Object.Version == assessment.Version)
                    assessmentReport = assessmentDb.Object;
                else
                {
                    var assessmentJson = this.GetVersion(assessment);

                    if (!assessmentJson.Sucess)
                        return ResponseMessage<string>.Fault(assessmentJson.Error.ToArray());

                    assessmentReport = assessmentJson.Object;
                }

                //RECUPERA O SUMÁRIO DO ASSESSMENT
                var result = this.CreateSummary(assessmentReport);

                if (!result.Sucess)
                    return ResponseMessage<string>.Fault(result.Error.ToArray());

                var engine = new RazorLightEngineBuilder()
                  .UseEmbeddedResourcesProject(typeof(AssessmentBusiness))
                  .UseMemoryCachingProvider()
                  .Build();

                string templateResult = await engine.CompileRenderStringAsync("templateKey", _template, result.Object);

                responseMessage = ResponseMessage<string>.Ok(templateResult);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<string>.Fault(ex.Message);
            }

            return responseMessage;
        }

        public async Task<ResponseMessage<Stream>> ReportPDFAsync(Assessment assessment,
                                                                string idCompany,
                                                                string reportCode)
        {
            ResponseMessage<Stream> responseMessage = ResponseMessage<Stream>.Fault("");

            try
            {
                var reportHTML = await this.ReportHTMLAsync(assessment, idCompany, reportCode);

                if (!reportHTML.Sucess)
                    return ResponseMessage<Stream>.Fault(reportHTML.Error.ToArray());

                var stream = new MemoryStream();

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                converter.Options.MarginTop = 10;
                converter.Options.MarginBottom = 10;
                converter.Options.MarginLeft = 10;
                converter.Options.MarginRight = 10;

                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertHtmlString(reportHTML.Object, "");

                // save pdf document
                doc.Save(stream);

                stream.Position = 0;

                // close pdf document
                doc.Close();

                responseMessage = ResponseMessage<Stream>.Ok(stream);

            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Stream>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        public async Task<ResponseMessage<bool>> SendReportByEmail(List<string> To, Assessment assessment, string reportCode)
        {
            ResponseMessage<bool> responseMessage = ResponseMessage<bool>.Ok(false);
            try
            {
                assessment = Find(assessment).Object;

                var resultReportPDF = await ReportPDFAsync(assessment, assessment.IdCompany, reportCode);

                if (resultReportPDF.Sucess)
                {
                    var attachament = new List<Attachament>();

                    attachament.Add(new Attachament()
                    {
                        File = resultReportPDF.Object,
                        FileName = $"Orcamento_{assessment.Plate}_V{assessment.Version}.pdf",
                        MediaTypeNames = MediaTypeNames.Application.Pdf
                    });

                    var result = _email.Send(To, null, $"Pequenos Reparos - Orçamento {assessment.Plate}", "", attachament);

                    responseMessage = ResponseMessage<bool>.Ok(result);
                }
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<bool>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }
        #endregion

        #region PRIVATE
        private ResponseMessage<Assessment> Calculate(Assessment assessment)
        {
            ResponseMessage<Assessment> responseMessage;

            try
            {
                Assessment assessmentDb = _repository.Context
                                                .Assessments
                                                .Include(a => a.AdditionalServices)
                                                .Include(a => a.ServicesValues)
                                                .FirstOrDefault(a => a.IdAssessment == assessment.IdAssessment);

                if (assessmentDb == null)
                    return ResponseMessage<Assessment>.Fault("Orçamento não encontrado.");

                assessmentDb.Parts = _repository.Context
                                                .Parts
                                                .Include(a => a.Services)
                                                .Where(a => a.IdAssessment == assessmentDb.IdAssessment)
                                                .ToList();

                assessmentDb.Total = assessmentDb.Parts.Sum(a => a.TotalPrice + a.TotalMaterial + a.TotalService) +
                                     assessmentDb.AdditionalServices.Sum(a => a.Value);

                //update
                _repository.Update(assessmentDb);

                //COMMIT
                _repository.SaveChanges();

                responseMessage = ResponseMessage<Assessment>.Ok(assessmentDb);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Assessment>.Fault(ex.StackTrace);
            }

            return responseMessage;

        }

        private ResponseMessage<AssessmentReport> CreateSummary(Assessment assessmentDb)
        {
            ResponseMessage<AssessmentReport> responseMessage = null;

            try
            {
                var service = assessmentDb?.Parts?.SelectMany(x => x.Services, (part, servico) => new Service()
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
                    AmountHours = x.Where(a => a.Time > 0)?.Sum(a => a.Time),
                    TotalService = x.Where(a => a.Value > 0)?.Sum(a => a.Value),
                    TotalMaterial = x.Sum(a => a.MaterialValue) > 0 ? x.Sum(a => a.MaterialValue) : new double?(),
                    Total = x.Where(a => a.Total > 0)?.Sum(a => a.Total),
                    ValuePerHour = x.FirstOrDefault().ValuePerHour
                })
                .ToList();

                var parts = assessmentDb?.Parts?.Sum(a => a.TotalPrice);
                var additionalParts = assessmentDb?.AdditionalServices?.Sum(a => a.Value);
                var material = service?.Where(a => a.TotalMaterial > 0)?.Sum(a => a.TotalMaterial);

                service.Add(new AssessmentSummary() { Description = "Parts", Total = parts });
                service.Add(new AssessmentSummary() { Description = "AdditionalService", Total = additionalParts });
                service.Add(new AssessmentSummary() { Description = "Material", Total = material });

                var assessmentReport = new AssessmentReport()
                {
                    Assessment = assessmentDb,
                    Summaries = service
                };

                responseMessage = ResponseMessage<AssessmentReport>.Ok(assessmentReport);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<AssessmentReport>.Fault(ex.StackTrace);
            }

            return responseMessage;
        }

        private ResponseMessage<Assessment> GetVersion(Assessment assessment)
        {
            ResponseMessage<Assessment> responseMessage = null;

            try
            {
                var assessmentJson = _repository
                                        .All<AssessmentVersion>(
                                            a => a.IdAssessment == assessment.IdAssessment &&
                                            a.Version == assessment.Version &&
                                            a.Type == EnmAssessmentVersion.AssessmentComplete
                                        )
                                        .OrderBy(a => a.ChangeDate)
                                        .FirstOrDefault();

                if (assessmentJson == null)
                    return ResponseMessage<Assessment>.Fault("Versão não encontrada.");

                var assessmentReport = JsonConvert.DeserializeObject<Assessment>(assessmentJson.AssessmentData);

                var inspector = _repository.All<User>(a => a.IdUser == assessmentJson.IdUser)
                                            .Select(a => new User() { Name = a.Name, Email = a.Email, IdUser = a.IdUser })
                                            .FirstOrDefault();

                inspector = inspector ?? new User() { Email = assessmentJson.Email, IdUser = assessmentJson.IdUser };

                assessmentReport.SetInspector(inspector);

                responseMessage = ResponseMessage<Assessment>.Ok(assessmentReport);

            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<Assessment>.Fault(ex.Message);
            }

            return responseMessage;
        }

        #endregion
    }
}
