/* ============================================================================
   SWIJIT ACCOUNTING DATABASE MASTER SETUP SCRIPT
   Re-runnable: Drops and recreates database, tables, views, procedures, seed data
   ============================================================================ */

USE master;
GO

IF DB_ID('[SJP.Accounting]') IS NOT NULL
BEGIN
    ALTER DATABASE [SJP.Accounting] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [SJP.Accounting];
END
GO

CREATE DATABASE [SJP.Accounting];
GO

USE [SJP.Accounting];
GO

/* DROP OBJECTS */

IF OBJECT_ID('dbo.vw_SettlementStatus','V') IS NOT NULL DROP VIEW dbo.vw_SettlementStatus;
IF OBJECT_ID('dbo.vw_PartnerCapitalPosition','V') IS NOT NULL DROP VIEW dbo.vw_PartnerCapitalPosition;
IF OBJECT_ID('dbo.vw_ProjectProfitability','V') IS NOT NULL DROP VIEW dbo.vw_ProjectProfitability;
IF OBJECT_ID('dbo.vw_Dashboard','V') IS NOT NULL DROP VIEW dbo.vw_Dashboard;
IF OBJECT_ID('dbo.vw_AllTransactions','V') IS NOT NULL DROP VIEW dbo.vw_AllTransactions;
IF OBJECT_ID('vw_SettlementRecommendation','V') IS NOT NULL DROP VIEW dbo.vw_SettlementRecommendation;
GO

IF OBJECT_ID('dbo.Asset','U') IS NOT NULL DROP TABLE dbo.Asset;
IF OBJECT_ID('dbo.TransactionMaster','U') IS NOT NULL DROP TABLE dbo.TransactionMaster;
IF OBJECT_ID('dbo.Category','U') IS NOT NULL DROP TABLE dbo.Category;
IF OBJECT_ID('dbo.Project','U') IS NOT NULL DROP TABLE dbo.Project;
IF OBJECT_ID('dbo.TransactionType','U') IS NOT NULL DROP TABLE dbo.TransactionType;
IF OBJECT_ID('dbo.Partner','U') IS NOT NULL DROP TABLE dbo.Partner;
IF OBJECT_ID('dbo.Entity','U') IS NOT NULL DROP TABLE dbo.Entity;
GO

