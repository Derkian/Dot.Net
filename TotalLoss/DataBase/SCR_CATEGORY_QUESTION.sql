CREATE TABLE Company
(
	[IdCompany]				INT IDENTITY(1,1),
	[Name]					VARCHAR(200)	NOT NULL,
	[RegistrationNumber]	VARCHAR(20)		NOT NULL,
	[Logo]					VARBINARY(MAX)	NOT NULL,
	[PrimaryColor]			VARCHAR(30),
	[SecondaryColor]		VARCHAR(30),
	[LimitTotalLoss]		INT
)

ALTER TABLE Company ADD CONSTRAINT Pk_Company PRIMARY KEY (IdCompany)

CREATE TABLE [User]
(
	[IdUser]		INT IDENTITY(1,1),
	[IdCompany]		INT,
	[Name]			VARCHAR(200),
	[Login]			VARCHAR(100) NOT NULL,
	[Password]		VARCHAR(100) NOT NULL,
	[Status]		INT
)

ALTER TABLE [User] ADD constraint Pk_User PRIMARY KEY (IdUser)
ALTER TABLE [User] add constraint Fk_CompanyUser FOREIGN KEY (IdCompany) REFERENCES [Company] (IdCompany)
ALTER TABLE [User] ADD CONSTRAINT UQ_USER UNIQUE ([Login])

CREATE TABLE Salvage
(
	[IdSalvage]				INT IDENTITY(1,1),
	[Name]					VARCHAR(200) NOT NULL,
	[RegistrationNumber]	VARCHAR(20)  NOT NULL
)

ALTER TABLE Salvage ADD CONSTRAINT Pk_Salvage PRIMARY KEY (IdSalvage)

CREATE TABLE SalvageCompany
(
	[IdSalvage]				INT NOT NULL,
	[IdCompany]				INT NOT NULL
)


ALTER TABLE [SalvageCompany] ADD constraint PK_SalvageCompany PRIMARY KEY (IdSalvage,IdCompany)
ALTER TABLE [SalvageCompany] ADD constraint Fk_Salvage FOREIGN KEY (IdSalvage) REFERENCES Salvage (IdSalvage)
ALTER TABLE [SalvageCompany] ADD constraint Fk_Company FOREIGN KEY (IdCompany) REFERENCES Company (IdCompany)


--ALTER TABLE QuestionType ADD CONSTRAINT Pk_QuestionType PRIMARY KEY ([IdQuestionType])

--INSERT INTO QuestionType VALUES (0,'Default')
--INSERT INTO QuestionType VALUES (1,'Button')


CREATE TABLE Category
(
	[IdCategory]		INT IDENTITY(1,1),
	[Label]				VARCHAR(100),
	[Image]				VARBINARY(MAX) NULL,
	[IdCompany]			INT not null,
	[Point]				INT
)

ALTER TABLE Category add constraint Fk_Category PRIMARY KEY (idCategory)
ALTER TABLE Category add constraint Fk_CategoryCompany FOREIGN KEY (IdCompany) REFERENCES [Company] (IdCompany)

CREATE TABLE Question
(
	[IdQuestion]		INT IDENTITY(1,1),
	[IdCategory]		INT NOT NULL,
	[Label]				VARCHAR(MAX),
	[QuestionType]		INT DEFAULT 0,
	[Point]				INT,
	[Enable]			BIT DEFAULT 1
)

ALTER TABLE Question ADD CONSTRAINT Pk_Question PRIMARY KEY ([idQuestion])
ALTER TABLE Question ADD CONSTRAINT fk_QuestionCategory FOREIGN KEY ([idCategory]) REFERENCES [Category] ([idCategory])
--ALTER TABLE Question ADD CONSTRAINT fk_QuestionType FOREIGN KEY ([IdQuestionType]) REFERENCES [QuestionType] ([IdQuestionType])

