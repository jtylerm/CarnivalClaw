
DROP TABLE `ARUser`;

CREATE TABLE `ARUser` (
    `ID` INT(11) NOT NULL AUTO_INCREMENT,
    `SubID` VARCHAR(60) NOT NULL DEFAULT ,
    `Username` VARCHAR(50) NOT NULL,
    `PwdHash` VARCHAR(50) NULL,
    `IsGuest` BOOL DEFAULT FALSE,
    `Activated` BOOL DEFAULT FALSE,
    PRIMARY KEY (`ID`),
    UNIQUE KEY `uc_Username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO ARUser (Username, PwdHash, Activated)
VALUE ('heroofhyrule', MD5('bacon123'), TRUE);

SELECT *
FROM ARUser
WHERE Username = 'heroofhyrule'
	AND PwdHash = MD5('bacon123');

ALTER TABLE ARUser
ADD COLUMN `SubID` VARCHAR(60) NULL AFTER ID;

DELIMITER ;;
CREATE TRIGGER before_insert_ARUser
BEFORE INSERT ON ARUser
FOR EACH ROW
BEGIN
  IF new.uuid IS NULL THEN
    SET new.SubID = uuid();
  END IF;
END
;;