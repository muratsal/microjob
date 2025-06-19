USE [ERP]
GO

/****** Object:  Table [dbo].[Products]    Script Date: 26.10.2021 17:35:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Code] [nvarchar](100) NOT NULL,
	[ProductImage] [nvarchar](1500) NULL,
	[Description] [nvarchar](max) NULL,
	[SupplierId] [int] NOT NULL,
	[BrandId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[PurchasePrice] [decimal](18, 4) NULL,
	[SalePrice] [decimal](18, 4) NULL,
	[CurrencyId] [int] NULL,
	[WeightInKg] [decimal](18, 4) NULL,
	[CapacityInCbm] [decimal](18, 4) NULL,
	[Note] [nvarchar](max) NULL,
	[ComissionRate] [decimal](18, 2) NULL,
	[UseComissionRate] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [int] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Parameters] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Parameters] ([ParamId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Parameters]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Parameters1] FOREIGN KEY([UnitId])
REFERENCES [dbo].[Parameters] ([ParamId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Parameters1]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Parameters2] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Parameters] ([ParamId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Parameters2]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Products] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Suppliers] ([SupplierId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Products]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Products1] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brands] ([BrandId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Products1]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Users] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Users]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Users1] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Users1]
GO


