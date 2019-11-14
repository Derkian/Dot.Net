using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Model;

namespace HumanAPIClient.Service.Interface
{
    /// <summary>
    /// Interface base para servicos que envolve envio de mensagens sms.
    /// </summary>
    public interface IBaseServiceSending
    {
        /// <summary>
        /// Envia uma requisicao ao servidor para envio de mensagem sms.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<String> send(Message parameters);
    }
}
