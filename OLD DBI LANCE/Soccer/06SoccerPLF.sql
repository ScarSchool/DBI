DROP TABLE scores CASCADE CONSTRAINTS;
DROP TABLE players CASCADE CONSTRAINTS;
DROP TABLE teams CASCADE CONSTRAINTS;

CREATE TABLE teams (
	id INTEGER,
	country VARCHAR2(30),
	coach VARCHAR2(30),
	CONSTRAINT pkteams PRIMARY KEY (id)
);
CREATE TABLE players (
	id INTEGER,
	name VARCHAR2(30),
	birthdate DATE,
	idteam INTEGER,
	CONSTRAINT pkplayers PRIMARY KEY (id),
	CONSTRAINT ukplayers UNIQUE (name),
	CONSTRAINT fkplayers FOREIGN KEY (idteam) REFERENCES teams(id)
);
CREATE TABLE scores (
	gamedate DATE,
	idopponent INTEGER, -- other team
	goals INTEGER,
	assists INTEGER,
	idplayer INTEGER,
  CONSTRAINT pkscores PRIMARY KEY (gamedate, idplayer),
	CONSTRAINT fkscoreplayer FOREIGN KEY (idplayer) REFERENCES players,
  CONSTRAINT fkscoreteam FOREIGN KEY (idopponent) REFERENCES teams
);


--INSERT INTO teams VALUES(0,'unknown','----------');
INSERT INTO teams VALUES(1,'Austria','J. Hickersberger');
INSERT INTO teams VALUES(2,'Italy','F. Capello');
INSERT INTO teams VALUES(3,'Germany','U. Seeler');
INSERT INTO teams VALUES(4,'North Corea','Kim');
INSERT INTO teams VALUES(14,'South Corea','Aul Sang');
INSERT INTO teams VALUES(15,'USA','D. Trump');

INSERT INTO players VALUES(1,'T. Polster',TO_DATE('10.11.1965','DD.MM.YYYY'),1);
INSERT INTO players VALUES(2,'H. Krankl',TO_DATE('10.12.1965','DD.MM.YYYY'),1);
INSERT INTO players VALUES(3,'H. Prohaska',TO_DATE('1.1.1956','DD.MM.YYYY'),1);
INSERT INTO players VALUES(4,'Kim Jong Il',TO_DATE('10.11.1948','DD.MM.YYYY'),4);
INSERT INTO players VALUES(5,'Kim Il Sung',TO_DATE('1.5.1954','DD.MM.YYYY'),4);
INSERT INTO players VALUES(6,'P. Rossi',TO_DATE('10.11.1965','DD.MM.YYYY'),2);
INSERT INTO players VALUES(7,'W. Zenga',TO_DATE('4.6.1952','DD.MM.YYYY'),2);
INSERT INTO players VALUES(8,'F. Gentile',TO_DATE('10.11.1965','DD.MM.YYYY'),2);
INSERT INTO players VALUES(9,'G. Mueller',TO_DATE('10.11.1955','DD.MM.YYYY'),3);
INSERT INTO players VALUES(10,'F. Beckenbauer',TO_DATE('10.11.1949','DD.MM.YYYY'),3);
INSERT INTO players VALUES(11,'O. Kahn',TO_DATE('10.11.1971','DD.MM.YYYY'),3);

INSERT INTO scores VALUES(TO_DATE('10.04.2009','DD.MM.YYYY'),14,0,2,1);
INSERT INTO scores VALUES(TO_DATE('10.4.2009','DD.MM.YYYY'),14,1,2,2);
INSERT INTO scores VALUES(TO_DATE('10.4.2009','DD.MM.YYYY'),14,0,1,3);
INSERT INTO scores VALUES(TO_DATE('10.4.2010','DD.MM.YYYY'),15,0,2,1);
INSERT INTO scores VALUES(TO_DATE('10.4.2010','DD.MM.YYYY'),15,0,2,2);
INSERT INTO scores VALUES(TO_DATE('10.4.2010','DD.MM.YYYY'),15,0,2,3);
INSERT INTO scores VALUES(TO_DATE('10.5.2009','DD.MM.YYYY'),1,1,0,4);
INSERT INTO scores VALUES(TO_DATE('10.5.2009','DD.MM.YYYY'),1,2,2,5);
INSERT INTO scores VALUES(TO_DATE('10.5.2009','DD.MM.YYYY'),1,0,5,6);
INSERT INTO scores VALUES(TO_DATE('10.1.2009','DD.MM.YYYY'),3,0,2,4);
INSERT INTO scores VALUES(TO_DATE('10.2.2009','DD.MM.YYYY'),3,0,2,5);
INSERT INTO scores VALUES(TO_DATE('10.3.2009','DD.MM.YYYY'),3,0,2,6);
INSERT INTO scores VALUES(TO_DATE('10.8.2009','DD.MM.YYYY'),3,0,2,1);

COMMIT;
