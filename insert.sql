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
	, [Username])
VALUES('test@test.com', 0, 'BqPuISzor2MSmmoheGLo5Wr+Tps1VZ2OHfzc3ll0n2Gj4EArJtDWRHm6/vDUOpu9fc2Al91urB5GNbgzabahaw==', 'ca1587f1-4eea-4f41-aad5-4f95bb8f9ae9', 'admin')
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
