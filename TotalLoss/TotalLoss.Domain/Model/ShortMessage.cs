using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;

namespace TotalLoss.Domain.Model
{
    public class ShortMessage
    {
        public string Id { get; set; }

        public string Fone { get; set; }

        public string Message { get; set; }
    }

    public class ShortMessageResult
    {
        public ShortMessageCode ShortMessageCode;

        public string Code { get; set; }

        public string Text { get; set; }
    }
}
