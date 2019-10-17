using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Enums
{
        public enum ShortMessageCode
        {
            NotSend = -1,
            MessageSent = 0,
            EmptyMessageContent = 10,
            MessageContentOverflow = 12,
            IncorrectOrIncompleteMobileNumber = 13,
            EmptyMobileNumber = 14,
            SchedulingDateInvalidOrIncorrect = 15,
            IDOverflow = 16,
            MessageWithSameIdAlreadySent = 80,
            InternationalSendingNotAllowed = 141,
            AuthenticationError = 900,
            AccountLimitReached = 990,
            WrongOperationRequested = 998,
            UnknownError = 999
        }
}
