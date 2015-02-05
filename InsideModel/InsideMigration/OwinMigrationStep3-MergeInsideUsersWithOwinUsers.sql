ALTER TABLE [User] 
ADD ClientId int,
    ReceiveEmail bit,
	UserId int,
	Name nvarchar(max),
	Phone nvarchar(max),
	ImageUrl nvarchar(max);

GO

UPDATE
    [User]
SET
    [User].ClientId = InsideUser.ClientId,
	[User].ReceiveEmail = InsideUser.ReceiveEmail,
	[User].UserId = insideUser.Id

FROM
    [User]
INNER JOIN
    InsideUser
ON
    [User].Id = InsideUser.MembershipProviderId

GO

UPDATE
    [User]
SET
    [User].Name = [Admin].Name,
	[User].Phone = [Admin].Phone,
	[User].ImageUrl = [Admin].ImageUrl

FROM
    [User]
INNER JOIN
    [Admin]
ON
    [User].Id = [Admin].MembershipProviderId

GO

EXEC sp_rename '[dbo].Client.ConsultantId', 'ConsultantIdInt', 'COLUMN';
GO

EXEC sp_rename '[dbo].Client.AccountManagerId', 'AccountManagerIdInt', 'COLUMN';
GO

EXEC sp_rename '[dbo].Token.UserId', 'UserIdInt', 'COLUMN';
GO

ALTER TABLE [dbo].Client  DROP CONSTRAINT  [FK_dbo.Client_dbo.Admin_AccountManagerId]
GO

ALTER TABLE [dbo].Client  DROP CONSTRAINT  [FK_dbo.Client_dbo.Consultant_ConsultantId]
GO

alter table [dbo].Token drop Constraint FK_Token_InsideUser
Go

ALTER TABLE dbo.Token
ADD UserId nvarchar(128) NULL
Go

ALTER TABLE dbo.Client
ADD ConsultantId nvarchar(128) NULL
Go

ALTER TABLE dbo.Client
ADD AccountManagerId nvarchar(128) NULL
GO

UPDATE
    Client
SET
    Client.AccountManagerId = [Admin].MembershipProviderId

FROM
    Client
INNER JOIN
    [Admin]
ON
    Client.AccountManagerIdInt = [Admin].Id

GO
UPDATE
    Client
SET
    Client.ConsultantId = [Admin].MembershipProviderId

FROM
    Client
INNER JOIN
    [Admin]
ON
    Client.ConsultantIdInt = [Admin].Id
GO
UPDATE
    Token
SET
    Token.UserId = [User].Id

FROM
    Token
INNER JOIN
    [User]
ON
    Token.UserIdInt =[User].UserId
GO

alter table Client
ADD CONSTRAINT fk_Client_AccountManager
FOREIGN KEY (AccountManagerId)
REFERENCES dbo.[User](Id)
GO

alter table Client
ADD CONSTRAINT fk_Client_Consultant
FOREIGN KEY (ConsultantId)
REFERENCES dbo.[User](Id)
Go

ALTER TABLE dbo.Token
DROp Column UserIdInt
Go

alter table Token
ADD CONSTRAINT fk_token_User
FOREIGN KEY (UserId)
REFERENCES dbo.[User](Id)
Go


drop table InsideUser
Go

drop table [Admin]
Go