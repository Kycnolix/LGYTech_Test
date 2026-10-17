CREATE TYPE dbo.OrderItemType AS TABLE
(
    ProductId INT NOT NULL PRIMARY KEY,
    Quantity INT NOT NULL
);