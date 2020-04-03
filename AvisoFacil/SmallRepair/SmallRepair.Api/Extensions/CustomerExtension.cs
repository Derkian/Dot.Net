using SmallRepair.Api.Model;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Extensions
{
    public static class CustomerExtension
    {
        public static CompanyViewModel ToView(this Company customer)
        {
            var cliente = new CompanyViewModel() { Id = customer.IdCompany, Name = customer.Name };

            if (customer?.ServiceValues != null)
            {
                cliente.ServiceValues = customer
                                        .ServiceValues
                                        .Select(a => new ServiceValueViewModel()
                                        {
                                            ServiceType = a.ServiceType,
                                            Value = a.Value
                                        })
                                        .ToList();
            }

            return cliente;
        }

        public static IList<AdditionalServiceViewModel> ToView(this IList<AdditionalService> services)
        {
            return services
                    .Select(a => new AdditionalServiceViewModel()
                    {   
                        Description = a.Description,
                        Value = a.Value
                    })
                    .ToList();
        }
    }
}
