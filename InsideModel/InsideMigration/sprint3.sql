CREATE TABLE [dbo].[UserInteraction](
	[Type] [nvarchar](128) NOT NULL,
	[Id] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ClientId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.UserInteraction] PRIMARY KEY CLUSTERED 
(
	[Type] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

ALTER TABLE [dbo].[UserInteraction]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInteraction_dbo.Client_ClientId] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserInteraction] CHECK CONSTRAINT [FK_dbo.UserInteraction_dbo.Client_ClientId]
GO