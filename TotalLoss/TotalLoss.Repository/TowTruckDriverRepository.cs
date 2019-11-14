using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository
{
    public class TowTruckDriverRepository : BaseRepository, Interface.ITowTruckDriverRepository
    {
        public TowTruckDriverRepository(IDbConnection connection)
            : base(connection) { }

        public void Create(TowTruckDriver towTruckDriver)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("@IdTowingCompany", towTruckDriver?.IdTowingCompany, DbType.Int32, ParameterDirection.Input);
                param.Add("@Name", towTruckDriver?.Name, DbType.String, ParameterDirection.Input, 100);
                param.Add("@Mobile", towTruckDriver?.Mobile, DbType.String, ParameterDirection.Input, 20);                

                string sqlStatement = @"INSERT INTO TOWTRUCKDRIVER 
                                            (IdTowingCompany, Name, Mobile) 
                                        OUTPUT INSERTED.IDTOWTRUCKDRIVER    
                                        VALUES 
                                            (@IdTowingCompany, @Name, @Mobile) ";

                towTruckDriver.Id = this.Conexao.QuerySingle<int>(sqlStatement, param: param, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(int id)
        {
            try
            {
                var @param = new { id };

                this.Conexao.Execute
                (
                    "UPDATE TOWTRUCKDRIVER SET ENABLE = 0 WHERE IDTOWTRUCKDRIVER = @id",
                    param: param,
                    transaction: this.Transacao
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TowTruckDriver Find(int id)
        {
            try
            {
                var @param = new { id };

                TowTruckDriver _towTruckDriver = this.Conexao
                                                     .QueryFirstOrDefault<TowTruckDriver>(
                                                                            @"SELECT 
                                                                                    DRV.[IDTOWTRUCKDRIVER] ID, 
                                                                                    DRV.[NAME]             NAME,
                                                                                    DRV.[MOBILE]		   MOBILE,
                                                                                    DRV.[ENABLE]           ENABLE,
                                                                                    TOW.[IDTOWINGCOMPANY]  IDTOWINGCOMPANY                                                                                    
                                                                               FROM [TOWTRUCKDRIVER] DRV WITH(NOLOCK)
                                                                              INNER JOIN TOWINGCOMPANY TOW WITH(NOLOCK)
                                                                                 ON DRV.IDTOWINGCOMPANY = TOW.IDTOWINGCOMPANY
                                                                              WHERE DRV.IDTOWTRUCKDRIVER = @id"
                                                                            ,
                                                                            param: param, transaction: this.Transacao);

                return _towTruckDriver;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<TowTruckDriver> ListByTowingCompany(Company towingCompany)
        {
            try
            {
                var @parameters = new { id = towingCompany.Id };

                IList<TowTruckDriver> listTowTruckDriver = this.Conexao
                                                              .Query<TowTruckDriver>
                                                              (
                                                                 @"SELECT 
                                                                         DRV.[IDTOWTRUCKDRIVER] ID, 
                                                                         DRV.[NAME]             NAME,
                                                                         DRV.[MOBILE]		    MOBILE,
                                                                         DRV.[IDTOWINGCOMPANY]	IDTOWINGCOMPANY,
                                                                         DRV.[ENABLE]	        ENABLE
                                                                    FROM [TOWTRUCKDRIVER] DRV WITH(NOLOCK)
                                                                   INNER JOIN TOWINGCOMPANY TOW WITH(NOLOCK)
                                                                      ON DRV.IDTOWINGCOMPANY = TOW.IDTOWINGCOMPANY
                                                                   WHERE DRV.IDTOWINGCOMPANY = @id",
                                                                 param: parameters
                                                              )
                                                              .Distinct()
                                                              .ToList();

                return listTowTruckDriver;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(TowTruckDriver towTruckDriver)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("@IdTowTruckDriver", towTruckDriver?.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@Name", towTruckDriver?.Name, DbType.String, ParameterDirection.Input, 100);
                param.Add("@Mobile", towTruckDriver?.Mobile, DbType.String, ParameterDirection.Input, 20);
                param.Add("@Enable", towTruckDriver?.Enable, DbType.String, ParameterDirection.Input, 20);

                string sqlStatement = @"UPDATE TOWTRUCKDRIVER
                                           SET
                                               [Name]   = ISNULL(@Name, [Name]),
                                               [Mobile] = ISNULL(@Mobile, Mobile),
                                               [Enable] = ISNULL(@Enable, Enable)
                                         WHERE 
                                              IdTowTruckDriver = @IdTowTruckDriver";

                this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
