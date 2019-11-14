using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TotalLoss.API.Models;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class CategoryController : ApiController
    {
        #region | Objects

        private readonly CategoryService _categoryService;

        #endregion

        #region | Constructor 

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        #endregion

        #region | Actions

        [HttpGet]
        [Route("api/Categories/GetCategoriesByCompany/{idCompany}")]
        public async Task<IHttpActionResult> GetCategoriesByCompany(int idCompany)
        {
            IList<Category> listCategory = null;
            Domain.Model.Company request = null;

            // Cria entidade Configuration
            request = new Domain.Model.Company() { Id = idCompany };

            // Busca todas Categorias por Companhia informada
            listCategory = await Task.Run(() => _categoryService.GetCategoriesByCompany(request));

            // Verifica se existem Categorias pela Companhia informada
            if (listCategory == null || listCategory.Count == 0)
                return NotFound();

            // Retorna response com todas Categorias por Campanhia
            return Ok(listCategory);
        }

        #endregion
    }
}
