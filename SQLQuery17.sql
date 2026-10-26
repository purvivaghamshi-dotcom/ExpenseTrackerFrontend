SELECT
    UserId,
    FullName,
    Email,
    Role,
    LEN(PasswordHash) AS HashLength
FROM dbo.Users
WHERE Email = 'heet@gmail.com';