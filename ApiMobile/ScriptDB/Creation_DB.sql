create database PayeTonKawa;
use PayeTonKawa
CREATE TABLE Description(
   Id INT NOT NULL IDENTITY(1,1) ,
   ISO VARCHAR(2),
   Short VARCHAR(50),
   Long TEXT,
   PRIMARY KEY(Id)
);

CREATE TABLE Rate(
   Id INT NOT NULL IDENTITY(1,1) ,
   Currency VARCHAR(50),
   Price DECIMAL(15,2),
   PRIMARY KEY(Id)
);

CREATE TABLE Tax(
   Id INT NOT NULL IDENTITY(1,1) ,
   Code VARCHAR(50),
   Percentage DECIMAL(15,2),
   PRIMARY KEY(Id)
);

CREATE TABLE Store(
   id INT NOT NULL IDENTITY(1,1) ,
   name VARCHAR(50),
   PRIMARY KEY(id)
);

CREATE TABLE Users(
   id INT NOT NULL IDENTITY(1,1) ,
   mail VARCHAR(50),
   password VARCHAR(250),
   token_Auth_API VARCHAR(50),
   PRIMARY KEY(id)
);

CREATE TABLE Product(
   Id INT  NOT NULL IDENTITY(1,1) ,
   Weight DECIMAL(15,2),
   name VARCHAR(50),
   Description INT,
   Tax INT,
   Rate INT,
   PRIMARY KEY(Id),
   FOREIGN KEY(Description) REFERENCES Description(Id),
   FOREIGN KEY(Tax) REFERENCES Tax(Id),
   FOREIGN KEY(Rate) REFERENCES Rate(Id)
);

CREATE TABLE Media(
   Id INT NOT NULL IDENTITY(1,1) ,
   Item VARBINARY(250),
   Keyword VARCHAR(50),
   Augmented_Reality VARCHAR(250),
   Product INT NOT NULL,
   PRIMARY KEY(Id),
   FOREIGN KEY(Product) REFERENCES Product(Id)
);

CREATE TABLE Stock(
   product INT  ,
   store INT,
   stock INT,
   PRIMARY KEY(product, store),
   FOREIGN KEY(product) REFERENCES Product(Id),
   FOREIGN KEY(store) REFERENCES Store(id)
);
INSERT INTO Description (Id, ISO, Short, Long)
VALUES
(1, 'FR', 'Article court', 'Article longue description'),
(2, 'EN', 'Short article', 'Long article description'),
(3, 'ES', 'Artículo corto', 'Descripción larga del artículo');

INSERT INTO Rate (Id, Currency, Price)
VALUES
(1, 'USD', 1.00),
(2, 'EUR', 0.85),
(3, 'JPY', 112.00),
(4, 'GBP', 0.72);

INSERT INTO Tax (Id, Code, Percentage)
VALUES
(1, 'VAT', 20.00),
(2, 'GST', 10.00),
(3, 'Sales Tax', 8.00);

INSERT INTO Store (id, name)
VALUES
(1, 'Store A'),
(2, 'Store B'),
(3, 'Store C');

INSERT INTO Users ( mail, password, token_Auth_API)
VALUES
('fakrimougni@gmail.com','$2a$10$qG51U1r2znHGr39neXwWXO4K36OpTPYaq/Ic/hDt9bYEachAorO6q','6SV5iEG1ZZ'),
( 'user1@mail.com', 'password1', 'token1'),
( 'user2@mail.com', 'password2', 'token2'),
( 'user3@mail.com', 'password3', 'token3'),
('fakrimougni93@gmail.com','$2a$10$DfgXWungemfDl1ToberxSu/UkQtFWnUrNtwgLpWBf6WPMEXskIhGm','6SV5iEG1EP');


INSERT INTO Product(Id, Weight, Description, name, Rate, Tax)
VALUES
(1, 0.50, 1, 'Article A', 2, 1),
(2, 1.20, 2, 'Article B', 1, 3),
(3, 0.80, 3, 'Article C', 4, 2);

INSERT INTO Media ( Item, Keyword, Augmented_Reality, Product)
VALUES
( 0x52617720496d616765, 'Raw Image', 'AR1', 1),
( 0x50646620496d616765, 'PDF Image', 'AR2', 2),
( 0x4a504720496d616765, 'JPG Image', 'AR3', 3);


INSERT INTO stock (product, store, stock)
VALUES
(1, 1, 100),
(2, 2, 200),
(3, 3, 150);
