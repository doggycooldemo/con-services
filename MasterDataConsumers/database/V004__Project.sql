
CREATE TABLE IF NOT EXISTS Project (
  ProjectUID varchar(36) DEFAULT NULL,
  LegacyProjectID bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  Name varchar(255) NOT NULL,
  fk_ProjectTypeID INT(10) UNSIGNED DEFAULT NULL,
  IsDeleted tinyint(4) DEFAULT 0,
  
  ProjectTimeZone varchar(255) NOT NULL,
  LandfillTimeZone varchar(255) NOT NULL,   
    
  StartDate date DEFAULT NULL,
  EndDate date DEFAULT NULL,
  
  GeometryWKT varchar(4000) DEFAULT NULL,
  PolygonST POLYGON NULL,
  
  LastActionedUTC datetime(6) DEFAULT NULL,
  InsertUTC datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  UpdateUTC datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  PRIMARY KEY (LegacyProjectID),
  UNIQUE KEY UIX_Project_ProjectUID (ProjectUID ASC),
	UNIQUE KEY UIX_Project_LegacyProjectID (LegacyProjectID)
) ENGINE=InnoDB CHARSET = DEFAULT COLLATE = DEFAULT AUTO_INCREMENT = 2000000;


SET @s = (SELECT IF(
    (SELECT COUNT(*)
       FROM INFORMATION_SCHEMA.INNODB_SYS_INDEXES 
        WHERE Name = 'UIX_Project_LegacyProjectID'
        ) > 0,
    "SELECT 1",
    "ALTER TABLE `Project` ADD UNIQUE KEY `UIX_Project_LegacyProjectID` (`LegacyProjectID`)"
));

PREPARE stmt FROM @s;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
        
/* currently defaults to null. This could be changed later when existing tables are backfilled */
SET @s = (SELECT IF(
    (SELECT COUNT(*)
       FROM INFORMATION_SCHEMA.COLUMNS
        WHERE table_name = 'Project'
        AND table_schema = DATABASE()
        AND column_name = 'GeometryWKT'
    ) > 0,
    "SELECT 1",
    "ALTER TABLE `Project` ADD COLUMN `GeometryWKT` varchar(4000) DEFAULT NULL, AFTER `EndDate`"
));

PREPARE stmt FROM @s;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @s = (SELECT IF(
    (SELECT COUNT(*)
       FROM INFORMATION_SCHEMA.COLUMNS
        WHERE table_name = 'Project'
        AND table_schema = DATABASE()
        AND column_name = 'PolygonST'
    ) > 0,
    "SELECT 1",
    "ALTER TABLE `Project` ADD COLUMN `PolygonST` POLYGON NULL AFTER `GeometryWKT`"
));   

PREPARE stmt FROM @s;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @s = (SELECT IF(
    (SELECT COUNT(*)
       FROM INFORMATION_SCHEMA.COLUMNS
        WHERE table_name = 'Project'
        AND table_schema = DATABASE()
        AND column_name = 'ID'
    ) > 0,    
    "ALTER TABLE `Project` 
         DROP COLUMN ID, 
         CHANGE COLUMN `LegacyProjectID` `LegacyProjectID` BIGINT(20) UNSIGNED NOT NULL primary key AUTO_INCREMENT, 
         AUTO_INCREMENT = 2000000",
    "SELECT 1"
)); 

PREPARE stmt FROM @s;
EXECUTE stmt;
DEALLOCATE PREPARE stmt; 