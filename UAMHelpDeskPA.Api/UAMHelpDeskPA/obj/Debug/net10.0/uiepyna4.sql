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
CREATE TABLE [Laboratories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Building] nvarchar(50) NOT NULL,
    [BuildingFloor] int NOT NULL,
    [Capacity] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Laboratories] PRIMARY KEY ([Id])
);

CREATE TABLE [Equipment] (
    [Id] int NOT NULL IDENTITY,
    [LaboratoryId] int NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [Brand] nvarchar(50) NOT NULL,
    [Model] nvarchar(50) NOT NULL,
    [SerialNumber] nvarchar(50) NOT NULL,
    [Type] nvarchar(30) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [PurchaseDate] date NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Equipment] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Equipment_Laboratories_LaboratoryId] FOREIGN KEY ([LaboratoryId]) REFERENCES [Laboratories] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Equipment_Code] ON [Equipment] ([Code]);

CREATE INDEX [IX_Equipment_LaboratoryId] ON [Equipment] ([LaboratoryId]);

CREATE UNIQUE INDEX [IX_Equipment_SerialNumber] ON [Equipment] ([SerialNumber]);

CREATE UNIQUE INDEX [IX_Laboratories_Name] ON [Laboratories] ([Name]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260601035813_InitialCreate', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607024429_AddRoles', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607204106_AddUsers', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [RefreshTokens] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Token] nvarchar(500) NOT NULL,
    [ExpiresAtUtc] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260624164313_AddRefreshTokens', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [OtpCodes] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Code] nvarchar(10) NOT NULL,
    [ExpiresAtUtc] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_OtpCodes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OtpCodes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PendingSessions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [SessionToken] nvarchar(100) NOT NULL,
    [ExpiresAtUtc] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_PendingSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PendingSessions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_OtpCodes_UserId] ON [OtpCodes] ([UserId]);

CREATE UNIQUE INDEX [IX_PendingSessions_SessionToken] ON [PendingSessions] ([SessionToken]);

CREATE INDEX [IX_PendingSessions_UserId] ON [PendingSessions] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629000414_AddOtpAuthentication', N'10.0.8');

COMMIT;
GO

