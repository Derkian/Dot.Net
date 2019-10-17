using System;
using System.Collections.Generic;
using System.Linq;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class QuestionService
    {
        private IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            this._questionRepository = questionRepository;
        }
        
        /// <summary>
        /// Retorna todas as Questões ativas associadas à Categoria informada
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionsByCategory(Category category)
        {
            IList<Question> listQuestion = null;
            try
            {
                // Retorna somentes Perguntas que estejam ativas
                listQuestion = _questionRepository.ListByCategory(category).Where(q => q.Enable == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listQuestion;
        }
    }
}
