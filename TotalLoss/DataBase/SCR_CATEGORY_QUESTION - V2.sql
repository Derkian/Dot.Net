/**************************************************************************
								COMPANY
**************************************************************************/
CREATE TABLE Company
(
	[IdCompany]				INT IDENTITY(1,1),
	[Name]					VARCHAR(200)	NOT NULL,
	[RegistrationNumber]	VARCHAR(20),
	[IdTypeCompany]			INT
)

ALTER TABLE Company ADD CONSTRAINT Pk_Company PRIMARY KEY (IdCompany)

/**************************************************************************
								USER
**************************************************************************/
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

/**************************************************************************
								TYPECOMPANY
**************************************************************************/
CREATE TABLE TypeCompanyEnum
(
	[IdTypeCompany]	INT NOT NULL,
	[Description]	VARCHAR(100)
)

ALTER TABLE [TypeCompanyEnum] ADD constraint Pk_TypeCompany PRIMARY KEY (IdTypeCompany)
ALTER TABLE [Company] ADD CONSTRAINT Fk_TypeCompany FOREIGN KEY ([IdTypeCompany]) REFERENCES TypeCompanyEnum ([IdTypeCompany])


INSERT INTO [TypeCompanyEnum] VALUES (1, 'Insurance Company')
INSERT INTO [TypeCompanyEnum] VALUES (2, 'Towing Service Company')

/*********************************************************************************
								TOWINGCOMPANY
**********************************************************************************/
CREATE TABLE TowingCompany
(
	[IdTowingCompany]		INT NOT NULL,
	[IdInsuranceCompany]	INT NOT NULL,
	[Description]			VARCHAR(100),
	[Email]					VARCHAR(100),
    [Enable]				BIT DEFAULT 1
)

ALTER TABLE TowingCompany ADD CONSTRAINT Pk_Towing PRIMARY KEY ([IdTowingCompany])
ALTER TABLE TowingCompany ADD CONSTRAINT Fk_TowingCompany FOREIGN KEY ([IdTowingCompany]) REFERENCES Company ([IdCompany])

/*********************************************************************************
								INSURANCECOMPANY
**********************************************************************************/
CREATE TABLE InsuranceCompany
(
	[IdInsuranceCompany]	INT NOT NULL,
	[Description]			VARCHAR(100),
	[Logo]					VARBINARY(MAX)	NOT NULL,
	[PrimaryColor]			VARCHAR(30),
	[SecondaryColor]		VARCHAR(30),
	[LimitTotalLoss]		INT
)

ALTER TABLE InsuranceCompany ADD CONSTRAINT Pk_Insurance PRIMARY KEY ([IdInsuranceCompany])
ALTER TABLE InsuranceCompany ADD CONSTRAINT Fk_InsuranceCompany FOREIGN KEY ([IdInsuranceCompany]) REFERENCES Company ([IdCompany])

ALTER TABLE TowingCompany ADD CONSTRAINT Fk_TowingCompanyInsuranceCompany FOREIGN KEY ([IdInsuranceCompany]) REFERENCES InsuranceCompany ([IdInsuranceCompany])


/*********************************************************************************
								TOW TRUCK DRIVER
**********************************************************************************/
CREATE TABLE TowTruckDriver
(
	[IdTowTruckDriver]	INT IDENTITY(1,1),
	[IdTowingCompany]	INT NOT NULL,
	[Name]				VARCHAR(100) NOT NULL,
	[Mobile]			VARCHAR(20) NOT NULL,
	[Enable]			BIT DEFAULT 1
)

ALTER TABLE TowTruckDriver ADD CONSTRAINT Pk_TowTruckDriver PRIMARY KEY ([IdTowTruckDriver])
ALTER TABLE TowTruckDriver ADD CONSTRAINT Fk_TowingCompanyDriver FOREIGN KEY ([IdTowingCompany]) REFERENCES TowingCompany ([IdTowingCompany])
/*********************************************************************************
								SALVAGE
**********************************************************************************/
CREATE TABLE Salvage
(
	[IdSalvage]				INT IDENTITY(1,1),
	[Name]					VARCHAR(200) NOT NULL,
	[RegistrationNumber]	VARCHAR(20)  NOT NULL
)

