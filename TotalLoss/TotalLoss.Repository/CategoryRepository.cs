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
        public CategoryRepository(IDbConnection connection)
            : base(connection) { }

        /// <summary>
        /// Busca todas as Categorias cadastradas por Companhia 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IList<Category> ListByCompany(Company company)
        {
            try
            {
                var @parameters = new { idConfiguration = company.Id };

                IList<Category> listCategory = this.Conexao
                                                   .Query<Category>
                                                   (
                                                        @"SELECT 
		                                                        CTG.IDCATEGORY	     ID, 
		                                                        CTG.LABEL		     NAME,                                                                 
		                                                        CTG.IMAGE		     IMAGE, 
		                                                        CTG.POINT		     POINT
                                                        FROM [CATEGORY] CTG
                                                        INNER JOIN [INSURANCECOMPANY] CON
	                                                        ON CTG.IDINSURANCECOMPANY = CON.IDINSURANCECOMPANY
                                                        WHERE 
	                                                        CTG.IDINSURANCECOMPANY = @idConfiguration",
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
