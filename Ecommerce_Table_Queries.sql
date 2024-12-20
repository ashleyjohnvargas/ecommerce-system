CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-incremented primary key
    Name NVARCHAR(255) NOT NULL,  -- Product name with a maximum length of 255
    Description NVARCHAR(MAX),  -- Optional description, no defined limit
    Price DECIMAL(18, 2) NOT NULL,  -- Decimal with precision 18 and scale 2
    Color NVARCHAR(100) NULL,  -- Optional color with a maximum length of 100
    Category NVARCHAR(50) NULL,  -- Optional category with a maximum length of 50
    OriginalStock INT ,  -- Non-negative integer for original stock
    CurrentStock INT ,  -- Non-negative integer for current stock
    StockStatus NVARCHAR(50) NULL,  -- Optional stock status with a maximum length of 50
    IsBeingSold BIT NOT NULL DEFAULT 1,  -- Boolean field with default value true (1)
    IsDeleted BIT NOT NULL DEFAULT 0,  -- Boolean field with default value false (0)
    DateAdded DATETIME NOT NULL DEFAULT GETDATE()  -- Defaults to current date and time
);

sp_help Products

CREATE TABLE ProductImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-incremented primary key
    ProductId INT NOT NULL,  -- Foreign key to link the image to a specific product
    FilePath NVARCHAR(MAX) NOT NULL,  -- Path of the uploaded image (variable-length text)
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)  -- Foreign key constraint
);


CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(15) NULL,
    Address NVARCHAR(255) NULL
);
