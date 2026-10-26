UPDATE dbo.Users
SET Role = 'Admin'
WHERE Email = 'vaghamshi.dilipbhai126547@marwadiuniversity.ac.in';
SELECT UserId, FullName, Email, Role
FROM dbo.Users
WHERE Email = 'vaghamshi.dilipbhai126547@marwadiuniversity.ac.in';