ALTER TABLE Salvage ADD CONSTRAINT Pk_Salvage PRIMARY KEY (IdSalvage)

/*********************************************************************************
								SALVAGE COMPANY
**********************************************************************************/
CREATE TABLE SalvageCompany
(
	[IdSalvage]				INT NOT NULL,
	[IdInsuranceCompany]	INT NOT NULL
)

ALTER TABLE [SalvageCompany] ADD constraint PK_SalvageCompany PRIMARY KEY (IdSalvage,IdInsuranceCompany)
ALTER TABLE [SalvageCompany] ADD constraint Fk_Salvage FOREIGN KEY (IdSalvage) REFERENCES Salvage (IdSalvage)
ALTER TABLE [SalvageCompany] ADD constraint Fk_SalvageInsuranceCompany	 FOREIGN KEY ([IdInsuranceCompany]) REFERENCES InsuranceCompany ([IdInsuranceCompany])

/*********************************************************************************
								CATEGORY
**********************************************************************************/
CREATE TABLE Category
(
	[IdCategory]		 INT IDENTITY(1,1),
	[IdInsuranceCompany] INT NOT NULL,
	[Label]				 VARCHAR(100),
	[Image]				 VARBINARY(MAX) NULL,	
	[Point]				 INT
)

ALTER TABLE Category add constraint Fk_Category PRIMARY KEY (idCategory)
ALTER TABLE Category ADD constraint Fk_CategoryCompany FOREIGN KEY (IdInsuranceCompany) REFERENCES [InsuranceCompany] (IdInsuranceCompany)

/*********************************************************************************
								QUESTION
**********************************************************************************/
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

/*********************************************************************************
								INCIDENT ASSESSMENT
**********************************************************************************/
CREATE TABLE IncidentAssessment
(
	[IdIncidentAssessment]	INT IDENTITY(1,1),
	[IdInsuranceCompany]	INT NOT NULL,
	[LicensePlate]			VARCHAR(10),
	[ClaimNumber]			VARCHAR(20),
	[InsuredName]			VARCHAR(100),
	[InsuredPhone]			VARCHAR(20),
	[IdTowingCompany]		INT,
	[IdTowTruckDriver]		INT,
	[TowTruckDriverName]	VARCHAR(100),
	[TowTruckDriverMobile]	VARCHAR(20),
	[ShortMessageCode]		INT,
	[IdIncidentType]		INT NOT NULL,
	[Status]				INT NOT NULL,
	[TotalPoint]			INT,
	[CreateDate]			DATETIME NOT NULL,
	[UpdateDate]			DATETIME	
)

ALTER TABLE IncidentAssessment ADD CONSTRAINT Pk_IncidentAssessment PRIMARY KEY (IdIncidentAssessment)
ALTER TABLE IncidentAssessment ADD CONSTRAINT fk_InsuranceCompanyIncidentAssessment FOREIGN KEY ([IdInsuranceCompany]) REFERENCES [InsuranceCompany] ([IdInsuranceCompany])
ALTER TABLE IncidentAssessment ADD CONSTRAINT fk_TowingCompanyIncidentAssessment FOREIGN KEY ([IdTowingCompany]) REFERENCES [TowingCompany] ([IdTowingCompany])
ALTER TABLE IncidentAssessment ADD CONSTRAINT fk_TowTruckDriverIncidentAssessment FOREIGN KEY ([IdTowTruckDriver]) REFERENCES [TowTruckDriver] ([IdTowTruckDriver])

/*********************************************************************************
								INCIDENT ASSESSMENT
**********************************************************************************/
CREATE TABLE IncidentTypeEnum
(
	[IdIncidentType]	INT NOT NULL,
	[Description]		VARCHAR(100)
)

