IF NOT EXISTS ( SELECT  *FROM    sys.objects
				 WHERE   object_id = OBJECT_ID(N'[DbCommandLog]')
                    AND type ='U')
BEGIN
	CREATE TABLE [dbo].[DbCommandLog](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Command] [varchar](2000) NOT NULL,
		[Duration] [int] NOT NULL,
		[Error] [varchar](4000) NULL,
		[CreatedOn] [datetime] NOT NULL,
		[CreatedBy] [varchar](255) NULL,
	 CONSTRAINT [PK_dbo.DbCommandLog] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	END
GO

IF NOT EXISTS ( SELECT  *FROM    sys.objects
				WHERE   object_id = OBJECT_ID(N'[DbCommand_INS]')
                    AND type IN ( N'P', N'PC' ))
BEGIN
	EXEC('CREATE PROCEDURE [DbCommand_INS]
		-- Add the parameters for the stored procedure here
		@command	VARCHAR(2000),
		@duration	int,
		@error		VARCHAR(4000),
		@createdOn  DATETIME,
		@createdBy	VARCHAR(255)
	AS
	BEGIN
		-- Insert statements for procedure here
		INSERT INTO [dbo].[DbCommandLog](Command,Duration,Error,CreatedOn,CreatedBy)
		VALUES(@command,@duration,@error,@createdOn,@createdBy)
	END')
END
GO


IF NOT EXISTS ( SELECT  *FROM    sys.objects
				WHERE   object_id = OBJECT_ID(N'[DeleteAllData]')
                    AND type IN ( N'P', N'PC' ))
BEGIN
	EXEC('CREATE PROCEDURE DeleteAllData
	AS
	BEGIN
		DELETE FROM AspNetRoles 
		DELETE FROM AspNetUserClaims 
		DELETE FROM AspNetUserLogins 
		DELETE FROM AspNetUserRoles 
		DELETE FROM AspNetUsers 
		DELETE FROM ContactUs
		DELETE FROM EMail 
		DELETE FROM JobDefinition 
		DELETE FROM Notification 
		DELETE FROM SettingDefinition 
		DELETE FROM SettingValue 
		DELETE FROM PasswordResetToken 
		DELETE FROM UserActivity  
		DELETE FROM StringResource
		DELETE FROM AppLog WHERE Level IN(''Debug'',''Info'')
	END')
END
GO
