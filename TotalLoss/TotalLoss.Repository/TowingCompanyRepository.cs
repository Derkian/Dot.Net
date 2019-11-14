using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Repository
{
    public class TowingCompanyRepository : BaseRepository, Interface.ITowingCompanyRepository
    {
        public TowingCompanyRepository(IDbConnection connection)
            : base(connection) { }

        public void Create(TowingCompany towingCompany)
        {
            try
            {
                var paramCompany = new DynamicParameters();
                var paramTowingCompany = new DynamicParameters();

                paramCompany.Add("@Name", towingCompany?.Name, DbType.String, ParameterDirection.Input, 200);
                paramCompany.Add("@RegistrationNumber", towingCompany?.CNPJ, DbType.String, ParameterDirection.Input, 20);
                paramCompany.Add("@IdTypeCompany", towingCompany?.TypeCompany, DbType.Int32, ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO Company 
                                            (Name, RegistrationNumber, IdTypeCompany) 
                                        OUTPUT INSERTED.IDCOMPANY    
                                        VALUES 
                                            (@Name, @RegistrationNumber, @IdTypeCompany) ";

                // Insere dados da Companhia base 
                towingCompany.Id = this.Conexao.QuerySingle<int>(sqlStatement, param: paramCompany, transaction: this.Transacao);

                paramTowingCompany.Add("@IdTowingCompany", towingCompany?.Id, DbType.Int32, ParameterDirection.Input);
                paramTowingCompany.Add("@IdInsuranceCompany", towingCompany?.IdInsuranceCompany, DbType.Int32, ParameterDirection.Input);
                paramTowingCompany.Add("@Description", towingCompany?.Description, DbType.String, ParameterDirection.Input, 100);
                paramTowingCompany.Add("@Email", towingCompany?.Email, DbType.String, ParameterDirection.Input, 100);
                paramTowingCompany.Add("@Enable", towingCompany?.Enable, DbType.Boolean, ParameterDirection.Input);
                
                sqlStatement = @"INSERT INTO TowingCompany 
                                    (IdTowingCompany,IdInsuranceCompany, Description, Email, Enable) 
                                OUTPUT INSERTED.IDTOWINGCOMPANY    
                                VALUES 
                                    (@IdTowingCompany, @IdInsuranceCompany, @Description, @Email, @Enable) ";

                // Insere dados da Empresa Guincho usando chave da Companhia base gerada
                this.Conexao.QuerySingle(sqlStatement, param: paramTowingCompany, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TowingCompany Find(TowingCompany towingCompany)
        {
            try
            {
                var @param = new { id = towingCompany.Id, email = towingCompany.Email };

                TowingCompany _towingCompany = this.Conexao
                                                   .QueryFirstOrDefault<TowingCompany>(
                                                                        @"SELECT 
                                                                                CMP.[IDCOMPANY]           ID, 
                                                                                CMP.[NAME]                NAME,
                                                                                CMP.[REGISTRATIONNUMBER]  CNPJ,
                                                                                CMP.[IDTYPECOMPANY]       TYPECOMPANY,
                                                                                TOW.[DESCRIPTION]         DESCRIPTION,
                                                                                TOW.[EMAIL]               EMAIL,
                                                                                TOW.[ENABLE]              ENABLE
                                                                        FROM [COMPANY] CMP WITH(NOLOCK)
                                                                        INNER JOIN TOWINGCOMPANY TOW WITH(NOLOCK)
                                                                           ON TOW.IDTOWINGCOMPANY = CMP.IDCOMPANY
                                                                        WHERE TOW.IDTOWINGCOMPANY = ISNULL(@id, TOW.IDTOWINGCOMPANY)
                                                                          AND TOW.EMAIL = ISNULL(@email, TOW.EMAIL)",
                                                                        param: param, 
                                                                        transaction: this.Transacao);

                return _towingCompany;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<TowingCompany> ListByInsuranceCompany(Company insuranceCompany)
        {
            try
            {
                var @parameters = new { id = insuranceCompany.Id };

                IList<TowingCompany> listTowingCompany = this.Conexao
                                                             .Query<TowingCompany>
                                                             (
                                                                 @"SELECT 
                                                                         CMP.[IDCOMPANY]           ID, 
                                                                         CMP.[NAME]                NAME,
                                                                         CMP.[REGISTRATIONNUMBER]  CNPJ,
                                                                         CMP.[IDTYPECOMPANY]       TYPECOMPANY,
                                                                         TOW.[DESCRIPTION]         DESCRIPTION,
                                                                         TOW.[EMAIL]               EMAIL,
                                                                         TOW.[ENABLE]              ENABLE
                                                                    FROM [COMPANY] CMP WITH(NOLOCK)
                                                                   INNER JOIN TOWINGCOMPANY TOW WITH(NOLOCK)
                                                                      ON TOW.IDTOWINGCOMPANY = CMP.IDCOMPANY                            
                                                                   INNER JOIN [INSURANCECOMPANY] INS WITH(NOLOCK)
                                                                      ON TOW.IDINSURANCECOMPANY = INS.IDINSURANCECOMPANY
                                                                   WHERE INS.IDINSURANCECOMPANY = @id",
                                                                 param: parameters
                                                             )
                                                             .Distinct()
                                                             .ToList();

                return listTowingCompany;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(TowingCompany towingCompany)
        {
            try
            {
                var paramCompany = new DynamicParameters();
                var paramTowingCompany = new DynamicParameters();

                paramCompany.Add("@IdCompany", towingCompany?.Id, DbType.Int32, ParameterDirection.Input);
                paramCompany.Add("@Name", towingCompany?.Name, DbType.String, ParameterDirection.Input, 200);
                paramCompany.Add("@RegistrationNumber", towingCompany?.CNPJ, DbType.String, ParameterDirection.Input, 20);

                string sqlStatement = @"UPDATE Company 
                                           SET
                                               [Name] = ISNULL(@Name, [Name]),
                                               RegistrationNumber = ISNULL(@RegistrationNumber, RegistrationNumber)                                      
                                         WHERE 
                                              IdCompany = @IdCompany";

                // Atualiza dados da Companhia base 
                this.Conexao.Execute(sqlStatement, param: paramCompany, transaction: this.Transacao);

                // Inizializa lista de Parâmetros
                paramTowingCompany.Add("@IdTowingCompany", towingCompany?.Id, DbType.Int32, ParameterDirection.Input);
                paramTowingCompany.Add("@Description", towingCompany?.Description, DbType.String, ParameterDirection.Input, 100);
                paramTowingCompany.Add("@Email", towingCompany?.Email, DbType.String, ParameterDirection.Input, 100);
                paramTowingCompany.Add("@Enable", towingCompany?.Enable, DbType.Boolean, ParameterDirection.Input);

                sqlStatement = @"UPDATE TowingCompany 
                                    SET
                                        Description = ISNULL(@Description, Description),
                                        Email       = ISNULL(@Email , Email),
                                        Enable      = ISNULL(@Enable, Enable)                                         
                                    WHERE 
                                        IdTowingCompany = @IdTowingCompany";

                // Atualiza dados da Empresa Guincho 
                this.Conexao.Execute(sqlStatement, param: paramTowingCompany, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