ALTER TABLE IncidentTypeEnum ADD CONSTRAINT Pk_IncidentTypeEnum PRIMARY KEY ([IdIncidentType])
ALTER TABLE IncidentAssessment ADD CONSTRAINT Fk_IncidentTypeEnum FOREIGN KEY ([IdIncidentType]) REFERENCES IncidentTypeEnum ([IdIncidentType])

INSERT INTO IncidentTypeEnum VALUES (0,'None')
INSERT INTO IncidentTypeEnum VALUES (1,'Recupered')
INSERT INTO IncidentTypeEnum VALUES (2,'Total Loss')


/*********************************************************************************
								SHORT MESSAGE CODE ENUM
**********************************************************************************/
CREATE TABLE ShortMessageCodeEnum
(
	 ShortMessageCode	INT NOT NULL,
	 [Description]		VARCHAR(100)		
)

ALTER TABLE ShortMessageCodeEnum ADD CONSTRAINT Pk_ShortMessageCodeEnum PRIMARY KEY ([ShortMessageCode])
ALTER TABLE IncidentAssessment ADD CONSTRAINT Fk_ShortMessageCodee FOREIGN KEY ([ShortMessageCode]) REFERENCES ShortMessageCodeEnum ([ShortMessageCode])


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


/*********************************************************************************
								STATUS ENUM
**********************************************************************************/
CREATE TABLE StatusEnum
(
	[Status]		INT NOT NULL,
	[Description]	VARCHAR(50)
)

ALTER TABLE StatusEnum ADD CONSTRAINT Pk_StatusEnum PRIMARY KEY ([Status])
ALTER TABLE IncidentAssessment ADD CONSTRAINT Fk_StatusEnum FOREIGN KEY ([Status]) REFERENCES StatusEnum ([Status])

GO
INSERT INTO StatusEnum VALUES (0,'Created')
INSERT INTO StatusEnum VALUES (1,'Pending')
INSERT INTO StatusEnum VALUES (2,'Answered')
INSERT INTO StatusEnum VALUES (3,'Finalized')

/*********************************************************************************
								INCIDENT ASSESSMENT ANSWER
**********************************************************************************/
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

/*********************************************************************************
								INCIDENT ASSESSMENT IMAGE
**********************************************************************************/
CREATE TABLE IncidentAssessmentImage
(
	[IdIncidentAssessmentImage]	INT IDENTITY(1,1),
	[IdIncidentAssessment]		INT NOT NULL,
	[Name]						VARCHAR(200),
	[MimeType]					VARCHAR(50),
	[Image]						VARBINARY(MAX),
	[Thumbnail]					VARBINARY(MAX)
)

ALTER TABLE IncidentAssessmentImage ADD CONSTRAINT Pk_IncidentAssessmentImage PRIMARY KEY (IdIncidentAssessmentImage)
ALTER TABLE IncidentAssessmentImage ADD CONSTRAINT fk_IncidentAssessmentImage_IncidentAssessment FOREIGN KEY ([IdIncidentAssessment]) REFERENCES [IncidentAssessment] ([IdIncidentAssessment])


/*********************************************************************************
								LOCATION
**********************************************************************************/
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

/*********************************************************************************
								LOCATION DETAIL
**********************************************************************************/
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

/*********************************************************************************
								LOG TYPE OPERATION
**********************************************************************************/
CREATE TABLE History
(
	[IdHistory]					INT IDENTITY(1,1),
	[IdIncidentAssessment]		INT NOT NULL,
	[Operation]					INT,
	[IdUser]					INT,
	[Login]						VARCHAR(100),
	[Data]						DATETIME,
	[Object]					VARCHAR(MAX)
)

ALTER TABLE History ADD CONSTRAINT PK_History PRIMARY KEY ([IdHistory])

/*********************************************************************************
								HISTORY OPERATION
**********************************************************************************/
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
INSERT INTO HistoryOperation VALUES (7 , 'Send Sms')
INSERT INTO HistoryOperation VALUES (8 , 'Delete Image')
