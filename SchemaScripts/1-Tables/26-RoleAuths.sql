USE [ERP]
GO

/****** Object:  Table [dbo].[RoleAuths]    Script Date: 20.10.2021 11:35:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RoleAuths](
	[RoleAuthId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[AuthId] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_RoleAuths] PRIMARY KEY CLUSTERED 
(
	[RoleAuthId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RoleAuths]  WITH CHECK ADD  CONSTRAINT [FK_RoleAuths_Auths] FOREIGN KEY([AuthId])
REFERENCES [dbo].[Auths] ([AuthId])
GO

ALTER TABLE [dbo].[RoleAuths] CHECK CONSTRAINT [FK_RoleAuths_Auths]
GO

ALTER TABLE [dbo].[RoleAuths]  WITH CHECK ADD  CONSTRAINT [FK_RoleAuths_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO

ALTER TABLE [dbo].[RoleAuths] CHECK CONSTRAINT [FK_RoleAuths_Roles]
GO

ALTER TABLE [dbo].[RoleAuths]  WITH CHECK ADD  CONSTRAINT [FK_RoleAuths_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[RoleAuths] CHECK CONSTRAINT [FK_RoleAuths_Users]
GO

ALTER TABLE [dbo].[RoleAuths]  WITH CHECK ADD  CONSTRAINT [FK_RoleAuths_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[RoleAuths] CHECK CONSTRAINT [FK_RoleAuths_Users1]
GO


