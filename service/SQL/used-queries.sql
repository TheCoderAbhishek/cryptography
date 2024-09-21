-- Query to create a Database
CREATE DATABASE cryptography;

-- Query to create a table
CREATE TABLE [cryptography].[dbo].[User] (
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    UserId            NVARCHAR(40) NOT NULL,
    Name              NVARCHAR(255) NOT NULL,
    UserName          NVARCHAR(32) NOT NULL,
    Email             NVARCHAR(255) NOT NULL,
    Password          NVARCHAR(512) NOT NULL,
    IsAdmin           BIT NOT NULL,
    IsActive          BIT NOT NULL,
    IsLocked          BIT NOT NULL,
    IsDeleted         BIT NOT NULL,
    LoginAttempts     INT NOT NULL,
    DeletedStatus     INT NOT NULL,
    CreatedOn         DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedOn         DATETIME NULL,
    DeletedOn         DATETIME NULL,
    AutoDeletedOn     DATETIME NULL,
    LastLoginDateTime DATETIME NULL,
    LockedUntil       DATETIME NULL,
    RoleId            INT NOT NULL,
    Salt              NVARCHAR(100) NOT NULL
);

-- View all data from `User` table
SELECT * FROM [cryptography].[dbo].[User];

-- Truncate and reset identity `User`
TRUNCATE TABLE [cryptography].[dbo].[User];

-- Unlock User Query
UPDATE [cryptography].[dbo].[User] SET IsLocked=1 WHERE Id = 1;

UPDATE [cryptography].[dbo].[User] SET LoginAttempts=0, IsLocked=0, LockedUntil=NULL WHERE Id = 1;

UPDATE [cryptography].[dbo].[User] SET LockedUntil=NULL, IsLocked=0, LoginAttempts=0 WHERE Id = 1;

UPDATE [cryptography].[dbo].[User] SET LockedUntil='2024-08-19 22:30:22.240' WHERE Id = 1;

-- Dummy Data in `User` table
INSERT INTO [cryptography].[dbo].[User] (
    UserId, Name, UserName, Email, Password, IsAdmin, IsActive, IsLocked, 
    IsDeleted, LoginAttempts, DeletedStatus, CreatedOn, UpdatedOn, 
    DeletedOn, AutoDeletedOn, LastLoginDateTime, LockedUntil, RoleId, Salt
)
VALUES 
('user1', 'John Doe', 'johndoe', 'john@example.com', 'hashed_password_1', 1, 1, 0, 0, 0, 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, 1, 'random_salt_1'),
('user2', 'Jane Smith', 'janesmith', 'jane@example.com', 'hashed_password_2', 0, 1, 0, 0, 0, 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, 2, 'random_salt_2'),
('user3', 'Alice Johnson', 'alicej', 'alice@example.com', 'hashed_password_3', 0, 1, 0, 0, 0, 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, 3, 'random_salt_3'),
('user4', 'Bob Brown', 'bobbrown', 'bob@example.com', 'hashed_password_4', 0, 1, 1, 0, 3, 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, 2, 'random_salt_4'),
('user5', 'Charlie White', 'charliew', 'charlie@example.com', 'hashed_password_5', 1, 0, 0, 1, 0, 1, GETDATE(), NULL, NULL, NULL, NULL, NULL, 1, 'random_salt_5');

-- Query to create a [OtpStorage] table
CREATE TABLE [cryptography].[dbo].[OtpStorage] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    GeneratedOn DATETIME NOT NULL,
    ValidUntil DATETIME NOT NULL,
    Otp NVARCHAR(50) NOT NULL,
    Salt NVARCHAR(50) NOT NULL,
    AttemptCount INT NOT NULL DEFAULT 0
);

-- View all data from `OtpStorage` table
SELECT * FROM [cryptography].[dbo].[OtpStorage];

-- Truncate and reset identity `OtpStorage`
TRUNCATE TABLE [cryptography].[dbo].[OtpStorage];

-- Table Deletion Query
DROP TABLE [cryptography].[dbo].[OtpStorage];

