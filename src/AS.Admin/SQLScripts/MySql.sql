DROP table IF EXISTS dbcommandlog;
;
CREATE TABLE `dbcommandlog` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Command` VARCHAR(4000) NOT NULL,
  `Duration` INT NOT NULL,
  `Error` VARCHAR(4000) NULL,
  `CreatedOn` DATETIME NOT NULL,
  `CreatedBy` VARCHAR(255) NULL,
  PRIMARY KEY (`ID`));
;

DROP PROCEDURE IF EXISTS DbCommand_INS;
;
CREATE PROCEDURE DbCommand_INS(
IN `command` VARCHAR(4000), IN `duration` int, IN `error` VARCHAR(4000), IN `createdOn` DATETIME,
IN `createdBy` VARCHAR(255))
BEGIN
	INSERT INTO DbCommandLog(Command,Duration,Error,CreatedOn,CreatedBy)
	VALUES(`command`,`duration`,`error`,`createdOn`,`createdBy`);
END
;
DROP PROCEDURE IF EXISTS DeleteAllData;
;

CREATE PROCEDURE DeleteAllData()
BEGIN
	DELETE FROM AspNetRoles ; 
	DELETE FROM AspNetUserClaims ; 
	DELETE FROM AspNetUserLogins ; 
	DELETE FROM AspNetUserRoles ; 
	DELETE FROM AspNetUsers ; 
	DELETE FROM ContactUs ; 
	DELETE FROM EMail ; 
	DELETE FROM JobDefinition ; 
	DELETE FROM Notification ; 
	DELETE FROM SettingDefinition ; 
	DELETE FROM SettingValue ; 
	DELETE FROM PasswordResetToken ; 
	DELETE FROM UserActivity ; 
	DELETE FROM StringResource;
	DELETE FROM AppLog WHERE Level IN('Debug','Info');
END
;
