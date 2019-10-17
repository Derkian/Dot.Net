using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository
{
    public class ConfigurationRepository 
                    : BaseRepository, Interface.IConfigurationRepository
    {
        public ConfigurationRepository(IDbConnection conexao)
            : base(conexao)
        {
        }

        public Configuration Find(int id)
        {
            try
            {
                var @param = new { id };                

                Configuration _company = this.Conexao
                                                .QueryFirst<Configuration>(
                                                        @"SELECT 
                                                                IDCOMPANY        ID, 
                                                                NAME             NAME,
                                                                LIMITTOTALLOSS   LIMITTOTALLOSS                                                                                                                               
					                                      FROM [COMPANY]
					                                     WHERE IDCOMPANY = @id"
                                                        ,
                                                        param: param, transaction: this.Transacao);


                return _company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Configuration FindByAuthenticatedCompany(string login, string password)
        {
            try
            {
                var @parameters = new { login , password };

                Configuration _configuration = this.Conexao
                                                        .QueryFirstOrDefault<Configuration>
                                                        (
                                                            @"SELECT 
	                                                            C.IDCOMPANY				ID,
	                                                            C.[NAME]				NAME,
	                                                            C.REGISTRATIONNUMBER	CNPJ,
	                                                            C.PRIMARYCOLOR			PRIMARYCOLOR,
	                                                            C.SECONDARYCOLOR		SECONDARYCOLOR,
	                                                            CAST(C.LOGO AS VARCHAR(MAX)) LOGO     
                                                            FROM COMPANY C
                                                            INNER JOIN [USER] U
	                                                            ON U.IDCOMPANY = C.IDCOMPANY
					                                        WHERE U.LOGIN = @login
                                                              AND U.PASSWORD = @password"
                                                            ,
                                                            param: parameters, transaction: this.Transacao
                                                        );
                
                return _configuration;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
