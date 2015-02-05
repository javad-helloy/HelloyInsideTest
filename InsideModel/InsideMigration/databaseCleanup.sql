DROP TABLE [dbo].[Chat]
GO

DROP TABLE [dbo].[EmailMessage]
GO

DROP TABLE [dbo].[PhoneCall]
GO

DROP TABLE [dbo].[PhoneStatistic]
GO

DROP TABLE [dbo].[UserInteraction]
GO

EXEC sp_rename '[dbo].Lead', 'Contact';
GO

EXEC sp_rename '[dbo].LeadInteraction', 'ContactInteraction';
GO

EXEC sp_rename '[dbo].LeadProperty', 'ContactProperty';
GO

EXEC sp_rename '[dbo].Contact.LeadType', 'ContactType', 'COLUMN';
GO

EXEC sp_rename '[dbo].ContactInteraction.LeadId', 'ContactId', 'COLUMN';
GO

EXEC sp_rename '[dbo].ContactProperty.LeadId', 'ContactId', 'COLUMN';
GO

sp_rename '[dbo].Contact.PK_Lead', 'PK_Contact';
GO

sp_rename '[dbo].FK_Lead_Client', 'FK_Contact_Client';
GO

sp_rename '[dbo].ContactInteraction.PK_LeadInteraction', 'PK_ContactInteraction';
GO

sp_rename '[dbo].FK_LeadInteraction_Lead', 'FK_ContactInteraction_Contact';
GO

sp_rename '[dbo].ContactProperty.PK_LeadProperty', 'PK_ContactProperty';
GO

sp_rename '[dbo].FK_LeadProperty_Lead', 'FK_ContactProperty_Contact';
GO

ALTER TABLE [dbo].[ContactInteraction]  DROP CONSTRAINT  [FK_ContactInteraction_Contact]
GO

ALTER TABLE [dbo].[ContactInteraction]  WITH NOCHECK ADD CONSTRAINT [FK_ContactInteraction_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([Id])
ON DELETE CASCADE
GO
