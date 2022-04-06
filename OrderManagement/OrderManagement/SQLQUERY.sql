/*
Connection string:
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook;Integrated Security=True;AttachDbFilename=|DataDirectory|\Chinook.mdf
*/

/*
!!! Change @DataDirectory value according to your project directory path !!!
@DataDirectory - path to the directory where LocalDb database should be created (should not end with back slash "\")
Inside your VS.NET project create folder "AppData" and set @DataDirectory to the full path of this directory  
*/
declare @DataDirectory varchar(2000) = 'D:\My projects\DBSD\OrderManagement\OrderManagement\AppData'

declare @sql nvarchar(max) = 'CREATE DATABASE OrderManagement            
        ON PRIMARY (
           NAME=db_data,
           FILENAME = ''{DataDirectory}\OrderManagement.mdf''
        )
        LOG ON (
            NAME=db_log,
            FILENAME = ''{DataDirectory}\OrderManagement.ldf''
        )'
set @sql = replace(@sql, '{DataDirectory}', @DataDirectory)
exec (@sql)
GO
use OrderManagement
GO
/*******************************************************************************
   Create Tables
********************************************************************************/
GO
set language english;
GO
CREATE TABLE [dbo].[Customer]
(
    [CustomerId] INT NOT NULL IDENTITY,
    [FirstName] NVARCHAR(160) NOT NULL,
    [LastName] NVARCHAR(160) NOT NULL,
    [Email] NVARCHAR(160) NOT NULL,
    [Mobile] INT NOT NULL,
    [DateOfBirth] DATE NOT NULL,
    [Address] NVARCHAR(160) NOT NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerId])
);
GO
CREATE TABLE [dbo].[Item]
(
    [ItemId] INT NOT NULL IDENTITY,
    [SupplierItemId] INT NOT NULL,
    [ItemType] NVARCHAR(200) NOT NULL,
    CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED ([ItemId])
);
GO
CREATE TABLE [dbo].[Order]
(
    [OrderId] INT NOT NULL IDENTITY,
    [StoreName] NVARCHAR(30) NOT NULL,
    [ManagerOrderId] INT NOT NULL,
    [CustomerOrderId] INT NOT NULL,
    [SellerOrderId] INT NOT NULL,
    [ItemOrderId] INT NOT NULL,
    [Price] DECIMAL NOT NULL,
    [DateOfOrder] DATE NOT NULL,
    [ImageUrl] NVARCHAR(max),
    [ItemQuantity] INT,
    [ItemQuality] NVARCHAR(30),
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderId])
);
GO
CREATE TABLE [dbo].[Manager]
(
    [ManagerId] INT NOT NULL IDENTITY,
    [FirstName] NVARCHAR(20) NOT NULL,
    [LastName] NVARCHAR(20) NOT NULL,
    [Email] NVARCHAR(30),
    [Mobile] NVARCHAR(30),
    [DateOfBirth] DATE,
    [Address] NVARCHAR(70),
    CONSTRAINT [PK_Manager] PRIMARY KEY CLUSTERED ([ManagerId])
);
GO
CREATE TABLE [dbo].[Seller]
(
    [SellerId] INT NOT NULL IDENTITY,
    [FirstName] NVARCHAR(120),
    [LastName] NVARCHAR(120),
    [Email] NVARCHAR(120),
    [Mobile] NVARCHAR(120),
    [DateOfBirth] DATE,
    [Address] NVARCHAR(120),
    CONSTRAINT [PK_Seller] PRIMARY KEY CLUSTERED ([SellerId])
);
GO
CREATE TABLE [dbo].[Supplier]
(
    [SupplierId] INT NOT NULL IDENTITY,
    [SupplierName] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([SupplierId])
);

/*******************************************************************************
   Create Primary Key Unique Indexes
********************************************************************************/

/*******************************************************************************
   Create Foreign Keys
********************************************************************************/
ALTER TABLE [dbo].[Item] ADD CONSTRAINT [FK_ItemSupplier]
    FOREIGN KEY ([SupplierItemId]) REFERENCES [dbo].[Supplier] ([SupplierId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_ItemSupplier] ON [dbo].[Item] ([SupplierItemId]);
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_ManagerOrderId]
    FOREIGN KEY ([ManagerOrderId]) REFERENCES [dbo].[Manager] ([ManagerId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_ManagerOrderId] ON [dbo].[Order] ([ManagerOrderId]);
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_CustomerOrderId]
    FOREIGN KEY ([CustomerOrderId]) REFERENCES [dbo].[Customer] ([CustomerId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_CustomerOrderId] ON [dbo].[Order] ([CustomerOrderId]);
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_SellerOrderId]
    FOREIGN KEY ([SellerOrderId]) REFERENCES [dbo].[Seller] ([SellerId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_SellerOrderId] ON [dbo].[Order] ([SellerOrderId]);
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_ItemOrderId]
    FOREIGN KEY ([ItemOrderId]) REFERENCES [dbo].[Item] ([ItemId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_ItemOrderId] ON [dbo].[Order] ([ItemOrderId]);
GO
