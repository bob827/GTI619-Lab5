INSERT INTO [dbo].[Roles] ([RoleName])
VALUES ('admin');
GO
INSERT INTO [dbo].[Roles] ([RoleName])
VALUES ('circle');
GO
INSERT INTO [dbo].[Roles] ([RoleName])
VALUES ('square');
GO

INSERT INTO [dbo].[Users] ([Email]
	, [IsLocked]
	, [PasswordHash]
	, [PasswordSalt]
	, [Username]
	, [HashingVersion]
	, [GridCardSeed])
VALUES('test@test.com', 0, 'BqPuISzor2MSmmoheGLo5Wr+Tps1VZ2OHfzc3ll0n2Gj4EArJtDWRHm6/vDUOpu9fc2Al91urB5GNbgzabahaw==', 'ca1587f1-4eea-4f41-aad5-4f95bb8f9ae9', 'admin', 'SHA512', 0)
GO

INSERT INTO [dbo].[UserRoles]([RoleId], [UserId])
SELECT r.[Id], u.[Id]
FROM [dbo].[Roles] r, [dbo].[Users] u
WHERE r.RoleName = 'admin' and u.[Username] = 'admin'
GO

INSERT INTO [dbo].[AdminOptions] ([IsLowerCaseCharacterRequired]
	, [IsNumberRequired]
	, [IsSpecialCharacterRequired]
	, [IsUpperCaseCharacterRequired]
	, [MaxLoginAttempt]
	, [MinPasswordLength]
	, [NumberOfPasswordToKeepInHistory]
	, [PasswordExpirationDurationInDays]
	, [TimeoutAfterMaxLoginReachedInMinutes])
VALUES (0, 0, 0, 0, 10, 10, 3, 90, 1)
GO

INSERT INTO [dbo].[Users] ([Email]
	, [IsLocked]
	, [PasswordHash]
	, [PasswordSalt]
	, [Username]
	, [HashingVersion]
	, [GridCardSeed])
VALUES('test1@test.com', 0, '+oPae+pyZC3phETLO7oKix1Ug3hF5SeZIHjg6wz22fsqdEAbGc7B7lhayheYnTu2ys7cq77D8m8+OIzyDbBPgg==', '2cc06aa9-09fa-4e92-b624-47588c24aa8e', 'userCarre', 'SHA512', 61319)
GO

INSERT INTO [dbo].[Users] ([Email]
	, [IsLocked]
	, [PasswordHash]
	, [PasswordSalt]
	, [Username]
	, [HashingVersion]
	, [GridCardSeed])
VALUES('test2@test.com', 0, 'etYeFS/faV8iMs/Iup2wwm4p6rhPObhA5kbtVj6AEq3Fo/NV1Tn4RDX3UzgcNg4C2qGF3rBo9CuGBA5cWuolAQ==', 'fa7ad77f-dbba-497b-ba11-b4f0d59a1ddb', 'userCercle', 'SHA512', 37640)
GO

INSERT INTO [dbo].[UserRoles]([RoleId], [UserId])
SELECT r.[Id], u.[Id]
FROM [dbo].[Roles] r, [dbo].[Users] u
WHERE r.RoleName = 'square' and u.[Username] = 'userCarre'
GO
INSERT INTO [dbo].[UserRoles]([RoleId], [UserId])
SELECT r.[Id], u.[Id]
FROM [dbo].[Roles] r, [dbo].[Users] u
WHERE r.RoleName = 'circle' and u.[Username] = 'userCercle'
GO