CREATE TABLE [dbo].[AdminOptions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [MinPasswordLength] INT NOT NULL, 
    [IsUpperCaseCharacterRequired] BIT NOT NULL, 
    [IsLowerCaseCharacterRequired] BIT NOT NULL, 
    [IsNumberRequired] BIT NOT NULL, 
    [IsSpecialCharacterRequired] BIT NOT NULL, 
    [MaxLoginAttempt] INT NOT NULL, 
    [TimeoutAfterMaxLoginReachedInMinutes] INT NOT NULL, 
    [PasswordExpirationDurationInDays] INT NOT NULL, 
    [NumberOfPasswordToKeepInHistory] INT NOT NULL
);

CREATE TABLE [dbo].[Roles]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RoleName] NVARCHAR(255) NOT NULL, 
    CONSTRAINT [UK_Roles_RoleName] UNIQUE ([RoleName])
);

CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Username] NVARCHAR(255) NOT NULL, 
    [Email] NVARCHAR(255) NOT NULL, 
    [PasswordHash] NVARCHAR(128) NOT NULL, 
    [PasswordSalt] NVARCHAR(128) NOT NULL, 
    [IsLocked] BIT NOT NULL, 
    [TimeoutEndDate] DATETIME NULL, 
    [PasswordExpirationDate] DATETIME NULL, 
    CONSTRAINT [UK_Users_PasswordAndSalt] UNIQUE ([PasswordHash], [PasswordSalt]), 
    CONSTRAINT [UK_Users_Username] UNIQUE ([Username]),
    CONSTRAINT [UK_Users_Email] UNIQUE ([Email])
);

CREATE TABLE [dbo].[UserRoles]
(
	[UserId] INT NOT NULL , 
    [RoleId] INT NOT NULL, 
    PRIMARY KEY ([UserId], [RoleId]), 
    CONSTRAINT [FK_UserRoles_ToRoles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]),
    CONSTRAINT [FK_UserRoles_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);

CREATE TABLE [dbo].[PasswordHistory]
(
	[Id] INT NOT NULL PRIMARY KEY  IDENTITY, 
    [UserId] INT NOT NULL,
    [PasswordHash] NVARCHAR(128) NOT NULL, 
    [PasswordSalt] NVARCHAR(128) NOT NULL, 
    CONSTRAINT [UK_PasswordHistory_PasswordAndSalt] UNIQUE ([PasswordHash], [PasswordSalt]),
    CONSTRAINT [FK_PasswordHistory_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);

CREATE TABLE [dbo].[AuthenticationTokens] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [UserId]         INT              NOT NULL,
    [ExpirationDate] DATE             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_AuthenticationTokens_ToUser] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);

CREATE TABLE [dbo].[LoginAttemps]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [Date] DATETIME NOT NULL, 
    [IsSuccessful] BIT NOT NULL,
    CONSTRAINT [FK_LoginAttemps_ToUser] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);


