SELECT
    LEN(PasswordHash) AS HashLength,

    CASE
        WHEN LEFT(PasswordHash, 6) = 'AQAAAA'
            THEN 'ASP.NET Identity-style prefix'
        WHEN LEN(PasswordHash) = 68
            THEN 'Possible System.Web.Helpers format'
        ELSE 'Other or incompatible format'
    END AS HashFormat,

    CASE
        WHEN RIGHT(PasswordHash, 2) = '=='
            THEN 'Ends with double padding'
        WHEN RIGHT(PasswordHash, 1) = '='
            THEN 'Ends with single padding'
        ELSE 'No Base64 padding at end'
    END AS PaddingStatus,

    CASE
        WHEN PasswordHash LIKE '%[^A-Za-z0-9+/=]%'
            THEN 'Contains non-Base64 characters'
        ELSE 'Contains only Base64 characters'
    END AS CharacterStatus

FROM dbo.Users
WHERE Email = 'purvi@gmail.com';