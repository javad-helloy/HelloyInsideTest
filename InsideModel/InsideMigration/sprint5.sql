Alter Table dbo.Client alter column ConsultantId int null
GO
Update dbo.Client set ConsultantId=NULL
GO
Delete From dbo.Consultant
GO
Alter Table dbo.Consultant add MembershipProviderId uniqueidentifier not null
GO
Insert into dbo.Consultant values
('Peter Weibull','18743A6F-0F95-4049-AA25-D9B9869A1CF8'),
('Julia Servenius','148c11c2-5d9e-4858-9f58-6e4e73fb5181'),
('Henrik Nilsson','28b887a3-b50e-4780-a7c9-59ff0d1b95a1')
GO