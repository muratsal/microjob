
/****** Object:  Table [dbo].[Auths]    Script Date: 20.10.2021 11:08:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Auths](
	[AuthId] [int] IDENTITY(1,1) NOT NULL,
	[AuthCode] [nvarchar](150) NULL,
	[AuthDesc] [nvarchar](150) NULL,
	[AuthType] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_Auths] PRIMARY KEY CLUSTERED 
(
	[AuthId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Auths] ADD  CONSTRAINT [DF_Auths_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[Auths] ADD  CONSTRAINT [DF_Auths_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO

ALTER TABLE [dbo].[Auths]  WITH CHECK ADD  CONSTRAINT [FK_Auths_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Auths] CHECK CONSTRAINT [FK_Auths_Users]
GO

ALTER TABLE [dbo].[Auths]  WITH CHECK ADD  CONSTRAINT [FK_Auths_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Auths] CHECK CONSTRAINT [FK_Auths_Users1]
GO


