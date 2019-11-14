using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Repository
{
    public class HistoryRepository : BaseRepository, IHistoryRepository
    {
        public HistoryRepository(IDbConnection connection)
            : base(connection) { }

        public bool AddHistory(History history)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdIncidentAssessment", history.incidentAssessment.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@Operation", history.Operation, DbType.Int32, ParameterDirection.Input);
                param.Add("@Data", history.Date, DbType.DateTime, ParameterDirection.Input);
                param.Add("@Object", history.ObjectJson, DbType.String, ParameterDirection.Input);
                param.Add("@IdUser", history?.User?.Id, DbType.String, ParameterDirection.Input);
                param.Add("@Login", history?.User?.Login, DbType.String, ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO [History]
                                               ([IdIncidentAssessment]
                                               ,[Operation]
                                               ,[IdUser]
                                               ,[Login]
                                               ,[Data]
                                               ,[Object])
                                         VALUES
                                               (@IdIncidentAssessment
                                               ,@Operation
                                               ,@IdUser
                                               ,@Login
                                               ,@Data
                                               ,@Object) ";

                return this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
