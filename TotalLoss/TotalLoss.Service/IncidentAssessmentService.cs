
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

        private IIncidentAssessmentRepository _incidentAssessmentRepository;
        private ShortMessageService _shortMessageService;
        private string _urlIncidentAssessment;

        #endregion

        #region | Constructor 

        public IncidentAssessmentService(IIncidentAssessmentRepository repository,
                                         string shortMessageAccount,
                                         string shortMessagePassword,
                                         string urlIncidentAssessment)
        {
            this._incidentAssessmentRepository = repository;
            this._urlIncidentAssessment = urlIncidentAssessment;

            _shortMessageService = new ShortMessageService(shortMessageAccount, shortMessagePassword);
        }

        #endregion

        #region | Public 

        /// <summary>
        /// Insere as respostas informadas por Incidente
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        public IncidentAssessment AddAnswers(IncidentAssessment incidentAssessment, List<Question> questions)
        {
            try
            {

                _incidentAssessmentRepository.BeginTransaction();

                // Adiciona Respostas somente se Incidente ainda não estiver finalizado (Type = TotalLoss/Recupered)
                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    //Apaga as respostas anteriores
                    this.DeleteAnswers(incidentAssessment);

                    //adiciona as respostas
                    foreach (Question question in questions)
                    {
                        _incidentAssessmentRepository.AddAnswers(incidentAssessment, question);
                    }

                    //Incluir Status Pendente
                    incidentAssessment.Status = StatusIncidentAssessment.Pending;

                    //Executa o cálculo do total de pontos
                    incidentAssessment = this.TotalPoint(incidentAssessment);

                    //Atualiza o total de pontos
                    _incidentAssessmentRepository.Update(incidentAssessment);
                }

                _incidentAssessmentRepository.Commit();

            }
            catch (Exception ex)
            {
                _incidentAssessmentRepository.RollBack();

                throw ex;
            }

            return incidentAssessment;
        }

        /// <summary>
        /// Adicionar Imagem ao Incidente
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="inicidentImage"></param>
        /// <returns></returns>
        public bool AddImage(IncidentAssessment incidentAssessment, IncidentAssessmentImage inicidentImage)
        {
            bool returnAdd = false;
            try
            {
                _incidentAssessmentRepository.BeginTransaction();

                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    returnAdd = _incidentAssessmentRepository.AddImage(incidentAssessment, inicidentImage);
                }

                _incidentAssessmentRepository.Commit();
            }
            catch (Exception ex)
            {
                _incidentAssessmentRepository.RollBack();

                throw ex;
            }
            return returnAdd;
        }

        /// <summary>
        /// Cria um Incidente por dados informados
        /// </summary>
        /// <param name="incidenteAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Create(IncidentAssessment incidenteAssessment)
        {
            try
            {
                _incidentAssessmentRepository.BeginTransaction();

                // Atribui o Tipo inicial de criação do Incident
                incidenteAssessment.Status = StatusIncidentAssessment.Created;

                // Cria o Incidente na base de dados 
                _incidentAssessmentRepository.Create(incidenteAssessment);

                #region | Carrega informações do Objeto SMS

                // Configura nome da Companhia de acordo cultura informada
                string strCompanyName = CultureInfo.GetCultureInfo("pt-BR").TextInfo.ToTitleCase(incidenteAssessment?.Configuration?.Name.ToLower());

                // Configura message do SMS 
                string smsMessage = $"{strCompanyName}: Prezado, acesse este link para classificar o tipo de avarias{Environment.NewLine}{this._urlIncidentAssessment}{incidenteAssessment.Key}";

                // Carrega informaçãoes do SMS que será enviada ao prestador
                ShortMessage shortMessage = new ShortMessage()
                {
                    Fone = incidenteAssessment.WorkProviderFone,
                    Message = smsMessage
                };

                #endregion

                // Envia mensagem SMS via API de Terceiros
                var result = this._shortMessageService.Send(shortMessage);

                // Carrega Código retornado pela API após envio de SMS
                incidenteAssessment.ShortMessageCode = result.ShortMessageCode;

                // Atualiza o Incidente na base de dados após envio de SMS ao prestador
                _incidentAssessmentRepository.Update(incidenteAssessment);

                incidenteAssessment = this.FindByKey(incidenteAssessment.Key);

                _incidentAssessmentRepository.Commit();

                return incidenteAssessment;
            }
            catch (Exception ex)
            {
                _incidentAssessmentRepository.RollBack();

                throw ex;
            }
        }

        /// <summary>
        /// Exclui Respostas por ID do Incidente informado
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        private bool DeleteAnswers(IncidentAssessment incidentAssessment)
        {
            bool returnUpdate = false;
            try
            {
                // Exclui Respostas somente se Incidente estiver como Pendente de finalização (Type = FinalizePending)
                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                    returnUpdate = this._incidentAssessmentRepository.DeleteAnswers(incidentAssessment.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnUpdate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incidenteAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Finalize(IncidentAssessment incidentAssessment)
        {
            try
            {
                _incidentAssessmentRepository.BeginTransaction();

                // Finaliza somente Incidentes que já possuem Respostas informadas
                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    // Realiza o cálculo de pontuação
                    incidentAssessment = this.TotalPoint(incidentAssessment);

                    //Finaliza o incidente
                    incidentAssessment.Status = StatusIncidentAssessment.Finalized;

                    // Atualiza na base de dados o Tipo do Incidente 
                    _incidentAssessmentRepository.Update(incidentAssessment);
                }

                _incidentAssessmentRepository.Commit();
            }
            catch (Exception ex)
            {
                _incidentAssessmentRepository.RollBack();

                throw ex;
            }

            return incidentAssessment;
        }

        private IncidentAssessment TotalPoint(IncidentAssessment incidentAssessment)
        {
            try
            {
                // Carrega as Respostas associadas ao Incidente juntamente com sua Categoria
                List<Category> listCategory = _incidentAssessmentRepository.GetAnswers(incidentAssessment.Id).ToList();

                if (listCategory.Count > 0)
                {
                    // Calcula total de pontos das Respostas somados ao pontos da sua Categoria
                    incidentAssessment.TotalPoint = listCategory
                                                        .Sum(c => c.Point + c.Questions.Where(i => i.Answer == true)
                                                        .Sum(q => q.Point));

                    if (incidentAssessment.TotalPoint >= incidentAssessment.Configuration.LimitTotalLoss)
                        incidentAssessment.Type = TypeIncidentAssessment.TotalLoss;
                    else
                        incidentAssessment.Type = TypeIncidentAssessment.Recupered;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return incidentAssessment;
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

            // Carrega Incidente cadastrado pelo ID
            incidentAssessment = _incidentAssessmentRepository.Find(incidentAssessment.Id);

            return incidentAssessment;
        }

        /// <summary>
        /// Atualiza o Incidente informado
        /// </summary>
        /// <param name="incidenteAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Update(IncidentAssessment incidentAssessment)
        {
            try
            {
                _incidentAssessmentRepository.BeginTransaction();

                if (incidentAssessment.Status != StatusIncidentAssessment.Finalized)
                {
                    _incidentAssessmentRepository.Update(incidentAssessment);
                }

                _incidentAssessmentRepository.Commit();
            }
            catch (Exception ex)
            {
                _incidentAssessmentRepository.RollBack();

                throw ex;
            }

            return incidentAssessment;
        }

        #endregion
    }
}
