using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Repository
{
    public class BaseRepository : IBaseRepository
    {
        private IDbTransaction Transaction;
        private IDbConnection _Conexao;
        protected IDbConnection Conexao { get { return this._Conexao; } }
        
        public BaseRepository(IDbConnection connection)
        {
            this._Conexao = connection;
        }

        public IDbTransaction Transacao
        {
            set
            {
                this.Transaction = value;
            }
            get
            {
                return this.Transaction;
            }
        }
    }
}
