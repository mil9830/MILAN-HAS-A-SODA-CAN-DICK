USE [master]
GO
/****** Object:  Database [myDB]    Script Date: 5/5/2018 5:10:59 PM ******/
CREATE DATABASE [myDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'myDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\myDB.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'myDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\myDB_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [myDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [myDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [myDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [myDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [myDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [myDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [myDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [myDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [myDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [myDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [myDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [myDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [myDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [myDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [myDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [myDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [myDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [myDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [myDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [myDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [myDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [myDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [myDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [myDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [myDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [myDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [myDB] SET  MULTI_USER 
GO
ALTER DATABASE [myDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [myDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [myDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [myDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [myDB]
GO
/****** Object:  Table [dbo].[admin]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[admin](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](32) NOT NULL,
	[password] [varchar](64) NOT NULL,
	[fname] [varchar](32) NULL,
	[lname] [varchar](32) NULL,
	[lvl] [smallint] NOT NULL,
 CONSTRAINT [PK_admin] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[brand]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[brand](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](32) NULL,
	[description] [varchar](140) NULL,
 CONSTRAINT [PK_brand] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[category]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](32) NOT NULL,
	[brand_id] [int] NULL,
 CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[item]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](32) NOT NULL,
	[description] [varchar](140) NULL,
	[price] [decimal](10, 2) NULL,
	[cat_id] [int] NOT NULL,
	[length] [int] NULL,
	[width] [int] NULL,
	[weight] [int] NULL,
	[shipping_info] [varchar](150) NULL,
 CONSTRAINT [PK_item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[rating]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rating](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[rating] [smallint] NOT NULL,
	[item_id] [int] NULL,
	[user_id] [int] NULL,
 CONSTRAINT [PK_rating] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[shopping_cart]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[shopping_cart](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[item_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
 CONSTRAINT [PK_shopping_cart] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user]    Script Date: 5/5/2018 5:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](32) NOT NULL,
	[password] [varchar](64) NOT NULL,
	[fname] [varchar](32) NULL,
	[lname] [varchar](32) NULL,
	[age] [smallint] NULL,
 CONSTRAINT [PK_user] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[admin] ON 

INSERT [dbo].[admin] ([id], [username], [password], [fname], [lname], [lvl]) VALUES (1, N'ad1', N'799C230F1435063E27EB0FC849447148', N'Billy', N'Gunn', 2)
INSERT [dbo].[admin] ([id], [username], [password], [fname], [lname], [lvl]) VALUES (2, N'ad2', N'C730A75CD30A64EB468F55947380DE11', N'Road Dogg', N'Jesse James', 0)
INSERT [dbo].[admin] ([id], [username], [password], [fname], [lname], [lvl]) VALUES (3, N'ad3', N'C730A75CD30A64EB468F55947380DE11', N'Hunter', N'Helmsley', 2)
INSERT [dbo].[admin] ([id], [username], [password], [fname], [lname], [lvl]) VALUES (4, N'ad4', N'C730A75CD30A64EB468F55947380DE11', N'Shawn', N'Michaels', 1)
INSERT [dbo].[admin] ([id], [username], [password], [fname], [lname], [lvl]) VALUES (6, N'ad5', N'C730A75CD30A64EB468F55947380DE11', N'Trish', N'Stratus', 1)
INSERT [dbo].[admin] ([id], [username], [password], [fname], [lname], [lvl]) VALUES (11, N'ad6', N'C730A75CD30A64EB468F55947380DE11', N'Milan', N'Robert', 1)
SET IDENTITY_INSERT [dbo].[admin] OFF
SET IDENTITY_INSERT [dbo].[brand] ON 

INSERT [dbo].[brand] ([id], [name], [description]) VALUES (1, N'Dodge', N'The maker of the dodge viper and charger')
INSERT [dbo].[brand] ([id], [name], [description]) VALUES (2, N'Ford', N'The maker of the F150 and the Mustang')
INSERT [dbo].[brand] ([id], [name], [description]) VALUES (3, N'Chevrolet', N'The maker of the Camaro and Corvette')
SET IDENTITY_INSERT [dbo].[brand] OFF
SET IDENTITY_INSERT [dbo].[category] ON 

INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (1, N'Charger', 1)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (2, N'Challenger', 1)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (6, N'Viper', 1)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (7, N'Escape', 2)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (8, N'Mustang', 2)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (9, N'F-150', 2)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (10, N'Camaro', 3)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (11, N'Corvette', 3)
INSERT [dbo].[category] ([id], [name], [brand_id]) VALUES (12, N'Silverado', 3)
SET IDENTITY_INSERT [dbo].[category] OFF
SET IDENTITY_INSERT [dbo].[item] ON 

INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (2, N'SXT', N'This is a general description for an item.', CAST(23900.00 AS Decimal(10, 2)), 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (3, N'R/T', N'This is a general description for an item.', CAST(26777.00 AS Decimal(10, 2)), 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (4, N'Scat', N'This is a general description for an item.', CAST(38993.00 AS Decimal(10, 2)), 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (5, N'Hellcat', N'This is a general description for an item.', CAST(54521.00 AS Decimal(10, 2)), 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (6, N'SXT', N'This is a general description for an item.', CAST(54000.00 AS Decimal(10, 2)), 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (7, N'Scat Pack', N'This is a general description for an item.', CAST(32995.00 AS Decimal(10, 2)), 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (8, N'SRT Hellcat', N'This is a general description for an item.', CAST(52987.00 AS Decimal(10, 2)), 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (9, N'R/T', N'This is a general description for an item.', CAST(27984.00 AS Decimal(10, 2)), 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (10, N'R/T 10', N'This is a general description for an item.', CAST(46988.00 AS Decimal(10, 2)), 6, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (11, N'GTS', N'This is a general description for an item.', CAST(79995.00 AS Decimal(10, 2)), 6, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (12, N'SRT10', N'This is a general description for an item.', CAST(49995.00 AS Decimal(10, 2)), 6, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (13, N'GTC', N'This is a general description for an item.', CAST(85991.00 AS Decimal(10, 2)), 6, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (14, N'Standard', N'This is a general description for an item.', CAST(4499.00 AS Decimal(10, 2)), 7, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (15, N'XLT', N'This is a general description for an item.', CAST(5700.00 AS Decimal(10, 2)), 7, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (16, N'SE', N'This is a general description for an item.', CAST(15913.00 AS Decimal(10, 2)), 7, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (17, N'Standard', N'This is a general description for an item.', CAST(21488.00 AS Decimal(10, 2)), 8, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (18, N'GT', N'This is a general description for an item.', CAST(27250.00 AS Decimal(10, 2)), 8, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (19, N'GT Premium', N'This is a general description for an item.', CAST(35920.00 AS Decimal(10, 2)), 8, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (20, N'XLT', N'This is a general description for an item.', CAST(30992.00 AS Decimal(10, 2)), 9, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (21, N'Lariat', N'This is a general description for an item.', CAST(38500.00 AS Decimal(10, 2)), 9, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (22, N'Raptor', N'This is a general description for an item.', CAST(54900.00 AS Decimal(10, 2)), 9, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (23, N'LT LT1', N'This is a general description for an item.', CAST(26031.00 AS Decimal(10, 2)), 10, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (24, N'SS SS1', N'This is a general description for an item.', CAST(32934.00 AS Decimal(10, 2)), 10, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (26, N'ZL1', N'This is a general description for an item.', CAST(61979.00 AS Decimal(10, 2)), 10, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (27, N'LT3', N'This is a general description for an item.', CAST(33972.00 AS Decimal(10, 2)), 11, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (28, N'Z51 LT1', N'This is a general description for an item.', CAST(49990.00 AS Decimal(10, 2)), 11, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (29, N'LT2', N'This is a general description for an item.', CAST(53000.00 AS Decimal(10, 2)), 11, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (30, N'1500 LT', N'This is a general description for an item.', CAST(32000.00 AS Decimal(10, 2)), 12, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (31, N'1500 LTZ', N'This is a general description for an item.', CAST(38980.00 AS Decimal(10, 2)), 12, NULL, NULL, NULL, NULL)
INSERT [dbo].[item] ([id], [name], [description], [price], [cat_id], [length], [width], [weight], [shipping_info]) VALUES (32, N'1500 LT Z71', N'This is a general description for an item.', CAST(39990.00 AS Decimal(10, 2)), 12, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[item] OFF
SET IDENTITY_INSERT [dbo].[rating] ON 

INSERT [dbo].[rating] ([id], [rating], [item_id], [user_id]) VALUES (1, 5, 2, 1)
INSERT [dbo].[rating] ([id], [rating], [item_id], [user_id]) VALUES (2, 3, 3, 5)
INSERT [dbo].[rating] ([id], [rating], [item_id], [user_id]) VALUES (3, 5, 12, 3)
SET IDENTITY_INSERT [dbo].[rating] OFF
SET IDENTITY_INSERT [dbo].[user] ON 

INSERT [dbo].[user] ([id], [username], [password], [fname], [lname], [age]) VALUES (1, N'user1', N'pass', N'Matt', N'Serra', NULL)
INSERT [dbo].[user] ([id], [username], [password], [fname], [lname], [age]) VALUES (2, N'user2', N'pass', N'Chuck', N'Liddell', NULL)
INSERT [dbo].[user] ([id], [username], [password], [fname], [lname], [age]) VALUES (3, N'user3', N'pass', N'Peyton', N'Manning', NULL)
INSERT [dbo].[user] ([id], [username], [password], [fname], [lname], [age]) VALUES (4, N'user4', N'pass', N'James', N'Braddock', NULL)
INSERT [dbo].[user] ([id], [username], [password], [fname], [lname], [age]) VALUES (5, N'user5', N'pass', N'Genki', N'Sudo', NULL)
SET IDENTITY_INSERT [dbo].[user] OFF
ALTER TABLE [dbo].[category]  WITH CHECK ADD  CONSTRAINT [category_fk_bid] FOREIGN KEY([brand_id])
REFERENCES [dbo].[brand] ([id])
GO
ALTER TABLE [dbo].[category] CHECK CONSTRAINT [category_fk_bid]
GO
ALTER TABLE [dbo].[item]  WITH CHECK ADD  CONSTRAINT [fk_item_catid] FOREIGN KEY([cat_id])
REFERENCES [dbo].[category] ([id])
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [fk_item_catid]
GO
ALTER TABLE [dbo].[item]  WITH CHECK ADD  CONSTRAINT [FK_item_ratingid] FOREIGN KEY([id])
REFERENCES [dbo].[item] ([id])
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [FK_item_ratingid]
GO
ALTER TABLE [dbo].[rating]  WITH CHECK ADD  CONSTRAINT [fk_rating_userid] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[rating] CHECK CONSTRAINT [fk_rating_userid]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Brand ID Foreign Key' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'category', @level2type=N'CONSTRAINT',@level2name=N'category_fk_bid'
GO
USE [master]
GO
ALTER DATABASE [myDB] SET  READ_WRITE 
GO
