CREATE PROCEDURE dbo.CreateOrder
    @CustomerName NVARCHAR(50),
    @Items dbo.OrderItemType READONLY,
    @OrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
        BEGIN TRANSACTION;

--Validate customer and order items
        IF NULLIF(LTRIM(RTRIM(@CustomerName)), '') IS NULL
            THROW 50001, 'Customer name is required.', 1;

IF NOT EXISTS (SELECT 1 FROM @Items)
            THROW 50002, 'At least one product is required.', 1;

IF EXISTS(
    SELECT 1 FROM @Items

    WHERE ProductId <= 0 OR Quantity <= 0
        )
            THROW 50003, 'Invalid product or quantity.', 1;

--Reject missing products
        IF EXISTS (
            SELECT 1
            FROM @Items i
            LEFT JOIN dbo.Products p ON p.Id = i.ProductId
            WHERE p.Id IS NULL
        )
            THROW 50004, 'One or more products could not be found.', 1;

--Deduct stock in a consistent product order
        DECLARE @ProductId INT;
DECLARE @Quantity INT;

DECLARE product_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT ProductId, Quantity
            FROM @Items
            ORDER BY ProductId;

OPEN product_cursor;

FETCH NEXT FROM product_cursor
        INTO @ProductId, @Quantity;

WHILE @@FETCH_STATUS = 0
        BEGIN
            UPDATE dbo.Products
            SET Stock = Stock - @Quantity
            WHERE Id = @ProductId
              AND Stock >= @Quantity;

IF @@ROWCOUNT<> 1
                THROW 50005, 'Insufficient stock for one or more products.', 1;

FETCH NEXT FROM product_cursor
            INTO @ProductId, @Quantity;
END;

CLOSE product_cursor;
DEALLOCATE product_cursor;

--Create the order using database prices
        INSERT INTO dbo.Orders
            (CustomerName, CreatedAt, Total)
        SELECT
            LTRIM(RTRIM(@CustomerName)),
            GETUTCDATE(),
            SUM(p.Price * i.Quantity)
        FROM @Items i
        INNER JOIN dbo.Products p ON p.Id = i.ProductId;

SET @OrderId = CAST(SCOPE_IDENTITY() AS INT);

--Save order lines and their original unit prices
        INSERT INTO dbo.OrderLines
            (OrderId, ProductId, Quantity, UnitPrice)
        SELECT
            @OrderId,
    i.ProductId,
    i.Quantity,
    p.Price
        FROM @Items i
        INNER JOIN dbo.Products p ON p.Id = i.ProductId;

--Record stock changes
        INSERT INTO dbo.StockMovements
            (ProductId, [Change], Reason, CreatedAt)
        SELECT
            i.ProductId,
            -i.Quantity,
            'Order created',
    GETUTCDATE()
        FROM @Items i;

COMMIT TRANSACTION;
END TRY
    BEGIN CATCH
        -- Release the cursor if an error occurred
        IF CURSOR_STATUS('local', 'product_cursor') >= 0
            CLOSE product_cursor;

IF CURSOR_STATUS('local', 'product_cursor') >= -1
            DEALLOCATE product_cursor;

--Undo all changes when the order fails
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

THROW;
END CATCH;
END;