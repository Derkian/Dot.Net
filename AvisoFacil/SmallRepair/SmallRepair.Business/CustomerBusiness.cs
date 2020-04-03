using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallRepair.Business
{
    public class CompanyBusiness : BaseBussiness
    {
        public CompanyBusiness(RepositoryEntity repository) : base(repository)
        {
        }

        //private readonly RepositoryEntity _repository;

        //public CustomerBusiness(RepositoryEntity repositoryEntity)
        //{
        //    _repository = repositoryEntity;
        //}

        public IList<AdditionalService> GetAdditionalServices(string idCustomer)
        {
            return _repository
                    .All<AdditionalService>(a => a.IdCompany == idCustomer)
                    .ToList();
        }

        public IList<ServiceValue> GetServiceValues(string idCustomer)
        {
            return _repository
                    .All<ServiceValue>(a => a.IdCompany == idCustomer)
                    .ToList();
        }

        public Company Get(string idCustomer)
        {
            return _repository.Find<Company>(idCustomer);
        }
    }
}
