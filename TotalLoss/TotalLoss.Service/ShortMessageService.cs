using HumanAPIClient.Model;
using HumanAPIClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;

using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.Service
{
    public class ShortMessageService
    {
        SimpleSending simpleSend;

        public ShortMessageService(string account, string password)
        {

            simpleSend = new SimpleSending(account, password);
        }

        public ShortMessageResult Send(ShortMessage sms)
        {
            ShortMessageResult shorMessageResult = new ShortMessageResult();

            SimpleMessage simpleMessage = new SimpleMessage()
            {
                To = sms.Fone,
                Message = sms.Message,
                Id = sms.Id
            };

            List<string> _simpleSendResponse = this.simpleSend.send(simpleMessage);

            string result = _simpleSendResponse.First();            
            string[] zenviaResult = result.Split('-');

            shorMessageResult.ShortMessageCode = ShortMessageCode.UnknownError;

            if (zenviaResult.Length > 1)
            {
                shorMessageResult.Code = zenviaResult.First().Trim();
                shorMessageResult.Text = zenviaResult.Last().Trim();
            }

            //000 - Message Sent
            Int32 resultCode;
            if (Int32.TryParse(shorMessageResult.Code, out resultCode))
            {   
                shorMessageResult.ShortMessageCode = (ShortMessageCode)resultCode;
            }

            return shorMessageResult;
        }
    }
}
