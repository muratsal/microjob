USE [ERP]
GO

/****** Object:  Table [dbo].[Counters]    Script Date: 20.10.2021 11:19:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Counters](
	[CounterId] [int] IDENTITY(1,1) NOT NULL,
	[CounterName] [nvarchar](50) NOT NULL,
	[CurrentValue] [int] NOT NULL,
	[Prefix] [nvarchar](50) NOT NULL,
	[AddYear] [bit] NOT NULL,
	[PaddingCount] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_Counters] PRIMARY KEY CLUSTERED 
(
	[CounterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Counters]  WITH CHECK ADD  CONSTRAINT [FK_Counters_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Counters] CHECK CONSTRAINT [FK_Counters_Users]
GO

ALTER TABLE [dbo].[Counters]  WITH CHECK ADD  CONSTRAINT [FK_Counters_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Counters] CHECK CONSTRAINT [FK_Counters_Users1]
GO


