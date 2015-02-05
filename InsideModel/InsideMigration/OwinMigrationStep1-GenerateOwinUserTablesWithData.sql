IF OBJECT_ID('UserRole', 'U') IS NOT NULL
BEGIN
DROP TABLE UserRole;
END
Go

IF OBJECT_ID('UserClaim', 'U') IS NOT NULL
BEGIN
DROP TABLE UserClaim;
END
Go

IF OBJECT_ID('UserLogin', 'U') IS NOT NULL
BEGIN
DROP TABLE UserLogin;
END
Go

IF OBJECT_ID('Role', 'U') IS NOT NULL
BEGIN
DROP TABLE [Role];
END
Go

IF OBJECT_ID('User', 'U') IS NOT NULL
BEGIN
DROP TABLE [User];
END
Go

CREATE TABLE [dbo].[User] (
    [Id]            NVARCHAR (128) NOT NULL,
    [UserName]      NVARCHAR (255) NOT NULL,
    [PasswordHash]  NVARCHAR (MAX) NULL,
    [SecurityStamp] NVARCHAR (MAX) NULL,
    [LegacyPasswordHash]  NVARCHAR (MAX) NULL,
    [LoweredUserName]  NVARCHAR (256)   NOT NULL,
    [LastActivityDate] DATETIME2         NOT NULL,
    [Email]                                  NVARCHAR (256)   NULL,
    [IsApproved]                             BIT              NOT NULL,
    [IsLockedOut]                            BIT              NOT NULL,
    [CreateDate]                             DATETIME2	         NOT NULL,
    [LastLoginDate]                          DATETIME2         NOT NULL,
    [LastLoginFailed]                        DATETIME2         NOT NULL,
    [LoginFailureCounter]					 INT              NOT NULL,
    
    CONSTRAINT [PK_dbo.User] PRIMARY KEY CLUSTERED ([Id] ASC),
 );
Go

INSERT INTO [User](Id,UserName,PasswordHash,SecurityStamp,
LoweredUserName,LastActivityDate,LegacyPasswordHash,
Email,IsApproved,IsLockedOut,CreateDate,
LastLoginDate,LastLoginFailed,LoginFailureCounter)
SELECT aspnet_Users.UserId,aspnet_Users.UserName,(aspnet_Membership.Password+'|'+CAST(aspnet_Membership.PasswordFormat as varchar)+'|'+aspnet_Membership.PasswordSalt),NewID(),
aspnet_Users.LoweredUserName,aspnet_Users.LastActivityDate,aspnet_Membership.Password,
aspnet_Membership.Email,aspnet_Membership.IsApproved,aspnet_Membership.IsLockedOut,aspnet_Membership.CreateDate,aspnet_Membership.LastLoginDate,
aspnet_Membership.LastLockoutDate,aspnet_Membership.FailedPasswordAttemptCount
FROM dbo.aspnet_Users
LEFT OUTER JOIN dbo.aspnet_Membership ON aspnet_Membership.ApplicationId = aspnet_Users.ApplicationId 
AND aspnet_Users.UserId = aspnet_Membership.UserId;
Go

ALTER TABLE [User]
ADD CONSTRAINT Unique_UserName UNIQUE (UserName)
Go

CREATE TABLE [dbo].[Role] (
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
);
Go

INSERT INTO [Role](Id,Name)
SELECT RoleId,RoleName
FROM dbo.aspnet_Roles; 
Go

CREATE TABLE [dbo].[UserRole] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.UserRole_dbo.Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserRole_dbo.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
);
Go

INSERT INTO UserRole(UserId,RoleId)
SELECT UserId,RoleId
FROM dbo.aspnet_UsersInRoles;
Go

CREATE TABLE [dbo].[UserClaim] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    [User_Id]    NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.UserClaim] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserClaim_dbo.User_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[UserClaim]([User_Id] ASC);

CREATE TABLE [dbo].[UserLogin] (
    [UserId]        NVARCHAR (128) NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.UserLogin] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_dbo.UserLogin_dbo.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserLogin]([UserId] ASC);
Go