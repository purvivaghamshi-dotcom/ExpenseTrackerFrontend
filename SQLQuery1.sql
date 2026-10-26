-- USER ACCOUNTS
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        Role NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Users_Role DEFAULT 'User',
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME()
    );
END;
GO

-- INCOME AND EXPENSE TRANSACTIONS
IF OBJECT_ID('dbo.Transactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transactions
    (
        TransactionId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        TransactionType NVARCHAR(10) NOT NULL,
        Title NVARCHAR(150) NOT NULL,
        Amount DECIMAL(12,2) NOT NULL,
        TransactionDate DATE NOT NULL,
        Notes NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Transactions_CreatedAt DEFAULT SYSDATETIME(),

        CONSTRAINT FK_Transactions_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),

        CONSTRAINT CK_Transactions_Type
            CHECK (TransactionType IN ('Income', 'Expense')),

        CONSTRAINT CK_Transactions_Amount
            CHECK (Amount > 0)
    );
END;
GO

-- USER BUDGETS
IF OBJECT_ID('dbo.Budgets', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Budgets
    (
        BudgetId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        BudgetName NVARCHAR(100) NOT NULL,
        BudgetAmount DECIMAL(12,2) NOT NULL,
        MonthNumber INT NOT NULL,
        YearNumber INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Budgets_CreatedAt DEFAULT SYSDATETIME(),

        CONSTRAINT FK_Budgets_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),

        CONSTRAINT CK_Budgets_Amount
            CHECK (BudgetAmount > 0),

        CONSTRAINT CK_Budgets_Month
            CHECK (MonthNumber BETWEEN 1 AND 12)
    );
END;
GO

-- VERIFY TABLES
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO