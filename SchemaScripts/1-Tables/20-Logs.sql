USE [ERP]
GO

/****** Object:  Table [dbo].[Logs]    Script Date: 20.10.2021 11:27:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Logs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[LogType] [int] NOT NULL,
	[Source] [nvarchar](250) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[FreeText1] [nvarchar](50) NULL,
	[FreeText2] [nvarchar](50) NULL,
	[FreeText3] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Logs] CHECK CONSTRAINT [FK_Logs_Users]
GO

ALTER TABLE [dbo].[Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Logs] CHECK CONSTRAINT [FK_Logs_Users1]
GO


