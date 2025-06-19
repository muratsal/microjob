USE [ERP]
GO

/****** Object:  Table [dbo].[EmailConfigs]    Script Date: 20.10.2021 11:27:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailConfigs](
	[EmailConfigId] [int] IDENTITY(1,1) NOT NULL,
	[ConfigName] [nvarchar](50) NULL,
	[Host] [nvarchar](100) NULL,
	[Port] [nvarchar](10) NULL,
	[EnableSSL] [bit] NULL,
	[UserName] [nvarchar](150) NULL,
	[Password] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_EmailConfigs] PRIMARY KEY CLUSTERED 
(
	[EmailConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmailConfigs]  WITH CHECK ADD  CONSTRAINT [FK_EmailConfigs_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[EmailConfigs] CHECK CONSTRAINT [FK_EmailConfigs_Users]
GO

ALTER TABLE [dbo].[EmailConfigs]  WITH CHECK ADD  CONSTRAINT [FK_EmailConfigs_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[EmailConfigs] CHECK CONSTRAINT [FK_EmailConfigs_Users1]
GO


