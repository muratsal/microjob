USE [ERP]
GO

/****** Object:  Table [dbo].[Employees]    Script Date: 24.08.2022 14:19:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employees](
	[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](150) NOT NULL,
	[LastName] [nvarchar](150) NOT NULL,
	[FullName] [nvarchar](250) NOT NULL,
	[WorkStartDate] [datetime] NOT NULL,
	[BirthDate] [datetime] NULL,
	[TitleId] [int] NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[OfficeId] [int] NOT NULL,
	[UserId] [int] NULL,
	[EmailAddress] [nvarchar](150) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[GsmNumber] [nvarchar](50) NULL,
	[Image] [nvarchar](250) NULL,
	[IsWorking] [bit] NOT NULL,
	[EndWorkDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
	[IntegrationCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Offices] FOREIGN KEY([OfficeId])
REFERENCES [dbo].[Offices] ([OfficeId])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Offices]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Parameters] FOREIGN KEY([TitleId])
REFERENCES [dbo].[Parameters] ([ParamId])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Parameters]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Parameters1] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Parameters] ([ParamId])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Parameters1]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Users]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Users1]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Users2] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Users2]
GO


