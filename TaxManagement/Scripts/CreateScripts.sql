CREATE TABLE [dbo].[Declarants](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdateAt] [datetime] NOT NULL,
	[CreatedByUser] [nvarchar](50) NOT NULL,
	[LastUpdateByUser] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Declarants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Declarants] ADD  CONSTRAINT [DF_Declarants_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Declarants] ADD  CONSTRAINT [DF_Declarants_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Declarants] ADD  CONSTRAINT [DF_Declarants_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Declarants] ADD  CONSTRAINT [DF_Declarants_UpdatedDate]  DEFAULT (getdate()) FOR [LastUpdateAt]
GO




CREATE TABLE [dbo].[Declarations](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DeclarantId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdateAt] [datetime] NOT NULL,
	[CreatedByUser] [nvarchar](50) NOT NULL,
	[LastUpdateByUser] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Declarations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Declarations] ADD  CONSTRAINT [DF_Declarations_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Declarations] ADD  CONSTRAINT [DF_Declarations_Status]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[Declarations] ADD  CONSTRAINT [DF_Declarations_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Declarations] ADD  CONSTRAINT [DF_Declarations_CreateDate]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Declarations] ADD  CONSTRAINT [DF_Declarations_UpdateDate]  DEFAULT (getdate()) FOR [LastUpdateAt]
GO

ALTER TABLE [dbo].[Declarations]  WITH CHECK ADD  CONSTRAINT [FK_Declarations_Declarants] FOREIGN KEY([DeclarantId])
REFERENCES [dbo].[Declarants] ([Id])
GO

ALTER TABLE [dbo].[Declarations] CHECK CONSTRAINT [FK_Declarations_Declarants]
GO


