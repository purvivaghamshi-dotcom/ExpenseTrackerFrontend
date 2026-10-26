
IF OBJECT_ID('dbo.SplitParticipants', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SplitParticipants
    (
        ParticipantId INT IDENTITY(1,1) PRIMARY KEY,
        SplitId INT NOT NULL,
        PersonName NVARCHAR(100) NOT NULL,
        ShareAmount DECIMAL(18,2) NOT NULL,
        PaymentStatus NVARCHAR(20) NOT NULL
            CONSTRAINT DF_SplitParticipants_Status DEFAULT 'Pending'
    );
END;
GO

IF OBJECT_ID('dbo.ExpenseSplits', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExpenseSplits
    (
        SplitId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        ExpenseName NVARCHAR(150) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        NumberOfPeople INT NOT NULL,
        CreatedAt DATETIME NOT NULL
            CONSTRAINT DF_ExpenseSplits_CreatedAt DEFAULT GETDATE()
    );
END;
GO
