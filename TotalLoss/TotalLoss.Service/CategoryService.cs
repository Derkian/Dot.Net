using System;
using System.Collections.Generic;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class CategoryService
    {
        private IWorkRepository _workRepository;

        public CategoryService(IWorkRepository workRepository)
        {
            this._workRepository = workRepository;
        }

        /// <summary>
        /// Retorna todas as Categorias associadas à Companhia informada
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IList<Category> GetCategoriesByCompany(Company company)
        {
            IList<Category> listCategory = null;

            try
            {
                listCategory = _workRepository.CategoryRepository.ListByCompany(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listCategory;
        }
    }
}
