
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class IncidentAssessmentService
    {
        #region | Objects

        private IWorkRepository _workRepository;
        private ShortMessageService _shortMessageService;
        private string _urlIncidentAssessment;

        public User LogHistoryUser { get; set; }

        #endregion

        #region | Constructor 

        public IncidentAssessmentService(IWorkRepository repository,
                                         string shortMessageAccount,
                                         string shortMessagePassword,
                                         string urlIncidentAssessment)
        {
            this._workRepository = repository;
            this._urlIncidentAssessment = urlIncidentAssessment;
            _shortMessageService = new ShortMessageService(shortMessageAccount, shortMessagePassword);
        }

        #endregion

        #region | Public 

        #region Respostas

        /// <summary>
        /// Insere as respostas informadas por Incidente
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        public IncidentAssessment AddAnswers(string key, List<Question> questions)
        {
            IncidentAssessment incidentAssessment = new IncidentAssessment(key);

            try
            {
                // Busca o Incidente pela chave informada
                incidentAssessment = this._workRepository.IncidentAssessmentRepository.Find(incidentAssessment.Id);

                if (incidentAssessment == null)
                    return incidentAssessment;

                // Carrega Seguradora associada ao Incidente
                Company companyFilter = new Company() { Id = incidentAssessment.IdInsuranceCompany, TypeCompany = TypeCompany.InsuranceCompany };

                // Busca Configuração da Seguradora associada ao Incidente
                InsuranceCompany configurationCompany = this._workRepository.ConfigurationRepository.Find(companyFilter);

                // Adiciona Respostas somente se Incidente ainda não estiver finalizado (Type = TotalLoss/Recupered)
                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    //abre uma transacão
                    this._workRepository.BeginTransaction();

                    //Apaga as respostas anteriores
                    this._workRepository.IncidentAssessmentRepository.DeleteAnswers(incidentAssessment.Id);

                    //adiciona as respostas
                    foreach (Question question in questions)
                        this._workRepository.IncidentAssessmentRepository.AddAnswers(incidentAssessment, question);

                    //Incluir Status Respostas adicionadas
                    incidentAssessment.Status = StatusIncidentAssessment.Answered;

                    //Executa o cálculo do total de pontos
                    incidentAssessment = this.TotalPoint(incidentAssessment, configurationCompany);

                    //Atualiza o total de pontos
                    this._workRepository.IncidentAssessmentRepository.Update(incidentAssessment);

                    //add history
                    this.CreateHistory(incidentAssessment, this.LogHistoryUser, TypeOperation.AddAnswear, this._workRepository);

                    //COMMIT 
                    this._workRepository.Commit();
                }

                //GET ANSWEARS ADD
                incidentAssessment.Answers = this._workRepository.IncidentAssessmentRepository.GetAnswers(incidentAssessment.Id);
            }
            catch (Exception ex)
            {
                //ROLLBACK TRANSACAO
                this._workRepository.RollBack();

                throw ex;
            }

            return incidentAssessment;
        }

        /// <summary>
        /// Exclui Respostas por ID do Incidente informado
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        private bool DeleteAnswers(IncidentAssessment incidentAssessment)
        {
            bool returnUpdate = false;

            // Exclui Respostas somente se Incidente estiver como Pendente de finalização (Type = FinalizePending)
            if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                returnUpdate = this._workRepository.IncidentAssessmentRepository.DeleteAnswers(incidentAssessment.Id);

            return returnUpdate;
        }

        #endregion

        #region Incident

        /// <summary>
        /// Cria um Incidente por dados informados
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Create(IncidentAssessment incidentAssessment, Company company)
        {
            try
            {
                //validação para criar somente para companhia de seguros
                if (company.TypeCompany != TypeCompany.InsuranceCompany)
                    return null;

                this._workRepository.BeginTransaction();

                // Atribui o Tipo inicial de criação do Incident
                incidentAssessment.Status = StatusIncidentAssessment.Created;

                //add configuration ID
                incidentAssessment.IdInsuranceCompany = company.Id;

                // Cria o Incidente na base de dados 
                this._workRepository.IncidentAssessmentRepository.Create(incidentAssessment);

                //add history                
                this.CreateHistory(incidentAssessment, this.LogHistoryUser, TypeOperation.CreateIncident, this._workRepository);

                // Comita a transação
                this._workRepository.Commit();

                //retorna o incident
                return this._workRepository.IncidentAssessmentRepository.Find(incidentAssessment.Id);
            }
            catch (Exception ex)
            {
                //rollback transação
                this._workRepository.RollBack();
                throw ex;
            }
        }

        /// <summary>
        /// Busca o Incidente gerado por key criptografada
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IncidentAssessment FindByKey(string key)
        {
            // Descriptografa a Key carregando o ID numérico
            IncidentAssessment incidentAssessment = new IncidentAssessment(key);

            try
            {
                // Carrega Incidente cadastrado pelo ID
                incidentAssessment = this._workRepository.IncidentAssessmentRepository.Find(incidentAssessment.Id);

                //recupera as respostas
                if (incidentAssessment != null)
                    incidentAssessment.Answers = this._workRepository.IncidentAssessmentRepository.GetAnswers(incidentAssessment.Id);

                //adiciona no historico
                this.CreateHistory(incidentAssessment, this.LogHistoryUser, TypeOperation.GetIncident, this._workRepository);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return incidentAssessment;
        }

        /// <summary>
        /// Atualiza o Incidente informado
        /// </summary>
        /// <param name="incidenteAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Update(string key, IncidentAssessment incidentAssessmentToUpdate)
        {
            try
            {
                IncidentAssessment _incidentAssessment = new IncidentAssessment(key);

                // Busca o Incidente pela chave informada
                _incidentAssessment = this._workRepository.IncidentAssessmentRepository.Find(_incidentAssessment.Id);

                // Verifica se o Incidente informado está cadastrado
                if (_incidentAssessment == null)
                    return null;

                //copia as propriedades enviada
                _incidentAssessment.Copy(incidentAssessmentToUpdate);

                // se o processo estiver finalizado
                if (_incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    //abre transação
                    this._workRepository.BeginTransaction();

                    //atualiza o nome e telefone do motorista que foi enviado o SMS
                    if (!string.IsNullOrEmpty(_incidentAssessment?.TowTruckDriver?.Mobile)
                        && string.IsNullOrEmpty(_incidentAssessment?.TowTruckDriverMobile))
                    {
                        _incidentAssessment.TowTruckDriverMobile = _incidentAssessment?.TowTruckDriver?.Mobile;
                        _incidentAssessment.TowTruckDriverName = _incidentAssessment?.TowTruckDriver?.Name;
                    }

                    // Altera o Incidente na base de dados 
                    this._workRepository.IncidentAssessmentRepository.Update(_incidentAssessment);

                    //adiciona no histórico
                    this.CreateHistory(_incidentAssessment, this.LogHistoryUser, TypeOperation.UpdateIncident, this._workRepository);

                    //commit da transação
                    this._workRepository.Commit();
                }
            }
            catch (Exception ex)
            {
                //rollback da transação
                this._workRepository.RollBack();
                throw ex;
            }

            return incidentAssessmentToUpdate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incidenteAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Finalize(string key)
        {
            IncidentAssessment _incidentAssessment = new IncidentAssessment(key);
            try
            {
                //Find Incident
                _incidentAssessment = this._workRepository.IncidentAssessmentRepository.Find(_incidentAssessment.Id);

                //Return if is null
                if (_incidentAssessment == null)
                    return _incidentAssessment;

                // Carrega Seguradora associada ao Incidente
                Company companyFilter = new Company() { Id = _incidentAssessment.IdInsuranceCompany, TypeCompany = TypeCompany.InsuranceCompany };

                // Busca Configuração da Seguradora associada ao Incidente
                InsuranceCompany configurationCompany = this._workRepository.ConfigurationRepository.Find(companyFilter);

                //Begin Transaction
                this._workRepository.BeginTransaction();

                // Finaliza somente Incidentes que já possuem Respostas informadas
                if (_incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    // Realiza o cálculo de pontuação
                    _incidentAssessment = this.TotalPoint(_incidentAssessment, configurationCompany);

                    //Finaliza o incidente
                    _incidentAssessment.Status = StatusIncidentAssessment.Finalized;

                    // Atualiza na base de dados o Tipo do Incidente 
                    this._workRepository.IncidentAssessmentRepository.Update(_incidentAssessment);
                }

                //add history    
                this.CreateHistory(_incidentAssessment, this.LogHistoryUser, TypeOperation.FinalizeIncident, this._workRepository);

                //Commit Transaction
                this._workRepository.Commit();
            }
            catch (Exception ex)
            {
                this._workRepository.RollBack();

                throw ex;
            }

            return _incidentAssessment;
        }

        /// <summary>
        /// Envia o SMS
        /// </summary>
        /// <param name="incidentKey"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public IncidentAssessment SendSMS(string incidentKey, Company company)
        {
            try
            {
                //valida se existe o incident
                var incidentAssessment = new IncidentAssessment(incidentKey);

                //busca o incident
                incidentAssessment = this._workRepository.IncidentAssessmentRepository.Find(incidentAssessment.Id);

                //valida se existe o incident 
                if (incidentAssessment == null)
                    throw new Exception("Registro não encontrado");

                //caso seja um incident criado atualiza para pendente - Inicio o processo de Avaliação
                if (incidentAssessment.Status == StatusIncidentAssessment.Created)
                {
                    //Alterar o status para pendente
                    incidentAssessment.Status = StatusIncidentAssessment.Pending;
                }

                // Configura nome da Companhia de acordo cultura informada
                string strCompanyName = CultureInfo.GetCultureInfo("pt-BR").TextInfo.ToTitleCase(incidentAssessment?.InsuranceCompany?.Name?.ToLower());

                // Configura message do SMS 
                string smsMessage = $"{strCompanyName}: Prezado, acesse este link para classificar o tipo de avarias{Environment.NewLine}{this._urlIncidentAssessment}{incidentAssessment.Key}";

                // Carrega informaçãoes do SMS que será enviada ao prestador
                ShortMessage shortMessage = new ShortMessage()
                {
                    Fone = incidentAssessment?.TowTruckDriver?.Mobile != null
                            ? incidentAssessment?.TowTruckDriver?.Mobile
                            : incidentAssessment.TowTruckDriverMobile,

                    Message = smsMessage
                };

                if (string.IsNullOrEmpty(shortMessage.Fone))
                    throw new Exception("Motorista não possui telefone");

                // Envia mensagem SMS via API de Terceiros
                var result = this._shortMessageService.Send(shortMessage);

                // Carrega Código retornado pela API após envio de SMS
                incidentAssessment.ShortMessageCode = result.ShortMessageCode;

                //abre a transação
                this._workRepository.BeginTransaction();

                // Atualiza o Incidente na base de dados após envio de SMS ao prestador
                this._workRepository.IncidentAssessmentRepository.Update(incidentAssessment);

                //adiciona histório de envio de sms
                this.CreateHistory(incidentAssessment, this.LogHistoryUser, TypeOperation.SendSms, this._workRepository);

                //commit da transação
                this._workRepository.Commit();

                //retorna o objeto
                return incidentAssessment;
            }
            catch (Exception ex)
            {
                //rollback
                this._workRepository.RollBack();

                throw ex;
            }
        }

        /// <summary>
        /// Recupera a lista paginada de Incidentes
        /// </summary>
        /// <param name="company"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Pagination<IncidentAssessment> Paginate(Company company, Pagination<IncidentAssessment> page, StatusIncidentAssessment? status)
        {
            try
            {
                var paginate = new Pagination<IncidentAssessment>();

                paginate = this._workRepository.IncidentAssessmentRepository.List(company, page, status);

                return paginate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Imagem

        /// <summary>
        /// Lista as imagens disponíveis no incident
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Pagination<IncidentAssessmentImage> PaginateImage(string key, Pagination<IncidentAssessmentImage> page)
        {
            try
            {
                //cria o incident
                IncidentAssessment incident = new IncidentAssessment(key);

                //Objeto de retorno
                var paginate = new Pagination<IncidentAssessmentImage>();

                //Listar as imagens disponíveis por incident
                paginate = this._workRepository.IncidentAssessmentRepository.ListImage(incident.Id, page);

                //retorna o conteudo
                return paginate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera a image thumbnail adicionada por ID
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="idIncidentImage"></param>
        /// <returns></returns>
        public IncidentAssessmentImage GetThumbnailImage(string key, int idIncidentImage)
        {
            try
            {
                IncidentAssessment IncidentAssessment = new IncidentAssessment(key);

                //Recupera a imagem
                var image = this._workRepository.IncidentAssessmentRepository.FindImage(IncidentAssessment.Id, idIncidentImage);

                //Objeto de retorno
                return image ?? (image = new IncidentAssessmentImage());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera a image adicionada por ID
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="idIncidentImage"></param>
        /// <returns></returns>
        public IncidentAssessmentImage GetImage(string key, int idIncidentImage)
        {
            try
            {
                IncidentAssessment IncidentAssessment = new IncidentAssessment(key);

                //Recupera a imagem
                var image = this._workRepository.IncidentAssessmentRepository.FindImage(IncidentAssessment.Id, idIncidentImage, true);

                //Objeto de retorno
                return image ?? (image = new IncidentAssessmentImage());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adicionar Imagem ao Incidente
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="inicidentImage"></param>
        /// <returns></returns>
        public bool AddImage(string incidentKey, IncidentAssessmentImage inicidentImage)
        {
            bool returnAdd = false;
            try
            {
                //Cria um novo Incident
                IncidentAssessment incidentAssessment = new IncidentAssessment(incidentKey);

                //Busca o incident na base
                incidentAssessment = this._workRepository.IncidentAssessmentRepository.Find(incidentAssessment.Id);

                //se não encontrar o incident retorna false
                if (incidentAssessment == null)
                    return false;

                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    //generate thumbanail from image stream
                    if (inicidentImage.Image.Length > 0)
                        inicidentImage.Thumbnail = GenerateThumbnail(inicidentImage.Image);

                    //cria uma transacao
                    this._workRepository.BeginTransaction();

                    //adiciona a imagem
                    returnAdd = this._workRepository.IncidentAssessmentRepository.AddImage(incidentAssessment, inicidentImage);

                    //add history
                    this.CreateHistory(incidentAssessment, this.LogHistoryUser, TypeOperation.AddImage, this._workRepository);

                    //Rollback na transacao
                    this._workRepository.Commit();
                }
            }
            catch (Exception ex)
            {
                //rollback na transacao
                this._workRepository.RollBack();

                throw ex;
            }

            return returnAdd;
        }

        /// <summary>
        /// Apaga uma imagem da base de dados
        /// </summary>
        /// <param name="incidentKey"></param>
        /// <param name="idIncidentImage"></param>
        /// <returns></returns>
        public bool DeleteImage(string incidentKey, int idIncidentImage)
        {
            bool removeImage = false; 

            try
            {
                //cria o incident
                IncidentAssessment incident = new IncidentAssessment(incidentKey);

                //busca na base
                incident = this._workRepository.IncidentAssessmentRepository.Find(incident.Id);

                //valida se é nulo
                if (incident == null)
                {
                    return false;
                }                 
                else if (incident.Status != StatusIncidentAssessment.Finalized)
                {
                    //abre uma transação
                    this._workRepository.BeginTransaction();

                    //deleta da base o registro
                    removeImage = this._workRepository.IncidentAssessmentRepository.DeleteImage(incident.Id, idIncidentImage);

                    //adiciona no historico caso tenha removido
                    if (removeImage)
                        this.CreateHistory(incident, this.LogHistoryUser, TypeOperation.DeleteImage, this._workRepository);

                    //efetiva a transação
                    this._workRepository.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return removeImage;
        }

        #endregion

        #endregion

        #region | Private 

        /// <summary>
        /// Gerar Thumbnail da imagem enviada
        /// </summary>
        /// <param name="imageToGenerate"></param>
        /// <returns></returns>
        private byte[] GenerateThumbnail(byte[] imageToGenerate)
        {
            byte[] imageContent;

            //Set image to MemoryStream
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageToGenerate))
            {
                //Generate Image
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                {
                    //Generate Thumbnail
                    using (System.Drawing.Image thumbnailImage = image.GetThumbnailImage(200, 200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
                    {
                        // make a memory stream to work with the image bytes
                        using (System.IO.MemoryStream imageStream = new System.IO.MemoryStream())
                        {
                            // put the image into the memory stream
                            thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            // make byte array the same size as the image
                            imageContent = new Byte[imageStream.Length];

                            // rewind the memory stream
                            imageStream.Position = 0;

                            // load the byte array with the image
                            imageStream.Read(imageContent, 0, (int)imageStream.Length);
                        }
                    }
                }
            }

            return imageContent;
        }

        public bool ThumbnailCallback()
        {
            return true;
        }

        /// <summary>
        /// Calcular o Total de Pontos por Incidente
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <returns></returns>
        private IncidentAssessment TotalPoint(IncidentAssessment incidentAssessment, InsuranceCompany company)
        {
            // Carrega as Respostas associadas ao Incidente juntamente com sua Categoria
            List<Category> listCategory = this._workRepository.IncidentAssessmentRepository.GetAnswersByCategory(incidentAssessment.Id).ToList();

            if (listCategory.Count > 0)
            {
                // Calcula total de pontos das Respostas somados ao pontos da sua Categoria
                incidentAssessment.TotalPoint = listCategory
                                                    .Sum(c => c.Point + c.Questions.Where(i => i.Answer == true)
                                                    .Sum(q => q.Point));

                if (incidentAssessment.TotalPoint >= company.LimitTotalLoss)
                    incidentAssessment.Type = TypeIncidentAssessment.TotalLoss;
                else
                    incidentAssessment.Type = TypeIncidentAssessment.Recupered;
            }

            return incidentAssessment;
        }

        /// <summary>
        /// Gera o Histórico
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="user"></param>
        /// <param name="operation"></param>
        /// <param name="repository"></param>
        internal void CreateHistory(IncidentAssessment incidentAssessment, User user, TypeOperation operation, IWorkRepository repository)
        {
            try
            {
                History history = new History()
                {
                    Date = DateTime.Now,
                    incidentAssessment = incidentAssessment,
                    User = user,
                    Operation = operation
                };

                repository.HistoryRepository.AddHistory(history);
            }
            catch { }
        }

        #endregion
    }
}
