CREATE TABLE Folder (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    CreatedAt       DATETIME NOT NULL,
    LastUpdateAt    DATETIME NOT NULL,
    CreatedByUser   NVARCHAR(MAX),
    LastUpdateByUser NVARCHAR(MAX),
    Deleted         BIT NOT NULL DEFAULT 0,
    ArchiveId       UNIQUEIDENTIFIER NOT NULL,
    Name            NVARCHAR(MAX),
    ParentId        UNIQUEIDENTIFIER,
    HasDocument     BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Folder_Parent FOREIGN KEY (ParentId) REFERENCES Folder(Id)
);
