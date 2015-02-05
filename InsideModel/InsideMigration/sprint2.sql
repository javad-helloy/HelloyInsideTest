
BEGIN TRAN ChangeIdValues;
SET IDENTITY_INSERT dbo.Client ON;

Alter table [dbo].[InsideUser]
	drop [FK_dbo.InsideUser_dbo.Client_ClientId]

UPDATE [dbo].[InsideUser]
SET [dbo].[InsideUser].ClientId = [dbo].[Client].ClientId
FROM [dbo].[InsideUser]
INNER JOIN [dbo].[Client] ON [dbo].[InsideUser].ClientId = [dbo].[Client].Id

CREATE TABLE #temp_copy(
	[Id] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Domain] [nvarchar](max) NULL,
	[AnalyticsTableId] [nvarchar](max) NULL,
	[ClientId] [int] NOT NULL,
	[EmailAddress] [nvarchar](max) NULL,
	[CallTrackingMetricId] [int] NULL,
	[ConsultantId] [int] NOT NULL,
	[Address] [nvarchar](max) NULL,
	[Longitude] [decimal](18, 9) NULL,
	[Latitude] [decimal](18, 9) NULL)

Insert INTO #temp_copy SELECT * FROM [dbo].[Client];

DELETE FROM [dbo].[Client]

INSERT INTO [dbo].[Client]
			([ID],[Name],[Domain] ,[AnalyticsTableId],[ClientId],[EmailAddress],[CallTrackingMetricId],[ConsultantId],[Address],[Longitude],[Latitude])
SELECT #temp_copy.ClientId, #temp_copy.Name, #temp_copy.Domain, #temp_copy.AnalyticsTableId,#temp_copy.ClientId,#temp_copy.EmailAddress,#temp_copy.CallTrackingMetricId,#temp_copy.ConsultantId,#temp_copy.[Address],#temp_copy.Longitude,#temp_copy.Latitude FROM #temp_copy

drop table #temp_copy;

Alter table [dbo].[InsideUser]
	add constraint [FK_dbo.InsideUser_dbo.Client_Id] FOREIGN KEY ( ClientId ) references [dbo].[Client] (Id)

Alter table [dbo].[Client]
drop column ClientId

Commit TRAN ChangeIdValues;
SET IDENTITY_INSERT dbo.Client OFF;
