using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallRepair.Business.Model
{
    public class ResponseMessage<TEntity>
    {
        public bool Sucess { get; set; }

        public TEntity Object { get; set; }

        public IList<string> Error { get; set; }

        private ResponseMessage(bool sucess, TEntity result, IList<string> erro)
        {
            Error = erro;
            Sucess = sucess;
            Object = result;
        }

        public static ResponseMessage<TEntity> Ok(TEntity value)
        {
            return new ResponseMessage<TEntity>(true, value, null);
        }

        public static ResponseMessage<TEntity> Fault(IList<string> error)
        {
            return new ResponseMessage<TEntity>(false, default(TEntity), error);
        }
    }
}
