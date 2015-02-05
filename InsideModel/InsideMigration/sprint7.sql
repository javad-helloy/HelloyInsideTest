CREATE TABLE [dbo].[Lead](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Date] [datetime] NULL,
	[LeadType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Lead] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 
GO

ALTER TABLE [dbo].[Lead]  WITH CHECK ADD  CONSTRAINT [FK_Lead_Client] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([Id])
ON DELETE CASCADE
GO

CREATE TABLE [dbo].[LeadProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeadId] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_LeadProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) 
GO

ALTER TABLE [dbo].[LeadProperty]  WITH CHECK ADD  CONSTRAINT [FK_LeadProperty_Lead] FOREIGN KEY([LeadId])
REFERENCES [dbo].[Lead] ([Id])
ON DELETE CASCADE
GO


ALTER TABLE [dbo].[LeadProperty] CHECK CONSTRAINT [FK_LeadProperty_Lead]
GO

create Index IX_LeadProperty
on LeadProperty ([Type]);



CREATE TABLE [dbo].[LeadInteraction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeadId] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](max) NULL,
	CONSTRAINT [PK_LeadInteraction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) 
GO

ALTER TABLE [dbo].[LeadInteraction]  WITH CHECK ADD  CONSTRAINT [FK_LeadInteraction_Lead] FOREIGN KEY([LeadId])
REFERENCES [dbo].[Lead] ([Id])
GO

ALTER TABLE [dbo].[LeadInteraction] CHECK CONSTRAINT [FK_LeadInteraction_Lead]
GO