CREATE TABLE dbo.Partner(
 PartnerId INT IDENTITY(1,1) PRIMARY KEY,
 PartnerName NVARCHAR(100) NOT NULL,
 ProfitSharePercentage DECIMAL(5,2) NOT NULL,
 IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.TransactionType(
 TransactionTypeId INT PRIMARY KEY,
 TransactionTypeName NVARCHAR(50) NOT NULL
);
GO

ALTER TABLE dbo.TransactionType ADD CONSTRAINT UQ_TransactionType_Name UNIQUE(TransactionTypeName);
GO

CREATE TABLE dbo.Project(
 ProjectId INT IDENTITY(1,1) PRIMARY KEY,
 ProjectName NVARCHAR(200) NOT NULL,
 IsActive BIT NOT NULL DEFAULT 1
);
GO

ALTER TABLE dbo.Project ADD CONSTRAINT UQ_Project_Name UNIQUE(ProjectName);
GO

CREATE TABLE dbo.Category(
 CategoryId INT IDENTITY(1,1) PRIMARY KEY,
 CategoryName NVARCHAR(100) NOT NULL
);
GO

ALTER TABLE dbo.Category ADD CONSTRAINT UQ_Category_Name UNIQUE(CategoryName);
GO

CREATE TABLE dbo.Entity
(
 EntityId INT IDENTITY(1,1) PRIMARY KEY,
 EntityType NVARCHAR(50) NOT NULL,
 EntityName NVARCHAR(200) NOT NULL,
 IsActive BIT NOT NULL DEFAULT 1
);
GO

ALTER TABLE dbo.Entity ADD CONSTRAINT UQ_Entity_Name UNIQUE(EntityName);
ALTER TABLE dbo.Entity ADD CONSTRAINT CK_Entity_Type CHECK (
    EntityType IN ('Partner', 'Company', 'Client','Vendor')
);
GO

CREATE TABLE dbo.TransactionMaster(
 TransactionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
 TransactionHash VARCHAR(64) NOT NULL,
 TransactionDate DATE NOT NULL,
 ProjectId INT NULL,
 CategoryId INT NULL,
 TransactionTypeId INT NOT NULL,
 Amount DECIMAL(18,2) NOT NULL,
 PaidByEntityId INT NULL,
 ReceivedByEntityId INT NULL,
 Narration NVARCHAR(2000) NULL,
 GoogleDriveLink NVARCHAR(2000) NULL,
 ImportedOn DATETIME2 NOT NULL,
 CONSTRAINT FK_TM_Project FOREIGN KEY(ProjectId) REFERENCES dbo.Project(ProjectId),
 CONSTRAINT FK_TM_Category FOREIGN KEY(CategoryId) REFERENCES dbo.Category(CategoryId),
 CONSTRAINT FK_TM_Type FOREIGN KEY(TransactionTypeId) REFERENCES dbo.TransactionType(TransactionTypeId),
 CONSTRAINT FK_TM_PaidBy FOREIGN KEY(PaidByEntityId) REFERENCES dbo.Entity(EntityId),
 CONSTRAINT FK_TM_ReceivedBy FOREIGN KEY(ReceivedByEntityId) REFERENCES dbo.Entity(EntityId)
);
GO

CREATE UNIQUE INDEX UX_TransactionHash ON dbo.TransactionMaster(TransactionHash);
GO

CREATE TABLE dbo.Asset(
 AssetId INT IDENTITY(1,1) PRIMARY KEY,
 TransactionId UNIQUEIDENTIFIER NOT NULL,
 AssetName NVARCHAR(200) NOT NULL,
 PurchaseValue DECIMAL(18,2) NOT NULL,
 PurchaseDate DATE NOT NULL,
 AssetStatus NVARCHAR(50) NOT NULL DEFAULT 'Active',
 CONSTRAINT FK_Asset_Transaction FOREIGN KEY(TransactionId)
 REFERENCES dbo.TransactionMaster(TransactionId)
);
GO

/* SEED DATA */
INSERT INTO dbo.Partner(PartnerName,ProfitSharePercentage)
VALUES ('Bijit',40),('Swathi',60);

INSERT INTO dbo.TransactionType VALUES
(1,'Investment'),
(2,'Income'),
(3,'Expense'),
(4,'AssetPurchase'),
(5,'Withdrawal'),
(6,'Settlement');

INSERT INTO dbo.Project(ProjectName)
VALUES ('Company Maintenance'),('S-factor'),('Ms Bengals Valentine');

INSERT INTO dbo.Category(CategoryName)
VALUES
('Venue'),('Food'),('Travel'),('Marketing'),('Equipment'),
('Maintenance'),('Tax'),('Vendor'),('ClientPayment'),('Room Rent'),
('Settlement'),('Withdrawal'),('Investment'),('General'),
('Photography'),('Videography'),('Video Editing'),('Graphics Design'),('Stationary'),('Printing');

INSERT INTO dbo.Entity (EntityType, EntityName)
VALUES ('Partner','Bijit'), ('Partner','Swathi'), ('Company','Swijit Productions');
GO

CREATE VIEW dbo.vw_AllTransactions AS
SELECT t.TransactionId,t.TransactionDate,p.ProjectName,c.CategoryName,
       tt.TransactionTypeName,t.Amount,
       ep.EntityName AS PaidBy,
       er.EntityName AS ReceivedBy,
       t.Narration,t.GoogleDriveLink,
       t.ImportedOn
FROM dbo.TransactionMaster t
LEFT JOIN dbo.Project p ON p.ProjectId=t.ProjectId
LEFT JOIN dbo.Category c ON c.CategoryId=t.CategoryId
LEFT JOIN dbo.TransactionType tt ON tt.TransactionTypeId=t.TransactionTypeId
LEFT JOIN dbo.Entity ep ON ep.EntityId = t.PaidByEntityId
LEFT JOIN dbo.Entity er ON er.EntityId = t.ReceivedByEntityId;
GO

CREATE VIEW dbo.vw_ProjectProfitability AS
SELECT p.ProjectName,
SUM(CASE WHEN t.TransactionTypeId=2 THEN t.Amount ELSE 0 END) Income,
SUM(CASE WHEN t.TransactionTypeId=3 THEN t.Amount ELSE 0 END) Expense,
SUM(CASE WHEN t.TransactionTypeId=2 THEN t.Amount ELSE 0 END)
- SUM(CASE WHEN t.TransactionTypeId=3 THEN t.Amount ELSE 0 END) ProfitLoss
FROM dbo.Project p
LEFT JOIN dbo.TransactionMaster t ON p.ProjectId=t.ProjectId
GROUP BY p.ProjectName;
GO

CREATE VIEW dbo.vw_PartnerCapitalPosition
AS

WITH PartnerBase AS
(
    SELECT PartnerId, PartnerName
    FROM dbo.Partner
),
PartnerContribution AS
(
    SELECT
        p.PartnerName,

        SUM(
            CASE
                WHEN tm.TransactionTypeId = 1
                     AND ePaid.EntityName = p.PartnerName
                THEN tm.Amount ELSE 0
            END
        ) AS Investment,

        SUM(
            CASE
                WHEN tm.TransactionTypeId = 3
                     AND ePaid.EntityName = p.PartnerName
                THEN tm.Amount ELSE 0
            END
        ) AS ExpenseFunding,

        SUM(
            CASE
                WHEN tm.TransactionTypeId = 4
                     AND ePaid.EntityName = p.PartnerName
                THEN tm.Amount ELSE 0
            END
        ) AS AssetFunding,

        SUM(
            CASE
                WHEN tm.TransactionTypeId = 5
                     AND eReceived.EntityName = p.PartnerName
                THEN tm.Amount ELSE 0
            END
        ) AS Withdrawal,

        SUM(
            CASE
                WHEN tm.TransactionTypeId = 6
                     AND ePaid.EntityName = p.PartnerName
                THEN tm.Amount ELSE 0
            END
        ) AS SettlementPaid,

        SUM(
            CASE
                WHEN tm.TransactionTypeId = 6
                     AND eReceived.EntityName = p.PartnerName
                THEN tm.Amount ELSE 0
            END
        ) AS SettlementReceived

    FROM PartnerBase p

    CROSS JOIN dbo.TransactionMaster tm

    LEFT JOIN dbo.Entity ePaid
        ON tm.PaidByEntityId = ePaid.EntityId

    LEFT JOIN dbo.Entity eReceived
        ON tm.ReceivedByEntityId = eReceived.EntityId

    GROUP BY p.PartnerName
)

SELECT

    PartnerName AS Partner,
    Investment,
    ExpenseFunding,
    AssetFunding,
    Withdrawal,
    SettlementPaid,
    SettlementReceived,
    (
        Investment
        + ExpenseFunding
        + AssetFunding
        - Withdrawal
        + SettlementPaid
        - SettlementReceived
    ) AS Contribution

FROM PartnerContribution;
GO

CREATE OR ALTER VIEW dbo.vw_Dashboard
AS

WITH DashboardData AS
(
    SELECT
        ISNULL(SUM(CASE WHEN TransactionTypeId = 2 THEN Amount END), 0)
            AS TotalIncome,

        ISNULL(SUM(CASE WHEN TransactionTypeId = 3 THEN Amount END), 0)
            AS TotalExpense,

        ISNULL(SUM(CASE WHEN TransactionTypeId = 4 THEN Amount END), 0)
            AS TotalAssetPurchase
    FROM dbo.TransactionMaster
),

ContributionData AS
(
    SELECT
        ISNULL(SUM(Contribution), 0)
            AS TotalContribution
    FROM dbo.vw_PartnerCapitalPosition
)

SELECT

    c.TotalContribution,

    d.TotalIncome,

    d.TotalExpense,

    d.TotalAssetPurchase,

    d.TotalIncome
    - d.TotalExpense
        AS NetProfitLoss,

    c.TotalContribution
    + d.TotalIncome
    - d.TotalExpense
    - d.TotalAssetPurchase
        AS FundBalance

FROM DashboardData d
CROSS JOIN ContributionData c;
GO

CREATE VIEW dbo.vw_SettlementStatus
AS

WITH ContributionData AS
(
    SELECT
        Partner,
        Contribution
    FROM dbo.vw_PartnerCapitalPosition
),

TotalContribution AS
(
    SELECT
        SUM(Contribution) AS TotalContributionPool
    FROM ContributionData
),

ProfitData AS
(
    SELECT
        NetProfitLoss AS TotalProfitLoss
    FROM dbo.vw_Dashboard
)

SELECT

    cd.Partner,

    CASE
        WHEN cd.Partner = 'Bijit'
            THEN 40.00
        ELSE 60.00
    END AS OwnershipPercentage,

    tc.TotalContributionPool,

    cd.Contribution
        AS ActualContribution,

    ROUND(
        cd.Contribution
        * 100.0
        / tc.TotalContributionPool,
        2
    ) AS ContributionPercentage,

    CASE
        WHEN cd.Partner = 'Bijit'
            THEN tc.TotalContributionPool * 0.40

        ELSE tc.TotalContributionPool * 0.60
    END AS ExpectedContribution,

    cd.Contribution -

    CASE
        WHEN cd.Partner = 'Bijit'
            THEN tc.TotalContributionPool * 0.40

        ELSE tc.TotalContributionPool * 0.60
    END
    AS ContributionVariance,

    pd.TotalProfitLoss,

    CASE
        WHEN cd.Partner = 'Bijit'
            THEN pd.TotalProfitLoss * 0.40

        ELSE pd.TotalProfitLoss * 0.60
    END AS ExpectedProfitShare,

    cd.Contribution +

    CASE
        WHEN cd.Partner = 'Bijit'
            THEN pd.TotalProfitLoss * 0.40

        ELSE pd.TotalProfitLoss * 0.60
    END AS CapitalPosition,

    CASE
        WHEN
            (
                cd.Contribution -

                CASE
                    WHEN cd.Partner = 'Bijit'
                        THEN tc.TotalContributionPool * 0.40

                    ELSE tc.TotalContributionPool * 0.60
                END
            ) > 0
        THEN 'Over Funded'

        WHEN
            (
                cd.Contribution -

                CASE
                    WHEN cd.Partner = 'Bijit'
                        THEN tc.TotalContributionPool * 0.40

                    ELSE tc.TotalContributionPool * 0.60
                END
            ) < 0
        THEN 'Under Funded'

        ELSE 'Balanced'
    END AS FundingStatus,

    CASE
        WHEN
            (
                cd.Contribution -

                CASE
                    WHEN cd.Partner = 'Bijit'
                        THEN tc.TotalContributionPool * 0.40

                    ELSE tc.TotalContributionPool * 0.60
                END
            ) > 0
        THEN 'Receive'

        WHEN
            (
                cd.Contribution -

                CASE
                    WHEN cd.Partner = 'Bijit'
                        THEN tc.TotalContributionPool * 0.40

                    ELSE tc.TotalContributionPool * 0.60
                END
            ) < 0
        THEN 'Pay'

        ELSE 'None'
    END AS SettlementDirection,

    ABS(
        cd.Contribution -

        CASE
            WHEN cd.Partner = 'Bijit'
                THEN tc.TotalContributionPool * 0.40

            ELSE tc.TotalContributionPool * 0.60
        END
    ) AS SettlementAmount

FROM ContributionData cd
CROSS JOIN TotalContribution tc
CROSS JOIN ProfitData pd;
GO



CREATE VIEW dbo.vw_SettlementRecommendation
AS

SELECT TOP 1

    SettlementAmount,

    MAX(
        CASE
            WHEN SettlementDirection = 'Pay'
            THEN Partner
        END
    ) OVER() AS PayingPartner,

    MAX(
        CASE
            WHEN SettlementDirection = 'Receive'
            THEN Partner
        END
    ) OVER() AS ReceivingPartner

FROM dbo.vw_SettlementStatus

WHERE SettlementAmount > 0;
GO