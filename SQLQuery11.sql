SELECT
    TransactionType,
    SUM(Amount) AS TotalAmount
FROM dbo.Transactions
GROUP BY TransactionType;