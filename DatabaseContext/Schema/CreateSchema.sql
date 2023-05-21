IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'FinancialData')
BEGIN
  CREATE DATABASE [FinancialData]
END
GO

-- create database objects
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='News' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[News]
	(
		[Id]				[BIGINT] IDENTITY(1,1)	NOT NULL,
		[ExternalId]		[NVARCHAR](64)			NOT NULL,
		[Description]		[NVARCHAR](MAX)			NOT NULL,
		[Contents]			[NVARCHAR](MAX)			NOT NULL,
		[Date]				[DATETIME2](7)			NOT NULL

		CONSTRAINT PK_News
		PRIMARY KEY CLUSTERED(Id)
	)

	CREATE UNIQUE INDEX [UX_News_ExternalId]
	ON [News] ([ExternalId])
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NewsTickerData' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[NewsTickerData]
	(
		[Id]				[BIGINT] IDENTITY(1,1)	NOT NULL,
		[NewsId]			[BIGINT]				NOT NULL,
		[Ticker]			[NVARCHAR](32)			NOT NULL,
		[CandleStickChart]	[NVARCHAR](MAX)

		CONSTRAINT PK_NewsTickerData
		PRIMARY KEY CLUSTERED(Id)
	)

	CREATE INDEX [IX_NewsTickerData_NewsId_Ticker]
	ON [NewsTickerData] ([NewsId], [Ticker])

	ALTER TABLE [dbo].[NewsTickerData] WITH CHECK 
	ADD  CONSTRAINT [FK_NewsTickerData_NewsId] 
	FOREIGN KEY([NewsId])
	REFERENCES [dbo].[News] ([Id])
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Outbox' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[Outbox]
	(
		[Id]	[BIGINT] IDENTITY(1,1)	NOT NULL,
		[Data]	[VARBINARY](MAX)		NOT NULL,
		[Date]	[DATETIME2](7)			NOT NULL,

		CONSTRAINT PK_Outbox
		PRIMARY KEY CLUSTERED(Id)
	)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AspNetUsers' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[AspNetUsers](
		[Id] [nvarchar](450) NOT NULL,
		[UserName] [nvarchar](256) NULL,
		[NormalizedUserName] [nvarchar](256) NULL,
		[Email] [nvarchar](256) NULL,
		[NormalizedEmail] [nvarchar](256) NULL,
		[EmailConfirmed] [bit] NOT NULL,
		[PasswordHash] [nvarchar](max) NULL,
		[SecurityStamp] [nvarchar](max) NULL,
		[ConcurrencyStamp] [nvarchar](max) NULL,
		[PhoneNumber] [nvarchar](max) NULL,
		[PhoneNumberConfirmed] [bit] NOT NULL,
		[TwoFactorEnabled] [bit] NOT NULL,
		[LockoutEnd] [datetimeoffset](7) NULL,
		[LockoutEnabled] [bit] NOT NULL,
		[AccessFailedCount] [int] NOT NULL,
	 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AspNetUserTokens' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[AspNetUserTokens](
		[UserId] [nvarchar](450) NOT NULL,
		[LoginProvider] [nvarchar](450) NOT NULL,
		[Name] [nvarchar](450) NOT NULL,
		[Value] [nvarchar](max) NULL,
	 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
	(
		[UserId] ASC,
		[LoginProvider] ASC,
		[Name] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AspNetUserClaims' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[AspNetUserClaims](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[UserId] [nvarchar](450) NOT NULL,
		[ClaimType] [nvarchar](max) NULL,
		[ClaimValue] [nvarchar](max) NULL,
	 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	
	ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([Id])
	ON DELETE CASCADE
	
	ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
	END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AspNetUserLogins' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[AspNetUserLogins](
		[LoginProvider] [nvarchar](450) NOT NULL,
		[ProviderKey] [nvarchar](450) NOT NULL,
		[ProviderDisplayName] [nvarchar](max) NULL,
		[UserId] [nvarchar](450) NOT NULL,
	 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
	(
		[LoginProvider] ASC,
		[ProviderKey] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([Id])
	ON DELETE CASCADE
	
	ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SaveNewsData]
(
	@events		[NVARCHAR](MAX)
)  
AS  
BEGIN
SET NOCOUNT ON
	BEGIN TRY
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED

		BEGIN TRANSACTION

		DROP TABLE IF EXISTS #News
		SELECT 
			[id]			AS [ExternalId]
			, [description] AS [Description]
			, [content]		AS [Contents]
			, [tickers]		AS [Tickers]
		INTO #News
		FROM OPENJSON(@events)
		WITH
		(
			[id]			NVARCHAR(100) '$.Content.id',
			[description]	NVARCHAR(MAX) '$.Content.description',
			[tickers]		NVARCHAR(MAX) '$.Content.tickers' AS JSON,
			[content]		NVARCHAR(MAX) '$.Content' AS JSON
		) a

		DROP TABLE IF EXISTS #InsertedNews
		CREATE TABLE #InsertedNews([Id] BIGINT, [ExternalId] NVARCHAR(100))
		
		INSERT INTO [dbo].[News] ([ExternalId], [Description], [Contents], [Date])
		OUTPUT INSERTED.[Id], INSERTED.[ExternalId] INTO #InsertedNews
		SELECT t.[ExternalId], t.[Description], t.[Contents], GETUTCDATE()
		FROM #News t
		LEFT JOIN [dbo].[News] n
			ON t.[ExternalId] = n.[ExternalId]
		WHERE n.ExternalId IS NULL

		INSERT INTO [NewsTickerData]([NewsId], [Ticker], [CandleStickChart])
		SELECT
			i.Id
			, a.value		AS [TickerCode]
			, b.value		AS [CandleSticks]
		FROM 
		(
			SELECT 
				[id]			AS [ExternalId]
				, [description] AS [Description]
				, [content]		AS [Contents]
				, [tickers]		AS [Tickers]
				, y.value		AS [TickerData]
			FROM OPENJSON(@events)
			WITH
			(
				[id]			NVARCHAR(100) '$.Content.id',
				[description]	NVARCHAR(MAX) '$.Content.description',
				[tickers]		NVARCHAR(MAX) '$.Content.tickers' AS JSON,
				[content]		NVARCHAR(MAX) '$.Content' AS JSON,
				[t]				NVARCHAR(MAX) '$.Tickers' AS JSON
			) a
			CROSS APPLY (SELECT * FROM OPENJSON(a.t)) y
		)n
		INNER JOIN #InsertedNews i
			ON i.[ExternalId] = n.[ExternalId]
		CROSS APPLY OPENJSON(n.TickerData) a
		CROSS APPLY OPENJSON(n.TickerData) b
		WHERE a.[key] = 'TickerCode'
		AND b.[key] = 'CandleSticks'

		INSERT INTO [dbo].[Outbox] ([Data], [Date])
		SELECT CAST(value AS VARBINARY(MAX)), GETUTCDATE()
		FROM OPENJSON(@events)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END
GO