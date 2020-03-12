
/*******************************************************************************
									CUSTOMER
********************************************************************************/
CREATE TABLE [dbo].[Customer] (
    [IdCustomer] NVARCHAR (450) NOT NULL,
    [Name]       NVARCHAR (200) NULL
);

ALTER TABLE [dbo].[Customer]
    ADD CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([IdCustomer] ASC);

/*******************************************************************************
									USER
********************************************************************************/
CREATE TABLE [dbo].[Users] (
    [IdUser]     INT            IDENTITY (1, 1) NOT NULL,
    [IdCustomer] NVARCHAR (450) NULL,
    [ClaimId]    NVARCHAR (450) NULL,
    [Name]       NVARCHAR (300) NULL,
    [Email]      NVARCHAR (200) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Users_IdCustomer]
    ON [dbo].[Users]([IdCustomer] ASC);


GO
ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([IdUser] ASC);


GO
ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [FK_Users_Customer_IdCustomer] FOREIGN KEY ([IdCustomer]) REFERENCES [dbo].[Customer] ([IdCustomer]);

/*******************************************************************************
									[CustomerServiceValue]
********************************************************************************/
CREATE TABLE [dbo].[CustomerServiceValue] (
    [IdServiceValue] INT            IDENTITY (1, 1) NOT NULL,
    [IdCustomer]     NVARCHAR (450) NULL,
    [ServiceType]    INT            NOT NULL,
    [Value]          FLOAT (53)     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerServiceValue_IdCustomer]
    ON [dbo].[CustomerServiceValue]([IdCustomer] ASC);


GO
ALTER TABLE [dbo].[CustomerServiceValue]
    ADD CONSTRAINT [PK_CustomerServiceValue] PRIMARY KEY CLUSTERED ([IdServiceValue] ASC);


GO
ALTER TABLE [dbo].[CustomerServiceValue]
    ADD CONSTRAINT [FK_CustomerServiceValue_Customer_IdCustomer] FOREIGN KEY ([IdCustomer]) REFERENCES [dbo].[Customer] ([IdCustomer]);
/*******************************************************************************
									[Catalog]
********************************************************************************/
CREATE TABLE [dbo].[Catalog] (
    [IdCatalog]   INT            IDENTITY (1, 1) NOT NULL,
    [Code]        NVARCHAR (100) NULL,
    [Description] NVARCHAR (300) NULL
);

ALTER TABLE [dbo].[Catalog]
    ADD CONSTRAINT [PK_Catalog] PRIMARY KEY CLUSTERED ([IdCatalog] ASC);
/*******************************************************************************
									[Baremo]
********************************************************************************/
CREATE TABLE [dbo].[Baremo] (
    [IdBaremo]        INT IDENTITY (1, 1) NOT NULL,
    [MalfunctionType] INT NOT NULL,
    [IntensityType]   INT NOT NULL
);

ALTER TABLE [dbo].[Baremo]
    ADD CONSTRAINT [PK_Baremo] PRIMARY KEY CLUSTERED ([IdBaremo] ASC);
/*******************************************************************************
									[[BaremoTime]]
********************************************************************************/
CREATE TABLE [dbo].[BaremoTime] (
    [IdBaremo]      INT        NOT NULL,
    [IdCatalog]     INT        NOT NULL,
    [ServiceType]   INT        NOT NULL,
    [ServiceTime]   FLOAT (53) NOT NULL,
    [MaterialValue] FLOAT (53) NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_BaremoTime_IdCatalog]
    ON [dbo].[BaremoTime]([IdCatalog] ASC);


GO
ALTER TABLE [dbo].[BaremoTime]
    ADD CONSTRAINT [PK_BaremoTime] PRIMARY KEY CLUSTERED ([IdBaremo] ASC, [IdCatalog] ASC, [ServiceType] ASC);


GO
ALTER TABLE [dbo].[BaremoTime]
    ADD CONSTRAINT [FK_BaremoTime_Baremo_IdBaremo] FOREIGN KEY ([IdBaremo]) REFERENCES [dbo].[Baremo] ([IdBaremo]) ON DELETE CASCADE;


GO
ALTER TABLE [dbo].[BaremoTime]
    ADD CONSTRAINT [FK_BaremoTime_Catalog_IdCatalog] FOREIGN KEY ([IdCatalog]) REFERENCES [dbo].[Catalog] ([IdCatalog]) ON DELETE CASCADE;
/*******************************************************************************
									[[Assessments]]
********************************************************************************/
CREATE TABLE [dbo].[Assessments] (
    [IdAssessment] INT            IDENTITY (1, 1) NOT NULL,
    [IdCustomer]   NVARCHAR (450) NULL,
    [Plate]        NVARCHAR (10)  NULL,
    [Model]        NVARCHAR (300) NULL,
    [Mileage]      NVARCHAR (100) NULL,
    [CustomerName] NVARCHAR (300) NULL,
    [BodyType]     NVARCHAR (100) NULL,
    [Total]        FLOAT (53)     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Assessments_IdCustomer]
    ON [dbo].[Assessments]([IdCustomer] ASC);


GO
ALTER TABLE [dbo].[Assessments]
    ADD CONSTRAINT [PK_Assessments] PRIMARY KEY CLUSTERED ([IdAssessment] ASC);


GO
ALTER TABLE [dbo].[Assessments]
    ADD CONSTRAINT [FK_Assessments_Customer_IdCustomer] FOREIGN KEY ([IdCustomer]) REFERENCES [dbo].[Customer] ([IdCustomer]);
