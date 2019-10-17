using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Repository
{
    public class CategoryRepository 
                : BaseRepository, Interface.ICategoryRepository
    {
        public CategoryRepository(IDbConnection conexao)
            : base(conexao) { }
        
        /// <summary>
        /// Busca todas as Categorias cadastradas por Companhia 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IList<Category> ListByCompany(Configuration configuration)
        {
            try
            {
                var @parameters = new { idConfiguration = configuration.Id };

                IList<Category> listCategory = this.Conexao
                                                   .Query<Category>
                                                   (
                                                        @"SELECT 
		                                                        CTG.IDCATEGORY	     ID, 
		                                                        CTG.LABEL		     NAME,                                                                 
		                                                        CTG.IMAGE		     IMAGE, 
		                                                        CTG.POINT		     POINT
                                                        FROM [CATEGORY] CTG
                                                        INNER JOIN [COMPANY] CON
	                                                        ON CTG.IDCOMPANY = CON.IDCOMPANY
                                                        WHERE 
	                                                        CTG.IDCOMPANY = @idConfiguration",
                                                        param: parameters
                                                   )
                                                   .Distinct()
                                                   .ToList();

                return listCategory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
