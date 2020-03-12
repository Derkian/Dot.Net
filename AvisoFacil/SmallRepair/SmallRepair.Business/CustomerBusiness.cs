using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallRepair.Business
{
    public class CustomerBusiness : BaseBussiness
    {
        public CustomerBusiness(RepositoryEntity repository) : base(repository)
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
                    .All<AdditionalService>(a => a.IdCustomer == idCustomer)
                    .ToList();
        }

        public IList<ServiceValue> GetServiceValues(string idCustomer)
        {
            return _repository
                    .All<ServiceValue>(a => a.IdCustomer == idCustomer)
                    .ToList();
        }

        public Customer GetCustomer(string idCustomer)
        {
            return _repository.Find<Customer>(idCustomer);
        }
    }
}
