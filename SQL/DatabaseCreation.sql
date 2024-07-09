IF NOT EXISTS (Select name From master.dbo.sysdatabases WHERE name = N'Person')
BEGIN
	CREATE DATABASE Person
END
GO

USE [Person]

CREATE TABLE dbo.Persons (
    PersonId INT IDENTITY(1,1) PRIMARY KEY,
    UniqueId NVARCHAR(50) UNIQUE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Language NVARCHAR(50),
    Version FLOAT,
	DateCreated DATETIME not NULL Default Getdate(),
)

GO

CREATE TABLE dbo.PersonBios (
    PersonBioId INT IDENTITY(1,1) PRIMARY KEY,
    PersonId INT,
    BioText NVARCHAR(MAX),
	DateCreated DATETIME not NULL Default Getdate(),
    FOREIGN KEY (PersonId) REFERENCES Persons(PersonId)
);

GO 

CREATE NONCLUSTERED INDEX [IX_Person_FirstName] ON [dbo].[Persons]
(
	[FirstName] Desc
)
INCLUDE([UniqueId],[Language],[Version])
GO

-- Create index on LastName
CREATE NONCLUSTERED INDEX [IX_Person_LastName] ON [dbo].[Persons]
(
	[LastName] Desc
)
INCLUDE([UniqueId],[Language],[Version])
GO


