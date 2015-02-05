UPDATE [dbo].[LeadProperty]
   SET [Type] = 'TrackingNumberName'   
 WHERE LeadId in( SELECT [Id]     
				  FROM Lead 
				  where LeadType='Phone') and [Type]='Source'
GO