using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository
{
    public class IncidentAssessmentRepository
                        : BaseRepository, Interface.IIncidentAssessmentRepository
    {
        public IncidentAssessmentRepository(IDbConnection conexao)
            : base(conexao)
        {
        }

        public bool AddAnswers(IncidentAssessment incidentAssessment, Question question)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdIncidentAssessment", incidentAssessment.Id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@IdQuestion", question.Id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@Answer", question.Answer, System.Data.DbType.Boolean, System.Data.ParameterDirection.Input);
                param.Add("@CreateDate", DateTime.Now, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO [IncidentAssessmentAnswer]
                                                   (
                                                     [IdIncidentAssessment],
                                                     [IdQuestion],
                                                     [Answer],
                                                     [CreateDate]
                                                   )
                                             VALUES
                                                   (
                                                     @IdIncidentAssessment,
                                                     @IdQuestion,
                                                     @Answer,
                                                     @CreateDate
                                                   )";

                return this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AddImage(IncidentAssessment incidentAssessment, IncidentAssessmentImage inicidentImage)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdIncidentAssessment", incidentAssessment.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@Name", inicidentImage?.Name, DbType.String, ParameterDirection.Input, 200);
                param.Add("@MimeType", inicidentImage?.MimeType, DbType.String, ParameterDirection.Input, 50);
                param.Add("@Image", inicidentImage?.Image, DbType.Binary, ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO IncidentAssessmentImage 
                                            (IdIncidentAssessment, Name, MimeType, Image)                                           
                                        VALUES 
                                            (@IdIncidentAssessment, @Name, @MimeType, @Image) ";

                return this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Create(IncidentAssessment incidentAssessment)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdCompany", incidentAssessment?.Configuration?.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@LicensePlate", incidentAssessment?.LicensePlate, DbType.String, ParameterDirection.Input, 10);
                param.Add("@ClaimNumber", incidentAssessment?.ClaimNumber, DbType.String, ParameterDirection.Input, 20);
                param.Add("@InsuredName", incidentAssessment?.InsuredName, DbType.String, ParameterDirection.Input, 100);
                param.Add("@InsuredFone", incidentAssessment?.InsuredFone, DbType.String, ParameterDirection.Input, 20);
                param.Add("@Provider", incidentAssessment?.Provider, DbType.String, ParameterDirection.Input, 100);
                param.Add("@WorkProvider", incidentAssessment?.WorkProvider, DbType.String, ParameterDirection.Input, 100);
                param.Add("@WorkProviderFone", incidentAssessment?.WorkProviderFone, DbType.String, ParameterDirection.Input, 20);
                param.Add("@ShortMessageCode", (Int32)incidentAssessment?.ShortMessageCode, DbType.Int32, ParameterDirection.Input);
                param.Add("@Type", (Int32)incidentAssessment?.Type, DbType.Int32, ParameterDirection.Input);
                param.Add("@Status", (Int32)incidentAssessment?.Status, DbType.Int32, ParameterDirection.Input);
                param.Add("@CreateDate", DateTime.Now, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO IncidentAssessment 
                                            (IdCompany, LicensePlate, ClaimNumber, InsuredName, InsuredFone, Provider, WorkProvider, WorkProviderFone, ShortMessageCode, Type, CreateDate, Status) 
                                        OUTPUT INSERTED.IDINCIDENTASSESSMENT    
                                        VALUES 
                                            (@IdCompany, @LicensePlate, @ClaimNumber, @InsuredName, @InsuredFone, @Provider, @WorkProvider, @WorkProviderFone, @ShortMessageCode, @Type, @CreateDate, @Status) ";

                incidentAssessment.Id = this.Conexao.QuerySingle<int>(sqlStatement, param: param, transaction: this.Transacao);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteAnswers(int idIncidentAssessment)
        {
            try
            {
                var @param = new { id = idIncidentAssessment };
                return this.Conexao
                           .Execute("DELETE FROM IncidentAssessmentAnswer WHERE IdIncidentAssessment = @id",
                                param: param,
                                transaction: this.Transacao) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IncidentAssessment Find(int idIncidentAssessment)
        {
            try
            {
                var @param = new { id = idIncidentAssessment };
                var categoryDictionary = new Dictionary<int, IncidentAssessment>();

                IncidentAssessment _IncidentAssessment = this.Conexao
                                                             .Query<IncidentAssessment, Configuration, IncidentAssessment>
                                                             (
                                                                @"SELECT 
                                                                         I.[IDINCIDENTASSESSMENT]       ID
                                                                        ,I.[LICENSEPLATE]               LICENSEPLATE
                                                                        ,I.[CLAIMNUMBER]                CLAIMNUMBER
                                                                        ,I.[INSUREDNAME]                INSUREDNAME
                                                                        ,I.[INSUREDFONE]                INSUREDFONE
                                                                        ,I.[PROVIDER]                   PROVIDER
                                                                        ,I.[WORKPROVIDER]               WORKPROVIDER
                                                                        ,I.[WORKPROVIDERFONE]           WORKPROVIDERFONE
                                                                        ,I.[TYPE]                       TYPE
                                                                        ,I.[STATUS]                     STATUS
                                                                        ,I.[TOTALPOINT]                 TOTALPOINT
                                                                        ,I.[SHORTMESSAGECODE]           SHORTMESSAGECODE
                                                                        ,I.[IDCOMPANY]                  IDCONFIGURATION                                                                        
                                                                        ,C.[IDCOMPANY]                  ID
                                                                        ,C.[NAME]                       NAME
                                                                        ,C.[REGISTRATIONNUMBER]         CNPJ
                                                                        ,C.[PRIMARYCOLOR]               PRIMARYCOLOR
                                                                        ,C.[SECONDARYCOLOR]             SECONDARYCOLOR
                                                                        ,C.[LOGO]                       IMAGE
                                                                        ,C.[LIMITTOTALLOSS]             LIMITTOTALLOSS
                                                                    FROM [INCIDENTASSESSMENT]  I
                                                                    INNER JOIN [COMPANY] C
	                                                                   ON I.IDCOMPANY = C.IDCOMPANY
                                                                    WHERE 
	                                                                      I.IDINCIDENTASSESSMENT = @id ",
                                                                (incidentAssessment, configuration) =>
                                                                {
                                                                    incidentAssessment.Configuration = configuration;
                                                                    return incidentAssessment;
                                                                },
                                                                splitOn: "IDCONFIGURATION",
                                                                param: param,
                                                                transaction: this.Transacao
                                                             )
                                                            .Distinct()
                                                            .FirstOrDefault();

                return _IncidentAssessment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Category> GetAnswers(int idIncidentAssessment)
        {
            try
            {
                var @param = new { id = idIncidentAssessment };
                var categoryDictionary = new Dictionary<int, Category>();

                IList<Category> _category = this.Conexao
                                                .Query<Category, Question, Category>(
                                                        @"SELECT 
	                                                            C.idCategory        Id,
	                                                            C.Label             Label,
	                                                            C.Image             Image,
	                                                            C.Point             Point,
                                                                Q.idQuestion        ID_QUESTION,
	                                                            Q.idQuestion        Id,
	                                                            Q.Label             Label,
	                                                            Q.QuestionType      Type,
	                                                            Q.Point             Point,
	                                                            A.Answer            Answer
                                                            FROM IncidentAssessment I
                                                            INNER JOIN IncidentAssessmentAnswer A
	                                                            ON I.IdIncidentAssessment = A.IdIncidentAssessment
                                                            INNER JOIN Question Q
	                                                            ON Q.idQuestion = A.IdQuestion
                                                            INNER JOIN Category C
	                                                            ON Q.idCategory = C.idCategory
                                                            WHERE
	                                                            I.IdIncidentAssessment = @id",
                                                        (category, question) =>
                                                        {
                                                            Category categoryEntry;

                                                            if (!categoryDictionary.TryGetValue(category.Id, out categoryEntry))
                                                            {
                                                                categoryEntry = category;
                                                                categoryEntry.Questions = new List<Question>();
                                                                categoryDictionary.Add(categoryEntry.Id, categoryEntry);
                                                            }

                                                            categoryEntry.Questions.Add(question);
                                                            return categoryEntry;
                                                        },
                                                        splitOn: "ID_QUESTION",
                                                        param: param,
                                                        transaction: this.Transacao)
                                                        .Distinct()
                                                        .ToList();


                return _category;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IncidentAssessment> ListByConfiguration(Configuration configuration)
        {
            try
            {
                var @param = new { id = configuration.Id };
                var categoryDictionary = new Dictionary<int, IncidentAssessment>();

                IList<IncidentAssessment> _IncidentAssessment = this.Conexao
                                                             .Query<IncidentAssessment, Configuration, IncidentAssessment>
                                                             (
                                                                @"SELECT 
                                                                        I.[IDINCIDENTASSESSMENT]       ID
                                                                       ,I.[LICENSEPLATE]               LICENSEPLATE
                                                                       ,I.[CLAIMNUMBER]                CLAIMNUMBER
                                                                       ,I.[INSUREDNAME]                INSUREDNAME
                                                                       ,I.[INSUREDFONE]                INSUREDFONE
                                                                       ,I.[PROVIDER]                   PROVIDER
                                                                       ,I.[WORKPROVIDER]               WORKPROVIDER
                                                                       ,I.[WORKPROVIDERFONE]           WORKPROVIDERFONE
                                                                       ,I.[TYPE]                       TYPE
                                                                       ,I.[STATUS]                     STATUS
                                                                       ,I.[TOTALPOINT]                 TOTALPOINT
                                                                       ,I.[IDCOMPANY]                  IDCONFIGURATION                                                                       
                                                                       ,C.[IDCOMPANY]                  ID
                                                                       ,C.[NAME]                       NAME
                                                                       ,C.[REGISTRATIONNUMBER]         CNPJ
                                                                       ,C.[PRIMARYCOLOR]               PRIMARYCOLOR
                                                                       ,C.[SECONDARYCOLOR]             SECONDARYCOLOR
                                                                       ,C.[LOGO]                       IMAGE
                                                                       ,C.[LIMITTOTALLOSS]             LIMITTOTALLOSS
                                                                  FROM [INCIDENTASSESSMENT]  I
                                                                  INNER JOIN [COMPANY] C
	                                                                 ON I.IDCOMPANY = C.IDCOMPANY
                                                                  WHERE C.[IDCOMPANY] = 1 @id ",
                                                                (_incidentAssessment, _configuration) =>
                                                                {
                                                                    _incidentAssessment.Configuration = configuration;
                                                                    return _incidentAssessment;
                                                                },
                                                                splitOn: "IDCONFIGURATION",
                                                                param: param,
                                                                transaction: this.Transacao
                                                             )
                                                            .Distinct()
                                                            .ToList();


                return _IncidentAssessment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(IncidentAssessment incidentAssessment)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdIncidentAssessment", incidentAssessment?.Id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                param.Add("@IdCompany", incidentAssessment?.Configuration.Id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@LicensePlate", incidentAssessment?.LicensePlate, System.Data.DbType.String, System.Data.ParameterDirection.Input, 10);
                param.Add("@ClaimNumber", incidentAssessment?.ClaimNumber, System.Data.DbType.String, System.Data.ParameterDirection.Input, 20);
                param.Add("@InsuredName", incidentAssessment?.InsuredName, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                param.Add("@InsuredFone", incidentAssessment?.InsuredFone, System.Data.DbType.String, System.Data.ParameterDirection.Input, 20);
                param.Add("@Provider", incidentAssessment?.Provider, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                param.Add("@WorkProvider", incidentAssessment?.WorkProvider, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                param.Add("@WorkProviderFone", incidentAssessment?.WorkProviderFone, System.Data.DbType.String, System.Data.ParameterDirection.Input, 20);
                param.Add("@ShortMessageCode", incidentAssessment?.ShortMessageCode, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@Type", incidentAssessment?.Type, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@Status", (Int32)incidentAssessment?.Status, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@TotalPoint", incidentAssessment?.TotalPoint, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@UpdateDate", DateTime.Now, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);

                string sqlStatement = @"UPDATE IncidentAssessment 
                                           SET 
                                              IdCompany        = ISNULL(@IdCompany, IdCompany),      
                                              LicensePlate     = ISNULL(@LicensePlate, LicensePlate),
                                              ClaimNumber      = ISNULL(@ClaimNumber , ClaimNumber),
                                              InsuredName      = ISNULL(@InsuredName , InsuredName),
                                              InsuredFone      = ISNULL(@InsuredFone , InsuredFone),
                                              Provider         = ISNULL(@Provider, Provider),
                                              WorkProvider     = ISNULL(@WorkProvider, WorkProvider),
                                              WorkProviderFone = ISNULL(@WorkProviderFone, WorkProviderFone),
                                              ShortMessageCode = ISNULL(@ShortMessageCode, ShortMessageCode),
                                              Type             = ISNULL(@Type, Type),
                                              Status           = ISNULL(@Status, Status),
                                              TotalPoint       = ISNULL(@TotalPoint, TotalPoint),
                                              UpdateDate       = ISNULL(@UpdateDate, UpdateDate)                                              
                                         WHERE 
                                              IdIncidentAssessment = @IdIncidentAssessment";

                this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
