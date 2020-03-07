using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallRepair.Business
{
    public class CustomerBusiness
    {
        private readonly RepositoryEntity _repository;

        public CustomerBusiness(RepositoryEntity repositoryEntity)
        {
            _repository = repositoryEntity;
        }

        public IList<AdditionalService> GetAdditionalServices(int idCustomer)
        {
            return _repository
                    .All<AdditionalService>(a => a.IdCustomer == idCustomer)
                    .ToList();
        }

        public Customer GetCustomer(int idCustomer)
        {
            return _repository.Find<Customer>(idCustomer);
        }
    }
}
