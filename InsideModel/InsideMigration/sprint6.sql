/****** Object:  Table [dbo].[Chat]    Script Date: 2013-12-02 11:35:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Chat](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Phone] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
 CONSTRAINT [PK_Chat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO

ALTER TABLE [dbo].[Chat]  WITH CHECK ADD  CONSTRAINT [FK_Chat_Client] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([Id])
GO

ALTER TABLE [dbo].[Chat] CHECK CONSTRAINT [FK_Chat_Client]
GO

/* CHANGE CONSULTANT TABLE NAME TO ADMINS AND ADD APPROPRIATE COLUMNS*/
sp_rename Consultant, [Admin]

Go

alter table [Admin] add Email nvarchar(max);

Go 

alter table [Admin] add Phone nvarchar(max);

Go 

alter table [Admin] add AdminRole nvarchar(max);

Go 

alter table [Admin] add ImageUrl nvarchar(max);

Go 

Update [Admin] set AdminRole='Consultant', ImageUrl='/Content/images/Employees/peter.jpg', Phone='0730-455 55 55', Email='peter@helloy.se' Where Name='Peter Weibull';
Update [Admin] set AdminRole='Consultant', ImageUrl='/Content/images/Employees/julia.jpg', Phone='0730-32 72 69', Email='julia.servenius@helloy.se' Where Name='Julia Servenius';
Update [Admin] set AdminRole='Consultant', ImageUrl='/Content/images/Employees/henrik.jpg', Phone='0761-86 14 11', Email='henrik.nilsson@helloy.se' Where Name='Henrik Nilsson';

Go
alter table [Admin] alter column MembershipProviderId uniqueidentifier NULL;

Go

alter table [Admin] alter column AdminRole nvarchar(max) NOT NULL;

Go

/*ADD FREIGN KEY FOR CLIENTS TO MANAGER*/
ALTER TABLE [dbo].[Client] ADD AccountManagerId INT
GO

ALTER TABLE [dbo].[Client]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.Client_dbo.Admin_AccountManagerId] FOREIGN KEY(AccountManagerId)
REFERENCES [dbo].[Admin] ([Id])
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[Client] CHECK CONSTRAINT [FK_dbo.Client_dbo.Admin_AccountManagerId]
GO

INSERT INTO [dbo].[Admin]
           ([Name]
           ,[MembershipProviderId]
           ,[Email]
           ,[Phone]
           ,[AdminRole]
           ,[ImageUrl])
     VALUES
           ('Sebastian Gomes', NULL,'sebastian.gomes@helloy.se','0704-04 83 04','AccountManager','/Content/images/Employees/sebastian.jpg'),
		   ('Lisa Skoglund', NULL,'lisa.skoglund@helloy.se','0735-41 27 73','AccountManager','/Content/images/Employees/lisa.jpg'),
		   ('Oscar Wilkens', NULL,'oscar.wilkens@helloy.se','0760-25 83 79','AccountManager','/Content/images/Employees/oscar.jpg'),
		   ('Patrik Wester', NULL,'patrik.wester@helloy.se','0763-25 21 14','AccountManager','/Content/images/Employees/patrik.jpg')
GO

/*MIGRATE THE DATA FROM EXCEL DATA SHEETS TO CLIENT TABLE*/
--Peter Weibull	2
--Julia Servenius	3
--Henrik Nilsson	4
--Sebastian Gomes	5
--Lisa Skoglund	6
--Oscar Wilkens	7
--Patrik Wester	8



UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	1	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	2	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	3	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	4	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	5	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	6	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	7	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	8	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	9	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	10	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	11	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	12	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	14	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	15	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	18	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	2	WHERE ID =	19	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	20	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	21	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	23	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	24	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	26	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	27	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	28	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	29	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	30	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	32	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	33	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	36	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	38	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	42	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	48	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	50	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	51	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	52	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	53	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	54	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	56	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	57	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	58	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	59	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	61	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	62	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	63	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	64	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	65	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	67	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	68	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	69	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	70	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	73	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	75	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	77	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	78	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	79	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	80	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	85	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	87	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	91	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	93	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	95	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	97	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	100	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	102	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	103	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	104	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	105	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	111	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	113	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	115	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	116	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	117	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	118	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	119	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	120	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	122	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	123	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	124	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	125	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	126	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	4	WHERE ID =	127	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	128	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	129	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	130	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	132	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	133	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	134	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	135	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	136	

GO
UPDATE [dbo].[Client] SET [ConsultantId] =	3	WHERE ID =	137	

GO

UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	2
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	3
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	4
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	5
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	6
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	7
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	8
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	9
UPDATE [dbo].[Client] SET [AccountManagerId] =	2	WHERE ID =	10
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	11
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	12
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	14
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	15
UPDATE [dbo].[Client] SET [AccountManagerId] =	2	WHERE ID =	18
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	19
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	20
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	21
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	23
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	24
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	26
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	27
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	28
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	29
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	30
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	32
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	33
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	36
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	38
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	42
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	48
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	50
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	51
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	52
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	53
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	54
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	56
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	57
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	58
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	59
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	61
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	62
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	63
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	64
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	65
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	67
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	68
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	69
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	70
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	73
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	77
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	78
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	79
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	80
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	85
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	87
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	91
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	93
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	95
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	97
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	100
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	102
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	103
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	104
UPDATE [dbo].[Client] SET [AccountManagerId] =	7	WHERE ID =	105
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	111
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	113
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	115
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	116
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	117
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	118
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	119
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	120
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	122
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	123
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	124
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	125
UPDATE [dbo].[Client] SET [AccountManagerId] =	7	WHERE ID =	128
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	129
UPDATE [dbo].[Client] SET [AccountManagerId] =	6	WHERE ID =	130
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	132
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	133
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	134
UPDATE [dbo].[Client] SET [AccountManagerId] =	8	WHERE ID =	135
UPDATE [dbo].[Client] SET [AccountManagerId] =	7	WHERE ID =	136
UPDATE [dbo].[Client] SET [AccountManagerId] =	5	WHERE ID =	137