/*******************************************************************************
									[[AssessmentServiceValues]]
********************************************************************************/
CREATE TABLE [dbo].[AssessmentServiceValues] (
    [IdAssessmentServiceValue] INT        IDENTITY (1, 1) NOT NULL,
    [IdAssessment]             INT        NOT NULL,
    [ServiceType]              INT        NOT NULL,
    [Value]                    FLOAT (53) NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_AssessmentServiceValues_IdAssessment]
    ON [dbo].[AssessmentServiceValues]([IdAssessment] ASC);


GO
ALTER TABLE [dbo].[AssessmentServiceValues]
    ADD CONSTRAINT [PK_AssessmentServiceValues] PRIMARY KEY CLUSTERED ([IdAssessmentServiceValue] ASC);


GO
ALTER TABLE [dbo].[AssessmentServiceValues]
    ADD CONSTRAINT [FK_AssessmentServiceValues_Assessments_IdAssessment] FOREIGN KEY ([IdAssessment]) REFERENCES [dbo].[Assessments] ([IdAssessment]) ON DELETE CASCADE;
/*******************************************************************************
									[[AdditionalServices]]
********************************************************************************/
CREATE TABLE [dbo].[AdditionalServices] (
    [IdAdditionalService] INT            IDENTITY (1, 1) NOT NULL,
    [IdCustomer]          NVARCHAR (450) NULL,
    [Description]         NVARCHAR (200) NULL,
    [Value]               FLOAT (53)     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_AdditionalServices_IdCustomer]
    ON [dbo].[AdditionalServices]([IdCustomer] ASC);


GO
ALTER TABLE [dbo].[AdditionalServices]
    ADD CONSTRAINT [PK_AdditionalServices] PRIMARY KEY CLUSTERED ([IdAdditionalService] ASC);


GO
ALTER TABLE [dbo].[AdditionalServices]
    ADD CONSTRAINT [FK_AdditionalServices_Customer_IdCustomer] FOREIGN KEY ([IdCustomer]) REFERENCES [dbo].[Customer] ([IdCustomer]);
/*******************************************************************************
									AssessmentAdditionalServices
********************************************************************************/
CREATE TABLE [dbo].[AssessmentAdditionalServices] (
    [IdAssessment]                  INT            NOT NULL,
    [IdAssessmentAdditionalService] INT            IDENTITY (1, 1) NOT NULL,
    [Description]                   NVARCHAR (200) NULL,
    [Value]                         FLOAT (53)     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_AssessmentAdditionalServices_IdAssessment]
    ON [dbo].[AssessmentAdditionalServices]([IdAssessment] ASC);


GO
ALTER TABLE [dbo].[AssessmentAdditionalServices]
    ADD CONSTRAINT [PK_AssessmentAdditionalServices] PRIMARY KEY CLUSTERED ([IdAssessmentAdditionalService] ASC);


GO
ALTER TABLE [dbo].[AssessmentAdditionalServices]
    ADD CONSTRAINT [FK_AssessmentAdditionalServices_Assessments_IdAssessment] FOREIGN KEY ([IdAssessment]) REFERENCES [dbo].[Assessments] ([IdAssessment]) ON DELETE CASCADE;

/*******************************************************************************
									Parts
********************************************************************************/
CREATE TABLE [dbo].[Parts] (
    [IdPart]          INT            IDENTITY (1, 1) NOT NULL,
    [IdAssessment]    INT            NOT NULL,
    [Code]            NVARCHAR (100) NULL,
    [Description]     NVARCHAR (250) NULL,
    [MalfunctionType] INT            NOT NULL,
    [IntensityType]   INT            NOT NULL,
    [UnitaryValue]    FLOAT (53)     NOT NULL,
    [Quantity]        INT            NOT NULL,
    [TotalPrice]      FLOAT (53)     NOT NULL,
    [TotalService]    FLOAT (53)     NOT NULL,
    [TotalMaterial]   FLOAT (53)     NOT NULL,
    [Total]           FLOAT (53)     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Parts_IdAssessment]
    ON [dbo].[Parts]([IdAssessment] ASC);


GO
ALTER TABLE [dbo].[Parts]
    ADD CONSTRAINT [PK_Parts] PRIMARY KEY CLUSTERED ([IdPart] ASC);


GO
ALTER TABLE [dbo].[Parts]
    ADD CONSTRAINT [FK_Parts_Assessments_IdAssessment] FOREIGN KEY ([IdAssessment]) REFERENCES [dbo].[Assessments] ([IdAssessment]) ON DELETE CASCADE;
/*******************************************************************************
									[Services]
********************************************************************************/
CREATE TABLE [dbo].[Services] (
    [IdService]     INT        IDENTITY (1, 1) NOT NULL,
    [IdPart]        INT        NOT NULL,
    [ServiceType]   INT        NOT NULL,
    [Time]          FLOAT (53) NOT NULL,
    [ValuePerHour]  FLOAT (53) NOT NULL,
    [Value]         FLOAT (53) NOT NULL,
    [MaterialValue] FLOAT (53) NOT NULL,
    [Total]         FLOAT (53) NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Services_IdPart]
    ON [dbo].[Services]([IdPart] ASC);


GO
ALTER TABLE [dbo].[Services]
    ADD CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED ([IdService] ASC);


GO
ALTER TABLE [dbo].[Services]
    ADD CONSTRAINT [FK_Services_Parts_IdPart] FOREIGN KEY ([IdPart]) REFERENCES [dbo].[Parts] ([IdPart]) ON DELETE CASCADE;

