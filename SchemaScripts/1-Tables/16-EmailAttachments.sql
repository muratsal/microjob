USE [ERP]
GO

/****** Object:  Table [dbo].[EmailAttachments]    Script Date: 20.10.2021 11:26:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailAttachments](
	[EmailAttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[EmailId] [int] NULL,
	[Name] [nvarchar](250) NULL,
	[FilePath] [nvarchar](550) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_EmailAttachments] PRIMARY KEY CLUSTERED 
(
	[EmailAttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmailAttachments]  WITH CHECK ADD  CONSTRAINT [FK_EmailAttachments_Emails] FOREIGN KEY([EmailId])
REFERENCES [dbo].[Emails] ([EmailId])
GO

ALTER TABLE [dbo].[EmailAttachments] CHECK CONSTRAINT [FK_EmailAttachments_Emails]
GO

ALTER TABLE [dbo].[EmailAttachments]  WITH CHECK ADD  CONSTRAINT [FK_EmailAttachments_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[EmailAttachments] CHECK CONSTRAINT [FK_EmailAttachments_Users]
GO

ALTER TABLE [dbo].[EmailAttachments]  WITH CHECK ADD  CONSTRAINT [FK_EmailAttachments_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[EmailAttachments] CHECK CONSTRAINT [FK_EmailAttachments_Users1]
GO


