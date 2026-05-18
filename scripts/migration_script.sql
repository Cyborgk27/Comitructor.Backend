IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [Password] nvarchar(255) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [CreatedBy] int NULL,
    [CreatedDate] datetime2 NULL,
    [LastModifiedBy] int NULL,
    [LastModifiedDate] datetime2 NULL,
    [DeletedBy] int NULL,
    [DeletedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Requests] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(20) NOT NULL,
    [Title] nvarchar(120) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Area] nvarchar(30) NOT NULL,
    [Priority] nvarchar(20) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [AssignedUserId] int NULL,
    [DueDate] datetime2 NULL,
    [ClosedDate] datetime2 NULL,
    [CreatedBy] int NULL,
    [CreatedDate] datetime2 NULL,
    [LastModifiedBy] int NULL,
    [LastModifiedDate] datetime2 NULL,
    [DeletedBy] int NULL,
    [DeletedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Requests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Requests_Users_AssignedUserId] FOREIGN KEY ([AssignedUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [RequestHistories] (
    [Id] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [PreviousStatus] nvarchar(20) NOT NULL,
    [NewStatus] nvarchar(20) NOT NULL,
    [ChangeReason] nvarchar(500) NULL,
    [CreatedBy] int NULL,
    [CreatedDate] datetime2 NULL,
    [LastModifiedBy] int NULL,
    [LastModifiedDate] datetime2 NULL,
    [DeletedBy] int NULL,
    [DeletedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_RequestHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RequestHistories_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_RequestHistories_RequestId] ON [RequestHistories] ([RequestId]);
GO

CREATE INDEX [IX_Requests_AssignedUserId] ON [Requests] ([AssignedUserId]);
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260517054926_Init', N'8.0.27');
GO

COMMIT;
GO

