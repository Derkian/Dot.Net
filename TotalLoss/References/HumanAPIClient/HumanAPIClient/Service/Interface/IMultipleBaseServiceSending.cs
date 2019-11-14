using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Service.Interface
{
    /// <summary>
    /// Interface base para envio de multiplos sms.
    /// </summary>
    public interface IMultipleBaseServiceSending : IBaseServiceSending
    {
        /// <summary>
        /// Envia uma requisicao ao servidor para consulta de status de sms.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<String> query(String[] ids);
    }
}
