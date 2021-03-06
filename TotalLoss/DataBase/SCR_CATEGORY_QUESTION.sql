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

CREATE TABLE TypeEnum
(
	[Type]			INT NOT NULL,
	[Description]	VARCHAR(100)
)

ALTER TABLE TypeEnum ADD CONSTRAINT Pk_TypeEnum PRIMARY KEY ([Type])
ALTER TABLE IncidentAssessment ADD CONSTRAINT Fk_TypeEnum FOREIGN KEY ([Type]) REFERENCES TypeEnum ([Type])

CREATE TABLE ShortMessageCodeEnum
(
	 ShortMessageCode	INT NOT NULL,
	 [Description]		VARCHAR(100)		
)

ALTER TABLE ShortMessageCodeEnum ADD CONSTRAINT Pk_ShortMessageCodeEnum PRIMARY KEY ([ShortMessageCode])
ALTER TABLE IncidentAssessment ADD CONSTRAINT Fk_ShortMessageCodee FOREIGN KEY ([ShortMessageCode]) REFERENCES ShortMessageCodeEnum ([ShortMessageCode])

CREATE TABLE StatusEnum
(
	[Status]		INT NOT NULL,
	[Description]	VARCHAR(50)
)

ALTER TABLE StatusEnum ADD CONSTRAINT Pk_StatusEnum PRIMARY KEY ([Status])
ALTER TABLE IncidentAssessment ADD CONSTRAINT Fk_StatusEnum FOREIGN KEY ([Status]) REFERENCES StatusEnum ([Status])


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
	[Longitude]				VARCHAR(50),
	[Latitude]				VARCHAR(50),
	[Email]					VARCHAR(100),
	[Fone]					VARCHAR(20),
	[Fax]					VARCHAR(20),
	[Manager]				VARCHAR(100)
)

ALTER TABLE [LocationDetail] ADD CONSTRAINT Pk_LocationDetail PRIMARY KEY (IdLocationDetail)
ALTER TABLE [LocationDetail] ADD CONSTRAINT Fk_LocationDetail FOREIGN KEY (IdLocation) REFERENCES [Location] (IdLocation)


GO

INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (-1, 'Not Send')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (0, 'Message Sent')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (10,'Empty Message Content')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (12,'Message Content Overflow')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (13,'Incorrect Or Incomplete Mobile Number')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (14, 'Empty Mobile Number')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (15,'Scheduling Date Invalid Or Incorrect')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (16,'ID Overflow')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (80,'Message With Same Id Already Sent')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (141,'International Sending Not Allowed')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (900,'Authentication Error')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (990,'Account Limit Reached')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (998,'Wrong Operation Requested')
INSERT INTO ShortMessageCodeEnum (ShortMessageCode,[Description]) VALUES (999,'Unknown Error')

GO
INSERT INTO StatusEnum VALUES (0,'Created')
INSERT INTO StatusEnum VALUES (1,'Pending')
INSERT INTO StatusEnum VALUES (2,'Finalized')

GO
INSERT INTO TypeEnum VALUES (0,'None')
INSERT INTO TypeEnum VALUES (1,'Recupered')
INSERT INTO TypeEnum VALUES (2,'TotalLoss')


GO
CREATE TABLE [LogTypeOperation]
(
 [IdTypeOperation] INT NOT NULL,
 [Name] VARCHAR(50) NOT NULL
)
GO
ALTER TABLE [LogTypeOperation] ADD constraint Pk_LogTypeOperation PRIMARY KEY (IdTypeOperation)

GO
INSERT INTO [LogTypeOperation](IdTypeOperation, [Name]) VALUES (1,'Insert')
INSERT INTO [LogTypeOperation](IdTypeOperation, [Name]) VALUES (2,'Update')
INSERT INTO [LogTypeOperation](IdTypeOperation, [Name]) VALUES (3,'Delete')

GO
CREATE TABLE [LogOperation] 
(
 [IdOperation]   BIGINT IDENTITY(1,1),
 [ClassName]     VARCHAR(80) NOT NULL,
 [IdTypeOperation] INT NOT NULL,
 [ValuePrimaryKey]  INT NOT NULL,
 [UserLogin]     VARCHAR(100) NOT NULL,
 [DateChanged]    DATETIME NOT NULL
)
GO
ALTER TABLE [LogOperation] ADD constraint Pk_LogOperation PRIMARY KEY (IdOperation)
ALTER TABLE [LogOperation] ADD constraint Fk_LogTypeOperation FOREIGN KEY (IdTypeOperation) REFERENCES [LogTypeOperation] (IdTypeOperation)

GO
CREATE TABLE [LogValues]	
(
 [IdValue]      BIGINT IDENTITY(1,1),
 [IdOperation]  BIGINT,
 [PropertyName] VARCHAR(80) NOT NULL,
 [NewValue]     VARCHAR(MAX) NULL,
 [OldValue]     VARCHAR(MAX) NULL
)
GO
ALTER TABLE [LogValues] ADD constraint Pk_LogValues PRIMARY KEY (IdValue)
ALTER TABLE [LogValues] ADD constraint Fk_LogOperation FOREIGN KEY (IdOperation) REFERENCES [LogOperation] (IdOperation)


CREATE TABLE History
(
	[IdHistory]					INT IDENTITY(1,1),
	[IdIncidentAssessment]		INT NOT NULL,
	[Operation]					INT,
	[Data]						DATETIME,
	[Object]					VARCHAR(MAX)
)

ALTER TABLE History ADD CONSTRAINT PK_History PRIMARY KEY ([IdHistory])

CREATE TABLE HistoryOperation
(
	[Operation]		INT NOT NULL,
	[Description]	VARCHAR(100),
)

ALTER TABLE HistoryOperation ADD CONSTRAINT PK_HistoryOperation PRIMARY KEY ([Operation])
ALTER TABLE History ADD CONSTRAINT FK_HistoryOperation FOREIGN KEY ([Operation]) REFERENCES HistoryOperation ([Operation])

INSERT INTO HistoryOperation VALUES (1 , 'Create Incident')
INSERT INTO HistoryOperation VALUES (2 , 'Update Incident')
INSERT INTO HistoryOperation VALUES (3 , 'Get Incident')
INSERT INTO HistoryOperation VALUES (4 , 'Add Image')
INSERT INTO HistoryOperation VALUES (5 , 'Add Answear')
INSERT INTO HistoryOperation VALUES (6 , 'Finalize Incident')
