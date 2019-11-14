using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanAPIClient.Model;
using System.IO;
using HumanAPIClient.Util.Exceptions;

namespace HumanAPIClient.Util
{
    class ResponseParser
    {

        public static List<SimpleMessage> getMessagesFromStream(Stream stream) {
            String RETORNO_OK = "#300#301#";
            List<SimpleMessage> messages = new List<SimpleMessage>();
            StreamReader sr = new StreamReader(stream);
            String header = sr.ReadLine();
            if (RETORNO_OK.IndexOf(header.Substring(0, 3)) < 0)
            {
                throw new ClientHumanException(header);
            }
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                SimpleMessage msg = convertResponse(line);
                if (msg != null)
                {
                    messages.Add(msg);
                }
                
            }
            return messages;
        
        }

        private static SimpleMessage convertResponse(String response)
        {
            String[] fields = response.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            SimpleMessage msg = null;
            if (fields != null && fields.Length > 0)
            {
                msg = new SimpleMessage();
                msg.Id = fields[0];
                msg.Schedule = fields[1];
                msg.From = fields[2];
                msg.Message = fields[3];
            }
            return msg;
        }

    }
}
