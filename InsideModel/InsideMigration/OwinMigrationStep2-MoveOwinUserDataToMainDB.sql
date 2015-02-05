
/****** Object:  Table [dbo].[Role]    Script Date: 2014-09-08 11:21:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[User]    Script Date: 2014-09-08 11:21:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [nvarchar](128) NOT NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[LegacyPasswordHash] [nvarchar](max) NULL,
	[LoweredUserName] [nvarchar](256) NOT NULL,
	[LastActivityDate] [datetime2](7) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[LastLoginDate] [datetime2](7) NOT NULL,
	[LastLoginFailed] [datetime2](7) NOT NULL,
	[LoginFailureCounter] [int] NOT NULL,
 CONSTRAINT [PK_dbo.User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[UserClaim]    Script Date: 2014-09-08 11:21:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClaim](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[User_Id] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.UserClaim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[UserLogin]    Script Date: 2014-09-08 11:21:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogin](
	[UserId] [nvarchar](128) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.UserLogin] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 2014-09-08 11:21:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (N'5F1B1DEE-272F-43C7-A2A4-D6E36E3433A3', N'demo')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C', N'consultant')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (N'ABBB7EC6-FCD2-40FB-B241-EB5A7C454F8E', N'admin')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F', N'sales')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81', N'client')
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'01724469-13A1-4D1B-BEBA-280285CDF7C8', N'fredrik.brannvall@tennisstadion.se', N'GQCiNyrrbbyPfIJg1BJnQI/RURE=|1|GBxv1d5Sgff3dRlQp2eRXQ==', N'26486D83-B8F6-4317-A72A-3C73AE8A2C19', N'GQCiNyrrbbyPfIJg1BJnQI/RURE=', N'fredrik.brannvall@tennisstadion.se', CAST(0x0710169CA95CEF380B AS DateTime2), N'fredrik.brannvall@tennisstadion.se', 1, 0, CAST(0x07002E15EA45ED370B AS DateTime2), CAST(0x07501FEE365709380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'0192FE21-A0E1-4791-98AE-25E967C2436B', N'Niklas@tomina.se', N'Th2J5vJ8yH+GJi/fQPK859zJykc=|1|bFexC9TTXFzzflpidaWgXg==', N'D88406AB-1C67-42F8-9C6D-9D58F942D91C', N'Th2J5vJ8yH+GJi/fQPK859zJykc=', N'niklas@tomina.se', CAST(0x07003310DD75F8380B AS DateTime2), N'Niklas@tomina.se', 1, 0, CAST(0x07003310DD75F8380B AS DateTime2), CAST(0x07003310DD75F8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'01D4C6A8-32BD-4688-B18A-5000F581B1C6', N'Alexander@studybuddy.se', N'DVy58tohSs3YHEzzGNASJF7fBD0=|1|GouNGhshX7M80TIdfXn9bw==', N'99EB7240-5396-4989-A4CC-55D5C4CB6CCD', N'DVy58tohSs3YHEzzGNASJF7fBD0=', N'alexander@studybuddy.se', CAST(0x07503DAF8C6DE5380B AS DateTime2), N'Alexander@studybuddy.se', 1, 0, CAST(0x070060C37E35A4380B AS DateTime2), CAST(0x071063196F6DE5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'02E70D66-8F67-4562-A575-85C950286A07', N'gunilla@hackebergaslott.se', N'PH6P/elARZyoXyiizsVkq4VxSiY=|1|TWQ7iurqZ9sBh5DjGeU5xA==', N'8520C00E-D1C7-4981-9794-6A325FA8C0F4', N'PH6P/elARZyoXyiizsVkq4VxSiY=', N'gunilla@hackebergaslott.se', CAST(0x07107F4B2F5F5C380B AS DateTime2), N'gunilla@hackebergaslott.se', 1, 0, CAST(0x0780BD64444B37380B AS DateTime2), CAST(0x07E002A82D4C38380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'039B3B56-9DCE-4380-90F7-5F7A741D8FF7', N'niklas@skandinaviskasmahus.se', N'lKnSmN7++yd4vOKwoz5o/nhk/9Y=|1|Thx4+MYYm0VF49rZE5fzDw==', N'764B9F83-C240-426D-A212-24712E0685A7', N'lKnSmN7++yd4vOKwoz5o/nhk/9Y=', N'niklas@skandinaviskasmahus.se', CAST(0x07E0848C8B915F380B AS DateTime2), N'niklas@skandinaviskasmahus.se', 1, 0, CAST(0x0700B39EC646ED370B AS DateTime2), CAST(0x07B0CDB303574C380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'03C55289-83B8-443A-BCB8-FCC8E85E5D42', N'sollentunalaser@gmail.com', N'ixp049Iwtob/whcx606oSDt8+FY=|1|u4sqh2z1ImWn8YmR58S/Rg==', N'46E7D131-1131-4BA8-A210-99329011CC60', N'ixp049Iwtob/whcx606oSDt8+FY=', N'sollentunalaser@gmail.com', CAST(0x0720A90B8569B6380B AS DateTime2), N'sollentunalaser@gmail.com', 1, 0, CAST(0x078080E6C63D92380B AS DateTime2), CAST(0x075074EE5C68B6380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'04762CC5-8105-4552-AAEF-5A3E3F874532', N'info@mikascatering.se', N'vhlznije8pzrBNqqb7ZCCYKUWHQ=|1|Fg1/2Sodm7KHGD2K1/MBTg==', N'B7AD5F42-43E5-4C34-9D20-2CEC411A87AF', N'vhlznije8pzrBNqqb7ZCCYKUWHQ=', N'info@mikascatering.se', CAST(0x078081B4D94C85380B AS DateTime2), N'info@mikascatering.se', 1, 0, CAST(0x078081B4D94C85380B AS DateTime2), CAST(0x078081B4D94C85380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'04AAF50C-6451-4BDC-9001-613B4B1FEE05', N'ludvig@hvgs.se', N'CjLNActFqcp36fjGBFxoepx4qcM=|1|83MWrPd/4gZJ3t+KRXqUpg==', N'3C4EE7BA-BF18-4AF5-8D24-AA0E4E2BB808', N'CjLNActFqcp36fjGBFxoepx4qcM=', N'ludvig@hvgs.se', CAST(0x072029D7BF5147380B AS DateTime2), N'ludvig@hvgs.se', 1, 0, CAST(0x0700FA40E048ED370B AS DateTime2), CAST(0x07E045CCAB4347380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'06BBD965-ADC1-4E09-A4EE-006C630C5CAD', N'per@hoglundsbil.se', N'9/PA4Rbc5RGWUUWT2QKysEKbiqw=|1|LsaBaJc/fi8LqEk3qB0Z1g==', N'172F7980-2C97-435A-A455-043747D15963', N'9/PA4Rbc5RGWUUWT2QKysEKbiqw=', N'per@hoglundsbil.se', CAST(0x0730C7A2FA9CD4380B AS DateTime2), N'per@hoglundsbil.se', 1, 0, CAST(0x07007816D15985360B AS DateTime2), CAST(0x0710AF2EF79CD4380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'075F2A37-9E0B-44AF-9591-DD7D1ACA11A5', N'hans@westergrentandreglering.se', N'oyzcSHlgeW6yWh7GaweHS6JA9BM=|1|aOaRFIfMYay7sTt4bv30RA==', N'291230F1-C203-458B-85C8-1E9B05BDF73E', N'oyzcSHlgeW6yWh7GaweHS6JA9BM=', N'hans@westergrentandreglering.se', CAST(0x07F0A3C4D74245380B AS DateTime2), N'hans@westergrentandreglering.se', 1, 0, CAST(0x0700F950075410380B AS DateTime2), CAST(0x07C00F69B84045380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'08C511F7-2337-434A-BED4-918D74F67B9F', N'annascampi@gmail.com', N'ukBijjK7QJFsx1jgC8iHMAGB4BM=|1|mKNl5E5/uo5iCClTMQEmmg==', N'88C59C63-B50D-421E-96E9-8E30083F9554', N'ukBijjK7QJFsx1jgC8iHMAGB4BM=', N'annascampi@gmail.com', CAST(0x07C015475E8488360B AS DateTime2), N'annascampi@gmail.com', 1, 0, CAST(0x0780D9EDAA5385360B AS DateTime2), CAST(0x07606CF1588488360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'09E3CAA2-EAC6-47A1-9431-CC646F2E21FA', N'annika.mccarthy@payzone.se', N'r53ssHS1BhWrh0flTFS8yfNghGY=|1|YNFq6AUCzD9h6G+VhA8e/g==', N'646FA3C3-A8F0-479A-B5E1-3DF1DFFECDC5', N'r53ssHS1BhWrh0flTFS8yfNghGY=', N'annika.mccarthy@payzone.se', CAST(0x078005C48D69ED370B AS DateTime2), N'annika.mccarthy@payzone.se', 1, 0, CAST(0x078005C48D69ED370B AS DateTime2), CAST(0x078005C48D69ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'0B78F080-4D1E-4237-B76C-F24D9768AB36', N'olle.ekwall@fambil.se', N'H2GPaymoVuEDm3ugxyO+5xDlQ0c=|1|UIFN0O1WnWazptPAB+E2Kg==', N'6F8049AE-A8A9-44FA-80F8-03B4D8C0183B', N'H2GPaymoVuEDm3ugxyO+5xDlQ0c=', N'olle.ekwall@fambil.se', CAST(0x0710AE51E14510380B AS DateTime2), N'olle.ekwall@fambil.se', 1, 0, CAST(0x0700DCC9A04F85360B AS DateTime2), CAST(0x07A02558244510380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'0BE7B8CA-06DF-4049-BC61-3080C763F3B3', N'Annica.Modin@wallenstam.se', N'Kdv9ZrWKA+PzvWXnxT0ZWOtcPW0=|1|P/8O5HzSfji70sfPHZJPpA==', N'C1EEAB4C-3800-4C48-8D85-84DABBC1120C', N'Kdv9ZrWKA+PzvWXnxT0ZWOtcPW0=', N'annica.modin@wallenstam.se', CAST(0x07D0CBA3BF4B71380B AS DateTime2), N'Annica.Modin@wallenstam.se', 1, 0, CAST(0x078050E27E73E5370B AS DateTime2), CAST(0x07C04C23BB4B71380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'0C2B18CE-3AC4-4F01-B468-2A565A8668E5', N'info@15medpaolo.se', N'9iANEm5cFCxrv4VPJO7oGuFRVrU=|1|lpycrO8UnNvQGJKdQdeVoQ==', N'3CA19DAC-64E4-451E-B9D5-8ABAB50DB9E6', N'9iANEm5cFCxrv4VPJO7oGuFRVrU=', N'info@15medpaolo.se', CAST(0x0740FA711543FB380B AS DateTime2), N'info@15medpaolo.se', 1, 0, CAST(0x078091D1686DA8380B AS DateTime2), CAST(0x07D0627A996DA8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'0CFC54A7-9010-4AE0-9F8C-B1ACCCB36EF2', N'richard@hvgs.se', N'6X2Q9RZapYZtVhOMAFFBnNm745E=|1|Wv+6ocddIQg9jez8/V1ctg==', N'F657F68B-C5C6-4AD6-BC48-281361294957', N'6X2Q9RZapYZtVhOMAFFBnNm745E=', N'richard@hvgs.se', CAST(0x07808D1DF548ED370B AS DateTime2), N'richard@hvgs.se', 1, 0, CAST(0x07808D1DF548ED370B AS DateTime2), CAST(0x07808D1DF548ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'0EB54251-A616-4D32-AC88-54D337C74B89', N'anders@atollform.com', N'1SEHkeZSGxpZTA0M8P2k6SPdrSo=|1|68Ew2hU4ulyH8TngsSN/vQ==', N'932AF98A-6FD5-490F-8E43-53A50339CC3B', N'1SEHkeZSGxpZTA0M8P2k6SPdrSo=', N'anders@atollform.com', CAST(0x078017138246ED370B AS DateTime2), N'anders@atollform.com', 1, 0, CAST(0x078017138246ED370B AS DateTime2), CAST(0x078017138246ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'1254CA19-1603-4B09-93DA-34726EFC6E18', N'marknadswebb2@bravura.se', N'obKOYnRR0skoqLayZHz6ggrZCKc=|1|T4SWGSO/nfAFroF3LXw3sw==', N'69D9CFEC-46C4-4060-87B0-0CA3CA067AF5', N'obKOYnRR0skoqLayZHz6ggrZCKc=', N'marknadswebb2@bravura.se', CAST(0x077079A36F5353380B AS DateTime2), N'marknadswebb2@bravura.se', 1, 0, CAST(0x0780BD07265DD9370B AS DateTime2), CAST(0x0730681F545353380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'13937429-0B0D-4FEF-A30F-FCFC7489EE4A', N'johan.salzmann@goldadam.com', N'MpfV0YGjzd3StOEHqY75Aqfeasw=|1|RJuOfWq5CnTV/IpL+Ehg5A==', N'A3497D8C-7415-4AA0-993F-C1AA46D9021D', N'MpfV0YGjzd3StOEHqY75Aqfeasw=', N'johan.salzmann@goldadam.com', CAST(0x07700C9B7879D2370B AS DateTime2), N'johan.salzmann@goldadam.com', 1, 0, CAST(0x0780035855383D370B AS DateTime2), CAST(0x07E07C855379D2370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'148C11C2-5D9E-4858-9F58-6E4E73FB5181', N'julia', N'hTL7lK485JrZzToRTZla5WCzlMk=|1|Z/ar9elGzM3RkNZPw6tTkA==', N'3AFB14E6-8BE4-4363-88AC-9D5FCB4DBBC9', N'hTL7lK485JrZzToRTZla5WCzlMk=', N'julia', CAST(0x07C0ECDE326473380B AS DateTime2), N'julia.servenius@helloy.se', 0, 0, CAST(0x0700508CD85E39360B AS DateTime2), CAST(0x0750EB2AAF7170380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'14DF176C-2538-4A09-830A-664E5E91D85E', N'conny.bjorkman@frosunda.se', N'85+4LG1Mi6HzR3T9Rd8kkPq4lIA=|1|vLp1qyQbJaM24kaKcUGPIA==', N'315AD60D-4D57-4ED3-919C-11F8CDDB72FA', N'85+4LG1Mi6HzR3T9Rd8kkPq4lIA=', N'conny.bjorkman@frosunda.se', CAST(0x07D0EA9DA272F5380B AS DateTime2), N'conny.bjorkman@frosunda.se', 1, 0, CAST(0x0700BB31A838C0380B AS DateTime2), CAST(0x0770DDFCEC6DF5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'167DCD6B-64DF-49D5-9199-19219B2E9E68', N'richard.ahnell@gcm.se', N'j1pureXzXhs8oX65weecb8Z7TUk=|1|dgkPK5VV4VmQjfeGTb0yoA==', N'5B059417-4B46-43FB-BD08-81F75FD9F87D', N'j1pureXzXhs8oX65weecb8Z7TUk=', N'richard.ahnell@gcm.se', CAST(0x07808B2C606CED370B AS DateTime2), N'richard.ahnell@gcm.se', 1, 0, CAST(0x07808B2C606CED370B AS DateTime2), CAST(0x07808B2C606CED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'18743A6F-0F95-4049-AA25-D9B9869A1CF8', N'peter', N'HOld3LupUXBfy6/pnHjLq9TBaBc=|1|kIXQjhVICX2ZeDQwjSTlRw==', N'9B788F97-241B-4C7B-A236-CB5F2DEB9BAF', N'HOld3LupUXBfy6/pnHjLq9TBaBc=', N'peter', CAST(0x0750167BED3AFB380B AS DateTime2), N'peter@helloy.se', 1, 0, CAST(0x0780A916948AE0350B AS DateTime2), CAST(0x07905E12EC3AFB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'1B509F67-4066-4E6B-AB96-B0EF842AEB25', N'christoffer.ohman@risbergsbil.se', N'21I38SWkmY01o9Hj55uONnn0wA8=|1|9ei29IUwn/rNJKakCqFAlA==', N'C3CC3F0D-A69F-4342-BA0C-806DDA6DF086', N'21I38SWkmY01o9Hj55uONnn0wA8=', N'christoffer.ohman@risbergsbil.se', CAST(0x07308C981D6A61380B AS DateTime2), N'christoffer.ohman@risbergsbil.se', 1, 0, CAST(0x070071F4E3880D380B AS DateTime2), CAST(0x07308C981D6A61380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'1B597D96-B7E8-4F13-A8D1-D5C30E75980B', N'carl.h@nordiskagalleriet.se', N'fuQHH7gA1ZapxNDqHTXLnd1SiIM=|1|KXTIB7z4a7/5h6C8XXIcPQ==', N'9E05E553-02D9-4685-A9EF-4A77695E6699', N'fuQHH7gA1ZapxNDqHTXLnd1SiIM=', N'carl.h@nordiskagalleriet.se', CAST(0x07B01C6C1A70E1380B AS DateTime2), N'carl.h@nordiskagalleriet.se', 1, 0, CAST(0x07800137D36E6A380B AS DateTime2), CAST(0x0770CFACCB6FE1380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'1CEEE997-11D2-484E-BD33-0FE907487974', N'john.stiltyger@telia.com', N'615RHBZ05HmDmtePzGiSvvBukzU=|1|etZ+ijApVb/Of9Y+gkHymw==', N'D97A3F1F-90DF-4EEB-9630-3E6DE997B73E', N'615RHBZ05HmDmtePzGiSvvBukzU=', N'john.stiltyger@telia.com', CAST(0x07F0E4F81B75E6380B AS DateTime2), N'john.stiltyger@telia.com', 1, 0, CAST(0x07009A062C6F42370B AS DateTime2), CAST(0x0720CDE40375E6380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'1DE4D974-FE94-4F3C-81EA-04581C23B36B', N'therese.v.lindgren@gmail.com', N'9lqi3QB1aEMefdmG8z6v7U2MUB0=|1|95v8IYP8+zLaFoCRJP27hw==', N'40787053-6042-433A-859C-090E2D6B74F9', N'9lqi3QB1aEMefdmG8z6v7U2MUB0=', N'therese.v.lindgren@gmail.com', CAST(0x0700A7210C6CED370B AS DateTime2), N'therese.v.lindgren@gmail.com', 1, 0, CAST(0x0700A7210C6CED370B AS DateTime2), CAST(0x0700A7210C6CED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'21F7B746-9913-4528-819B-6D5454D91DA6', N'peter@kg10.se', N'pIsPipuKu/wMWN/nng0EqF0ul8E=|1|7vla5G9KizpS7UYPGrtp0Q==', N'FC099574-1C79-48B4-96E7-B74D5B929EF5', N'pIsPipuKu/wMWN/nng0EqF0ul8E=', N'peter@kg10.se', CAST(0x07B080189D37A3380B AS DateTime2), N'peter@kg10.se', 1, 0, CAST(0x078031B1CB49ED370B AS DateTime2), CAST(0x079002398437A3380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'22795063-BFDC-4BC4-80CF-F4CD928DE4E8', N'stina@studybuddy.se', N'HaQIGn1td+G1Vp8UNBbvpqIl0fw=|1|NnLINdxFZLy2mh9yqkbI4g==', N'C0EA4081-7E33-4BF7-A124-BD42504CBF94', N'HaQIGn1td+G1Vp8UNBbvpqIl0fw=', N'stina@studybuddy.se', CAST(0x07B09351FB6EF1380B AS DateTime2), N'stina@studybuddy.se', 1, 0, CAST(0x0700C1A6C34CD1370B AS DateTime2), CAST(0x07E067F8F56EF1380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'23AED1D0-EEB8-4933-A19F-E3DCB61CBA0C', N'fabrice.wasielewski@citroen.com', N'zV8shfq42/LunnXujGYy5oCZ8Lw=|1|EqHzMGSaj5Y8m28YkfeuEg==', N'5BD44412-1304-4123-BA6F-A3B4AA53C7B2', N'zV8shfq42/LunnXujGYy5oCZ8Lw=', N'fabrice.wasielewski@citroen.com', CAST(0x0740ED8DA46DD3380B AS DateTime2), N'fabrice.wasielewski@citroen.com', 1, 0, CAST(0x0780CDAE046DBF360B AS DateTime2), CAST(0x07F0AC56B36CD3380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'243923E8-8A09-4868-BDD9-3AC412BB299A', N'anders@flane.se', N'eiQXpFGBByQnaWimP2n9JSwg5Ys=|1|IawG7jFt88yp7gNtaN7png==', N'05B14C11-B9CC-48D4-816C-C7E55E47B029', N'eiQXpFGBByQnaWimP2n9JSwg5Ys=', N'anders@flane.se', CAST(0x07D01AC4845318380B AS DateTime2), N'anders@flane.se', 1, 0, CAST(0x07007D06596E88360B AS DateTime2), CAST(0x0780A9B8D55118380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2547F64F-3417-4574-BA1B-5EE4C7858DAD', N'kristofer@helloy.se', N'SJFUiki3DP28GfRe/mPhssAfQFI=|1|RG8OVzcywqhb23QTmQYtUw==', N'C42E530B-488A-45BE-AC67-85ED35D5F364', N'SJFUiki3DP28GfRe/mPhssAfQFI=', N'kristofer@helloy.se', CAST(0x07E05007797BE3380B AS DateTime2), N'kristofer@helloy.se', 1, 0, CAST(0x07805EE2CC4985360B AS DateTime2), CAST(0x07F069E95237A3380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'25614DCA-233D-40B0-BE39-75C66F6941D1', N'niklas@henryeriksson.se', N'ZxAl4OKvvE0AZ4nEQ6299HxFsOM=|1|ekCxR3cmzclvZ8F2sJsM7g==', N'D2C62245-8AA7-43A8-A48C-249118E61E32', N'ZxAl4OKvvE0AZ4nEQ6299HxFsOM=', N'niklas@henryeriksson.se', CAST(0x0780CC5C1A46ED370B AS DateTime2), N'niklas@henryeriksson.se', 1, 0, CAST(0x0780CC5C1A46ED370B AS DateTime2), CAST(0x0780CC5C1A46ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'27259D53-E913-4282-AAF1-F1D42F316542', N'info@arnhildshus.se', N'l8gsbKE6H7qHOtqHearAu1wnQTY=|1|6SiNTJRgSWLr1L12B8svxw==', N'0EEB6168-0841-44A0-A460-0C2085E51774', N'l8gsbKE6H7qHOtqHearAu1wnQTY=', N'info@arnhildshus.se', CAST(0x07E02613E082D5370B AS DateTime2), N'info@arnhildshus.se', 1, 0, CAST(0x0700FA83E780D5370B AS DateTime2), CAST(0x0780998F5682D5370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'275677F6-5AA5-4291-87D8-F841497F961C', N'bengt@soderkok-luckor.se', N'MKGrQbe0L8jRNujLJx1kz1ohMm4=|1|p4QPuAa3Sm7EN7fTfYnXCw==', N'48A6EC2D-FE69-4C11-A2D1-2BE8FC8BC541', N'MKGrQbe0L8jRNujLJx1kz1ohMm4=', N'bengt@soderkok-luckor.se', CAST(0x07B053C37B6775370B AS DateTime2), N'bengt@soderkok-luckor.se', 1, 0, CAST(0x07008FEA075685360B AS DateTime2), CAST(0x07B075BF5E6675370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'27DC37B9-7084-4B31-8F21-311709558017', N'Jakob', N'Y2qdiSBALOE7PK2gpeso4eXbZjk=|1|CaialbyQqqJdwAIRoolIiw==', N'87D6B61C-51EB-4740-919B-0ABC1C3CA8C3', N'Y2qdiSBALOE7PK2gpeso4eXbZjk=', N'jakob', CAST(0x07E07B944452EE380B AS DateTime2), N'jakob.aronsson@helloy.se', 1, 0, CAST(0x0700C35E37797A380B AS DateTime2), CAST(0x077007ADBB51EE380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'28070B24-AD45-4A0E-AAC2-0B27848F3BEB', N'marcus.lindgren@lindgrensradio.se', N'9VAMBxuJUVGm65gnOBXLGS2Y3i0=|1|dMbQu3gAYSOivbR+n7JTbg==', N'9EF4327B-B007-41F8-AB6B-04546BDC285B', N'9VAMBxuJUVGm65gnOBXLGS2Y3i0=', N'marcus.lindgren@lindgrensradio.se', CAST(0x0750D5A7514FCB360B AS DateTime2), N'marcus.lindgren@lindgrensradio.se', 1, 0, CAST(0x07008E9A548785360B AS DateTime2), CAST(0x07F0EDA5AC4CCB360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2815BB49-5E4F-48FD-AB14-CD417EFD9AB0', N'anna-karin.lindberg@lararfortbildning.se', N'udpCPwQgVp5BXJNHLZJAkxKKfG8=|1|l54Mec7UZqv0tHEwBNXvGQ==', N'2EE31475-707C-43D7-B3DC-4188AA7393E8', N'udpCPwQgVp5BXJNHLZJAkxKKfG8=', N'anna-karin.lindberg@lararfortbildning.se', CAST(0x0770C5C74B7F2A380B AS DateTime2), N'anna-karin.lindberg@lararfortbildning.se', 1, 0, CAST(0x07802733CD52F9360B AS DateTime2), CAST(0x07403002507E2A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'28B887A3-B50E-4780-A7C9-59FF0D1B95A1', N'henrik', N'S+7uMHUYjoGXyAIrI+OFsGBdE3Q=|1|6I+1nitx0/vXVHDN7RGzUQ==', N'EDF81AE2-FF9E-4596-AF5A-00869128FF82', N'S+7uMHUYjoGXyAIrI+OFsGBdE3Q=', N'henrik', CAST(0x07C0A260ED35FB380B AS DateTime2), N'henrik@helloy.se', 1, 0, CAST(0x07007B079D8AE0350B AS DateTime2), CAST(0x0780D6249035FB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'29A52BF5-A2BA-4A48-BBFB-DF327993F616', N'kontoret@harganget.com', N'3QZsnOH6WLuBHSnAXJVNFSW6ye8=|1|VBj5K2lniD1pOt5pmk3yig==', N'F8F12231-78B3-4F21-873F-09556A7AEDA5', N'3QZsnOH6WLuBHSnAXJVNFSW6ye8=', N'kontoret@harganget.com', CAST(0x0770870AEE6EDF370B AS DateTime2), N'kontoret@harganget.com', 1, 0, CAST(0x0700AEB9A94D82360B AS DateTime2), CAST(0x07003654896EDF370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2A889710-F6D5-43BD-8EB3-5944A2BADAC7', N'lina@monrads.se', N'2//F9UJHptKZ7m9+it78Gv5ekJI=|1|KjLwKTg4G3cFZhI1cFK4EA==', N'9B00B676-7CCD-416A-A5E9-D7C03BD66671', N'2//F9UJHptKZ7m9+it78Gv5ekJI=', N'lina@monrads.se', CAST(0x0740E71A18509C380B AS DateTime2), N'lina@monrads.se', 1, 0, CAST(0x07802507E06DCA370B AS DateTime2), CAST(0x07B05DBB13509C380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2C902585-366C-47C0-B594-E76B333CE966', N'niklas.eriksson@foreningssupport.com', N'sxPzLeETTyWE6B0WQpYizlRv0z4=|1|z5AkfV8odpGD7v4wMYMdhw==', N'4D658129-9150-42B5-9226-23730B9992F1', N'sxPzLeETTyWE6B0WQpYizlRv0z4=', N'niklas.eriksson@foreningssupport.com', CAST(0x07D0B4DD0948B7360B AS DateTime2), N'niklas.eriksson@foreningssupport.com', 1, 0, CAST(0x0700011F414EB2360B AS DateTime2), CAST(0x07B016380448B7360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2CEE7D32-B5F6-4AE2-BC78-9D7B599D524B', N'sebastian.jofred@telemission.se', N'5eJKd4Qp3WeKtAVOONuss9qGVis=|1|f0SYtHo3Y4hUflrh0r/RAQ==', N'C7D3AA2F-A498-4A5D-841A-25450DF6C99E', N'5eJKd4Qp3WeKtAVOONuss9qGVis=', N'sebastian.jofred@telemission.se', CAST(0x0770B05CD93FF8380B AS DateTime2), N'sebastian.jofred@telemission.se', 1, 0, CAST(0x07006C318D75EB370B AS DateTime2), CAST(0x0730848BD53FF8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2D7782FA-A07C-4BEA-9FBA-117254FCEC82', N'lina.telander@restaurangakademien.se', N'WVE8DIEM87KY6AkI3XmOwlqL5KE=|1|5Si6ZUsMvDrv6G/SeWiNCQ==', N'2AB8247F-CC46-4743-9CDE-AD8361055017', N'WVE8DIEM87KY6AkI3XmOwlqL5KE=', N'lina.telander@restaurangakademien.se', CAST(0x078050D51A493A380B AS DateTime2), N'lina.telander@restaurangakademien.se', 1, 0, CAST(0x07001575A03E39380B AS DateTime2), CAST(0x0730828010493A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2DD065E3-3774-49F5-9227-398DEF2B72FB', N'lennartsjoberg@gmail.com', N'8vlXA8GbbCOR+JZX9tHYGvSGX44=|1|v1RpZ/qKeA/U94hRVXaNzA==', N'B58B8084-3030-4B76-B6B5-B5C70CF4309E', N'8vlXA8GbbCOR+JZX9tHYGvSGX44=', N'lennartsjoberg@gmail.com', CAST(0x0700D98656585F380B AS DateTime2), N'lennartsjoberg@gmail.com', 1, 0, CAST(0x0780F282893F20370B AS DateTime2), CAST(0x0710C5712E585F380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2E200A0B-0E4A-41AA-AC41-DD17C601336C', N'lars.andersson@battrebolan.se', N'kNLOGVnvFBWnDf/LRc4NM3DTv9w=|1|MfdRZjIyNhJroaXg8OUI3Q==', N'776ED472-C7A3-4CA1-AA85-2D6D00269F90', N'kNLOGVnvFBWnDf/LRc4NM3DTv9w=', N'lars.andersson@battrebolan.se', CAST(0x0710AF23474EE3380B AS DateTime2), N'lars.andersson@battrebolan.se', 1, 0, CAST(0x07008F7C0675BD370B AS DateTime2), CAST(0x0770DA93DA4BE3380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'2E291644-BA67-49D5-9B99-02DA458B912F', N'peter@ballingslovcity.se', N'REVW0qeCPE414esqdtXDZZoY0qw=|1|6w2+vEJ7cOISGKLw5Lr40g==', N'DBF123A8-99EA-4B45-8C87-210D8B595F33', N'REVW0qeCPE414esqdtXDZZoY0qw=', N'peter@ballingslovcity.se', CAST(0x0770F6ACE58868380B AS DateTime2), N'peter@ballingslovcity.se', 1, 0, CAST(0x07008773747285360B AS DateTime2), CAST(0x0740E21B7A8868380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'30DA714F-1400-4B30-8925-FD82A81C5E1E', N'ulf@animalen.se', N'0bNW9GX3+0xgMxkX6UxjKhW8Jk8=|1|/5JncHVxlGk0OXwfP+kZmA==', N'A3007A16-70BE-4C74-A183-3F0042D1F4B6', N'0bNW9GX3+0xgMxkX6UxjKhW8Jk8=', N'ulf@animalen.se', CAST(0x07B089D0374FF2370B AS DateTime2), N'ulf@animalen.se', 1, 0, CAST(0x07008CB86B4585360B AS DateTime2), CAST(0x07509AEC1A4FF2370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 3)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'320DE6B8-C141-4F88-9E62-12DF8E28CCBB', N'tina@gkoptik.se', N'hzpIxV+FXDi20IWctTSK2KmNxhg=|1|WJUy695nlu0gvBbITE6Mpg==', N'4ADFDA1E-A509-4318-807D-9FB8CC816A5E', N'hzpIxV+FXDi20IWctTSK2KmNxhg=', N'tina@gkoptik.se', CAST(0x07D0D891FD44B2370B AS DateTime2), N'tina@gkoptik.se', 1, 0, CAST(0x0700C20C4D5285360B AS DateTime2), CAST(0x0790C85FBC44B2370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'34266622-6086-4EFB-8C6D-1A7DABC70DF6', N'Sebastian Gomes', N'HPKehFzMrQIo2X4krbnC2cxzmkI=|1|8uvxj6f1GmwUyhegdB57+w==', N'A3CC08BC-2E13-4946-A47D-15B445074A3F', N'HPKehFzMrQIo2X4krbnC2cxzmkI=', N'sebastian gomes', CAST(0x0770A4FD5C45F8380B AS DateTime2), N'sebastian.gomes@helloy.se', 1, 0, CAST(0x0700CAB5047B17380B AS DateTime2), CAST(0x0710DFD8467DF7380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'34329C73-26F2-4891-98A4-161451CCF52D', N'urban@fraktochfix.se', N'qzyKPJhYVUITo+6h13CTkSstIDk=|1|qKbGIrUAbqn0nFy+T591zA==', N'52151B6E-AAC9-40C0-B6BB-F695C1335FA5', N'qzyKPJhYVUITo+6h13CTkSstIDk=', N'urban@fraktochfix.se', CAST(0x0730F7D2E34431380B AS DateTime2), N'urban@fraktochfix.se', 1, 0, CAST(0x0780004AB665F9360B AS DateTime2), CAST(0x07B0584A444031380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'35786EAF-F33B-4DDA-8938-F4BD60C6D1DD', N'peter.rehnstrom@ostcamp.se', N'8s7d2MaecQ9W6JYZ8GmGsmIA6iA=|1|Ut/56VewpB9k1ZwfURExmA==', N'19B565F4-DD6F-464A-B349-CA5BFA1845B8', N'8s7d2MaecQ9W6JYZ8GmGsmIA6iA=', N'peter.rehnstrom@ostcamp.se', CAST(0x07800E66524A54380B AS DateTime2), N'peter.rehnstrom@ostcamp.se', 1, 0, CAST(0x07800E66524A54380B AS DateTime2), CAST(0x07800E66524A54380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'37CC7360-5EA0-462E-A489-D221F1B56E13', N'christina@claessonkok.com', N'WgcR9Lm6g3N8+Qm+2So0TuqXnw8=|1|uLOqUqKx5eeUR05XUwEX8A==', N'FF2447E6-8A66-4B9D-8D21-9246840344DA', N'WgcR9Lm6g3N8+Qm+2So0TuqXnw8=', N'christina@claessonkok.com', CAST(0x071003819242E3360B AS DateTime2), N'christina@claessonkok.com', 1, 0, CAST(0x07803DFBDC4B85360B AS DateTime2), CAST(0x07B0BE3D4171E1360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'39EC022D-A45F-49FC-AC07-7476E86ACF3A', N'marknadswebb1@bravura.se', N'iqfa6FzHAJm33Ic0WMglMdMyM84=|1|cPL53gWkoKss40ERvm5q2g==', N'8125B719-777C-437C-938B-28B10A51FE37', N'iqfa6FzHAJm33Ic0WMglMdMyM84=', N'marknadswebb1@bravura.se', CAST(0x07107B73383C33380B AS DateTime2), N'marknadswebb1@bravura.se', 1, 0, CAST(0x07000B21175DD9370B AS DateTime2), CAST(0x07D02588363C33380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'3A02AD00-0DBE-4C5E-A7CC-5EE294BA0CEF', N'daniel.sellevoll@gulladam.no', N'1St5OQmb8DDjZ/czXzVq5Ub/wx8=|1|+QkmgSpWdot5jBdkdadMSg==', N'AAA46816-E310-4F0A-940B-737D2FC68D2D', N'1St5OQmb8DDjZ/czXzVq5Ub/wx8=', N'daniel.sellevoll@gulladam.no', CAST(0x070085156348ED370B AS DateTime2), N'daniel.sellevoll@gulladam.no', 1, 0, CAST(0x070085156348ED370B AS DateTime2), CAST(0x070085156348ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'3C7D1A1B-CA4E-41A3-AA77-F2906CE9F00C', N'andreas.landin@dinmotorroslagen.se', N'9hPHPNpfRwD/sbwAQEu+5eSMkV0=|1|HKmZGpBrVmNvTE6X1nqZeg==', N'5A925278-5A84-4DA3-BF41-738A705CF5E9', N'9hPHPNpfRwD/sbwAQEu+5eSMkV0=', N'andreas.landin@dinmotorroslagen.se', CAST(0x07A0569F0B7740380B AS DateTime2), N'andreas.landin@dinmotorroslagen.se', 1, 0, CAST(0x07007C43B17A90370B AS DateTime2), CAST(0x0730F5EDB16C40380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'3DBCE4E4-798E-4174-A10B-F4C69D00FB3E', N'patrik@agilokliniken.se', N'M/IArJHXGjxAzr/C26hwu5HX5Cw=|1|vrHM00JdC/B5DKt1qLAHQg==', N'AC4095F8-08E6-4F88-A287-2398B0F27DEE', N'M/IArJHXGjxAzr/C26hwu5HX5Cw=', N'patrik@agilokliniken.se', CAST(0x0790669CFA4452380B AS DateTime2), N'patrik@agilokliniken.se', 1, 0, CAST(0x0700DC15DC4AE6370B AS DateTime2), CAST(0x07704836E04452380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'40B74293-0F6E-4070-B2BB-AAF9F12EB2E9', N'helloy@grundbolaget.se', N'JDRG2eWUXDxXqtXdXsBfsOt66i4=|1|B2UHgePCC1RFxu/8b1ZBwg==', N'372DACB3-173F-404E-AFDC-62BFCBE3240E', N'JDRG2eWUXDxXqtXdXsBfsOt66i4=', N'helloy@grundbolaget.se', CAST(0x07F098B77A7B8C380B AS DateTime2), N'helloy@grundbolaget.se', 1, 0, CAST(0x0780FFA8D45753380B AS DateTime2), CAST(0x07E0F08D7A7B8C380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'41284117-5F6C-44FA-81AB-113C394B1C96', N'apple@helloy.se', N's2t9gA9bboots7S2GVc1Akt+oZU=|1|6vpTjGkUrjaDrmdgxb04mg==', N'19C344CC-F3DC-4DEE-8AFA-A956E5AA9785', N's2t9gA9bboots7S2GVc1Akt+oZU=', N'apple@helloy.se', CAST(0x07F0B02E1255F0380B AS DateTime2), N'apple@helloy.se', 1, 0, CAST(0x07004AAF33417D380B AS DateTime2), CAST(0x07F0B02E1255F0380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'430095F6-5E3E-437B-9F36-63CC8792A5B7', N'erik.dahlbergh@gmail.com', N'mJEZ66nIgNp4k0/S1sWglLATLZo=|1|pagk8moTIKa+6IPkypFfwQ==', N'9F78337D-80E4-4E13-B497-1EA454577ADD', N'mJEZ66nIgNp4k0/S1sWglLATLZo=', N'erik.dahlbergh@gmail.com', CAST(0x07005A38354CFB380B AS DateTime2), N'erik.dahlbergh@gmail.com', 1, 0, CAST(0x07001291768187360B AS DateTime2), CAST(0x0710FE20206518370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'43C3101F-4FBC-4EA2-95CA-B0ED7971BD9F', N'yaki@ligula.se', N'OhBA7ZnzpVvVsoIFIOi3Dx6vGPM=|1|zzt1bRLG4UnMfCklWad0iw==', N'A99E0DE3-4F39-4EEC-809C-BFC8F7AEF8A5', N'OhBA7ZnzpVvVsoIFIOi3Dx6vGPM=', N'yaki@ligula.se', CAST(0x07D0E70505654C370B AS DateTime2), N'yaki@ligula.se', 1, 0, CAST(0x0700C9CBA35DFD360B AS DateTime2), CAST(0x07404A5683644C370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'46183FD0-3A55-4951-9432-0946EA523513', N'victor@kallhagen.se', N'kfLWbLsejwCRiiEWQuEeFuBuvKs=|1|+fstjRYrV7Zb39f8jGQ7AQ==', N'5AAD9B11-EE7A-4649-A7BF-46234BF52255', N'kfLWbLsejwCRiiEWQuEeFuBuvKs=', N'victor@kallhagen.se', CAST(0x0780D7F1754F0D380B AS DateTime2), N'victor@kallhagen.se', 1, 0, CAST(0x0700C8D31B39EC370B AS DateTime2), CAST(0x0740CAD25C4D0D380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'46195680-0932-4C96-A78A-324B3C104D0D', N'ulf@winfo.se', N'1H2MNVTMkoO6bKuPm7xESbTcvyo=|1|5VZdmrsE64QtnShIR0ruXQ==', N'5757FF3B-A52E-4FCC-A6F7-C9BBF9704808', N'1H2MNVTMkoO6bKuPm7xESbTcvyo=', N'ulf@winfo.se', CAST(0x075045DA3E474C370B AS DateTime2), N'ulf@winfo.se', 1, 0, CAST(0x078076B9F68585360B AS DateTime2), CAST(0x07F08D923E474C370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'48348720-DB47-4607-A35D-C950D5CEB961', N'forg@viessmann.com', N'bozrdVnrtju0R1qrbMi1qM1FsBM=|1|zUunnzbugvQ/oke0pylS4A==', N'3359320C-0C42-4993-BB9B-E04F763AAAE5', N'bozrdVnrtju0R1qrbMi1qM1FsBM=', N'forg@viessmann.com', CAST(0x07D08F60832ADE380B AS DateTime2), N'forg@viessmann.com', 1, 0, CAST(0x07006CCCBF5789360B AS DateTime2), CAST(0x07D08F60832ADE380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'484F8F4F-4D0B-4CEA-9624-6973FCDF8092', N'mattias.axengard@melody.se', N'kevPxHyzzalKiwUb+yL3MPBjhmw=|1|wSWzS9fJO//EsupAzfpCng==', N'3E797523-E7EE-4E03-A2B4-967209DED24A', N'kevPxHyzzalKiwUb+yL3MPBjhmw=', N'mattias.axengard@melody.se', CAST(0x078061CBE94AED370B AS DateTime2), N'mattias.axengard@melody.se', 1, 0, CAST(0x078061CBE94AED370B AS DateTime2), CAST(0x078061CBE94AED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'48AC7D2C-7E2A-4435-835A-49EDE29A8A3F', N'Alena', N'GA9kCawKmbRaUdd8ivRv0axhE9s=|1|jFDgPYDTdrCBiJcUukCK7A==', N'1006EA58-BEFC-4EF0-848E-8F7CECD8F3F7', N'GA9kCawKmbRaUdd8ivRv0axhE9s=', N'alena', CAST(0x0790D47986685A380B AS DateTime2), N'alena.smajlagic@helloy.se', 1, 1, CAST(0x0780414D3F3E5A380B AS DateTime2), CAST(0x07F0D28CAA415A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4932B931-65A4-441E-9F39-C07082A65624', N'fredrik@selectedoffice.se', N'VGIG3Vv1tJ4NowNLxDt2J0wPkyQ=|1|mXlTgKmRpo4FUbNEhU8FnQ==', N'A5016BFF-9F40-4331-AB45-9D648998A4BA', N'VGIG3Vv1tJ4NowNLxDt2J0wPkyQ=', N'fredrik@selectedoffice.se', CAST(0x07101334E47DF7380B AS DateTime2), N'fredrik@selectedoffice.se', 1, 0, CAST(0x070011E5295853380B AS DateTime2), CAST(0x0790CA923B8BA3380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4939DD3F-1970-443D-8C9C-9C3BC901752A', N'elin.ostberg@payzone.se', N'DuoTU6h7KEtWD/YYcUY9MWBRBoU=|1|cFGPA9VpA7IRBapod1bfww==', N'4C984BDF-F20F-486A-9DD0-CB2949FBBB7B', N'DuoTU6h7KEtWD/YYcUY9MWBRBoU=', N'elin.ostberg@payzone.se', CAST(0x07B0293BEB4063380B AS DateTime2), N'elin.ostberg@payzone.se', 1, 0, CAST(0x07803FCAAB42B4370B AS DateTime2), CAST(0x0780E45CE44063380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'49B3964F-4CE6-46F0-83E6-2F96DDC608F0', N'cg.sanne@internetborder.se', N'oxz9eaBSQicgwQrcjOMSxuqFiIM=|1|n+H3L/Lh02QVN6Ilwzp3Gw==', N'819DB2D8-469F-4807-BF4A-2420CFAE58EC', N'oxz9eaBSQicgwQrcjOMSxuqFiIM=', N'cg.sanne@internetborder.se', CAST(0x07B090201B514E380B AS DateTime2), N'cg.sanne@internetborder.se', 1, 0, CAST(0x0700E75E316589360B AS DateTime2), CAST(0x07E0320B96504E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4A151079-907A-4518-83F4-1224EBB6B29D', N'eva@animalen.se', N'jIiJIpqfkgDxTQtvhYqgjVJeilk=|1|aPqbQ6bBXPFmVbzxubn9oQ==', N'406FAF69-3615-4263-947D-06E663F74D52', N'jIiJIpqfkgDxTQtvhYqgjVJeilk=', N'eva@animalen.se', CAST(0x0700DBAF5245ED370B AS DateTime2), N'eva@animalen.se', 1, 0, CAST(0x0700DBAF5245ED370B AS DateTime2), CAST(0x0700DBAF5245ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4B3737F3-405A-48B2-9A46-1E92F73D147F', N'lennart.ferngren@lundqvistbil.se', N'NW9H87g1nxBMGuaZVw/zWRDEsW8=|1|HYF/bSsqOqD+C9HJXgkjGA==', N'5D0B1F3C-0742-4D31-BD82-8089B6E08A99', N'NW9H87g1nxBMGuaZVw/zWRDEsW8=', N'lennart.ferngren@lundqvistbil.se', CAST(0x07B05EF5FB7726370B AS DateTime2), N'lennart.ferngren@lundqvistbil.se', 1, 0, CAST(0x07806BB1714BC0360B AS DateTime2), CAST(0x071051E6F46D26370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 1)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4BA12452-80FD-41CC-9E52-CDCE931980EF', N'kaj@achieveglobal.se', N'sSdNrR99IPGDp6F8QMUzkpfvLUE=|1|yK0VUHu9Ie/DHbC/9prACw==', N'538ABCBC-2BB4-4F87-B84E-3925B0499AB0', N'sSdNrR99IPGDp6F8QMUzkpfvLUE=', N'kaj@achieveglobal.se', CAST(0x0700F934B945ED370B AS DateTime2), N'kaj@achieveglobal.se', 1, 0, CAST(0x0700F934B945ED370B AS DateTime2), CAST(0x0700F934B945ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4D0FC590-F2FF-4261-B62F-AF2624011566', N'linda@studybuddy.se', N'cjRKnBu5mD30Y4u8yhmEwdtxMdk=|1|rNTjBPx98Dr3uty8TjhMzA==', N'122EAA71-320A-44DB-A62B-5AC002A372C3', N'cjRKnBu5mD30Y4u8yhmEwdtxMdk=', N'linda@studybuddy.se', CAST(0x0790B1E7E743FB380B AS DateTime2), N'linda@studybuddy.se', 1, 0, CAST(0x07806BB1714BEC370B AS DateTime2), CAST(0x07908ACDE243FB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4D0FCE28-8B39-4E7A-8B5F-4ABA08093D2F', N'Joakim@allklimat.se', N'YdyIlQA8hxv0aO0HExrbZgYu7aM=|1|eFWYEJeHs5Pw/RzOFy7KtA==', N'4006D55F-607A-4A6E-980B-ED9E5CF6ED26', N'YdyIlQA8hxv0aO0HExrbZgYu7aM=', N'joakim@allklimat.se', CAST(0x0780EFB3956EE9380B AS DateTime2), N'Joakim@allklimat.se', 1, 0, CAST(0x0700995ADF45AE370B AS DateTime2), CAST(0x07D08776AA6DE9380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4F8BCD3F-BF44-4851-9FF8-F2BCC7A24538', N'kenneth@smartrecycling.se', N'+gZPWjHROxnMAUyjzXVxKM5+oho=|1|8embZ3MCnN2I4TMpd2lFCw==', N'4F06FCEE-3526-4524-A870-B6EAEA984F4A', N'+gZPWjHROxnMAUyjzXVxKM5+oho=', N'kenneth@smartrecycling.se', CAST(0x07102ACE144CF8380B AS DateTime2), N'kenneth@smartrecycling.se', 1, 0, CAST(0x07804460D15485360B AS DateTime2), CAST(0x0730FEF4D268F7380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'4FD730D3-898A-44EC-B15A-569EBE8D6683', N'johan.rosenkvist@enoterra.se', N'cJ2S/n6stUV0i+KDEkOrgH3qvTg=|1|XiMcMT7rf6iyoAKHkR4gxw==', N'4C561634-11C2-44B5-B3FB-89E52384C9E6', N'cJ2S/n6stUV0i+KDEkOrgH3qvTg=', N'johan.rosenkvist@enoterra.se', CAST(0x07D0C9E63549AF370B AS DateTime2), N'johan.rosenkvist@enoterra.se', 1, 0, CAST(0x0700AD07E54C85360B AS DateTime2), CAST(0x078032F81549AF370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'515346F1-69C8-4F8F-9A6A-97D97526ED63', N'anneli', N'TPA5nizteikBrrWsyJpfyYugHi0=|1|PXhQHOidJHBMMoV4I0NTTg==', N'AD2277FE-84FF-49F5-9CAE-A7CC60D4C5FB', N'TPA5nizteikBrrWsyJpfyYugHi0=', N'anneli', CAST(0x07A0D8D31A43FB380B AS DateTime2), NULL, 1, 0, CAST(0x070000B9B748B1380B AS DateTime2), CAST(0x07503B316939FB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'51F256B1-E0A7-43B2-92D3-D3A076634222', N'leif@welinoco.com', N'6RnJDWDVHGShkeKXdpGgh73N3yc=|1|ZWrTP3oerpW0Zwa+gYOcdw==', N'B6A79C9E-C805-4B98-A522-446F75BF25B4', N'6RnJDWDVHGShkeKXdpGgh73N3yc=', N'leif@welinoco.com', CAST(0x075022B5B257A5360B AS DateTime2), N'leif@welinoco.com', 1, 0, CAST(0x0700CE26716785360B AS DateTime2), CAST(0x07204C15A957A5360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'521B5DA8-298D-42E1-B0C1-4F7ACD91F671', N'soren@widforss.se', N'JgZ9FZKbJj7LJa3UckS4FE0ze9I=|1|TD8yeWfbC14FF+rCEvjHrg==', N'D92627F2-50B9-49AE-B793-CE1BC1B5D247', N'JgZ9FZKbJj7LJa3UckS4FE0ze9I=', N'soren@widforss.se', CAST(0x070012CDFB7E7D370B AS DateTime2), N'soren@widforss.se', 1, 0, CAST(0x07007132F87C7D370B AS DateTime2), CAST(0x07804C3FF77E7D370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'52419678-6C8E-422C-96F0-0D6763ADDFBB', N'dean@swagg.se', N'BzBe79Q9eSyVHW+wTXoJ9aL8hMA=|1|AIeq57pwIbTYAOdYSNoF7g==', N'09EDAC23-3BEC-4609-AE50-E7FD95288B17', N'BzBe79Q9eSyVHW+wTXoJ9aL8hMA=', N'dean@swagg.se', CAST(0x07909975B94723380B AS DateTime2), N'dean@swagg.se', 1, 0, CAST(0x0700320A2E4AC8370B AS DateTime2), CAST(0x076004BF824523380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'52798545-FAB4-433B-A31A-F29AE3D0A083', N'oscar.tryvall@istone.se', N'qB3KOCmLcFg87S/0i2/dop/ipfM=|1|q3fjAjV9oH2BDKewbUcUBw==', N'930C8C30-43FD-41DE-B06F-3FD86627D023', N'qB3KOCmLcFg87S/0i2/dop/ipfM=', N'oscar.tryvall@istone.se', CAST(0x079004282942A4370B AS DateTime2), N'oscar.tryvall@istone.se', 1, 0, CAST(0x0780CBC95F3F82360B AS DateTime2), CAST(0x07C03191DB41A4370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'53563AA1-C780-48E9-A119-4E6884A7B183', N'jonas.brannvall@tennisstadion.se', N'AjsP4PVcAiHyhrqe90Rq+fEqYFs=|1|G8qWTpt1b5L9EqSg/fpRfg==', N'75BA32AD-E064-48FD-B86C-A1862D540084', N'AjsP4PVcAiHyhrqe90Rq+fEqYFs=', N'jonas.brannvall@tennisstadion.se', CAST(0x0770CEB67166E6380B AS DateTime2), N'jonas.brannvall@tennisstadion.se', 1, 0, CAST(0x0780678FFC45ED370B AS DateTime2), CAST(0x073003447964E6380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'55D47AD7-AEE3-4003-9B93-ADB1E962A1DE', N'info@batochtrailer.se', N'ou5CPEmNT7K2XTyv03pQN0niMxA=|1|JxBmifr8gUOv+jXhTpRElg==', N'E1C839BC-71B4-4096-9D55-FDB9B0378E6B', N'ou5CPEmNT7K2XTyv03pQN0niMxA=', N'info@batochtrailer.se', CAST(0x078004647C3B4E380B AS DateTime2), N'info@batochtrailer.se', 1, 0, CAST(0x078004647C3B4E380B AS DateTime2), CAST(0x078004647C3B4E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'58A52FB0-B29A-4F7A-AD00-F45C7153210A', N'matti.tianen@kultarahaksi.fi', N'VGbsJIH7bWeI/s4TQE84S0J0HZc=|1|8gjScRCmb8dRIU8Xmsjdqw==', N'9829B966-77ED-4D62-AC7D-F49BB4E27233', N'VGbsJIH7bWeI/s4TQE84S0J0HZc=', N'matti.tianen@kultarahaksi.fi', CAST(0x0700C7C4384AED370B AS DateTime2), N'matti.tianen@kultarahaksi.fi', 1, 0, CAST(0x0700C7C4384AED370B AS DateTime2), CAST(0x0700C7C4384AED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'5A6379DB-EF2C-4216-94DE-E037F13507E7', N'ulrika.audelius@levandekok.se', N'+Vq56tuRq6cYMqtdkofVlqVIKSk=|1|czgvw+nnixWfFvb1DuqEzw==', N'87E05FC3-0F7A-4740-8E5D-758D8A5083B1', N'+Vq56tuRq6cYMqtdkofVlqVIKSk=', N'ulrika.audelius@levandekok.se', CAST(0x07905938055193380B AS DateTime2), N'ulrika.audelius@levandekok.se', 1, 0, CAST(0x0700BFBBA64785360B AS DateTime2), CAST(0x07004F2CD06D92380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'5C7FC603-0C3C-40A6-8D64-E3CC2A48E229', N'bwj@goldadam.com', N'DDyBQgP/E/ahgbnSb+auiTom8rs=|1|GoFSvofUDNEbTAcVATcGHw==', N'3AC594CF-BF8D-4AE6-8669-991043370396', N'DDyBQgP/E/ahgbnSb+auiTom8rs=', N'bwj@goldadam.com', CAST(0x07F0F03F3C7FC1370B AS DateTime2), N'bwj@goldadam.com', 1, 0, CAST(0x070054BC746BC0370B AS DateTime2), CAST(0x0710E675926FC0370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'5CABFFCE-482B-4EF5-9603-BF13371B4DFC', N'anders@scampi.se', N'12MJGEdYt2L2Nr1DDXszKYrXo7k=|1|SpefJdIHWOzlw57I5x6VAw==', N'E7C2193A-3F35-41C3-83EA-D9B449686AE3', N'12MJGEdYt2L2Nr1DDXszKYrXo7k=', N'anders@scampi.se', CAST(0x0780D247E66AED370B AS DateTime2), N'anders@scampi.se', 1, 0, CAST(0x0780D247E66AED370B AS DateTime2), CAST(0x0780D247E66AED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'5CD57E87-AA17-47DF-9D33-5714C875BB85', N'demo3', N'qoTbSLkd2qgdD/fLj0K3DOCpt0s=|1|NxoT02wTg6j/8tC0gcTWpQ==', N'BEDA9010-FA02-4C83-907E-BAAE1DEF9BEC', N'qoTbSLkd2qgdD/fLj0K3DOCpt0s=', N'demo3', CAST(0x0770CC13D34AFB380B AS DateTime2), NULL, 1, 0, CAST(0x0780CD08676F17380B AS DateTime2), CAST(0x0710CE45D24AFB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'60407DFE-9EC4-4887-939E-E3E976CE771E', N'Patrik Wester', N'YcoYWtrtXa3DQFMRDHXgrSv1IXg=|1|4JslfMhHaPrQ/jJSjC5KuQ==', N'AD69C12C-E225-440A-BFA2-CA59B4F4CDFE', N'YcoYWtrtXa3DQFMRDHXgrSv1IXg=', N'patrik wester', CAST(0x0740EC66F238B8380B AS DateTime2), N'patrik.wester@helloy.se', 1, 1, CAST(0x0700B8BE3B4818380B AS DateTime2), CAST(0x07909856F238B8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'6046B7DB-7BDA-49E3-AD6E-B32878C037E2', N'sales@netcompetence.se', N'FNF8vmaOOboqpo1q8UcTaHgGIh4=|1|47pnCvEB0B/aQMyON+9xoQ==', N'C91F0600-775C-4551-8703-4962113933B8', N'FNF8vmaOOboqpo1q8UcTaHgGIh4=', N'sales@netcompetence.se', CAST(0x070031D98475A7380B AS DateTime2), N'sales@netcompetence.se', 1, 0, CAST(0x070031D98475A7380B AS DateTime2), CAST(0x070031D98475A7380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'6392C7AB-D61B-4387-8D69-EBF83C3A5BD7', N'marianne.naas@swipnet.se', N'MRe+HauCAqZ+r4iZV4bDDwadAdY=|1|9T4iG50l0OqBb/O62AXKWA==', N'35B6C6EC-086F-44EB-94A4-F2F8224A96A9', N'MRe+HauCAqZ+r4iZV4bDDwadAdY=', N'marianne.naas@swipnet.se', CAST(0x07A08DD1AC538E380B AS DateTime2), N'marianne.naas@swipnet.se', 1, 0, CAST(0x07809C23F24A1E380B AS DateTime2), CAST(0x07B012F8AA538E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'64C2AECA-FAE2-40A4-BEC2-4538E494EF39', N'peter@lekit.se', N'Hn24u2pLHSuI8qx0I24n+pjxvHU=|1|Xmv2atLbRnOKuZOCwkn8UA==', N'3A236371-BD23-4BC8-B064-02E23C69A5D6', N'Hn24u2pLHSuI8qx0I24n+pjxvHU=', N'peter@lekit.se', CAST(0x0700915F744AED370B AS DateTime2), N'peter@lekit.se', 1, 0, CAST(0x0700915F744AED370B AS DateTime2), CAST(0x0700915F744AED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'66608EB8-6AF3-418B-B77E-8DF90D9F3814', N'leohei@flexpay.se', N'WGR1C2n5wT0Qnd5iD9dicNsKYDk=|1|JY3JaD0EoVCqrSm+grBzyg==', N'8D3AB133-B6E4-4C24-BD99-E4DC92560B7F', N'WGR1C2n5wT0Qnd5iD9dicNsKYDk=', N'leohei@flexpay.se', CAST(0x0790AA3D9B557D380B AS DateTime2), N'leohei@flexpay.se', 1, 0, CAST(0x07809240263677380B AS DateTime2), CAST(0x07B043C48D557D380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'691CBBE8-AF39-43FA-90ED-EDA871F03615', N'sales@telemission.se', N'p9yD/KKuugoiARyFXtFN3bneDiE=|1|9dRfEdndtqCfsoB4ZXqIBQ==', N'5D5FEF5C-6C83-4291-9DDA-32916C325DF1', N'p9yD/KKuugoiARyFXtFN3bneDiE=', N'sales@telemission.se', CAST(0x07C0F5B7666CF0380B AS DateTime2), N'sales@telemission.se', 1, 0, CAST(0x070090127D675D380B AS DateTime2), CAST(0x070090127D675D380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'6B9D7D11-7283-4629-9167-B7E9FE42D7D2', N'info@havoline.se', N'HqPoAvule9N8TepSjnz+/9ZRZ28=|1|hFGgmYkc8Us0xVEIbbnarQ==', N'9E05DE6A-809F-49B4-A920-294E198412E6', N'HqPoAvule9N8TepSjnz+/9ZRZ28=', N'info@havoline.se', CAST(0x07F09ABD1975F0380B AS DateTime2), N'info@havoline.se', 1, 0, CAST(0x07004C3A766C37370B AS DateTime2), CAST(0x07B04394D574F0380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'6BA39B93-1BA8-4DE0-BC53-8C1F7B8781D2', N'richard@studybuddy.se', N'CpofHw/Eiw5MrhDqCTFD7pTqDdk=|1|iTVh8gEhd2pszlI9upkdeQ==', N'EC6D0261-C282-41DC-9FD9-07DDA88645D2', N'CpofHw/Eiw5MrhDqCTFD7pTqDdk=', N'richard@studybuddy.se', CAST(0x07003CBFC737EE380B AS DateTime2), N'richard@studybuddy.se', 1, 0, CAST(0x07004374B44D85360B AS DateTime2), CAST(0x079078AD9E37EE380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'6E2282A8-10CA-4148-AAFF-CA7D34AC2EA9', N'Christian.Johansson@estatesm.se', N'InHlpqUi5QBQqRoO+93vgrwYt1k=|1|HaKg8NdQxVkyt+1C286zCQ==', N'8FD3E824-8334-44C7-845B-F897BF3235C7', N'InHlpqUi5QBQqRoO+93vgrwYt1k=', N'christian.johansson@estatesm.se', CAST(0x0790623A7152F8380B AS DateTime2), N'Christian.Johansson@estatesm.se', 1, 0, CAST(0x070085AA1D53EA380B AS DateTime2), CAST(0x070085AA1D53EA380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'6EEECDCA-FA9B-4ACD-9FB4-87EE04EA3AA8', N'jrw@winassist.se', N'vDjI/JQze4IvqFMaYlezdNYags4=|1|8Hu2gutbFfwAl0duB/jEiQ==', N'9D52335C-7EBC-48BF-B92F-0A34B8B8E08F', N'vDjI/JQze4IvqFMaYlezdNYags4=', N'jrw@winassist.se', CAST(0x0770AA7962590A380B AS DateTime2), N'jrw@winassist.se', 1, 0, CAST(0x0780573C0861AB370B AS DateTime2), CAST(0x0750F2D84E590A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'70EEB53B-BA14-4A13-BB60-8524FC63EB23', N'info@gardshotell.com', N'1v1HoDUqAfUdiT03s/4L9+DX6sA=|1|jdNzduKkZw4Fp7KJnrkJqQ==', N'17A14DBF-D4BC-4E07-9250-BF4DDC187B6C', N'1v1HoDUqAfUdiT03s/4L9+DX6sA=', N'info@gardshotell.com', CAST(0x07A0548E623DA1380B AS DateTime2), N'info@gardshotell.com', 1, 0, CAST(0x070058036C427D380B AS DateTime2), CAST(0x074023FCA56DA0380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7509C31E-5653-45F6-B216-D81CB4463C27', N'colibriresort@hotmail.com', N'15vMdMKmvbbGhc9vmRwCgg8mTDg=|1|0geRvfzvJxELEuFJJXe4KA==', N'D315A557-A7F3-424F-9D07-2C7EA511FF93', N'15vMdMKmvbbGhc9vmRwCgg8mTDg=', N'colibriresort@hotmail.com', CAST(0x07103FCEA27186360B AS DateTime2), N'colibriresort@hotmail.com', 1, 0, CAST(0x078008B3224285360B AS DateTime2), CAST(0x0770F28D9D7186360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'76C9243A-D3C2-43D0-85CF-B7E7B059BB9F', N'Oscar Wilkens', N'vwf5jCBYUohXeKPztoCuVPwPZcc=|1|7ueKc8cQKzWeKPoZlVnUNA==', N'041E9E1A-91CE-4141-8853-CC4E3440F8E2', N'vwf5jCBYUohXeKPztoCuVPwPZcc=', N'oscar wilkens', CAST(0x07205881897CF8380B AS DateTime2), N'oscar.wilkens@helloy.se', 1, 0, CAST(0x070004FA364818380B AS DateTime2), CAST(0x0750721BCA75F8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7726AC73-18D2-4A5B-A89A-59F1398E4DF9', N'emil@har.se', N'StkG5eyAU5OophLRD3u9XRg/xJk=|1|K66wWZklszCYI84JZQ+YcQ==', N'B19B2363-1CB9-40F5-A8C0-A9D8BFB34884', N'StkG5eyAU5OophLRD3u9XRg/xJk=', N'emil@har.se', CAST(0x07B05651986EEA370B AS DateTime2), N'emil@har.se', 1, 0, CAST(0x078066E22A8987360B AS DateTime2), CAST(0x07C0290B296EEA370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7757B9C2-61A0-4E1D-B445-537ED78DCDC8', N'kontakt@sarnmark.se', N'HUNvNnpZYK7ef1uV8Tmj+7imKoU=|1|I+Gr36sWJShW/m4WBNdWTw==', N'B407CEAF-17EE-447B-B787-67116E3973D8', N'HUNvNnpZYK7ef1uV8Tmj+7imKoU=', N'kontakt@sarnmark.se', CAST(0x0780FF5C995CF6380B AS DateTime2), N'kontakt@sarnmark.se', 1, 0, CAST(0x0780FF5C995CF6380B AS DateTime2), CAST(0x0780FF5C995CF6380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'77AE8D8A-D5A5-40E6-81FD-5DD28B8936DF', N'Sandra', N'JpExoAM0/B+xf/7m0BpBcLYLt+I=|1|fyfDSREzvswV5vw2aiW16A==', N'BD6E9774-4CBA-4FCF-9C20-03D943866A22', N'JpExoAM0/B+xf/7m0BpBcLYLt+I=', N'sandra', CAST(0x0720A05E813CFB380B AS DateTime2), N'sandra.ohlsson@helloy.se', 1, 0, CAST(0x0700A30A0282F5380B AS DateTime2), CAST(0x0790EC9D7033FB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'790912DF-3BC3-4BC3-AE8E-088A5494C938', N'jacob@susannepersson.se', N'rchYL2D4JjG+3t3sEZSdEY/Y35E=|1|ERyLwko43AgJ1254gT+k7w==', N'39D88850-B6C1-4972-A7E0-FEE96885BD9E', N'rchYL2D4JjG+3t3sEZSdEY/Y35E=', N'jacob@susannepersson.se', CAST(0x07609BE8EB7AC9370B AS DateTime2), N'jacob@susannepersson.se', 1, 0, CAST(0x0700287E084CA4360B AS DateTime2), CAST(0x0720E8F6936CC9370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7B2A3240-7D1B-4D91-939C-4DD5DFC9EE9B', N'Anna', N'yo1pyVTt0BU/rk2q1xRc9C2m+0k=|1|uk6FFPvYn4QQRfUoy9cTBQ==', N'B3DA3CB7-381A-4452-A45D-CA0F1FCE218A', N'yo1pyVTt0BU/rk2q1xRc9C2m+0k=', N'anna', CAST(0x07F074481470E7380B AS DateTime2), N'anna.lindqvist@helloy.se', 1, 0, CAST(0x07001C15ED4F41380B AS DateTime2), CAST(0x07501FD50763E7380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7B8A2497-3354-4C62-98E0-F834BC2328D5', N'therese@henryeriksson.se', N'86Iw12sOUwRBOM3iO6Gpj0jgBuw=|1|mNhr1XQLSdwdZ9NEGO2eWQ==', N'A6F4F937-8A4A-48D4-B59B-2269B0AD36D8', N'86Iw12sOUwRBOM3iO6Gpj0jgBuw=', N'therese@henryeriksson.se', CAST(0x071092D67C5FF1380B AS DateTime2), N'therese@henryeriksson.se', 1, 0, CAST(0x078044175245AE370B AS DateTime2), CAST(0x07F07E00635FF1380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7BA92B2A-DE7E-48FD-9291-750FC8430939', N'susanna.ekecrantz@freixenet.se', N'Cqdvxjv+7kdpYABZ0l9G6TPoTdQ=|1|/K5x4/Tb9lGICr4U9aM3TQ==', N'DF011F24-EE8E-499F-93AA-B17B45B3E424', N'Cqdvxjv+7kdpYABZ0l9G6TPoTdQ=', N'susanna.ekecrantz@freixenet.se', CAST(0x0770BADA7D45A2380B AS DateTime2), N'susanna.ekecrantz@freixenet.se', 1, 0, CAST(0x0700DFAF016593380B AS DateTime2), CAST(0x07A0CF515845A2380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7BED65D4-BDFA-499D-8E83-89ED396D09BB', N'tom@newkitchen.se', N'GCJMTrRYD8YgAS/bJ9G3ZYOi0zQ=|1|pJ3xmfqCzBqZq+KLYmOlyQ==', N'B66275AC-7C50-46F6-9468-A2B1D37C7CA2', N'GCJMTrRYD8YgAS/bJ9G3ZYOi0zQ=', N'tom@newkitchen.se', CAST(0x0760F904D1398E380B AS DateTime2), N'tom@newkitchen.se', 1, 0, CAST(0x070061836A3D6B380B AS DateTime2), CAST(0x07305A96A5398E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7E54A5C6-FAD7-4D34-83A9-AA2658F6BE5E', N'ulf.malmgren@havoline.se', N'ew/mfuTMvVMlI6SAsrPXVsWZj60=|1|8PM5X4t4SqDBGxF+TLEHUA==', N'5BD9B533-2B12-4454-80D8-7F64954020C3', N'ew/mfuTMvVMlI6SAsrPXVsWZj60=', N'ulf.malmgren@havoline.se', CAST(0x0730D65D1568F5380B AS DateTime2), N'ulf.malmgren@havoline.se', 1, 0, CAST(0x0780E485F75485360B AS DateTime2), CAST(0x0730AAE4EF64F5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'7F7E8022-0068-456A-B41A-99926E3045BD', N'linus', N'zaB2GjFxQ5mDM7P3w4x1Ew9Svag=|1|8sgo1gLdM/6tKGc8UUrujQ==', N'614DA79F-1320-4757-80A9-2A6F7195E05B', N'zaB2GjFxQ5mDM7P3w4x1Ew9Svag=', N'linus', CAST(0x075097ABC96672360B AS DateTime2), N'linus@helloy.se', 1, 0, CAST(0x0780EB52756739360B AS DateTime2), CAST(0x07F09202B33F72360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 1)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'831C737D-E7FF-4B75-ACFA-F0644C5869F9', N'pernilla.jangendahl.lilja@dalecarnegie.com', N'Zjo4iWVkHgBCFYVhHGcpyTvbSW4=|1|U74CnKfm25BKuSozMyzp+w==', N'259E841F-88B8-4C4B-BC82-DF817A81BE39', N'Zjo4iWVkHgBCFYVhHGcpyTvbSW4=', N'pernilla.jangendahl.lilja@dalecarnegie.com', CAST(0x07B02F03864192380B AS DateTime2), N'pernilla.jangendahl.lilja@dalecarnegie.com', 1, 0, CAST(0x0700C5BDCD3664380B AS DateTime2), CAST(0x07B01883653F92380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'83A95875-AC78-4BA6-BFB1-CD2162E4A14E', N'jan-wiklund@telia.com', N'gT3+25farl5OXWsNuyi+W7GQGWc=|1|CrPQd/jaZY8RIzpUrA4CKw==', N'539C27FE-024E-4467-B4EB-CE52B174E185', N'gT3+25farl5OXWsNuyi+W7GQGWc=', N'jan-wiklund@telia.com', CAST(0x07800F7DE468ED370B AS DateTime2), N'jan-wiklund@telia.com', 1, 0, CAST(0x07800F7DE468ED370B AS DateTime2), CAST(0x07800F7DE468ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'860D4067-6BFF-4F4D-92E0-EE4529AB39DB', N'info@hvgs.se', N'wvrJhtFTowjkzj49NMMw9FwuprA=|1|+yEjZ8VTYWmRI0V7esCiPw==', N'FB3576AC-A67F-4060-931F-CED5347FBFBB', N'wvrJhtFTowjkzj49NMMw9FwuprA=', N'info@hvgs.se', CAST(0x077074665A69DF380B AS DateTime2), N'info@hvgs.se', 1, 0, CAST(0x070077151572C2370B AS DateTime2), CAST(0x07B0544A6267DF380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'8663C0C0-8BA0-4B07-A192-52A0A07B1B81', N'lennart.sjoberg@omegaekonomi.se', N'PQ1MhSukJR31/BF3fdWhzR8qOt4=|1|vHWpKCYvE4pFD3MmxuEq+g==', N'9216812B-83E2-4B42-9D42-0382AF49B274', N'PQ1MhSukJR31/BF3fdWhzR8qOt4=', N'lennart.sjoberg@omegaekonomi.se', CAST(0x07E005E9526553380B AS DateTime2), N'lennart.sjoberg@omegaekonomi.se', 1, 0, CAST(0x07009F975E3DFD360B AS DateTime2), CAST(0x0750D8F9FF6925380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'88BCFA48-BB59-487D-8ABD-3101CCB7AC3C', N'info@gavan.nu', N'fBJqPbeyALZXlWA8lT99VsuUxac=|1|4++DlLtMI52cW0MfJ51C6A==', N'BE75D1BC-9614-4CF5-8A68-1FA174E928CB', N'fBJqPbeyALZXlWA8lT99VsuUxac=', N'info@gavan.nu', CAST(0x07702981CAABCB380B AS DateTime2), N'info@gavan.nu', 1, 0, CAST(0x07805E5B3946ED370B AS DateTime2), CAST(0x07C01CB361721D380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'8D4CD213-09AB-4864-880D-6B1C8497CC3E', N'Richard@folkhemmet.com', N'vXJekih1RZuqdZtTm2aO2wiEieY=|1|dRqYKrVOdm/SuI9h1Ctp3g==', N'79DA3D36-75E7-4F29-8676-CD7929E70237', N'vXJekih1RZuqdZtTm2aO2wiEieY=', N'richard@folkhemmet.com', CAST(0x07004E71CE6CED370B AS DateTime2), N'Richard@folkhemmet.com', 1, 0, CAST(0x07004E71CE6CED370B AS DateTime2), CAST(0x07004E71CE6CED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9077587C-91BC-4198-A96D-68075024B15B', N'demo2', N'RTdn7P1bu9nT+hGAX7ryJlb+bZQ=|1|PEs2fCtMOkb/PvYLDinu6g==', N'C4CA1B51-56B0-4610-A11F-7AA797A98ACD', N'RTdn7P1bu9nT+hGAX7ryJlb+bZQ=', N'demo2', CAST(0x0730BB4FA35D87380B AS DateTime2), N'demo2@helloy.se', 1, 1, CAST(0x070046F5474591370B AS DateTime2), CAST(0x0730C1387D5D87380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'911D9B9E-A653-4E97-988D-A44F8680B4B9', N'jonas@skelack.se', N'p31fJJpOeSTk8UW8y4vQvB42FpA=|1|iKG8pnmme8edHxIKkV9CbA==', N'831CF469-ED5E-4521-9A30-8F73309584F1', N'p31fJJpOeSTk8UW8y4vQvB42FpA=', N'jonas@skelack.se', CAST(0x07D0050AC13BC6380B AS DateTime2), N'jonas@skelack.se', 1, 0, CAST(0x0700A128194061380B AS DateTime2), CAST(0x07108332E2437D380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'93F37069-A9F8-4996-AB36-5D779D835471', N'kok@fredells.com', N'3+JdDJdOaXhZY5qRWkii78YUY9c=|1|1XXUmAFfYI36IrS7I9Zudw==', N'31B493BE-98A7-4F65-8329-81CCB1329CE3', N'3+JdDJdOaXhZY5qRWkii78YUY9c=', N'kok@fredells.com', CAST(0x07303631766ADF380B AS DateTime2), N'kok@fredells.com', 1, 0, CAST(0x07000FB18D43A2380B AS DateTime2), CAST(0x079036393165DF380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'95DBD494-0AAF-4803-9A9F-9325311721DA', N'caroline.henryson@mpsa.com', N'Kbb+hphBA8i9v4Plqndg1w8TByA=|1|pJly2il9i4/vGMAkVmMUdA==', N'0764A92E-4883-4B30-B717-5910D46B3A2D', N'Kbb+hphBA8i9v4Plqndg1w8TByA=', N'caroline.henryson@mpsa.com', CAST(0x0770CE47AB554C380B AS DateTime2), N'caroline.henryson@mpsa.com', 1, 0, CAST(0x07006B96233FCB370B AS DateTime2), CAST(0x0790B62F9D554C380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9686C480-DDA7-4C3C-9DEE-6BC7C8E63184', N'info@africanatravel.se', N'eLOKynCPGnrnkB/9NSqekUGv5Vg=|1|YX15LReMM9tEHGE7jbCEug==', N'8EC7E6F4-1A75-4240-9DEC-0EBC79672979', N'eLOKynCPGnrnkB/9NSqekUGv5Vg=', N'info@africanatravel.se', CAST(0x0700423BFB6D93380B AS DateTime2), N'info@africanatravel.se', 1, 0, CAST(0x0780311C113F6A380B AS DateTime2), CAST(0x0770B86AF46D93380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9802EA8A-FBC9-4A3E-B99E-EC35321D607F', N'anders.engstrom@cateringdirekt.se', N'22BhAQShTl+IazpfDf6Bxu2PHgA=|1|JZorgBlVnGGHlcLqq0pO2g==', N'077746F7-BB5D-4D97-9DB6-98CF4C1322D2', N'22BhAQShTl+IazpfDf6Bxu2PHgA=', N'anders.engstrom@cateringdirekt.se', CAST(0x0750817F4E51F5380B AS DateTime2), N'anders.engstrom@cateringdirekt.se', 1, 0, CAST(0x0780EE2AAF75C4370B AS DateTime2), CAST(0x07D00D14EA4BF5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'981E3886-B0E0-49F2-BDAD-5D70E885E340', N'otto.johansson@bravura.se', N'+GeSqP9E6Coh41xpgpVYRb0n9F0=|1|NypIVUEzNpDpp3r8BnE44Q==', N'99C4EBC1-BC5E-44E5-844A-B3856309C01F', N'+GeSqP9E6Coh41xpgpVYRb0n9F0=', N'otto.johansson@bravura.se', CAST(0x07A04B1BC86C02380B AS DateTime2), N'otto.johansson@bravura.se', 1, 0, CAST(0x0700A1EA044C89360B AS DateTime2), CAST(0x07404CEBC56C02380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'98383F72-9F60-45E9-80F0-054F1A7329E0', N'catherine.caldefors@pb.com', N'aCTuk4bSQK5nzeKx2Rb3Q/BW1/g=|1|fjlqg8dpEx74TQ/KuaFebw==', N'3138678C-1DEA-4067-AB3A-EC10369D0999', N'aCTuk4bSQK5nzeKx2Rb3Q/BW1/g=', N'catherine.caldefors@pb.com', CAST(0x079080D11C47B6380B AS DateTime2), N'catherine.caldefors@pb.com', 1, 0, CAST(0x07000A99C7717D380B AS DateTime2), CAST(0x07309E7A4445B6380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 2)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9BBB3604-29A1-4731-A30A-B8EABEA3E734', N'thesidebar@ligula.se', N'GIThi5jfkaGurRgdBq/KXGyV0EU=|1|0r1pWwnm5z8CvVS1iL+qUA==', N'AA81713A-1F97-4886-867D-303296D833AB', N'GIThi5jfkaGurRgdBq/KXGyV0EU=', N'thesidebar@ligula.se', CAST(0x07D01DA00A71FD360B AS DateTime2), N'thesidebar@ligula.se', 1, 0, CAST(0x078085D64E52FD360B AS DateTime2), CAST(0x0720FA7CF770FD360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9C83857D-F64D-4788-AB08-F2855FD32DCD', N'sonny@godisbolaget.com', N'EdX66jB/9qckSK954JC6b6zpx7c=|1|sMgzz6sDXUDypePdQ2BfSQ==', N'68C9B1DB-7566-4CA1-97C5-210DC672EDC3', N'EdX66jB/9qckSK954JC6b6zpx7c=', N'sonny@godisbolaget.com', CAST(0x0760BB16953D64380B AS DateTime2), N'sonny@godisbolaget.com', 1, 0, CAST(0x07000C76BD6F2C380B AS DateTime2), CAST(0x07205A30E33A64380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9D0B0BEB-66D4-407D-BA28-2ECF8DE2D907', N'nils.wanhainen@pandalus.se', N'iEtJ2YTuyRuecbInoiLqdWh0Omc=|1|G8kTsUEDvDxmgC0yMrVKww==', N'51A28C44-0042-46C7-82F0-C9AA4CA9810E', N'iEtJ2YTuyRuecbInoiLqdWh0Omc=', N'nils.wanhainen@pandalus.se', CAST(0x078006492169ED370B AS DateTime2), N'nils.wanhainen@pandalus.se', 1, 0, CAST(0x078006492169ED370B AS DateTime2), CAST(0x078006492169ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'9F8D5DF6-3119-4047-96A1-D94EE4A09269', N'caroline.wallander@pulsochtraning.se', N'rrrZbiOdZlUMpS8SfDb8CB/8EJM=|1|U3B76pFOHE9+nH+Up5KjzQ==', N'687216E8-1B8C-43BD-9CB7-8EBED17BCAFC', N'rrrZbiOdZlUMpS8SfDb8CB/8EJM=', N'caroline.wallander@pulsochtraning.se', CAST(0x07E02B14AC65F4380B AS DateTime2), N'caroline.wallander@pulsochtraning.se', 1, 0, CAST(0x0700AC66033FCB370B AS DateTime2), CAST(0x0700AC66033FCB370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'A41EE36C-6CFB-4858-9A6D-F622D4D5BC0B', N'info@studiohud-spa.se', N'LiD/taeK/f9UGebfv+TZaLQYO84=|1|Lxo1KY28nV5pTct9/DO2Ww==', N'C59783E4-B6E0-48C6-8CD7-410A6C02FEC9', N'LiD/taeK/f9UGebfv+TZaLQYO84=', N'info@studiohud-spa.se', CAST(0x0770DB1A4275CA370B AS DateTime2), N'info@studiohud-spa.se', 1, 0, CAST(0x07005FA6743F45370B AS DateTime2), CAST(0x071084678673CA370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'A47BE3FE-8699-4DC1-9816-BA88A0D30A28', N'Robin.Thyselius@ligula.se', N'PjmmS+U1Ews7fbNlRXF1eiyXAnQ=|1|0uGUgr/c6Y/4IY9MFHlHMA==', N'FF94CAEC-4588-435C-997C-97D0394B7492', N'PjmmS+U1Ews7fbNlRXF1eiyXAnQ=', N'robin.thyselius@ligula.se', CAST(0x07E068BB334A38370B AS DateTime2), N'Robin.Thyselius@ligula.se', 1, 0, CAST(0x07800DCD1F6CDC360B AS DateTime2), CAST(0x07E0E9C6136534370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'A4D95590-7C5C-46B1-B0D2-F27F20CD0A2C', N'guido@delightstudios.com', N'h1cC4mtIvU/DgCtzU3bWDCC3fNw=|1|0Mcxbrz/gdwrI6gagCRwEA==', N'3DC85C53-76CB-4A4A-B763-108443491734', N'h1cC4mtIvU/DgCtzU3bWDCC3fNw=', N'guido@delightstudios.com', CAST(0x0710CB54E3871E380B AS DateTime2), N'guido@delightstudios.com', 1, 0, CAST(0x0700699A2347ED370B AS DateTime2), CAST(0x07A08283E0871E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'A836BD68-914F-46C8-A99B-A67239B37E53', N'jonas.larsson@studybuddy.se', N'1eJmbADmrpd5K9QvLAWEBkiHxKY=|1|7ggYbcS8rS+OLWi/U+SzaQ==', N'49E98AE3-41FB-48F5-9813-3FB2A0F5DF4F', N'1eJmbADmrpd5K9QvLAWEBkiHxKY=', N'jonas.larsson@studybuddy.se', CAST(0x07D095FB9238F4380B AS DateTime2), N'jonas.larsson@studybuddy.se', 1, 0, CAST(0x0700F0811172F1380B AS DateTime2), CAST(0x07409B871438F4380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'A96687B3-845E-442E-A53B-DC08CA3F477C', N'david.skoog@mockfjards.se', N'U8FK5ziXUYtPbyoUIoooBXQ9QBc=|1|3kzA1ij1eaNWn99hgnfC1g==', N'373E6F2B-2948-4EBC-9810-9E3FF82419A9', N'U8FK5ziXUYtPbyoUIoooBXQ9QBc=', N'david.skoog@mockfjards.se', CAST(0x07908C858B7C30380B AS DateTime2), N'david.skoog@mockfjards.se', 1, 0, CAST(0x0700FBA0F17630380B AS DateTime2), CAST(0x07F0FBDB4C7C30380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'AC57DB94-5C80-4BED-93D5-78D5CE1ADD62', N'tiina@cittraverkstaden.se', N'MaJDXrhpB8MTr6EhZRARhGZBSDc=|1|MlmHRzEsNxs7w6yw0bxzZw==', N'EED213AD-7B1D-4A07-AA57-FA5394C698A7', N'MaJDXrhpB8MTr6EhZRARhGZBSDc=', N'tiina@cittraverkstaden.se', CAST(0x07E07E5C293ED2380B AS DateTime2), N'tiina@cittraverkstaden.se', 1, 0, CAST(0x07000348724785360B AS DateTime2), CAST(0x0770FB85D236A2380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'AEE0713E-8514-4D92-AC19-8BF5093FE99B', N'karin@achieveglobal.se', N'/eUsvWF+gbSfZgdw5uHLa40+61E=|1|UsDmTJzJktU+xWc8+dBEkQ==', N'6C4F9717-C8DF-4CDB-9090-6E3480A64C03', N'/eUsvWF+gbSfZgdw5uHLa40+61E=', N'karin@achieveglobal.se', CAST(0x07606CC1F84130380B AS DateTime2), N'karin@achieveglobal.se', 1, 0, CAST(0x0700F942E04CD0370B AS DateTime2), CAST(0x07501CC92A4030380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'B28642ED-A26A-4D8A-868C-34491E0FDAED', N'robin.svensson@battrebolan.se', N'N1ILDwfT+tsjV7vpo5h9JasH4ig=|1|6U1X6C/HwQhv4zTnyVqRug==', N'19F99EFB-924D-4C45-8655-3D3A0CBF8CA0', N'N1ILDwfT+tsjV7vpo5h9JasH4ig=', N'robin.svensson@battrebolan.se', CAST(0x0700BDF4868047380B AS DateTime2), N'robin.svensson@battrebolan.se', 1, 0, CAST(0x0700BDF4868047380B AS DateTime2), CAST(0x0700BDF4868047380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'B2D9B264-6D17-4319-BB1B-9AB3F0F5BE75', N'per@gignos.se', N'TCWSvtZUuwGgBg/RolYwVNd1bWY=|1|CfjAwgONOXvfoSZyb/WPQA==', N'ACEBC8FE-2D55-4949-8A71-29B49BB91E02', N'TCWSvtZUuwGgBg/RolYwVNd1bWY=', N'per@gignos.se', CAST(0x07E08529B08DBB380B AS DateTime2), N'per@gignos.se', 1, 0, CAST(0x07809E985E3F6A380B AS DateTime2), CAST(0x07B04C9DED2CB1380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'B8A88D28-C63D-4D19-9E6C-16E5EB96B02A', N'anna.sjoberg@frosunda.se', N'WPur8Sbm+XzR+nVZE2wgOHKJi5U=|1|XrxXDCWQz7jq7jLtwYtUTw==', N'59630791-DC3C-4BED-8074-A8CD4F627838', N'WPur8Sbm+XzR+nVZE2wgOHKJi5U=', N'anna.sjoberg@frosunda.se', CAST(0x0730C4767C72F5380B AS DateTime2), N'anna.sjoberg@frosunda.se', 1, 0, CAST(0x0700375A9038C0380B AS DateTime2), CAST(0x07C0F9E82F6EF5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'BA24E9B8-37D4-4484-B339-00C37D44C8E3', N'kalle@eklundbil.se', N'yz0THdA/5vuT+d/+K48Ltt3u6kA=|1|0CE1+KmecmwVQETLtYIKvA==', N'73B03966-E32D-461B-82B0-A92EBDF5ED95', N'yz0THdA/5vuT+d/+K48Ltt3u6kA=', N'kalle@eklundbil.se', CAST(0x07504371714D1E380B AS DateTime2), N'kalle@eklundbil.se', 1, 0, CAST(0x07007579EF5389360B AS DateTime2), CAST(0x0750E66B764B1E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'BD857383-FC31-429A-8F53-EDAEA80C32CB', N'info@bluechip.nu', N'jd8O6yPRp6TLbWNGY9ug0CAsQCs=|1|TkW5XZhnzZ6GiwFE8NfXTw==', N'4FE32C7B-6559-4292-B6FA-3D38D5054702', N'jd8O6yPRp6TLbWNGY9ug0CAsQCs=', N'info@bluechip.nu', CAST(0x07D09D43D03CF5380B AS DateTime2), N'info@bluechip.nu', 1, 0, CAST(0x0780FA9942499A380B AS DateTime2), CAST(0x07D0D28E0A3CF5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'BF5F9F3F-508A-485B-A9C2-BD8372E1473C', N'jonas.axelsson@ballingslov-infracity.se', N'P+HO+z+pv0BrzQ+1yFLumHK1gBk=|1|rdopaVG/Lzo+FXNtcbEoCA==', N'DFFAFC63-42F7-41CA-9818-916D75913FF2', N'P+HO+z+pv0BrzQ+1yFLumHK1gBk=', N'jonas.axelsson@ballingslov-infracity.se', CAST(0x0700822C4647ED370B AS DateTime2), N'jonas.axelsson@ballingslov-infracity.se', 1, 0, CAST(0x0700822C4647ED370B AS DateTime2), CAST(0x0700822C4647ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C00DD1D2-726F-49DE-B717-A8919A3BAF7D', N'gabriella@lekit.se', N'vd+Cd79QXhu6uFCuU1b8BAMY/bM=|1|FQE+Ldxr8F71Y6j1foSneQ==', N'B7E20162-4764-4876-B63A-FA1210EF7FD9', N'vd+Cd79QXhu6uFCuU1b8BAMY/bM=', N'gabriella@lekit.se', CAST(0x07D003DF5A3D3C370B AS DateTime2), N'gabriella@lekit.se', 1, 0, CAST(0x07802E543590F8360B AS DateTime2), CAST(0x07A0F2182F393C370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C1DAFF5C-B5F8-4E87-84E5-E7FA3FDFF996', N'anders@parad.se', N'gBtTtBq39FkkzP0K3Csup+d2D7Q=|1|xUs/Tjm/Nf7maE3BfX+TKw==', N'CE8D10E2-B7BB-4A42-8CD0-6409C70F9DBE', N'gBtTtBq39FkkzP0K3Csup+d2D7Q=', N'anders@parad.se', CAST(0x07301EBC387B19370B AS DateTime2), N'anders@parad.se', 1, 0, CAST(0x07808C8A3A42DC360B AS DateTime2), CAST(0x07303A64387B19370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C4EE8DE1-BDE1-4758-B895-5AE1DEA10936', N'bjorn.montnemery@battrebolan.se', N'8Lj1cnH6mXbKGvRLU4J6TUEyfac=|1|MVlI4cGUNaBXJ/BOaHjQHA==', N'6633C026-42A4-4445-962C-417796E97160', N'8Lj1cnH6mXbKGvRLU4J6TUEyfac=', N'bjorn.montnemery@battrebolan.se', CAST(0x0780E95CED46ED370B AS DateTime2), N'bjorn.montnemery@battrebolan.se', 1, 0, CAST(0x0780E95CED46ED370B AS DateTime2), CAST(0x0780E95CED46ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C4F38A10-9FE1-43BE-84AB-607366E352D7', N'magnus@hhlk.se', N'RXaH6+B3npWihLDHoSyNmZljsoI=|1|V8HcLge1s1jToZe2wMQTUg==', N'B98D6321-9774-4630-9554-7F01F4C07844', N'RXaH6+B3npWihLDHoSyNmZljsoI=', N'magnus@hhlk.se', CAST(0x07F0B742E845A0370B AS DateTime2), N'magnus@hhlk.se', 1, 0, CAST(0x0700ABD34838A0370B AS DateTime2), CAST(0x07507ECC6A45A0370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C584C952-3A54-4FAF-A2B4-819B1CF3D233', N'lars@protrans.se', N'Ag///OJ/vpT4NQrxjlpknnOGoSY=|1|AvxScPmY9zr1HDStwGYClg==', N'C5404FF1-9657-4DFC-B14B-E47E17BAC03A', N'Ag///OJ/vpT4NQrxjlpknnOGoSY=', N'lars@protrans.se', CAST(0x07B07B58AA7125380B AS DateTime2), N'lars@protrans.se', 1, 0, CAST(0x0780987AE941C7360B AS DateTime2), CAST(0x078066D1A87125380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C5A32A3E-2DD6-4887-8EED-8E8AE46E366D', N'frida@studybuddy.se', N'RV66tTa5WmqKS/lGBN3hZcHBmdw=|1|ctn4jISPBGhk58hJFaArPA==', N'B9CCCD3A-DEAB-4478-A10C-C759BD71C8AF', N'RV66tTa5WmqKS/lGBN3hZcHBmdw=', N'frida@studybuddy.se', CAST(0x07F01E885C48F5380B AS DateTime2), N'frida@studybuddy.se', 1, 0, CAST(0x07009ED77235A4380B AS DateTime2), CAST(0x0790FA3D4E48F5380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C5AD322C-4CA2-46D5-B129-E603D80F58D7', N'lars.forsbom@telemission.se', N'n2xliEp/NO7rdaBYaTtl+zu0u78=|1|vepTDy9XUdjZYOM/xYBeVw==', N'03F659A0-66AF-447D-B439-1C8AF9F23087', N'n2xliEp/NO7rdaBYaTtl+zu0u78=', N'lars.forsbom@telemission.se', CAST(0x0700B082006DED370B AS DateTime2), N'lars.forsbom@telemission.se', 1, 0, CAST(0x0700B082006DED370B AS DateTime2), CAST(0x0700B082006DED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C6941C68-4AB8-40AD-A5E9-16D48CB1B806', N'patrik.lark@xenter.se', N'+D75BSODL9SesRGbIuh8+34ACpQ=|1|Nd3SbgKQ7yk7OO6da0xdVg==', N'A39ACE65-0356-4175-8133-5F74BC0B9377', N'+D75BSODL9SesRGbIuh8+34ACpQ=', N'patrik.lark@xenter.se', CAST(0x0750F9F84DABEF380B AS DateTime2), N'patrik.lark@xenter.se', 1, 0, CAST(0x0700D3AE6F7216380B AS DateTime2), CAST(0x073098815CA9EF380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C72ED085-B8A5-4AB3-9A5C-C573F8D16CFB', N'helloyrapport@ryskaposten.se', N'mUqa2N+g7l+cK2Kt0SQ2pN4A6ec=|1|FVy+urPZfNwQ71QGwrnxOQ==', N'28C78B0F-20CD-44A5-AF22-59A9610E5410', N'mUqa2N+g7l+cK2Kt0SQ2pN4A6ec=', N'helloyrapport@ryskaposten.se', CAST(0x07907228373E46380B AS DateTime2), N'helloyrapport@ryskaposten.se', 1, 0, CAST(0x0780F78BA376AF360B AS DateTime2), CAST(0x076083EC6B3C46380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'C8A87FB1-0C71-4779-BB85-9AF304012498', N'irene.englen.glommen@gavan.nu', N'0kUFvuPwO3l8G2ySn04dABQH8Yo=|1|/Y2VUnqNBVZ6878P4D1pIg==', N'672D250C-2B04-4ED3-809D-EFAD21D73476', N'0kUFvuPwO3l8G2ySn04dABQH8Yo=', N'irene.englen.glommen@gavan.nu', CAST(0x07008AAE4446ED370B AS DateTime2), N'irene.englen.glommen@gavan.nu', 1, 0, CAST(0x07008AAE4446ED370B AS DateTime2), CAST(0x07008AAE4446ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'CA6195AA-4DAA-4D6D-8D90-5CF1C059764E', N'hernan.gil@soderbergpartners.se', N'pL7UJA/v3KfLtjGMu5TiDWV9F/Q=|1|XH9h8gV7BycNJKKMaDC7lw==', N'1C7D091B-51B4-4076-A3C1-618936BCF0E3', N'pL7UJA/v3KfLtjGMu5TiDWV9F/Q=', N'hernan.gil@soderbergpartners.se', CAST(0x07003513566B38380B AS DateTime2), N'hernan.gil@soderbergpartners.se', 1, 0, CAST(0x07003F1F966F23380B AS DateTime2), CAST(0x0770BEF5187237380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'CB37DA7D-0F62-45B2-9F81-F8E96EF11229', N'bennie.gillberg@boson.nu', N'51Gn6V3Qa0H6WbwCabXh3kC2RKc=|1|yXE/c15PjO+W2rnHKRuhlg==', N'502D4252-B434-4C48-9F49-B1BF77D33ED6', N'51Gn6V3Qa0H6WbwCabXh3kC2RKc=', N'bennie.gillberg@boson.nu', CAST(0x07D0FB2B494604370B AS DateTime2), N'bennie.gillberg@boson.nu', 1, 0, CAST(0x070081E20A5089360B AS DateTime2), CAST(0x07309AB5E64304370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'CBF1C53B-4E57-4D45-8DEB-8B4714B67CE1', N'camilla.hedstrom@gcm.se', N'6ngEg50snCcGlXv6xgUXh+umvCk=|1|GC+Pf8hTyN5VTD1S/3cNtg==', N'3C230945-424E-460D-93E7-E23AB6769E3F', N'6ngEg50snCcGlXv6xgUXh+umvCk=', N'camilla.hedstrom@gcm.se', CAST(0x079095855F4AEE380B AS DateTime2), N'camilla.hedstrom@gcm.se', 1, 0, CAST(0x07004347834CED370B AS DateTime2), CAST(0x07B007985B4AEE380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'CD115855-C2C6-4A5C-B741-419269C61071', N'oskar@aotaxi.se', N'hL/hCyTiZfZlt9zg3p7f7T4je18=|1|XmiuUX7zqc/JS42cU9+zIQ==', N'F7DAC642-9EA9-4A5C-9635-25E34C42A42E', N'hL/hCyTiZfZlt9zg3p7f7T4je18=', N'oskar@aotaxi.se', CAST(0x0710D36A417AA1370B AS DateTime2), N'oskar@aotaxi.se', 1, 0, CAST(0x0700A24A167AA1370B AS DateTime2), CAST(0x07C0B1613E7AA1370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'CDA44D86-D455-4711-BA3F-BEF3730C3F54', N'fredrik@youschool.se', N'zc0ZN+K/e+97TIDQOYx8u7zTO/M=|1|fEfbo97bl3x6+fO1IWV9uA==', N'43C656B8-E2FF-4259-A018-309035C7D323', N'zc0ZN+K/e+97TIDQOYx8u7zTO/M=', N'fredrik@youschool.se', CAST(0x0790BB12F64B65380B AS DateTime2), N'fredrik@youschool.se', 1, 0, CAST(0x0780C6192377DD360B AS DateTime2), CAST(0x07508155435431380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'CE00A828-1F46-41B1-B0B3-5408158346E9', N'JonT@viessmann.com', N'vgeCLcBc/DUdlSxYiKMJ43Ywkig=|1|n6eYIGeU05+AiUuh8x0NbQ==', N'2D2E6055-5F76-4E6E-AA4D-4F7839D0C294', N'vgeCLcBc/DUdlSxYiKMJ43Ywkig=', N'jont@viessmann.com', CAST(0x078040A3296DED370B AS DateTime2), N'JonT@viessmann.com', 1, 0, CAST(0x078040A3296DED370B AS DateTime2), CAST(0x078040A3296DED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'D03ADF5C-4DB7-4D46-895D-2DE9285CF902', N'bengt@lindholm-bil.se', N'yLi5DtWjl9yHpIFQTVqCrK6ROTA=|1|nRtSEk3P9MvLHLjmD3cCRg==', N'20563E91-C4D5-4A4C-A35B-103E48E4C59C', N'yLi5DtWjl9yHpIFQTVqCrK6ROTA=', N'bengt@lindholm-bil.se', CAST(0x0790048FBA62F7360B AS DateTime2), N'bengt@lindholm-bil.se', 1, 0, CAST(0x070027EB4D4585360B AS DateTime2), CAST(0x07C00B49A662F7360B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'D11BE9FE-D93B-4592-95D4-30E755A6F3D9', N'magnus@habnj.se', N'9xS03F2V4MltgMxQ7UmkBJrgP64=|1|Rv9v8NSAYu/PSIKkTzjY5g==', N'8E26B402-E735-4F22-9F94-F8145819FAEB', N'9xS03F2V4MltgMxQ7UmkBJrgP64=', N'magnus@habnj.se', CAST(0x07B09718D83F29370B AS DateTime2), N'magnus@habnj.se', 1, 0, CAST(0x07809A2AAE3E1B370B AS DateTime2), CAST(0x07A015F63C3F29370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'D15C88D4-1304-402D-988E-8516F11167AC', N'linda.almqvist@kemibolaget.se', N'yvzJstnipEVeOotN5gcJCsKX3Ds=|1|7kDfX0YuwXP8dDNYrru34w==', N'777AF5E6-0478-41E9-B4F9-7F0B03DA8DD5', N'yvzJstnipEVeOotN5gcJCsKX3Ds=', N'linda.almqvist@kemibolaget.se', CAST(0x0770F45E2044AE370B AS DateTime2), N'linda.almqvist@kemibolaget.se', 1, 0, CAST(0x070007A53F55F9360B AS DateTime2), CAST(0x07B09C25713FAE370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'D214437A-5B85-40B4-8BD7-27294A10D826', N'henrik.jonsby@ryskaposten.se', N'IQ3h8FvLTHbJR1dMVNd8l+FghAA=|1|hAYF+APQ1qmAgY5PGYEumw==', N'8914F629-8F8B-479E-82D0-380282A8E0CB', N'IQ3h8FvLTHbJR1dMVNd8l+FghAA=', N'henrik.jonsby@ryskaposten.se', CAST(0x07107F2E4A43FB380B AS DateTime2), N'henrik.jonsby@ryskaposten.se', 1, 0, CAST(0x0700D5EB3F4A88360B AS DateTime2), CAST(0x0770CBFB4843FB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'D623CA5C-9152-46AB-9361-1B781C81BBFC', N'Lisa Skoglund', N'Iz1Jcpa+3091u+/shz+PkQfrgy0=|1|V0//dfkPgQkP7eO1KSwqUA==', N'3E035163-D02E-40F4-B2FA-31F59E65D407', N'Iz1Jcpa+3091u+/shz+PkQfrgy0=', N'lisa skoglund', CAST(0x07E0ABB5456934380B AS DateTime2), N'lisa.skoglund@helloy.se', 1, 0, CAST(0x07002304314818380B AS DateTime2), CAST(0x0770B85C3E6934380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'D8D94BEC-3C6D-45F1-BE60-0363313ADEEC', N'info@work-shop.se', N'2mWdbUXXJxv5rKAoqFugo8AxUmA=|1|vyM8piRTHripJKOcfyQuqQ==', N'138AB448-9E7C-4DCF-BE18-34F06AEF39BD', N'2mWdbUXXJxv5rKAoqFugo8AxUmA=', N'info@work-shop.se', CAST(0x077059890241F4380B AS DateTime2), N'info@work-shop.se', 1, 0, CAST(0x0780CEAC04695A380B AS DateTime2), CAST(0x07F0AF140041F4380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'DA3BD4A2-C5BF-4FA1-82AE-00FFB9F5ECA9', N'olav@millionmind.com', N'5EcgBM2veOVWRHXDrxyijJKbgfA=|1|50ENgf8k/dh8IssVhJ7XBg==', N'D7B0E124-64E2-4A1C-B0D8-76BB4C3AF20C', N'5EcgBM2veOVWRHXDrxyijJKbgfA=', N'olav@millionmind.com', CAST(0x075039649351BC380B AS DateTime2), N'olav@millionmind.com', 1, 0, CAST(0x0700656A873A9A380B AS DateTime2), CAST(0x0780676B3D51BC380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'DA574ACC-D354-4F31-840B-837EBE989188', N'info@gastromix.se', N'w1rvs3CzWLFiaz1blzs7d6e8B40=|1|sB41o3R5V5awd138kZWV+A==', N'F9C753D3-E235-4C95-AA04-B2CA7F8CB8C0', N'w1rvs3CzWLFiaz1blzs7d6e8B40=', N'info@gastromix.se', CAST(0x0790775C8967EA380B AS DateTime2), N'info@gastromix.se', 1, 0, CAST(0x0700020C5E57F4370B AS DateTime2), CAST(0x0750D097924BEA380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'DA8FAA7C-1EDC-4916-858A-AD69EFB98545', N'info@spoltec.se', N'SuNpMi+aMk8xfH/fEYn9cribv6M=|1|2vq07ZkAPDA2x9UNAL/L4A==', N'3340E29C-5256-42BA-BC88-9EE4C2CCB47E', N'SuNpMi+aMk8xfH/fEYn9cribv6M=', N'info@spoltec.se', CAST(0x07F021592D80F5380B AS DateTime2), N'info@spoltec.se', 1, 0, CAST(0x0700152965439B380B AS DateTime2), CAST(0x073076E8704BBC380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'DAE19531-7A92-4CCD-9C6D-955AA73CEE83', N'weborder@folkhemmet.com', N'h2Zuy7lqKd95GJgAy3JHTAZ4iBQ=|1|wf5fZJkzfwG70TGLRLEB2g==', N'DEDEAE80-76F0-4947-9132-4A2288443E12', N'h2Zuy7lqKd95GJgAy3JHTAZ4iBQ=', N'weborder@folkhemmet.com', CAST(0x07C060B47B8121380B AS DateTime2), N'weborder@folkhemmet.com', 1, 0, CAST(0x07805CA5FC719D360B AS DateTime2), CAST(0x07A0B302F18021380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'DBFAA886-F0FC-4A8D-B1F6-9BCB8EEC30CB', N'maria@ballingslov-infracity.se', N'KuSVH5rxUp/qMwBg21y1HtQmziA=|1|WnNuP5PO2UJoeH/jLKPmkw==', N'E27C2850-7235-4C93-AC50-4F1027469FD0', N'KuSVH5rxUp/qMwBg21y1HtQmziA=', N'maria@ballingslov-infracity.se', CAST(0x07008BC69941E1380B AS DateTime2), N'maria@ballingslov-infracity.se', 1, 0, CAST(0x07002A019C4785360B AS DateTime2), CAST(0x07204BA38741E1380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'DFEEFA3C-04B8-40AE-929F-FC958D10336C', N'anette@welin.com', N'stmFfZ4mqJWkBuCuYZGUcASVQNc=|1|F8jaCAs/tjVRKeTEAuatFg==', N'65CB8B0C-5B28-460D-94DE-41D5B52BFCA3', N'stmFfZ4mqJWkBuCuYZGUcASVQNc=', N'anette@welin.com', CAST(0x07A0A990B66949370B AS DateTime2), N'anette@welin.com', 1, 0, CAST(0x0700106096580E370B AS DateTime2), CAST(0x0710307EE94349370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'E03E9DB2-717B-4650-84CB-03487E576A7F', N'info@bohem.se', N'UPzNj621pV2Xlkjsh+EsfSeoukY=|1|8fuLbwtMv1HjRIgo+iSKbQ==', N'B9A09CB7-D929-4FFA-BAB9-9D663F56A6FC', N'UPzNj621pV2Xlkjsh+EsfSeoukY=', N'info@bohem.se', CAST(0x070019C9CF702D370B AS DateTime2), N'info@bohem.se', 1, 0, CAST(0x0700872DB14E82360B AS DateTime2), CAST(0x0750402D2F6D2D370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'E0F4ABD4-7B9B-4A11-B9E2-B10A90A6CD2F', N'martin.hammar@jeanssonpilotti.se', N'Zfj+KPx7LK4vpQgBvqzlIgvmsrY=|1|CRtll1kovtYBtHWlGPXUpA==', N'EAE8571F-E1F2-42B1-830B-B57F0E4067DD', N'Zfj+KPx7LK4vpQgBvqzlIgvmsrY=', N'martin.hammar@jeanssonpilotti.se', CAST(0x07A0D93A064A1D380B AS DateTime2), N'martin.hammar@jeanssonpilotti.se', 1, 0, CAST(0x0780250D5845AF370B AS DateTime2), CAST(0x078098AC044A1D380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'E61C5938-50CB-47B3-80E6-0385E114A205', N'robert.fredell@svenskventilationsservice.se', N'VXvm1cB/Oo9OClfZeuF2UQ5Bv7s=|1|wE0hkYyJLve2CWO94y+7Cg==', N'665EB235-EFF9-4F87-B8B6-DCEFFB91F768', N'VXvm1cB/Oo9OClfZeuF2UQ5Bv7s=', N'robert.fredell@svenskventilationsservice.se', CAST(0x07F08C71773FF8380B AS DateTime2), N'robert.fredell@svenskventilationsservice.se', 1, 0, CAST(0x070039D133853E380B AS DateTime2), CAST(0x070033D7743FF8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'E708751C-1B42-4AD2-A90F-62EAE9F558B7', N'info@stadsbud.se', N'Hbeiv4LxobfRcKjZi9mKDggCJIA=|1|Pv5iVSW+Ec1MSQN6iyRRsQ==', N'96CA8AB7-422E-4495-9DED-26658C2A33E4', N'Hbeiv4LxobfRcKjZi9mKDggCJIA=', N'info@stadsbud.se', CAST(0x07509C7ED677EE380B AS DateTime2), N'info@stadsbud.se', 1, 0, CAST(0x070097F65544AE370B AS DateTime2), CAST(0x0730AB42D377EE380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'EA928128-A400-4509-9253-2A9055F6E9E2', N'mia@parad.se', N'1WIkFO59z0L1s40dj/2tr5BHsN8=|1|1gxweqdRy83U5donAEj5fA==', N'302BB116-73B8-4C1D-B69C-A17E5635376A', N'1WIkFO59z0L1s40dj/2tr5BHsN8=', N'mia@parad.se', CAST(0x07003B3EF1490A380B AS DateTime2), N'mia@parad.se', 1, 0, CAST(0x07003B3EF1490A380B AS DateTime2), CAST(0x07003B3EF1490A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'EB1529FF-10AE-4DD7-8677-86B87278D411', N'moniekmusa@hotmail.com', N'zGwhEraoxfgiECXmcvK0d7euVlI=|1|bpRFABPiTw/ng3mVDJQPNA==', N'4B9D686A-6387-46E9-A423-21C57B4689E5', N'zGwhEraoxfgiECXmcvK0d7euVlI=', N'moniekmusa@hotmail.com', CAST(0x075051F7637BF6380B AS DateTime2), N'moniekmusa@hotmail.com', 1, 0, CAST(0x070043FB475185360B AS DateTime2), CAST(0x07106144546E1E380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'ED6A1041-C4EB-4796-97F3-DFA16DB34B5D', N'elias.demir@signon.se', N'Xu4CZJG7A/crlJfIyhz31+s7hjg=|1|XYeOYsvlEukKIwUItHKzGw==', N'A1FB24D6-23E5-48F0-A0FC-9840FCD3E7A8', N'Xu4CZJG7A/crlJfIyhz31+s7hjg=', N'elias.demir@signon.se', CAST(0x0700718F166BED370B AS DateTime2), N'elias.demir@signon.se', 1, 0, CAST(0x0700718F166BED370B AS DateTime2), CAST(0x0700718F166BED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'EDC6D69B-BD89-422E-9D60-BE3E675E6228', N'helsingborg@crawfordcenter.com', N'A7Ufmp1g07RBRF7TJENfBqt21mg=|1|3T1Ychxi3Iv180uE7hbznA==', N'DC98BB82-2832-4926-B224-0ED70023CBA7', N'A7Ufmp1g07RBRF7TJENfBqt21mg=', N'helsingborg@crawfordcenter.com', CAST(0x07E019FFAD8FED380B AS DateTime2), N'helsingborg@crawfordcenter.com', 1, 0, CAST(0x0780A9EC1E75B8380B AS DateTime2), CAST(0x07E019FFAD8FED380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'EE53A82A-53FB-49E5-BB9A-DC3739999A0E', N'info@foretagsmobler.se', N'PYRcANY+rQYGb1P5D2i2mxSxns4=|1|l+20y3ZeRA5ZNeNtds0M6Q==', N'8641EE0B-1CF0-47C7-8F81-1ED340EEA953', N'PYRcANY+rQYGb1P5D2i2mxSxns4=', N'info@foretagsmobler.se', CAST(0x0790AABD1637FB380B AS DateTime2), N'info@foretagsmobler.se', 1, 0, CAST(0x0780CD70F078B8380B AS DateTime2), CAST(0x0780CD70F078B8380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F04999BD-D61A-4F47-967C-892E2BC5732A', N'info@prognoscentret.se', N'pr/URLZ+uLSH+KWSQ3UwyBRpPus=|1|hbSlbifZpuCOWkFFUs7+lg==', N'01FF8F90-1205-4715-8F39-59A483EED795', N'pr/URLZ+uLSH+KWSQ3UwyBRpPus=', N'info@prognoscentret.se', CAST(0x07E014080E59F4380B AS DateTime2), N'info@prognoscentret.se', 1, 0, CAST(0x07008F06566465380B AS DateTime2), CAST(0x07604ECE9060ED380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F08CAD69-0815-4CCA-B34F-BBC4D0DD1F7E', N'demo', N'c/cxMq97yNemjFkQCi9zSxBfjsw=|1|/l89eUaEdE5GKcxW1LDQtg==', N'5A9ABA38-BA7C-44FC-BEDE-38816AAFABB6', N'c/cxMq97yNemjFkQCi9zSxBfjsw=', N'demo', CAST(0x078012D9EC518F370B AS DateTime2), N'allasalj@helloy.se', 0, 1, CAST(0x0700B7E7F47518370B AS DateTime2), CAST(0x078012D9EC518F370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F170C627-92BF-46FB-A7C3-497BCFE0CDE4', N'annika@delightstudios.com', N'jOYv75+jMVI9EbqJcEChkclO3f8=|1|O6i4s2xeFuxCf7Fl6hzHeg==', N'FF55D435-AAB0-497F-8EAA-9402BAA72FE5', N'jOYv75+jMVI9EbqJcEChkclO3f8=', N'annika@delightstudios.com', CAST(0x0710358BB74F6A380B AS DateTime2), N'annika@delightstudios.com', 1, 0, CAST(0x070058B73047ED370B AS DateTime2), CAST(0x07D000DFB44F6A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F1BA5F2F-F703-4225-A902-FB7D68851CAF', N'mats.winberg@strd.se', N'OVnQV2Vl5IrEhZIYNy60zlX8Kl0=|1|dNebsGjJVECIUxWrl7OFwg==', N'C685ACCB-E674-499A-BFD3-A9290F1FD076', N'OVnQV2Vl5IrEhZIYNy60zlX8Kl0=', N'mats.winberg@strd.se', CAST(0x07F096CA894CFB380B AS DateTime2), N'mats.winberg@strd.se', 1, 0, CAST(0x078011223E4A09380B AS DateTime2), CAST(0x0780EDA1BF3FFB380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F221FA34-AD02-446B-9A5B-263018FA7AB4', N'info@pmscan.se', N'tls9sohf3fORUYIWTnUeNti1AWQ=|1|x19vC8QKx9UQHZ+spvDSFA==', N'1B20B5EC-C975-4E9E-9867-985E70DC9845', N'tls9sohf3fORUYIWTnUeNti1AWQ=', N'info@pmscan.se', CAST(0x07800ABD4941E1380B AS DateTime2), N'info@pmscan.se', 1, 0, CAST(0x07800ABD4941E1380B AS DateTime2), CAST(0x07800ABD4941E1380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F453F5A8-539F-494C-A02F-FED6CE5F3CAB', N'birgitta.thorsell@yhhs.se', N'25kHtp2sYo0HCAwbGQitsejt/0E=|1|mAFROElEPwdd/K54NOhifw==', N'71B0DCEA-A8DB-48BD-A652-75905F2DD295', N'25kHtp2sYo0HCAwbGQitsejt/0E=', N'birgitta.thorsell@yhhs.se', CAST(0x0700C097156A9C380B AS DateTime2), N'birgitta.thorsell@yhhs.se', 1, 0, CAST(0x0780A9A39F659C380B AS DateTime2), CAST(0x07309CAD32679C380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F562EF50-56EC-46BC-BDEB-7D8C2B631232', N'daniel.pilotti@ryskaposten.se', N'hi9Jbzpk9bwRBHW9NiIlN4/wyxc=|1|k/O02U5Zq53vr05egriK1Q==', N'D5922744-F369-46B7-83DA-46805D52ECA3', N'hi9Jbzpk9bwRBHW9NiIlN4/wyxc=', N'daniel.pilotti@ryskaposten.se', CAST(0x07802692EB44AF370B AS DateTime2), N'daniel.pilotti@ryskaposten.se', 1, 0, CAST(0x07802692EB44AF370B AS DateTime2), CAST(0x07802692EB44AF370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F69F1CFE-3664-448A-A0E4-6C8B4CFA6F1A', N'jan.strandmark@telemission.se', N'RKz6aAI0jd869glvk03+WV+UBK8=|1|NEDyXWh62qQIhbQ1fTUc/g==', N'5B8B19E8-C093-47A6-B179-B20F0EA1C740', N'RKz6aAI0jd869glvk03+WV+UBK8=', N'jan.strandmark@telemission.se', CAST(0x07A067BAFB6EF5380B AS DateTime2), N'jan.strandmark@telemission.se', 1, 0, CAST(0x0780FD9BF16CED370B AS DateTime2), CAST(0x0780FD9BF16CED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F7632FF1-4FA3-4327-867B-BF5242E04B30', N'ak@studiohud-spa.se', N'G0fHwr45T1wOjeElBKfqFOCUQRU=|1|2Z5VneWuNYu81CRUSUaKcQ==', N'4EB2D421-7356-43FA-AC02-E106851C6849', N'G0fHwr45T1wOjeElBKfqFOCUQRU=', N'ak@studiohud-spa.se', CAST(0x070010908345ED370B AS DateTime2), N'ak@studiohud-spa.se', 1, 0, CAST(0x070010908345ED370B AS DateTime2), CAST(0x070010908345ED370B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'F94F22C0-0B60-41BE-AE09-8EAC1BD1DAB5', N'magnus@ballingslov-arninge.se', N'DbU1T1vXmMmtywEDj62G5fpVM9Y=|1|tc8XyN5OVFwX7WSgIP2w/Q==', N'41D34DF3-209D-4A62-A063-4D77363B2652', N'DbU1T1vXmMmtywEDj62G5fpVM9Y=', N'magnus@ballingslov-arninge.se', CAST(0x07509562515724380B AS DateTime2), N'magnus@ballingslov-arninge.se', 1, 0, CAST(0x0700C6BD048BAA370B AS DateTime2), CAST(0x07903E5B105724380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'FAC2F901-5473-4059-9C7B-84AF0DB64EA2', N'info@ekebybruk.se', N'ujbUHOOfMujpdikybZwUAVUj4bo=|1|fskjGnI25XDFx4+rcP7sgA==', N'55E48253-ED48-4254-8BDB-EFC609B258C3', N'ujbUHOOfMujpdikybZwUAVUj4bo=', N'info@ekebybruk.se', CAST(0x0740CFB09F6DF7380B AS DateTime2), N'info@ekebybruk.se', 1, 0, CAST(0x078076CAD978B8380B AS DateTime2), CAST(0x0720E93E406DF7380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'FD25914C-696A-4FD7-B3F3-24F87AEFE33F', N'sofia.noren@medley.se', N'utBeYzA/xS4jL4NuUnJbNd9UtaY=|1|9BhBm4BHVllyV0Jy3lMDYQ==', N'08948D60-1D04-4CFB-AD0F-24AEDB48CC61', N'utBeYzA/xS4jL4NuUnJbNd9UtaY=', N'sofia.noren@medley.se', CAST(0x0700E50CB84B5C380B AS DateTime2), N'sofia.noren@medley.se', 1, 0, CAST(0x0780C1A5C34AED370B AS DateTime2), CAST(0x0720A89C544B5C380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'FE6F279B-178F-44A3-BE03-B7CB70A497DC', N'per.ankerton@signon.se', N'skvA+u4h7vYv8Ksq5z3QycpbDgA=|1|X1PueNWblABAvgHOux5GHQ==', N'2424355D-AA0C-490F-A004-F48F9386CAEB', N'skvA+u4h7vYv8Ksq5z3QycpbDgA=', N'per.ankerton@signon.se', CAST(0x0770A6904C4D6A380B AS DateTime2), N'per.ankerton@signon.se', 1, 0, CAST(0x078060B19B40AC370B AS DateTime2), CAST(0x07D04D7C404D6A380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [SecurityStamp], [LegacyPasswordHash], [LoweredUserName], [LastActivityDate], [Email], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastLoginFailed], [LoginFailureCounter]) VALUES (N'FECE4801-6077-4798-8B50-394DA5770CD4', N'grabbarna@lrtv.se', N'Y7g595tJvvoDixaIwC1RKSSOAm8=|1|9qQo4t3d1fKzILofj88pSQ==', N'1B4FBC7F-08A3-49C2-8F60-AB7729A6E7FC', N'Y7g595tJvvoDixaIwC1RKSSOAm8=', N'grabbarna@lrtv.se', CAST(0x07904C00AA5080380B AS DateTime2), N'grabbarna@lrtv.se', 1, 0, CAST(0x078090117D6589360B AS DateTime2), CAST(0x07F022E88B5080380B AS DateTime2), CAST(0x0700000000000EC509 AS DateTime2), 0)
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'01724469-13A1-4D1B-BEBA-280285CDF7C8', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'0192FE21-A0E1-4791-98AE-25E967C2436B', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'01D4C6A8-32BD-4688-B18A-5000F581B1C6', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'02E70D66-8F67-4562-A575-85C950286A07', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'039B3B56-9DCE-4380-90F7-5F7A741D8FF7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'03C55289-83B8-443A-BCB8-FCC8E85E5D42', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'04762CC5-8105-4552-AAEF-5A3E3F874532', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'04AAF50C-6451-4BDC-9001-613B4B1FEE05', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'06BBD965-ADC1-4E09-A4EE-006C630C5CAD', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'075F2A37-9E0B-44AF-9591-DD7D1ACA11A5', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'08C511F7-2337-434A-BED4-918D74F67B9F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'09E3CAA2-EAC6-47A1-9431-CC646F2E21FA', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'0B78F080-4D1E-4237-B76C-F24D9768AB36', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'0BE7B8CA-06DF-4049-BC61-3080C763F3B3', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'0C2B18CE-3AC4-4F01-B468-2A565A8668E5', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'0CFC54A7-9010-4AE0-9F8C-B1ACCCB36EF2', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'0EB54251-A616-4D32-AC88-54D337C74B89', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'1254CA19-1603-4B09-93DA-34726EFC6E18', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'13937429-0B0D-4FEF-A30F-FCFC7489EE4A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'148C11C2-5D9E-4858-9F58-6E4E73FB5181', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'14DF176C-2538-4A09-830A-664E5E91D85E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'167DCD6B-64DF-49D5-9199-19219B2E9E68', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'18743A6F-0F95-4049-AA25-D9B9869A1CF8', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'18743A6F-0F95-4049-AA25-D9B9869A1CF8', N'ABBB7EC6-FCD2-40FB-B241-EB5A7C454F8E')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'1B509F67-4066-4E6B-AB96-B0EF842AEB25', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'1B597D96-B7E8-4F13-A8D1-D5C30E75980B', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'1CEEE997-11D2-484E-BD33-0FE907487974', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'1DE4D974-FE94-4F3C-81EA-04581C23B36B', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'21F7B746-9913-4528-819B-6D5454D91DA6', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'22795063-BFDC-4BC4-80CF-F4CD928DE4E8', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'23AED1D0-EEB8-4933-A19F-E3DCB61CBA0C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'243923E8-8A09-4868-BDD9-3AC412BB299A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2547F64F-3417-4574-BA1B-5EE4C7858DAD', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'25614DCA-233D-40B0-BE39-75C66F6941D1', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'27259D53-E913-4282-AAF1-F1D42F316542', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'275677F6-5AA5-4291-87D8-F841497F961C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'27DC37B9-7084-4B31-8F21-311709558017', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'28070B24-AD45-4A0E-AAC2-0B27848F3BEB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2815BB49-5E4F-48FD-AB14-CD417EFD9AB0', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'28B887A3-B50E-4780-A7C9-59FF0D1B95A1', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'29A52BF5-A2BA-4A48-BBFB-DF327993F616', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2A889710-F6D5-43BD-8EB3-5944A2BADAC7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2C902585-366C-47C0-B594-E76B333CE966', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2CEE7D32-B5F6-4AE2-BC78-9D7B599D524B', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2D7782FA-A07C-4BEA-9FBA-117254FCEC82', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2DD065E3-3774-49F5-9227-398DEF2B72FB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2E200A0B-0E4A-41AA-AC41-DD17C601336C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'2E291644-BA67-49D5-9B99-02DA458B912F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'30DA714F-1400-4B30-8925-FD82A81C5E1E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'320DE6B8-C141-4F88-9E62-12DF8E28CCBB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'34266622-6086-4EFB-8C6D-1A7DABC70DF6', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'34329C73-26F2-4891-98A4-161451CCF52D', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'35786EAF-F33B-4DDA-8938-F4BD60C6D1DD', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'37CC7360-5EA0-462E-A489-D221F1B56E13', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'39EC022D-A45F-49FC-AC07-7476E86ACF3A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'3A02AD00-0DBE-4C5E-A7CC-5EE294BA0CEF', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'3C7D1A1B-CA4E-41A3-AA77-F2906CE9F00C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'3DBCE4E4-798E-4174-A10B-F4C69D00FB3E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'40B74293-0F6E-4070-B2BB-AAF9F12EB2E9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'41284117-5F6C-44FA-81AB-113C394B1C96', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'430095F6-5E3E-437B-9F36-63CC8792A5B7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'43C3101F-4FBC-4EA2-95CA-B0ED7971BD9F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'46183FD0-3A55-4951-9432-0946EA523513', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'46195680-0932-4C96-A78A-324B3C104D0D', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'48348720-DB47-4607-A35D-C950D5CEB961', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'484F8F4F-4D0B-4CEA-9624-6973FCDF8092', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'48AC7D2C-7E2A-4435-835A-49EDE29A8A3F', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4932B931-65A4-441E-9F39-C07082A65624', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4939DD3F-1970-443D-8C9C-9C3BC901752A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'49B3964F-4CE6-46F0-83E6-2F96DDC608F0', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4A151079-907A-4518-83F4-1224EBB6B29D', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4B3737F3-405A-48B2-9A46-1E92F73D147F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4BA12452-80FD-41CC-9E52-CDCE931980EF', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4D0FC590-F2FF-4261-B62F-AF2624011566', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4D0FCE28-8B39-4E7A-8B5F-4ABA08093D2F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4F8BCD3F-BF44-4851-9FF8-F2BCC7A24538', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'4FD730D3-898A-44EC-B15A-569EBE8D6683', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'515346F1-69C8-4F8F-9A6A-97D97526ED63', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'51F256B1-E0A7-43B2-92D3-D3A076634222', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'521B5DA8-298D-42E1-B0C1-4F7ACD91F671', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'52419678-6C8E-422C-96F0-0D6763ADDFBB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'52798545-FAB4-433B-A31A-F29AE3D0A083', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'53563AA1-C780-48E9-A119-4E6884A7B183', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'55D47AD7-AEE3-4003-9B93-ADB1E962A1DE', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'58A52FB0-B29A-4F7A-AD00-F45C7153210A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'5A6379DB-EF2C-4216-94DE-E037F13507E7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'5C7FC603-0C3C-40A6-8D64-E3CC2A48E229', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'5CABFFCE-482B-4EF5-9603-BF13371B4DFC', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'5CD57E87-AA17-47DF-9D33-5714C875BB85', N'5F1B1DEE-272F-43C7-A2A4-D6E36E3433A3')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'60407DFE-9EC4-4887-939E-E3E976CE771E', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'6046B7DB-7BDA-49E3-AD6E-B32878C037E2', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'6392C7AB-D61B-4387-8D69-EBF83C3A5BD7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'64C2AECA-FAE2-40A4-BEC2-4538E494EF39', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'66608EB8-6AF3-418B-B77E-8DF90D9F3814', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'691CBBE8-AF39-43FA-90ED-EDA871F03615', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'6B9D7D11-7283-4629-9167-B7E9FE42D7D2', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'6BA39B93-1BA8-4DE0-BC53-8C1F7B8781D2', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'6E2282A8-10CA-4148-AAFF-CA7D34AC2EA9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'6EEECDCA-FA9B-4ACD-9FB4-87EE04EA3AA8', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'70EEB53B-BA14-4A13-BB60-8524FC63EB23', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7509C31E-5653-45F6-B216-D81CB4463C27', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'76C9243A-D3C2-43D0-85CF-B7E7B059BB9F', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7726AC73-18D2-4A5B-A89A-59F1398E4DF9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7757B9C2-61A0-4E1D-B445-537ED78DCDC8', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'77AE8D8A-D5A5-40E6-81FD-5DD28B8936DF', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'790912DF-3BC3-4BC3-AE8E-088A5494C938', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7B2A3240-7D1B-4D91-939C-4DD5DFC9EE9B', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7B8A2497-3354-4C62-98E0-F834BC2328D5', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7BA92B2A-DE7E-48FD-9291-750FC8430939', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7BED65D4-BDFA-499D-8E83-89ED396D09BB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7E54A5C6-FAD7-4D34-83A9-AA2658F6BE5E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7F7E8022-0068-456A-B41A-99926E3045BD', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'7F7E8022-0068-456A-B41A-99926E3045BD', N'ABBB7EC6-FCD2-40FB-B241-EB5A7C454F8E')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'831C737D-E7FF-4B75-ACFA-F0644C5869F9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'83A95875-AC78-4BA6-BFB1-CD2162E4A14E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'860D4067-6BFF-4F4D-92E0-EE4529AB39DB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'8663C0C0-8BA0-4B07-A192-52A0A07B1B81', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'88BCFA48-BB59-487D-8ABD-3101CCB7AC3C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'8D4CD213-09AB-4864-880D-6B1C8497CC3E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9077587C-91BC-4198-A96D-68075024B15B', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9077587C-91BC-4198-A96D-68075024B15B', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'911D9B9E-A653-4E97-988D-A44F8680B4B9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'93F37069-A9F8-4996-AB36-5D779D835471', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'95DBD494-0AAF-4803-9A9F-9325311721DA', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9686C480-DDA7-4C3C-9DEE-6BC7C8E63184', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9802EA8A-FBC9-4A3E-B99E-EC35321D607F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'981E3886-B0E0-49F2-BDAD-5D70E885E340', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'98383F72-9F60-45E9-80F0-054F1A7329E0', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9BBB3604-29A1-4731-A30A-B8EABEA3E734', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9C83857D-F64D-4788-AB08-F2855FD32DCD', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9D0B0BEB-66D4-407D-BA28-2ECF8DE2D907', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'9F8D5DF6-3119-4047-96A1-D94EE4A09269', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'A41EE36C-6CFB-4858-9A6D-F622D4D5BC0B', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'A47BE3FE-8699-4DC1-9816-BA88A0D30A28', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'A4D95590-7C5C-46B1-B0D2-F27F20CD0A2C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'A836BD68-914F-46C8-A99B-A67239B37E53', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'A96687B3-845E-442E-A53B-DC08CA3F477C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'AC57DB94-5C80-4BED-93D5-78D5CE1ADD62', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'AEE0713E-8514-4D92-AC19-8BF5093FE99B', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'B28642ED-A26A-4D8A-868C-34491E0FDAED', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'B2D9B264-6D17-4319-BB1B-9AB3F0F5BE75', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'B8A88D28-C63D-4D19-9E6C-16E5EB96B02A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'BA24E9B8-37D4-4484-B339-00C37D44C8E3', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'BD857383-FC31-429A-8F53-EDAEA80C32CB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'BF5F9F3F-508A-485B-A9C2-BD8372E1473C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C00DD1D2-726F-49DE-B717-A8919A3BAF7D', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C1DAFF5C-B5F8-4E87-84E5-E7FA3FDFF996', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C4EE8DE1-BDE1-4758-B895-5AE1DEA10936', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C4F38A10-9FE1-43BE-84AB-607366E352D7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C584C952-3A54-4FAF-A2B4-819B1CF3D233', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C5A32A3E-2DD6-4887-8EED-8E8AE46E366D', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C5AD322C-4CA2-46D5-B129-E603D80F58D7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C6941C68-4AB8-40AD-A5E9-16D48CB1B806', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C72ED085-B8A5-4AB3-9A5C-C573F8D16CFB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'C8A87FB1-0C71-4779-BB85-9AF304012498', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'CA6195AA-4DAA-4D6D-8D90-5CF1C059764E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'CB37DA7D-0F62-45B2-9F81-F8E96EF11229', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'CBF1C53B-4E57-4D45-8DEB-8B4714B67CE1', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'CD115855-C2C6-4A5C-B741-419269C61071', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'CDA44D86-D455-4711-BA3F-BEF3730C3F54', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'CE00A828-1F46-41B1-B0B3-5408158346E9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'D03ADF5C-4DB7-4D46-895D-2DE9285CF902', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'D11BE9FE-D93B-4592-95D4-30E755A6F3D9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'D15C88D4-1304-402D-988E-8516F11167AC', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'D214437A-5B85-40B4-8BD7-27294A10D826', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'D623CA5C-9152-46AB-9361-1B781C81BBFC', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'D8D94BEC-3C6D-45F1-BE60-0363313ADEEC', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'DA3BD4A2-C5BF-4FA1-82AE-00FFB9F5ECA9', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'DA574ACC-D354-4F31-840B-837EBE989188', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'DA8FAA7C-1EDC-4916-858A-AD69EFB98545', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'DAE19531-7A92-4CCD-9C6D-955AA73CEE83', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'DBFAA886-F0FC-4A8D-B1F6-9BCB8EEC30CB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'DFEEFA3C-04B8-40AE-929F-FC958D10336C', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'E03E9DB2-717B-4650-84CB-03487E576A7F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'E0F4ABD4-7B9B-4A11-B9E2-B10A90A6CD2F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'E61C5938-50CB-47B3-80E6-0385E114A205', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'E708751C-1B42-4AD2-A90F-62EAE9F558B7', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'EA928128-A400-4509-9253-2A9055F6E9E2', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'EB1529FF-10AE-4DD7-8677-86B87278D411', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'ED6A1041-C4EB-4796-97F3-DFA16DB34B5D', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'EDC6D69B-BD89-422E-9D60-BE3E675E6228', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'EE53A82A-53FB-49E5-BB9A-DC3739999A0E', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F04999BD-D61A-4F47-967C-892E2BC5732A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F08CAD69-0815-4CCA-B34F-BBC4D0DD1F7E', N'9356D8E7-0D8C-4009-880F-DD8C2F2A8B0C')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F08CAD69-0815-4CCA-B34F-BBC4D0DD1F7E', N'B55FF512-9807-4BEA-BF9E-AA29CDA60E3F')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F170C627-92BF-46FB-A7C3-497BCFE0CDE4', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F1BA5F2F-F703-4225-A902-FB7D68851CAF', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F221FA34-AD02-446B-9A5B-263018FA7AB4', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F453F5A8-539F-494C-A02F-FED6CE5F3CAB', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F562EF50-56EC-46BC-BDEB-7D8C2B631232', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F69F1CFE-3664-448A-A0E4-6C8B4CFA6F1A', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F7632FF1-4FA3-4327-867B-BF5242E04B30', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'F94F22C0-0B60-41BE-AE09-8EAC1BD1DAB5', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'FAC2F901-5473-4059-9C7B-84AF0DB64EA2', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'FD25914C-696A-4FD7-B3F3-24F87AEFE33F', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'FE6F279B-178F-44A3-BE03-B7CB70A497DC', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'FECE4801-6077-4798-8B50-394DA5770CD4', N'FDAC7B14-A352-459C-AFC5-FE5ABFF21C81')
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_UserName]    Script Date: 2014-09-08 11:21:14 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [Unique_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [dbo].[UserClaim]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserClaim_dbo.User_User_Id] FOREIGN KEY([User_Id])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserClaim] CHECK CONSTRAINT [FK_dbo.UserClaim_dbo.User_User_Id]
GO
ALTER TABLE [dbo].[UserLogin]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserLogin_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogin] CHECK CONSTRAINT [FK_dbo.UserLogin_dbo.User_UserId]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRole_dbo.Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_dbo.UserRole_dbo.Role_RoleId]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRole_dbo.User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_dbo.UserRole_dbo.User_UserId]
GO
