DROP TABLE owners CASCADE CONSTRAINTS;
DROP TABLE cars CASCADE CONSTRAINTS;
DROP SEQUENCE seqOwner;

CREATE SEQUENCE seqOwner;

CREATE TABLE cars (
    carid INTEGER,
    carname VARCHAR2(30),
    cartype VARCHAR2(30),
    carprice NUMBER(8,2),
    CONSTRAINT pkCars PRIMARY KEY (carid)
) ROWDEPENDENCIES;

CREATE TABLE owners (
	ownerid INTEGER,
    ownername VARCHAR(30),
    ownerfrom DATE NOT NULL,
    ownerto DATE,
    carid INTEGER,
    CONSTRAINT pkOwners PRIMARY KEY (ownerid),
    CONSTRAINT uqOwners UNIQUE (ownerid, carid),
    CONSTRAINT fkOwners FOREIGN KEY (carid) REFERENCES cars
) ROWDEPENDENCIES;

INSERT INTO cars VALUES(11,'Ford Edsel','SEDAN', 12000.99);
INSERT INTO cars VALUES(22,'Fiat 500','MICRO', 500.55);

INSERT INTO owners VALUES(seqOwner.NEXTVAL,'Hagen', TO_DATE('12.01.2002','DD.MM.YYYY'), TO_DATE( '15.04.2009','DD.MM.YYYY'),11);
INSERT INTO owners VALUES(seqOwner.NEXTVAL,'Siegfried', TO_DATE( '12.01.2010','DD.MM.YYYY'), TO_DATE( '15.07.2012','DD.MM.YYYY'),11);
INSERT INTO owners VALUES(seqOwner.NEXTVAL,'Hagen', TO_DATE( '12.01.2012','DD.MM.YYYY'), TO_DATE( '15.04.2013','DD.MM.YYYY'),22);
INSERT INTO owners VALUES(seqOwner.NEXTVAL,'Giselher', TO_DATE( '12.09.2013','DD.MM.YYYY'), TO_DATE( '15.07.2014','DD.MM.YYYY'),22);
INSERT INTO owners VALUES(seqOwner.NEXTVAL,'Gunther',TO_DATE( '12.01.2015','DD.MM.YYYY'), NULL,22);
COMMIT;


-- LOCK TABLE cars IN SHARE MODE;
COMMIT;