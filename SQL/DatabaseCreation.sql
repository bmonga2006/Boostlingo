IF NOT EXISTS (SELECT name From master.dbo.sysdatabases WHERE name = N'Person')
BEGIN
	CREATE DATABASE Person
END
GO

USE [Person]

IF NOT EXISTS (SELECT 1 FROM sys.Objects WHERE Object_id = OBJECT_ID(N'Persons'))
BEGIN
	CREATE TABLE dbo.Persons (
		PersonId INT IDENTITY(1,1) PRIMARY KEY,
		UniqueId NVARCHAR(50) UNIQUE,
		FirstName NVARCHAR(100),
		LastName NVARCHAR(100),
		Language NVARCHAR(50),
		Version FLOAT,
		DateCreated DATETIME NOT NULL Default Getdate(),
	)
END
GO

IF NOT  EXISTS (SELECT 1 FROM sys.Objects WHERE Object_id = OBJECT_ID(N'PersonBios'))
BEGIN
	CREATE TABLE dbo.PersonBios (
		PersonBioId INT IDENTITY(1,1) PRIMARY KEY,
		PersonId INT,
		BioText NVARCHAR(MAX),
		DateCreated DATETIME NOT NULL Default Getdate(),
		FOREIGN KEY (PersonId) REFERENCES Persons(PersonId)
	);
END
GO 

IF NOT  EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_Person_LastName' AND OBJECT_ID = OBJECT_ID(N'Persons'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Person_LastName] ON [dbo].[Persons]
	(
		[LastName]
	)
	INCLUDE([UniqueId],[Language],[Version])
END
GO

IF NOT  EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_Person_FirstName' AND OBJECT_ID = OBJECT_ID(N'Persons'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Person_FirstName] ON [dbo].[Persons]
	(
		[FirstName]
	)
	INCLUDE([UniqueId],[Language],[Version])
END
GO

