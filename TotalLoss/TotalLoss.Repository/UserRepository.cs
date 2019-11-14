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
    public class UserRepository
                       : BaseRepository, Interface.IUserRepository
    {
        public UserRepository(IDbConnection connection)
            : base(connection)
        {
        }

        public void Create(User user)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("@IdCompany", user?.Company.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@Name", user?.Name, DbType.String, ParameterDirection.Input, 200);
                param.Add("@Login", user?.Login, DbType.String, ParameterDirection.Input, 100);
                param.Add("@Password", user?.Password, DbType.String, ParameterDirection.Input, 100);
                param.Add("@Status", user?.Status, DbType.Boolean, ParameterDirection.Input);
                
                string sqlStatement = @"INSERT INTO [User] 
                                            (IdCompany, Name, Login, Password, Status) 
                                        OUTPUT INSERTED.IDUSER    
                                        VALUES 
                                            (@IdCompany, @Name, @Login, @Password, @Status) ";

                // Insere dados do Usuário 
                user.Id = this.Conexao.QuerySingle<int>(sqlStatement, param: param, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User Find(string login, string password)
        {
            try
            {
                var @parameters = new { login, password };

                User _user = this.Conexao
                                 .Query<User, Company, User>
                                (
                                @"SELECT U.[IDUSER]	            ID	                                            
                                        ,U.[NAME]               NAME
                                        ,U.[STATUS]	            STATUS
                                        ,U.[LOGIN]              LOGIN
                                        ,U.[IDCOMPANY]          IDCOMPANY                                                
                                        ,C.[IDCOMPANY]          ID
                                        ,C.[NAME]               NAME
                                        ,C.[REGISTRATIONNUMBER] CNPJ
                                        ,C.[IDTYPECOMPANY]	    TYPECOMPANY                                                
                                   FROM [USER] U WITH(NOLOCK)
                                  INNER JOIN [COMPANY] C WITH(NOLOCK)
                                     ON U.IDCOMPANY = C.IDCOMPANY
                                  WHERE U.LOGIN    = ISNULL(@login, U.LOGIN)
                                    AND U.PASSWORD = ISNULL(@password, U.PASSWORD)",
                                (user, company) =>
                                {
                                    user.Company = company;
                                    return user;
                                },
                                splitOn: "IDCOMPANY",
                                param: @parameters,
                                transaction: this.Transacao
                                )
                            .Distinct()
                            .FirstOrDefault();

                return _user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
