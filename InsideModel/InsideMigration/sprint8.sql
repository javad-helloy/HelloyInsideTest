CREATE TABLE [dbo].[Budget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Month] [datetime] NOT NULL,
	[Value] [Decimal](18,4) NOT NULL
	
	CONSTRAINT [PK_Budget] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 
GO

ALTER TABLE [dbo].[Budget]  WITH CHECK ADD  CONSTRAINT [FK_Budget_Client] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([Id])
ON DELETE CASCADE
GO

Update [Admin] set ImageUrl= 'http://inside.helloy.se'+ImageUrl