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
        public static ClienteViewModel ToView(this Customer customer)
        {
            var cliente = new ClienteViewModel() { Id = customer.IdCustomer, Nome = customer.Name };

            if (customer?.ServiceValues != null)
            {
                cliente.ValorServico = customer
                                        .ServiceValues
                                        .Select(a => new ValorServicoViewModel()
                                        {
                                            ServiceType = a.ServiceType,
                                            Value = a.Value
                                        })
                                        .ToList();
            }

            return cliente;
        }

        public static IList<ServicoAdicionalModel> ToView(this IList<AdditionalService> services)
        {
            return services
                    .Select(a => new ServicoAdicionalModel()
                    {
                        Id = a.IdAdditionalService,
                        Description = a.Description,
                        Value = a.Value
                    })
                    .ToList();
        }
    }
}
