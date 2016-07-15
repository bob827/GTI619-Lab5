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
GO;

CREATE TABLE [dbo].[Roles]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RoleName] NVARCHAR(255) NOT NULL, 
    CONSTRAINT [UK_Roles_RoleName] UNIQUE ([RoleName])
);
GO;

CREATE TABLE [dbo].[Users] (
    [Id]                            INT            IDENTITY (1, 1) NOT NULL,
    [Username]                      NVARCHAR (255) NOT NULL,
    [Email]                         NVARCHAR (255) NOT NULL,
    [PasswordHash]                  NVARCHAR (88)  NOT NULL,
    [PasswordSalt]                  NVARCHAR (36)  NOT NULL,
    [IsLocked]                      BIT            NOT NULL,
    [TimeoutEndDate]                DATETIME       NULL,
    [PasswordExpirationDate]        DATETIME       NULL,
    [MustChangePasswordAtNextLogon] BIT            DEFAULT ((0)) NOT NULL,
    [DefaultPasswordValidUntil] DATETIME NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_Users_PasswordAndSalt] UNIQUE NONCLUSTERED ([PasswordHash] ASC, [PasswordSalt] ASC),
    CONSTRAINT [UK_Users_Username] UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [UK_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);
GO;

CREATE TABLE [dbo].[UserRoles]
(
	[UserId] INT NOT NULL , 
    [RoleId] INT NOT NULL, 
    PRIMARY KEY ([UserId], [RoleId]), 
    CONSTRAINT [FK_UserRoles_ToRoles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]),
    CONSTRAINT [FK_UserRoles_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);
GO;

CREATE TABLE [dbo].[PasswordHistory] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [UserId]       INT           NOT NULL,
    [PasswordHash] NVARCHAR (88) NOT NULL,
    [PasswordSalt] NVARCHAR (36) NOT NULL,
    [DateChanged]  DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_PasswordHistory_PasswordAndSalt] UNIQUE NONCLUSTERED ([PasswordHash] ASC, [PasswordSalt] ASC),
    CONSTRAINT [FK_PasswordHistory_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
GO;

CREATE TABLE [dbo].[LoginAttempts] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [UserId]          INT            NULL,
    [Date]            DATETIME       NOT NULL,
    [IsSuccessful]    BIT            NOT NULL,
    [ClientIpAddress] NVARCHAR (45)  NULL,
    [ClientUserAgent] NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LoginAttempts_ToUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
GO;

