using SmallRepair.Management.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Business
{
    public class BaseBussiness
    {
        protected readonly RepositoryEntity _repository;

        public BaseBussiness(RepositoryEntity repository)
        {
            _repository = repository;
        }
    }
}
