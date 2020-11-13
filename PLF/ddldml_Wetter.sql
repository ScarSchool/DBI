DROP TABLE anstalt CASCADE CONSTRAINTS;
DROP TABLE aufzeichnungen cascade constraints;
DROP TABLE station cascade constraints;
DROP TABLE gebiet cascade constraints;
DROP TABLE station_anstalt cascade constraints;

CREATE TABLE anstalt
( anr INTEGER,
  aname VARCHAR2(30),
  CONSTRAINT pkanstalt PRIMARY KEY (anr)
);
CREATE TABLE gebiet
( gnr INTEGER,
  gname VARCHAR2(30),
  CONSTRAINT pkgebiet PRIMARY KEY (gnr)
);
CREATE TABLE station
( stnr INTEGER,
  stname VARCHAR2(30),
  gnr INTEGER,
  CONSTRAINT pkstation PRIMARY KEY (stnr),
  CONSTRAINT fkstation FOREIGN KEY (gnr) REFERENCES gebiet
);
CREATE TABLE station_anstalt
( stnr INTEGER,
  anr INTEGER,
  CONSTRAINT pksa PRIMARY KEY (stnr, anr),
  CONSTRAINT fksa1 FOREIGN KEY (stnr) REFERENCES station,
  CONSTRAINT fksa2 FOREIGN KEY (anr) REFERENCES anstalt
);

CREATE TABLE aufzeichnungen
	(stnr integer,  datum date, bewoelkung varchar2(10), c number(4,1), so integer,
	 CONSTRAINT pkaufzeichnungen PRIMARY KEY(stnr, datum),
	 CONSTRAINT fkaufzeichnungen FOREIGN KEY (stnr) REFERENCES station
  );

INSERT INTO anstalt VALUES(11, 'ZAMG');
INSERT INTO anstalt VALUES(22, 'ORF');
INSERT INTO anstalt VALUES(33, 'wetter.de');
INSERT INTO anstalt VALUES(44, 'weather-bbc.uk');

INSERT INTO gebiet VALUES(1, 'Hermagor');
INSERT INTO gebiet VALUES(2, 'Klagenfurt Zentrum');
INSERT INTO gebiet VALUES(3, 'Villach Zentrum');

INSERT INTO station VALUES(1,'Nassfeldpass',1);
INSERT INTO station VALUES(2,'Rathaus',2);
INSERT INTO station VALUES(3,'Uni KLU',2);
INSERT INTO station VALUES(4,'Rathaus',3);

INSERT INTO station_anstalt VALUES(1, 11);
INSERT INTO station_anstalt VALUES(1, 22);
INSERT INTO station_anstalt VALUES(1, 33);
INSERT INTO station_anstalt VALUES(2, 11);
INSERT INTO station_anstalt VALUES(2, 44);
INSERT INTO station_anstalt VALUES(3, 11);
INSERT INTO station_anstalt VALUES(3, 22);
INSERT INTO station_anstalt VALUES(3, 44);
INSERT INTO station_anstalt VALUES(4, 11);

insert into aufzeichnungen values(1,TO_DATE('01.06.2001') ,'stark' ,21.3 ,10);
insert into aufzeichnungen values(1,TO_DATE('02.06.2001') ,'mittel' ,18.2 ,12);
insert into aufzeichnungen values(1,TO_DATE('03.07.2001') ,'schwach',22.8 ,14);
insert into aufzeichnungen values(2,TO_DATE('01.06.2001') ,'stark' ,21.8 ,21);
insert into aufzeichnungen values(2,TO_DATE('02.06.2001') ,'mittel' ,18.9 ,20);
insert into aufzeichnungen values(2,TO_DATE('13.06.2001') ,'schwach' ,13.8 ,22);
insert into aufzeichnungen values(3,TO_DATE('01.06.2001') ,'stark' ,21.1 ,23);
insert into aufzeichnungen values(3,TO_DATE('02.06.2001') ,'mittel',17.2 ,26);
insert into aufzeichnungen values(3,TO_DATE('03.06.2001') ,'schwach' ,14.8 ,28);
insert into aufzeichnungen values(3,TO_DATE('08.04.2001') ,'mittel',14.2 ,26);
insert into aufzeichnungen values(3,TO_DATE('05.06.2001') ,'schwach' ,12.8 ,29);
insert into aufzeichnungen values(4,TO_DATE('01.04.2001') ,'mittel' ,23.3 ,21);
insert into aufzeichnungen values(4,TO_DATE('02.06.2001') ,'mittel ',19.2 ,36);
insert into aufzeichnungen values(4,TO_DATE('03.06.2001') ,'schwach' ,24.8 ,18);
insert into aufzeichnungen values(4,TO_DATE('04.06.2001') ,'stark',11.1,21);
insert into aufzeichnungen values(4,TO_DATE('31.05.2001') ,'stark',11.1,29);
commit;
