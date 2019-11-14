using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository
{
    public class QuestionRepository : BaseRepository, Interface.IQuestionRepository
    {
        public QuestionRepository(IDbConnection connection)
            : base(connection) { }

        /// <summary>
        /// Busca todas as Questões cadastradas por Categoria 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IList<Question> ListByCategory(Category category)
        {
            try
            {
                var @parameters = new { id = category.Id };

                IList<Question> listQuestion = this.Conexao
                                                .Query<Question>
                                                (
                                                  @"SELECT 
		                                                  QUE.IDQUESTION	  ID, 
		                                                  QUE.LABEL		      LABEL,                                                           
	                                                      QUE.QUESTIONTYPE    TYPE,
		                                                  QUE.POINT		      POINT, 	
                                                          QUE.ENABLE	      ENABLE
                                                  FROM [QUESTION] QUE
                                                  INNER JOIN [CATEGORY] CTG
	                                                 ON CTG.IDCATEGORY = QUE.IDCATEGORY
                                                  WHERE 
	                                                  QUE.IDCATEGORY = @id",
                                                   param: parameters
                                                )
                                                .Distinct()
                                                .ToList();

                return listQuestion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
