using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanAPIClient.Enum
{
    /// <summary>
    /// Enumerador para os tipos de layout existentes
    /// </summary>
    public enum LayoutTypeEnum
    {
        /// <summary>
        /// to;msg
        /// </summary>
        TYPE_A = 'A',
        /// <summary>
        /// to;msg;from
        /// </summary>
        TYPE_B = 'B',
        /// <summary>
        /// to;msg;id
        /// </summary>
        TYPE_C = 'C',
        /// <summary>
        /// to;msg;id;from
        /// </summary>
        TYPE_D = 'D',
        /// <summary>
        /// to;msg;id;from;schedule
        /// </summary>
        TYPE_E = 'E'
    }
}
