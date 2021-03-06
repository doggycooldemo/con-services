CREATE TABLE IF NOT EXISTS ImportedFileHistory (
  ID bigint(20) NOT NULL AUTO_INCREMENT,
  fk_ImportedFileUID varchar(100) NOT NULL,
  FileCreatedUTC datetime(6) NOT NULL,
  FileUpdatedUTC datetime(6) NOT NULL,
  ImportedBy varchar(256) NOT NULL,
  InsertUTC datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  UpdateUTC datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  PRIMARY KEY (ID),
  KEY IX_ImportedFileHistory_ImportedFileUID (fk_ImportedFileUID)
) ENGINE=InnoDB CHARSET = DEFAULT COLLATE = DEFAULT;
