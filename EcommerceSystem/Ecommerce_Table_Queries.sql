CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(15) NULL,
    Address NVARCHAR(255) NULL
);

CREATE TABLE Products (
	Id int IDENTITY(1,1) PRIMARY KEY,
	Name nvarchar(255) NOT NULL,
	Description nvarchar(max) NOT NULL,
	Price decimal(18, 2) NOT NULL,
	StockQuantity int NOT NULL,
	Category nvarchar(255) NOT NULL,
	IsDeleted bit NOT NULL
);