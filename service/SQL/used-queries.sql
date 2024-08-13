-- Query to create a Database
CREATE DATABASE cryptography;

-- Query to create a table
CREATE TABLE [dbo].[User] (
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

