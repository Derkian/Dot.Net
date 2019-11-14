using System;
using System.Collections.Generic;
using System.Linq;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class QuestionService
    {
        private IWorkRepository _workRepository;

        public QuestionService(IWorkRepository workRepository)
        {
            this._workRepository = workRepository;
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
                listQuestion = _workRepository.QuestionRepository.ListByCategory(category).Where(q => q.Enable == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listQuestion;
        }
    }
}
