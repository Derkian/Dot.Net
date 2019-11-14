using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository
{
    public class IncidentAssessmentRepository
                        : BaseRepository, Interface.IIncidentAssessmentRepository
    {
        #region Construtor
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="connection"></param>
        public IncidentAssessmentRepository(IDbConnection connection)
            : base(connection) { }
        #endregion

        #region Incident
        /// <summary>
        /// Cria um incident
        /// </summary>
        /// <param name="incidentAssessment"></param>
        public void Create(IncidentAssessment incidentAssessment)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdInsuranceCompany", incidentAssessment?.IdInsuranceCompany, DbType.Int32, ParameterDirection.Input);
                param.Add("@LicensePlate", incidentAssessment?.LicensePlate, DbType.String, ParameterDirection.Input, 10);
                param.Add("@ClaimNumber", incidentAssessment?.ClaimNumber, DbType.String, ParameterDirection.Input, 20);
                param.Add("@InsuredName", incidentAssessment?.InsuredName, DbType.String, ParameterDirection.Input, 100);
                param.Add("@InsuredPhone", incidentAssessment?.InsuredPhone, DbType.String, ParameterDirection.Input, 20);
                param.Add("@IdTowingCompany", incidentAssessment?.TowingCompany?.Id == 0 ? null : incidentAssessment?.TowingCompany?.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@IdTowTruckDriver", incidentAssessment?.TowTruckDriver?.Id == 0 ? null : incidentAssessment?.TowTruckDriver?.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@TowTruckDriverName", incidentAssessment?.TowTruckDriverName, DbType.String, ParameterDirection.Input, 100);
                param.Add("@TowTruckDriverMobile", incidentAssessment?.TowTruckDriverMobile, DbType.String, ParameterDirection.Input, 20);
                param.Add("@ShortMessageCode", (Int32)incidentAssessment?.ShortMessageCode, DbType.Int32, ParameterDirection.Input);
                param.Add("@IdIncidentType", (Int32)incidentAssessment?.Type, DbType.Int32, ParameterDirection.Input);
                param.Add("@CreateDate", DateTime.Now, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                param.Add("@Status", (Int32)incidentAssessment?.Status, DbType.Int32, ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO IncidentAssessment 
                                            (IdInsuranceCompany, LicensePlate, ClaimNumber, InsuredName, InsuredPhone, IdTowingCompany, IdTowTruckDriver, TowTruckDriverName, TowTruckDriverMobile, ShortMessageCode, IdIncidentType, CreateDate, Status) 
                                        OUTPUT INSERTED.IDINCIDENTASSESSMENT    
                                        VALUES 
                                            (@IdInsuranceCompany, @LicensePlate, @ClaimNumber, @InsuredName, @InsuredPhone, @IdTowingCompany, @IdTowTruckDriver, @TowTruckDriverName, @TowTruckDriverMobile, @ShortMessageCode, @IdIncidentType, @CreateDate, @Status) ";

                incidentAssessment.Id = this.Conexao.QuerySingle<int>(sqlStatement, param: param, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Atualiza um incident
        /// </summary>
        /// <param name="incidentAssessment"></param>
        public void Update(IncidentAssessment incidentAssessment)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdIncidentAssessment", incidentAssessment?.Id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@IdInsuranceCompany", incidentAssessment?.IdInsuranceCompany, DbType.Int32, ParameterDirection.Input);
                param.Add("@LicensePlate", incidentAssessment?.LicensePlate, System.Data.DbType.String, System.Data.ParameterDirection.Input, 10);
                param.Add("@ClaimNumber", incidentAssessment?.ClaimNumber, System.Data.DbType.String, System.Data.ParameterDirection.Input, 20);
                param.Add("@InsuredName", incidentAssessment?.InsuredName, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                param.Add("@InsuredPhone", incidentAssessment?.InsuredPhone, System.Data.DbType.String, System.Data.ParameterDirection.Input, 20);
                param.Add("@IdTowingCompany", incidentAssessment?.TowingCompany?.Id == 0 ? null : incidentAssessment?.TowingCompany?.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@IdTowTruckDriver", incidentAssessment?.TowTruckDriver?.Id == 0 ? null : incidentAssessment?.TowTruckDriver?.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@TowTruckDriverName", incidentAssessment?.TowTruckDriverName, DbType.String, ParameterDirection.Input, 100);
                param.Add("@TowTruckDriverMobile", incidentAssessment?.TowTruckDriverMobile, DbType.String, ParameterDirection.Input, 20);
                param.Add("@ShortMessageCode", incidentAssessment?.ShortMessageCode, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@IdIncidentType", incidentAssessment?.Type, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@Status", (Int32)incidentAssessment?.Status, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@TotalPoint", incidentAssessment?.TotalPoint, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                param.Add("@UpdateDate", DateTime.Now, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);

                string sqlStatement = @"UPDATE IncidentAssessment 
                                           SET
                                              IdInsuranceCompany   = ISNULL(@IdInsuranceCompany, IdInsuranceCompany),
                                              LicensePlate         = ISNULL(@LicensePlate, LicensePlate),
                                              ClaimNumber          = ISNULL(@ClaimNumber , ClaimNumber),
                                              InsuredName          = ISNULL(@InsuredName , InsuredName),
                                              INSUREDPHONE          = ISNULL(@InsuredPhone , InsuredPhone),
                                              IdTowingCompany      = ISNULL(@IdTowingCompany, IdTowingCompany),
                                              IdTowTruckDriver     = ISNULL(@IdTowTruckDriver, IdTowTruckDriver),
                                              TowTruckDriverName   = ISNULL(@TowTruckDriverName, TowTruckDriverName),
                                              TowTruckDriverMobile = ISNULL(@TowTruckDriverMobile, TowTruckDriverMobile),
                                              ShortMessageCode     = ISNULL(@ShortMessageCode, ShortMessageCode),
                                              IdIncidentType       = ISNULL(@IdIncidentType, IdIncidentType),
                                              Status               = ISNULL(@Status, Status),
                                              TotalPoint           = ISNULL(@TotalPoint, TotalPoint),
                                              UpdateDate           = ISNULL(@UpdateDate, UpdateDate)                                              
                                         WHERE 
                                              IdIncidentAssessment = @IdIncidentAssessment";

                this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Busca um incident
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        public IncidentAssessment Find(int idIncidentAssessment)
        {
            try
            {
                var @param = new { id = idIncidentAssessment };

                IncidentAssessment _IncidentAssessment = this.Conexao
                                                             .Query<IncidentAssessment, InsuranceCompany, TowingCompany, TowTruckDriver, IncidentAssessment>
                                                             (
                                                                @"SELECT 
                                                                     I.[IDINCIDENTASSESSMENT]    ID                                                                    
                                                                    ,I.[LICENSEPLATE]            LICENSEPLATE
                                                                    ,I.[CLAIMNUMBER]             CLAIMNUMBER
                                                                    ,I.[INSUREDNAME]             INSUREDNAME
                                                                    ,I.[INSUREDPHONE]            INSUREDPHONE
                                                                    ,I.[IDTOWINGCOMPANY]         IDTOWINGCOMPANY
                                                                    ,I.[IDTOWTRUCKDRIVER]        IDTOWTRUCKDRIVER
                                                                    ,I.[TOWTRUCKDRIVERNAME]      TOWTRUCKDRIVERNAME
                                                                    ,I.[TOWTRUCKDRIVERMOBILE]    TOWTRUCKDRIVERMOBILE
                                                                    ,I.[IDINCIDENTTYPE]          IDINCIDENTTYPE
                                                                    ,I.[STATUS]                  STATUS
                                                                    ,I.[TOTALPOINT]              TOTALPOINT
                                                                    ,I.[SHORTMESSAGECODE]        SHORTMESSAGECODE
                                                                    ,I.[CreateDate]              CREATEDATE    
                                                                    ,I.[IDINSURANCECOMPANY]      IDINSURANCECOMPANY
                                                                    ,C.[IDCOMPANY]               ID
	                                                                ,C.[Name]					 NAME
                                                                    ,C.[IdTypeCompany]           TYPECOMPANY
                                                                    ,C.[RegistrationNumber]      CNPJ
                                                                    ,T.[IdTowingCompany]         IDTOWINGCOMPANY
                                                                    ,T.[IdTowingCompany]         ID
                                                                    ,TT.[Name]                   NAME
	                                                                ,T.[Description]			 DESCRIPTION
	                                                                ,T.[Email]					 EMAIL
                                                                    ,TT.[IdTypeCompany]          TYPECOMPANY
                                                                    ,TT.[RegistrationNumber]     CNPJ
                                                                    ,D.[IdTowTruckDriver]        IDTOWTRUCKDRIVER
                                                                    ,D.[IdTowTruckDriver]        ID
	                                                                ,D.[Name]					 NAME
	                                                                ,D.[Mobile]					 MOBILE
                                                                FROM [INCIDENTASSESSMENT]  I WITH(NOLOCK)
                                                                LEFT JOIN [Company] C WITH(NOLOCK)
	                                                                ON C.IdCompany = I.IdInsuranceCompany	
                                                                LEFT JOIN [TOWINGCOMPANY] T WITH(NOLOCK)
	                                                                ON T.IdTowingCompany = I.IdTowingCompany
                                                                LEFT JOIN [Company] TT WITH(NOLOCK)
                                                                    ON TT.IdCompany = T.IdTowingCompany
                                                                LEFT JOIN [TowTruckDriver] D WITH(NOLOCK)
	                                                                ON D.[IdTowTruckDriver] = I.[IdTowTruckDriver] AND D.[IdTowingCompany] = T.[IdTowingCompany]
                                                                WHERE 
	                                                                    I.IDINCIDENTASSESSMENT = @id ",
                                                                (incidentAssessment, insuranceCompany, towingCompany, towTruckDriver) =>
                                                                {

                                                                    incidentAssessment.IdInsuranceCompany = insuranceCompany.Id;
                                                                    incidentAssessment.InsuranceCompany = insuranceCompany;

                                                                    incidentAssessment.TowTruckDriver = towTruckDriver;

                                                                    incidentAssessment.TowingCompany = towingCompany;

                                                                    return incidentAssessment;
                                                                },
                                                                splitOn: "IDINSURANCECOMPANY,IDTOWINGCOMPANY,IDTOWTRUCKDRIVER",
                                                                param: param,
                                                                transaction: this.Transacao
                                                             ).FirstOrDefault();

                return _IncidentAssessment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Lista paginado um incident
        /// </summary>
        /// <param name="company"></param>
        /// <param name="pagination"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Pagination<IncidentAssessment> List(Company company, Pagination<IncidentAssessment> pagination, StatusIncidentAssessment? status)
        {
            try
            {
                var @param = new
                {
                    PageSize = pagination.PageSize,
                    PageNumber = pagination.PageNumber,
                    IdCompany = company.Id,
                    TypeCompany = (Int32)company.TypeCompany,
                    Status = (int?)status
                };

                IList<IncidentAssessment> _incidentAssessment = this.Conexao
                                               .Query<IncidentAssessment, InsuranceCompany, TowingCompany, TowTruckDriver, IncidentAssessment>(
                                                          @"SELECT 
                                                                 I.[IDINCIDENTASSESSMENT]    ID                                                                    
                                                                ,I.[LICENSEPLATE]            LICENSEPLATE
                                                                ,I.[CLAIMNUMBER]             CLAIMNUMBER
                                                                ,I.[INSUREDNAME]             INSUREDNAME
                                                                ,I.[INSUREDPHONE]            INSUREDPHONE
                                                                ,I.[IDTOWINGCOMPANY]         IDTOWINGCOMPANY
                                                                ,I.[IDTOWTRUCKDRIVER]        IDTOWTRUCKDRIVER
                                                                ,I.[TOWTRUCKDRIVERNAME]      TOWTRUCKDRIVERNAME
                                                                ,I.[TOWTRUCKDRIVERMOBILE]    TOWTRUCKDRIVERMOBILE
                                                                ,I.[IDINCIDENTTYPE]          IDINCIDENTTYPE
                                                                ,I.[STATUS]                  STATUS
                                                                ,I.[TOTALPOINT]              TOTALPOINT
                                                                ,I.[SHORTMESSAGECODE]        SHORTMESSAGECODE
                                                                ,I.[CreateDate]              CREATEDATE
                                                                ,I.[IDINSURANCECOMPANY]      IDINSURANCECOMPANY
                                                                ,C.[IDCOMPANY]               ID
	                                                            ,C.[Name]					 NAME
                                                                ,C.[IdTypeCompany]           TYPECOMPANY
                                                                ,C.[RegistrationNumber]      CNPJ
                                                                ,T.[IdTowingCompany]         IDTOWINGCOMPANY
                                                                ,T.[IdTowingCompany]         ID
                                                                ,TT.[Name]                   NAME
	                                                            ,T.[Description]			 DESCRIPTION
	                                                            ,T.[Email]					 EMAIL
                                                                ,TT.[IdTypeCompany]          TYPECOMPANY
                                                                ,TT.[RegistrationNumber]     CNPJ
                                                                ,D.[IdTowTruckDriver]        IDTOWTRUCKDRIVER
                                                                ,D.[IdTowTruckDriver]        ID
	                                                            ,D.[Name]					 NAME
	                                                            ,D.[Mobile]					 MOBILE
                                                            FROM [INCIDENTASSESSMENT]  I    WITH(NOLOCK)
                                                            LEFT JOIN [Company] C           WITH(NOLOCK)
	                                                            ON C.IdCompany = I.IdInsuranceCompany	
                                                            LEFT JOIN [TOWINGCOMPANY] T     WITH(NOLOCK)
	                                                            ON T.IdTowingCompany = I.IdTowingCompany
                                                            LEFT JOIN [Company] TT          WITH(NOLOCK)
                                                                ON TT.IdCompany = T.IdTowingCompany
                                                            LEFT JOIN [TowTruckDriver] D    WITH(NOLOCK)
	                                                            ON D.[IdTowTruckDriver] = I.[IdTowTruckDriver] AND D.[IdTowingCompany] = T.[IdTowingCompany]
                                                            WHERE (
                                                                    (I.IdInsuranceCompany = @IdCompany AND  C.[IdTypeCompany] = @TypeCompany) OR 
                                                                    (I.[IdTowingCompany]  = @IdCompany AND TT.[IdTypeCompany] = @TypeCompany)
                                                                  ) AND (I.Status = ISNULL(@STATUS,I.STATUS)) 
                                                            ORDER BY [CreateDate] DESC  
                                                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                                                            FETCH NEXT @PageSize ROWS ONLY",
                                                        (incidentAssessment, insuranceCompany, towingCompany, towTruckDriver) =>
                                                        {

                                                            incidentAssessment.IdInsuranceCompany = insuranceCompany.Id;
                                                            incidentAssessment.InsuranceCompany = insuranceCompany;

                                                            incidentAssessment.TowTruckDriver = towTruckDriver;

                                                            incidentAssessment.TowingCompany = towingCompany;

                                                            return incidentAssessment;
                                                        },
                                                        splitOn: "IDINSURANCECOMPANY,IDTOWINGCOMPANY,IDTOWTRUCKDRIVER",
                                                        param: param,
                                                        transaction: this.Transacao)
                                                        .ToList();


                pagination.TotalPage = this.Conexao
                                            .ExecuteScalar<int>(@"SELECT COUNT(1) 
                                                                  FROM [IncidentAssessment] I   WITH(NOLOCK) 
                                                                  LEFT JOIN [Company] C         WITH(NOLOCK)
	                                                                   ON C.IdCompany = I.IdInsuranceCompany	
                                                                  LEFT JOIN [TOWINGCOMPANY] T   WITH(NOLOCK)
	                                                                    ON T.IdTowingCompany = I.IdTowingCompany
                                                                  LEFT JOIN [Company] TT        WITH(NOLOCK)
                                                                        ON TT.IdCompany = T.IdTowingCompany
                                                                  WHERE (
                                                                            (I.IdInsuranceCompany = @IdCompany AND  C.[IdTypeCompany] = @TypeCompany) OR 
                                                                            (I.[IdTowingCompany]  = @IdCompany AND TT.[IdTypeCompany] = @TypeCompany)
                                                                        ) AND (I.Status = ISNULL(@STATUS,I.STATUS)) ",
                                                   param: param, transaction: this.Transacao);


                pagination.Page = _incidentAssessment;

                return pagination;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region Respostas
        /// <summary>
        /// Adiciona respostas
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="question"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Apaga todas as respostas
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Lista as respostas com categoria
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        public IList<Category> GetAnswersByCategory(int idIncidentAssessment)
        {
            try
            {
                var @param = new { id = idIncidentAssessment };
                var categoryDictionary = new Dictionary<int, Category>();

                IList<Category> _categoryAnswers = this.Conexao
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


                return _categoryAnswers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera as respostas
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <returns></returns>
        public IList<Question> GetAnswers(int idIncidentAssessment)
        {
            try
            {
                var @param = new { id = idIncidentAssessment };
                var categoryDictionary = new Dictionary<int, Category>();

                IList<Question> _Answers = this.Conexao
                                               .Query<Question>(
                                                        @"SELECT
                                                                 Q.IdQuestion    Id,
                                                                 Q.Label         Label,
                                                                 Q.QuestionType  Type,
                                                                 Q.Point         Point,
                                                                 A.Answer        Answer
                                                            FROM IncidentAssessment I
                                                           INNER JOIN IncidentAssessmentAnswer A
                                                              ON I.IdIncidentAssessment = A.IdIncidentAssessment
                                                           INNER JOIN Question Q
                                                              ON Q.idQuestion = A.IdQuestion
                                                           WHERE
                                                                 I.IdIncidentAssessment = @id",
                                                        param: param,
                                                        transaction: this.Transacao)
                                                        .ToList();

                return _Answers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region Imagem
        /// <summary>
        /// Adiciona uma image
        /// </summary>
        /// <param name="incidentAssessment"></param>
        /// <param name="inicidentImage"></param>
        /// <returns></returns>
        public bool AddImage(IncidentAssessment incidentAssessment, IncidentAssessmentImage inicidentImage)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdIncidentAssessment", incidentAssessment.Id, DbType.Int32, ParameterDirection.Input);
                param.Add("@Name", inicidentImage?.Name, DbType.String, ParameterDirection.Input, 200);
                param.Add("@MimeType", inicidentImage?.MimeType, DbType.String, ParameterDirection.Input, 50);
                param.Add("@Image", inicidentImage?.Image, DbType.Binary, ParameterDirection.Input);
                param.Add("@Thumbnail", inicidentImage?.Thumbnail, DbType.Binary, ParameterDirection.Input);

                string sqlStatement = @"INSERT INTO IncidentAssessmentImage 
                                            (IdIncidentAssessment, Name, MimeType, Image, Thumbnail)                                           
                                        VALUES 
                                            (@IdIncidentAssessment, @Name, @MimeType, @Image, @Thumbnail) ";

                return this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Lista paginado as imagens
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public Pagination<IncidentAssessmentImage> ListImage(int idIncident, Pagination<IncidentAssessmentImage> pagination)
        {
            try
            {
                var @param = new
                {
                    PageSize = pagination.PageSize,
                    PageNumber = pagination.PageNumber,
                    IdIncident = idIncident
                };

                IList<IncidentAssessmentImage> _incidentAssessmentImage = this.Conexao
                                               .Query<IncidentAssessmentImage>(
                                                          @"SELECT 
                                                                    IdIncidentAssessmentImage Id,
		                                                            Name, 
		                                                            MimeType
                                                            FROM IncidentAssessmentImage WITH(NOLOCK)
                                                            WHERE IdIncidentAssessment = @IdIncident
                                                            ORDER BY [IdIncidentAssessmentImage] DESC  
                                                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                                                            FETCH NEXT @PageSize ROWS ONLY",
                                                        param: param,
                                                        transaction: this.Transacao)
                                                        .ToList();


                pagination.TotalPage = this.Conexao
                                            .ExecuteScalar<int>(@"SELECT COUNT(1)
                                                                  FROM IncidentAssessmentImage WITH(NOLOCK)
                                                                  WHERE IdIncidentAssessment = @IdIncident",
                                                   param: param, transaction: this.Transacao);


                pagination.Page = _incidentAssessmentImage;

                return pagination;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Busca uma imagem por ID
        /// </summary>
        /// <param name="idIncident"></param>
        /// <param name="idIncidentImage"></param>
        /// <param name="fullImage"></param>
        /// <returns></returns>
        public IncidentAssessmentImage FindImage(int idIncident, int idIncidentImage, bool fullImage = false)
        {
            try
            {
                var @param = new
                {
                    idIncident = idIncident,
                    idIncidentAssessmentImage = idIncidentImage,
                    FullImage = fullImage
                };

                IncidentAssessmentImage _incidentAssessmentImage = this.Conexao
                                               .QueryFirstOrDefault<IncidentAssessmentImage>(
                                                          @"SELECT 
		                                                            Name, 
		                                                            MimeType,
                                                                    case when @fullimage = 1 then [Image] Else null END as [Image],
                                                                    Thumbnail
                                                            FROM IncidentAssessmentImage WITH(NOLOCK)
                                                            WHERE IdIncidentAssessment = @idIncident AND
                                                                  IdIncidentAssessmentImage = @IdIncidentAssessmentImage
                                                            ",
                                                        param: param,
                                                        transaction: this.Transacao);


                return _incidentAssessmentImage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Apaga uma imagem da base
        /// </summary>
        /// <param name="idIncidentAssessment"></param>
        /// <param name="idIncidentAssessmentImage"></param>
        /// <returns></returns>
        public bool DeleteImage(int idIncidentAssessment, int idIncidentAssessmentImage)
        {
            try
            {
                var param = new
                {
                    IdIncidentAssessment = idIncidentAssessment,
                    IdIncidentAssessmentImage = idIncidentAssessmentImage
                };

                string sqlStatement = @"DELETE FROM IncidentAssessmentImage 
                                        WHERE IdIncidentAssessment      = @IdIncidentAssessment AND 
                                              IdIncidentAssessmentImage = @IdIncidentAssessmentImage ";

                return this.Conexao.Execute(sqlStatement, param: param, transaction: this.Transacao) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion
    }
}
