create procedure udpUpdateOrder(@OrderId int, @StoreName nvarchar(30),@ManagerOrderId int,@CustomerOrderId int, @SellerOrderId int, @ItemOrderId int, @Price decimal(18), @DateOfOrder date, @ImageUrl nvarchar(max), @ItemQuantity int, @ItemQuality nvarchar(30))
as
begin

		update [dbo].[Order] set 
		StoreName = @StoreName,
		ManagerOrderId= @ManagerOrderId,
		CustomerOrderId = @CustomerOrderId,
		SellerOrderId = @SellerOrderId,
		ItemOrderId = @ItemOrderId,
		Price = @Price,
		DateOfOrder = @DateOfOrder,
		ImageUrl = @ImageUrl,
		ItemQuantity = @ItemQuantity,
		ItemQuality = @ItemQuality
		where OrderId = @OrderId
 
end
go
CREATE procedure udpGetOrderById(@OrderId int)
as 
begin
select [StoreName],[ManagerOrderId],[CustomerOrderId],[SellerOrderId],[ItemOrderId],[Price],[DateOfOrder],[ImageUrl],[ItemQuantity],[ItemQuality],[Delivered]
from [dbo].[Order] where OrderId = @OrderId

end
go
Alter procedure udpGetOrderById_out(@OrderId int, @StoreName nvarchar(20) out, @ManagerOrderId int out, @CustomerOrderId int out, @SellerOrderId int out, @ItemOrderId int out, @Price decimal out, @DateOfOrder date out, @ImageUrl nvarchar(max) out, @ItemQuantity int out, @ItemQuality nvarchar(30) out, @Photo varbinary (max))
as 
begin
select @StoreName = [StoreName],
	@ManagerOrderId = [ManagerOrderId],
	@CustomerOrderId = [CustomerOrderId],
	@SellerOrderId = [SellerOrderId],
	@ItemOrderId  = [ItemOrderId],
	@Price = [Price],
	@ItemQuantity = [ItemQuantity],
	@DateOfOrder = [DateOfOrder],
	@ImageUrl = [ImageUrl],
	@ItemQuality = [ItemQuality]
from [dbo].[Order] where OrderId = @OrderId
return (0)


end
go

Create procedure udpDelete           
(            
   @OrderId int            
)            
as             
begin            
   Delete from [dbo].[Order] where OrderId=@OrderId            
End  
go
CREATE procedure udpUpdate            
(            
    @OrderId INTEGER ,          
    @StoreName VARCHAR(50),           
    @ManagerOrderId int,          
    @CustomerOrderId int,          
    @SellerOrderId int,          
    @ItemOrderId int,          
    @Price decimal,  
    @DateOfOrder date,            
    @ImageUrl VARCHAR(220),            
    @ItemQuantity int,            
    @ItemQuality VARCHAR(220),
	@Delivered bit
)            
as            
begin            
   Update [dbo].[Order]            
   set StoreName=@StoreName,            
   ManagerOrderId=@ManagerOrderId,            
   CustomerOrderId=@CustomerOrderId,            
   SellerOrderId=@SellerOrderId,            
   ItemOrderId=@ItemOrderId,            
   Price=@Price,          
   DateOfOrder=@DateOfOrder,          
   ImageUrl=@ImageUrl,          
   ItemQuantity=@ItemQuantity,   
   ItemQuality=@ItemQuality,
   Delivered = @Delivered
   where OrderId = @OrderId            
End
go
CREATE procedure udpAddOrder          
(          
    @StoreName VARCHAR(50),           
    @ManagerOrderId int,                  
    @CustomerOrderId int,                  
    @SellerOrderId int,                  
    @ItemOrderId int,                  
    @Price decimal,          
    @DateOfOrder date,          
    @ImageUrl VARCHAR(max),          
    @ItemQuantity int,  
    @ItemQuality VARCHAR(220),
	@Delivered bit
)          
as           
Begin           
    Insert into [dbo].[Order] (StoreName,ManagerOrderId,CustomerOrderId, SellerOrderId,ItemOrderId, Price, DateOfOrder, ImageUrl, ItemQuantity, ItemQuality, Delivered)           
    Values (@StoreName,@ManagerOrderId,@CustomerOrderId, @SellerOrderId,@ItemOrderId,@Price,@DateOfOrder,@ImageUrl, @ItemQuantity, @ItemQuality, @Delivered)           
End
go
create table AuditTable
(TableName varchar(30), ModifiedBy varchar(30), ModifiedDate varchar(30), AuditAction varchar(30))
go
GO
CREATE trigger LogChanges
on [dbo].[Order]
after insert
as
insert into dbo.AuditTable
(TableName,ModifiedBy,ModifiedDate,AuditAction)

values 
('Order',SUSER_ID(),GETDATE(),'Inserted')
GO
Create trigger LogChangesUpdate
on [dbo].[Order]
after update
as
insert into dbo.AuditTable
(TableName,ModifiedBy,ModifiedDate,AuditAction)

values 
('Order',SUSER_ID(),GETDATE(),'Updated')
GO
Create trigger LogChangesDelete
on [dbo].[Order]
after update
as
insert into dbo.AuditTable
(TableName,ModifiedBy,ModifiedDate,AuditAction)

values 
('Order',SUSER_ID(),GETDATE(),'Deleted')
go
Create trigger LogChangesUpdate
on [dbo].[Order]
after update
as
insert into dbo.AuditTable
(TableName,ModifiedBy,ModifiedDate,AuditAction)

values 
('Order',SUSER_ID(),GETDATE(),'Updated')
go
Create trigger LogChangesDelete
on [dbo].[Order]
after update
as
insert into dbo.AuditTable
(TableName,ModifiedBy,ModifiedDate,AuditAction)

values 
('Order',SUSER_ID(),GETDATE(),'Deleted')
go

CREATE TRIGGER [dbo].[AvoidDeleteOrUpdateWithoutWhere] ON [dbo].[Order]
FOR UPDATE, DELETE AS 
BEGIN 
  
    DECLARE 
        @parametr INT = @@ROWCOUNT,
        @parametr2 INT = (SELECT SUM(row_count) FROM sys.dm_db_partition_stats WHERE [object_id] = OBJECT_ID('Order') AND (index_id <= 1))
 
    IF (@parametr >= @parametr2)
    BEGIN 
        ROLLBACK TRANSACTION; 
        RAISERROR ('Operation Cannot run without Where condition"', 15, 1); 
        RETURN;
    END 
  
  
END;

go
