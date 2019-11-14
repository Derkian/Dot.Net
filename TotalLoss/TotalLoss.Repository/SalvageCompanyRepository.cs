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
    public class SalvageCompanyRepository
        : BaseRepository, Interface.ISalvageCompanyRepository
    {
        public SalvageCompanyRepository(IDbConnection connection) 
            : base(connection)
        {
        }

        public IList<SalvageCompany> ListSalvageByCompany(Company company)
        {
            try
            {
                var @parameters = new { idConfiguration = company.Id };

                IList<SalvageCompany> listSalvage = this.Conexao
                                                   .Query<SalvageCompany>
                                                   (
                                                        @"SELECT 
                                                               S.IDSALVAGE			ID,
	                                                           S.NAME				NAME,
	                                                           S.REGISTRATIONNUMBER	REGISTRATIONNUMBER
                                                        FROM SALVAGE S
                                                        INNER JOIN SALVAGECOMPANY SC
	                                                        ON S.IDSALVAGE = SC.IDSALVAGE
                                                        INNER JOIN INSURANCECOMPANY C
	                                                        ON SC.IDINSURANCECOMPANY = C.IDINSURANCECOMPANY
	                                                    WHERE    C.IDINSURANCECOMPANY = @idConfiguration",
                                                        param: parameters
                                                   )
                                                   .Distinct()
                                                   .ToList();

                return listSalvage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Location> ListSalvageLocation(SalvageCompany salvage)
        {
            try
            {
                var @param = new { id = salvage.Id };
                var salvageDictionary = new Dictionary<int, Location>();

                IList<Location> _salvageLocation = this.Conexao
                                                             .Query<Location, LocationDetail, Location>
                                                             (
                                                                @"SELECT 
                                                                        L.IDLOCATION		ID,
	                                                                    L.STATE				STATE,
	                                                                    L.CITY				CITY,
	                                                                    L.EMAIL				EMAIL,
	                                                                    D.IDLOCATIONDETAIL	IDLOCATIONDETAIL,
	                                                                    D.IDLOCATIONDETAIL	ID,
	                                                                    D.STREET			STREET,
	                                                                    D.DISTRICT			DISTRICT,
	                                                                    D.ZIPCODE			ZIPCODE,
	                                                                    D.EMAIL				EMAIL,
	                                                                    D.FONE				FONE,
	                                                                    D.FAX				FAX,
                                                                        D.MANAGER           MANAGER,
                                                                        D.LATITUDE          LATITUDE,
                                                                        D.LONGITUDE         LONGITUDE
                                                                    FROM [SALVAGE] S
                                                                    INNER JOIN [LOCATION]  L 
	                                                                    ON S.IDSALVAGE = L.IDSALVAGE
                                                                    INNER JOIN [LOCATIONDETAIL] D
	                                                                    ON L.IDLOCATION = D.IDLOCATION
                                                                    WHERE S.IDSALVAGE = @ID ",
                                                                (salvageLocation, salvageLocationDetail) =>
                                                                {
                                                                    Location orderSalvageLocation;

                                                                    if (!salvageDictionary.TryGetValue(salvageLocation.Id, out orderSalvageLocation))
                                                                    {
                                                                        orderSalvageLocation = salvageLocation;
                                                                        orderSalvageLocation.Detail = new List<LocationDetail>();
                                                                        salvageDictionary.Add(orderSalvageLocation.Id, orderSalvageLocation);
                                                                    }

                                                                    orderSalvageLocation.Detail.Add(salvageLocationDetail);
                                                                    
                                                                    return orderSalvageLocation;
                                                                },
                                                                splitOn: "IDLOCATIONDETAIL",
                                                                param: param,
                                                                transaction: this.Transacao
                                                             )
                                                            .Distinct()
                                                            .ToList();

                return _salvageLocation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
