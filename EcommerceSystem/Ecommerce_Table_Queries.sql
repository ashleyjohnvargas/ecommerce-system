CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(15) NULL,
    Address NVARCHAR(255) NULL
);

CREATE TABLE ProductImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,        -- Auto-incrementing primary key
    ProductId INT NOT NULL,                   -- External product identifier from Inventory system
    FilePath VARCHAR(255) NOT NULL,           -- Path to the image file
    
    -- Optional: Foreign key constraint if you have an Inventory database to reference
    -- CONSTRAINT FK_ProductImages_InventoryProducts FOREIGN KEY (ProductId)
    --     REFERENCES InventoryProducts(Id)  -- Replace 'InventoryProducts' and 'Id' with the actual table and column names
);
