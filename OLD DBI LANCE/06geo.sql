drop table geoobj cascade constraints;
drop table geotree cascade constraints;
drop table towns cascade constraints;
drop table states cascade constraints;

create table geoobj
(
    name      varchar2(30),
    geotype   varchar2(30),
    CONSTRAINT pk_geoobj PRIMARY KEY (name),
    CONSTRAINT ck_geoobj CHECK (geotype 
    			IN ('TOWN','STATE','CONTINENT','ATTRACTION', 'OTHER'))
);
create table geotree
(
    parent  varchar2(30),
    child   varchar2(30),
    CONSTRAINT pk_geotree PRIMARY KEY (child),
    CONSTRAINT fk_geotree1 FOREIGN KEY 
    		(parent) REFERENCES geoobj(name),
    CONSTRAINT fk_geotree2 FOREIGN KEY 
    		(child) REFERENCES geoobj(name)
);
create table towns
(
	name varchar2(30),
	inhabitants integer,
	CONSTRAINT pk_town PRIMARY KEY (name),
    CONSTRAINT fk_town FOREIGN KEY (name) 
    		REFERENCES geoobj(name)
);
create table states
(	name varchar2(30),
	primeminister varchar2(30),
	CONSTRAINT pk_states PRIMARY KEY (name),
    CONSTRAINT fk_states FOREIGN KEY (name) REFERENCES geoobj(name)
);

INSERT INTO geoobj VALUES ('Earth','OTHER');
INSERT INTO geoobj VALUES ('Ontario','STATE');
INSERT INTO geoobj VALUES ('California','STATE');
INSERT INTO geoobj VALUES ('England','STATE');
INSERT INTO geoobj VALUES ('New South Wales','STATE');
INSERT INTO geoobj VALUES ('Canada','STATE');
INSERT INTO geoobj VALUES ('China','STATE');
INSERT INTO geoobj VALUES ('Japan','STATE');
INSERT INTO geoobj VALUES ('USA','STATE');
INSERT INTO geoobj VALUES ('Austria','STATE');
INSERT INTO geoobj VALUES ('United Kingdom','STATE');
INSERT INTO geoobj VALUES ('Asia','CONTINENT');
INSERT INTO geoobj VALUES ('Australia','CONTINENT');
INSERT INTO geoobj VALUES ('Europe','CONTINENT');
INSERT INTO geoobj VALUES ('North America','CONTINENT');
INSERT INTO geoobj VALUES ('Ottawa','TOWN');
INSERT INTO geoobj VALUES ('Toronto','TOWN');
INSERT INTO geoobj VALUES ('Sydney','TOWN');
INSERT INTO geoobj VALUES ('Redwood Shores','TOWN');
INSERT INTO geoobj VALUES ('Beijing','TOWN');
INSERT INTO geoobj VALUES ('London','TOWN');
INSERT INTO geoobj VALUES ('Osaka','TOWN');
INSERT INTO geoobj VALUES ('Vienna','TOWN');
INSERT INTO geoobj VALUES ('New York','TOWN');
INSERT INTO geoobj VALUES ('Tokyo','TOWN');
INSERT INTO geoobj VALUES ('Stephansdom','ATTRACTION');
INSERT INTO geoobj VALUES ('Empire State Building','ATTRACTION');

insert into geotree values(NULL,'Earth');
insert into geotree values('Ontario','Ottawa');
insert into geotree values('Ontario','Toronto');
insert into geotree values('USA','California');
insert into geotree values('United Kingdom','England');
insert into geotree values('Earth','Asia');
insert into geotree values('Earth','Australia');
insert into geotree values('Earth','Europe');
insert into geotree values('Earth','North America');
insert into geotree values('Asia','China');
insert into geotree values('Asia','Japan');
insert into geotree values('Australia','New South Wales');
insert into geotree values('New South Wales','Sydney');
insert into geotree values('California','Redwood Shores');
insert into geotree values('Canada','Ontario');
insert into geotree values('China','Beijing');
insert into geotree values('England','London');
insert into geotree values('Europe','United Kingdom');
insert into geotree values('Europe','Austria');
insert into geotree values('Japan','Osaka');
insert into geotree values('Japan','Tokyo');
insert into geotree values('North America','Canada');
insert into geotree values('North America','USA');
insert into geotree VALUES('Austria','Vienna');
insert into geotree VALUES('USA','New York');
insert into geotree values('Vienna','Stephansdom');
insert into geotree values('New York','Empire State Building');

insert into towns values('Ottawa', 1500000);
insert into towns values('Toronto', 2000000);
insert into towns values('Sydney', 2500000);
insert into towns values('Redwood Shores', 500000);
insert into towns values('Beijing', 8500000);
insert into towns values('London', 7500000);
insert into towns values('Osaka', 3500000);
insert into towns values('Tokyo', 9500000);

insert into states values('USA','D. Trump');
insert into states values('United Kingdom','Th. May');
insert into states values('Australia','K. Kanguruh');
insert into states values('Canada','B. Stronach');
insert into states values('China','M Zedong');
insert into states values('Japan','Y. Kasai');
commit;



SELECT level, child, SYS_CONNECT_BY_PATH(child, '/') "Path", CONNECT_BY_ISLEAF leaf, CONNECT_BY_ROOT child
  FROM geotree  
 -- WHERE CONNECT_BY_ISLEAF = 1
  START WITH child = 'Europe' 
  CONNECT BY PRIOR child = parent AND child != 'Asia'
  ORDER SIBLINGS BY child;
  
SELECT connect_by_root child, parent, child, geoobj.GEOTYPE
  FROM geotree
  INNER JOIN geoobj ON child = geoobj.name
  WHERE child LIKE 'Toronto'
  --WHERE child LIKE 'TISoronto'
  START WITH geoobj.GEOTYPE LIKE 'CONTINENT'
  CONNECT BY PRIOR child = parent
  ORDER SIBLINGS BY child;  
  
SELECT connect_by_root child, child
  FROM geotree main
  INNER JOIN geoobj ON child = geoobj.name
  WHERE geoobj.GEOTYPE LIKE 'TOWN'
  START WITH child = (SELECT connect_by_root child
                        FROM geotree
                        INNER JOIN geoobj ON child = geoobj.name
                        WHERE child LIKE 'Toronto'
                        START WITH geoobj.GEOTYPE LIKE 'CONTINENT'
                        CONNECT BY PRIOR child = parent)
  CONNECT BY PRIOR child = parent;  
  
-- start with default: all elements are root elements (1 = 1)

SELECT * FROM geotree;
  
SELECT parent, COUNT(*) "#"
  FROM geotree main
  INNER JOIN geoobj ON child = geoobj.name AND GEOTYPE LIKE 'TOWN'
  GROUP BY parent;


CREATE OR REPLACE VIEW v AS
SELECT connect_by_root child c, child o
  FROM geotree main
  INNER JOIN geoobj ON child = geoobj.name
  START WITH geotype LIKE 'CONTINENT'
  CONNECT BY PRIOR child = parent;  
  
SELECT c "CONTINENT", COUNT(*) - 1 "#" FROM v GROUP BY c;
DROP VIEW v;

SELECT * FROM GEOTREE;
SELECT * FROM GEOOBJ;
SELECT * FROM TOWNS;
SELECT * FROM STATES;


