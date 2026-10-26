SELECT TOP 10
    TransactionId,
    UserId,
    TransactionType,
    Title,
    Amount,
    TransactionDate
FROM dbo.Transactions
ORDER BY TransactionId DESC;