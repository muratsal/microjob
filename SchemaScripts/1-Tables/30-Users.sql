USE [ERP]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 30.03.2022 17:09:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Password] [nvarchar](50) NULL,
	[FirstName] [nvarchar](70) NULL,
	[LastName] [nvarchar](70) NULL,
	[IsActive] [bit] NOT NULL,
	[LastLoginDate] [datetime] NULL,
	[LastLoginIp] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
	[ProfileImage] [nvarchar](350) NULL,
	[UserType] [int] NOT NULL,
	[CustomerId] [int] NULL,
	[AccesFromOutside] [bit] NOT NULL,
	[Language] [int] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_User_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_User_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_User_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [UserType]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [AccesFromOutside]
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Parameters] FOREIGN KEY([Language])
REFERENCES [dbo].[Parameters] ([ParamId])
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Parameters]
GO


