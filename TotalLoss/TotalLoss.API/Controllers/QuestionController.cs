using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TotalLoss.API.Models;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class QuestionController : ApiController
    {
        #region | Objects

        private readonly QuestionService _questionService;

        #endregion

        #region | Constructor 

        public QuestionController(QuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        #region | Actions

        [HttpGet]
        [Route("api/Questions/GetQuestionsByCategory/{idCategory}")]
        public async Task<IHttpActionResult> GetQuestionsByCategory(int idCategory)
        {
            IList<Question> listQuestion = null;
            Domain.Model.Category request = null;
            try
            {
                // Cria entidade Category
                request = new Domain.Model.Category() { Id = idCategory };

                // Busca todas Questões por Categoria informada
                listQuestion = await Task.Run(() => _questionService.GetQuestionsByCategory(request));

                // Verifica se existem Questões pela Categoria informada
                if (listQuestion == null)
                    return NotFound();
            }
            catch (System.Exception ex)
            {
                // Retorna Json de Erro interno gerado 
                return InternalServerError(new System.Exception(ex.Message));
            }

            // Retorna response com todas Questões por Categoria
            return Ok(listQuestion);
        }

        #endregion           
    }
}
