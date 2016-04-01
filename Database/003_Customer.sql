


CREATE TABLE IF NOT EXISTS `Customer` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CustomerUID` varchar(64) NOT NULL,
  `CustomerName` varchar(200) NOT NULL,
  `fk_CustomerTypeID` int(10) NOT NULL,
  `LastActionedUTC` datetime(6) DEFAULT NULL,
  `InsertUTC` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateUTC` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`ID`)  COMMENT '',
  UNIQUE KEY `UIX_Customer_CustomerUID` (`CustomerUID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
