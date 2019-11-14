using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface IIncidentAssessmentRepository : IBaseRepository
    {
        #region Incident
        /// <summary>
        /// Cria um incident
        /// </summary>
        /// <param name="incidentAssessment"></param>
        void Create(IncidentAssessment incidentAssessment);
        /// <summary>
        /// Atualiza o incident
        /// </summary>
        /// <param name="incidentAssessment"></param>
        void Update(IncidentAssessment incidentAssessment);
        /// <summary>
        /// Busca um incident por ID
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        IncidentAssessment Find(int idIncidentAssessment);
        /// <summary>
        /// Lista Paginado o Incident
        /// </summary>
        /// <param name="company"></param>
        /// <param name="pagination"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Pagination<IncidentAssessment> List(Company company, Pagination<IncidentAssessment> pagination, StatusIncidentAssessment? status);
        #endregion

        #region Respostas
        /// <summary>
        /// Lista as Respostas
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        IList<Question> GetAnswers(int idIncidentAssessment);
        /// <summary>
        /// Lista as respostas com Categoria
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        IList<Category> GetAnswersByCategory(int idIncidentAssessment);
        /// <summary>
        /// Remove todas as respostas
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        bool DeleteAnswers(int idIncidentAssessment);
        /// <summary>
        /// Adiciona uma resposta
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        bool AddAnswers(IncidentAssessment incidentAssessment, Question question);
        #endregion

        #region Imagem
        /// <summary>
        /// Adiciona uma imagem
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="inicidentImage"></param>
        /// <returns></returns>
        bool AddImage(IncidentAssessment incidentAssessment, IncidentAssessmentImage inicidentImage);
        /// <summary>
        /// Lista as imagens Paginado
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        Pagination<IncidentAssessmentImage> ListImage(int idIncident, Pagination<IncidentAssessmentImage> pagination);
        /// <summary>
        /// Encontra uma imagem 
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="idIncidentImage"></param>
        /// <param name="fullImage"></param>
        /// <returns></returns>
        IncidentAssessmentImage FindImage(int idIncident, int idIncidentImage, bool fullImage = false);
        /// <summary>
        /// Remove uma imagem
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <param name="idIncidentAssessmentImage"></param>
        /// <returns></returns>
        bool DeleteImage(int idIncidentAssessment, int idIncidentAssessmentImage); 
        #endregion
    }
}
