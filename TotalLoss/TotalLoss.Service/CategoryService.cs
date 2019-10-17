using System;
using System.Collections.Generic;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class CategoryService
    {
        private ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Retorna todas as Categorias associadas à Companhia informada
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IList<Category> GetCategoriesByCompany(Configuration configuration)
        {
            IList<Category> listCategory = null;
            try
            {
                listCategory = _categoryRepository.ListByCompany(configuration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listCategory;
        }
    }
}
