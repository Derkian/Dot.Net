using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository
{
    public class ConfigurationRepository
                    : BaseRepository, Interface.IConfigurationRepository
    {
        public ConfigurationRepository(IDbConnection connection)
            : base(connection) { }

        public InsuranceCompany Find(Company company)
        {
            try
            {
                var @param = new { id = company.Id, idTypeCompany = company.TypeCompany };

                InsuranceCompany _company = this.Conexao
                                                .QueryFirstOrDefault<InsuranceCompany>(
                                                        @"SELECT 
                                                                CMP.IDCOMPANY           ID, 
                                                                CMP.NAME                NAME,
                                                                CMP.REGISTRATIONNUMBER  CNPJ,
                                                                CMP.IDTYPECOMPANY       TYPECOMPANY,
                                                                INS.DESCRIPTION         DESCRIPTION,
                                                                INS.LIMITTOTALLOSS      LIMITTOTALLOSS,
                                                                INS.PRIMARYCOLOR        PRIMARYCOLOR,
                                                                INS.SECONDARYCOLOR      SECONDARYCOLOR,
                                                                INS.LOGO                IMAGE
                                                            FROM [COMPANY] CMP WITH(NOLOCK)
                                                           INNER JOIN INSURANCECOMPANY INS WITH(NOLOCK)
                                                              ON CMP.IDCOMPANY = INS.IDINSURANCECOMPANY
                                                           WHERE CMP.IDCOMPANY = @id
                                                             AND CMP.IDTYPECOMPANY = @idTypeCompany"
                                                        ,
                                                        param: param, transaction: this.Transacao);

                return _company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InsuranceCompany FindByTowingCompany(Company company)
        {
            try
            {
                var @param = new { id = company.Id, idTypeCompany = company.TypeCompany };

                InsuranceCompany _company = this.Conexao
                                                .QueryFirstOrDefault<InsuranceCompany>(
                                                        @"SELECT 
		                                                        CMP.IDCOMPANY           ID, 
		                                                        CMP.NAME                NAME,
		                                                        CMP.REGISTRATIONNUMBER  CNPJ,
		                                                        CMP.IDTYPECOMPANY       TYPECOMPANY,
		                                                        INS.DESCRIPTION         DESCRIPTION,
		                                                        INS.LIMITTOTALLOSS      LIMITTOTALLOSS,
		                                                        INS.PRIMARYCOLOR        PRIMARYCOLOR,
		                                                        INS.SECONDARYCOLOR      SECONDARYCOLOR,
		                                                        INS.LOGO                IMAGE
                                                            FROM [COMPANY] CMP WITH(NOLOCK)
                                                        INNER JOIN [dbo].[TowingCompany] TOW WITH(NOLOCK)
                                                           ON CMP.IDCOMPANY = TOW.IdTowingCompany
                                                        INNER JOIN [INSURANCECOMPANY] INS WITH(NOLOCK)
                                                           ON TOW.IdInsuranceCompany = INS.IdInsuranceCompany
                                                        WHERE CMP.IDCOMPANY = @id
                                                          AND CMP.IDTYPECOMPANY = @idTypeCompany"
                                                        ,
                                                        param: param, transaction: this.Transacao);

                return _company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
