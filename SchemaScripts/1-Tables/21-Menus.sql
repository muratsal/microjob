USE [ERP]
GO

/****** Object:  Table [dbo].[Menus]    Script Date: 20.10.2021 11:28:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Menus](
	[MenuId] [int] IDENTITY(1,1) NOT NULL,
	[MenuIcon] [nvarchar](50) NULL,
	[Name] [nvarchar](70) NOT NULL,
	[State] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUser] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUser] [int] NULL,
 CONSTRAINT [PK_Menus] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Menus]  WITH CHECK ADD  CONSTRAINT [FK_Menus_Menus] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Menus] CHECK CONSTRAINT [FK_Menus_Menus]
GO

ALTER TABLE [dbo].[Menus]  WITH CHECK ADD  CONSTRAINT [FK_Menus_Users] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Menus] CHECK CONSTRAINT [FK_Menus_Users]
GO