-- Add new column in table `OtpStorage`
ALTER TABLE [cryptography].[dbo].[OtpStorage]
ADD OtpUseCase INT;

-- Update Record SQL Query
UPDATE [cryptography].[dbo].[OtpStorage] SET ValidUntil='2024-08-14 19:00:54.183' WHERE Id=1;

-- Update Record SQL Query
UPDATE [cryptography].[dbo].[OtpStorage] SET ValidUntil='2024-08-14 19:00:54.183' WHERE Id=1;

-- Update Record SQL Query
UPDATE [cryptography].[dbo].[OtpStorage] SET ValidUntil='2024-08-14 19:00:54.183' WHERE Id=1;

-- Update User Record to disable user.
UPDATE [cryptography].[dbo].[User] SET IsActive=0 WHERE Id=1;

-- Fetch all data from 'User' table
SELECT * FROM [cryptography].[dbo].[User];

-- Delete Record from table
DELETE FROM [cryptography].[dbo].[User] WHERE Id=2;

-- Query to create a [tblUsers] table
CREATE TABLE [cryptography].[dbo].[tblUsers] (
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    UserId            NVARCHAR(40) NOT NULL,
    Name              NVARCHAR(255) NOT NULL,
    UserName          NVARCHAR(32) NOT NULL,
    Email             NVARCHAR(255) NOT NULL,
    Password          NVARCHAR(512) NOT NULL,
    IsAdmin           BIT NOT NULL,
    IsActive          BIT NOT NULL,
    IsLocked          BIT NOT NULL,
    IsDeleted         BIT NOT NULL,
    LoginAttempts     INT NOT NULL,
    DeletedStatus     INT NOT NULL,
    CreatedOn         DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedOn         DATETIME NULL,
    DeletedOn         DATETIME NULL,
    AutoDeletedOn     DATETIME NULL,
    LastLoginDateTime DATETIME NULL,
    LockedUntil       DATETIME NULL,
    RoleId            INT NOT NULL,
    Salt              NVARCHAR(100) NOT NULL
);

-- Insert dummy data in [tblUsers] table
INSERT INTO [cryptography].[dbo].[tblUsers]
(UserId, Name, UserName, Email, Password, IsAdmin, IsActive, IsLocked, IsDeleted, LoginAttempts, DeletedStatus, CreatedOn, RoleId, Salt)
VALUES
('user1', 'John Doe', 'johndoe', 'johndoe@example.com', 'hashed_password_1', 0, 1, 0, 0, 0, 0, GETDATE(), 1, 'salt1'),
('user2', 'Jane Smith', 'janesmith', 'janesmith@example.com', 'hashed_password_2', 0, 1, 0, 0, 0, 0, GETDATE(), 2, 'salt2'),
('user3', 'Admin User', 'admin', 'admin@example.com', 'hashed_password_3', 1, 1, 0, 0, 0, 0, GETDATE(), 3, 'salt3');

-- View all data from `[tblUsers]` table
SELECT * FROM [cryptography].[dbo].[tblUsers];

-- Truncate and reset identity `[tblUsers]`
TRUNCATE TABLE [cryptography].[dbo].[tblUsers];

-- Update role of registered user.
UPDATE [cryptography].[dbo].[tblUsers] SET RoleId=0 WHERE Id=1;

-- Select User Details based upon Email or Username
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM [cryptography].[dbo].[tblUsers] 
            WHERE Email = 'ap5747811@gmail.com'
        ) THEN 1
        WHEN EXISTS (
            SELECT 1 FROM [cryptography].[dbo].[tblUsers] 
            WHERE Username = 'ap574781'
        ) THEN 2
        ELSE 0
    END AS DuplicateStatus;

-- Update Lock or Unlock status
UPDATE [cryptography].[dbo].[tblUsers]
SET IsLocked = CASE WHEN IsLocked = 1 THEN 0 ELSE 1 END
WHERE id = 1;

-- Update deleted status
UPDATE [cryptography].[dbo].[tblUsers]
SET IsDeleted = 1 WHERE id=1;
