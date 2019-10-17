using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace TotalLoss.Repository
{
    public class BaseRepository
    {
        protected IDbConnection Conexao { get; }
        protected IDbTransaction Transacao { get; set; }

        public BaseRepository(IDbConnection conexao)
        {
            Conexao = conexao;            
        }

        public void BeginTransaction()
        {
            if (this.Conexao.State == ConnectionState.Closed)
                this.Conexao.Open();

            this.Transacao = this.Conexao.BeginTransaction();

        }

        public void Commit()
        {
            this.Transacao.Commit();

            if (this.Conexao.State == ConnectionState.Open)
                this.Conexao.Close();

        }

        public void RollBack()
        {
            this.Transacao.Rollback();

            if (this.Conexao.State == ConnectionState.Open)
                this.Conexao.Close();
        }
    }
}