create TABLE IncidentAssessment
(
	[IdIncidentAssessment]	INT IDENTITY(1,1),
	[IdCompany]				INT NOT NULL,
	[LicensePlate]			VARCHAR(10),
	[ClaimNumber]			VARCHAR(20),
	[InsuredName]			VARCHAR(100),
	[InsuredFone]			VARCHAR(20),
	[Provider]				VARCHAR(100),
	[WorkProvider]			VARCHAR(100),
	[WorkProviderFone]		VARCHAR(20),
	[ShortMessageCode]		INT,
	[Type]					INT NOT NULL,
	[Status]				INT NOT NULL,
	[TotalPoint]			INT,
	[CreateDate]			DATETIME NOT NULL,
	[UpdateDate]			DATETIME	
)

ALTER TABLE IncidentAssessment ADD CONSTRAINT Pk_IncidentAssessment PRIMARY KEY (IdIncidentAssessment)
ALTER TABLE IncidentAssessment ADD CONSTRAINT fk_IncidentAssessmentCompany FOREIGN KEY (IdCompany) REFERENCES [Company] (IdCompany)

CREATE TABLE IncidentAssessmentAnswer
(
	[IdIncidentAssessment]		INT NOT NULL,
	[IdQuestion]				INT NOT NULL,
	[Answer]					BIT DEFAULT 0,
	[CreateDate]				DATETIME
)

ALTER TABLE IncidentAssessmentAnswer ADD CONSTRAINT Pk_incidentAssessmentAnswer PRIMARY KEY (IdIncidentAssessment,IdQuestion)
ALTER TABLE IncidentAssessmentAnswer ADD CONSTRAINT fk_IncidentAssessmentAnswer FOREIGN KEY ([IdIncidentAssessment]) REFERENCES [IncidentAssessment] ([IdIncidentAssessment])
ALTER TABLE IncidentAssessmentAnswer ADD CONSTRAINT fk_IncidentAssessmentAnswerQuestion FOREIGN KEY ([IdQuestion]) REFERENCES Question ([IdQuestion])

CREATE TABLE IncidentAssessmentImage
(
	[IdIncidentAssessmentImage]	INT IDENTITY(1,1),
	[IdIncidentAssessment]		INT NOT NULL,
	[Name]						VARCHAR(200),
	[MimeType]					VARCHAR(50),
	[Image]						VARBINARY(MAX)	
)

ALTER TABLE IncidentAssessmentImage ADD CONSTRAINT Pk_IncidentAssessmentImage PRIMARY KEY (IdIncidentAssessmentImage)
ALTER TABLE IncidentAssessmentImage ADD CONSTRAINT fk_IncidentAssessmentImage_IncidentAssessment FOREIGN KEY ([IdIncidentAssessment]) REFERENCES [IncidentAssessment] ([IdIncidentAssessment])



CREATE TABLE [Location]
(
	[IdLocation]	INT IDENTITY(1,1),
	[IdSalvage]		INT,
	[State]			VARCHAR(2),
	[City]			VARCHAR(100),
	[Email]			VARCHAR(100)
)

ALTER TABLE [Location] ADD CONSTRAINT Pk_Location PRIMARY KEY (IdLocation)
ALTER TABLE [Location] ADD CONSTRAINT Fk_LocationSalvage FOREIGN KEY (IdSalvage) REFERENCES [SALVAGE] (IdSalvage)

CREATE TABLE [LocationDetail]
(
	[IdLocationDetail]		INT IDENTITY(1,1),
	[IdLocation]			INT,
	[Street]				VARCHAR(200),
	[District]				VARCHAR(100),
	[ZipCode]				VARCHAR(10),
	[Email]					VARCHAR(100),
	[Fone]					VARCHAR(20),
	[Fax]					VARCHAR(20),
	[Manager]				VARCHAR(100)
)

ALTER TABLE [LocationDetail] ADD CONSTRAINT Pk_LocationDetail PRIMARY KEY (IdLocationDetail)
ALTER TABLE [LocationDetail] ADD CONSTRAINT Fk_LocationDetail FOREIGN KEY (IdLocation) REFERENCES [Location] (IdLocation)



