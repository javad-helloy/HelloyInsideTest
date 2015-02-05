CREATE TABLE [dbo].[Task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[NumTries] [int] NULL,
	[EarliestExecution] [datetime2](7) NOT NULL
	CONSTRAINT [PK_dbo.Task] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)

ALTER TABLE dbo.InsideUser
ADD ReceiveEmail bit NOT NULL DEFAULT(1)

ALTER TABLE dbo.Client
ADD IsActive bit NOT NULL DEFAULT(1)