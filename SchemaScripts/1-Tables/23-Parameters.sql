USE [ERP]
GO

/****** Object:  Table [dbo].[Parameters]    Script Date: 20.10.2021 11:32:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Parameters](
	[ParamId] [int] IDENTITY(1,1) NOT NULL,
	[ParamType] [int] NOT NULL,
	[ParamCode] [nvarchar](150) NULL,
	[ParamDesc] [nvarchar](250) NULL,
	[ParentId] [int] NULL,
	[FreeText1] [nvarchar](50) NULL,
	[FreeText2] [nvarchar](50) NULL,
	[FreeText3] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED 
(
	[ParamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Parameters]  WITH CHECK ADD  CONSTRAINT [FK_Parameters_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Parameters] CHECK CONSTRAINT [FK_Parameters_Users]
GO

ALTER TABLE [dbo].[Parameters]  WITH CHECK ADD  CONSTRAINT [FK_Parameters_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Parameters] CHECK CONSTRAINT [FK_Parameters_Users1]
GO


