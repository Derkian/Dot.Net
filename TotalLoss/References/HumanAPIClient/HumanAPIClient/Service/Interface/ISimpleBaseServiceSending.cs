using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Service.Interface
{
    /// <summary>
    /// Interface base para envio de mensagem sms simples.
    /// </summary>
    public interface ISimpleBaseServiceSending : IBaseServiceSending
    {
        /// <summary>
        /// Envia uma requisicao ao servidor para consulta de status de sms.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<String> query(String id);
    }
}
