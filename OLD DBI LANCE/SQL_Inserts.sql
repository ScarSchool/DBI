DROP TABLE discoveries;
DROP TABLE mushrooms ;
CREATE TABLE mushrooms (
  id INT NOT NULL IDENTITY,
  name VARCHAR(50),
  useful VARCHAR(20),
  cap_color VARCHAR(50),
  stem_color VARCHAR(50),
  descr VARCHAR(500),
  image VARBINARY(MAX) NULL,
  CONSTRAINT pk_mushroom PRIMARY KEY (id),
  CONSTRAINT ck_useful CHECK(useful IN ('EDIBLE','INEDIBLE','TOXIC')),
);
CREATE TABLE discoveries (
   	id INT NOT NULL IDENTITY,
    idMushroom INT,
    dateDiscovery DATE,
    geopoint GEOGRAPHY,
	x INT,
	y INT,
    CONSTRAINT pkDiscovery PRIMARY KEY(id),
    CONSTRAINT fkDiscovery FOREIGN KEY (idMushroom) REFERENCES mushrooms
);

INSERT INTO dbo.mushrooms
(
    name,
    useful,
    cap_color,
    stem_color,
    descr
)
VALUES
(   'Chanterelle', -- name - varchar(50)
    'EDIBLE', -- useful - varchar(20)
    'yellow', -- cap_color - varchar(50)
    'yellow', -- stem_color - varchar(50)
    'frequently found' -- descr - varchar(500)
    )
;
INSERT INTO dbo.mushrooms
(
    name,
    useful,
    cap_color,
    stem_color,
    descr
)
VALUES
(   'Fly Amanita', -- name - varchar(50)
    'TOXIC', -- useful - varchar(20)
    'red', -- cap_color - varchar(50)
    'white', -- stem_color - varchar(50)
    'frequently found' -- descr - varchar(500)
    )
;
INSERT INTO dbo.mushrooms
(
    name,
    useful,
    cap_color,
    stem_color,
    descr
)
VALUES
(   'Penny Bun', -- name - varchar(50)
    'INEDIBLE', -- useful - varchar(20)
    'brown', -- cap_color - varchar(50)
    'grey', -- stem_color - varchar(50)
    'not frequently found' -- descr - varchar(500)
    )
;

   INSERT INTO dbo.discoveries (idMushroom, dateDiscovery, geopoint, x, y) 
		VALUES(1, CONVERT(DATE,'14.04.2018',104),geography::STPointFromText('POINT(46.743941 13.579347)',4326), 332, 122);
   INSERT INTO dbo.discoveries (idMushroom, dateDiscovery, geopoint, x, y) 
		VALUES(1, CONVERT(DATE,'14.04.2018',104),geography::STPointFromText('POINT(46.726884 13.615478)',4326), 640, 342);
   INSERT INTO dbo.discoveries (idMushroom, dateDiscovery, geopoint, x, y) 
		VALUES(1, CONVERT(DATE,'14.04.2018',104),geography::STPointFromText('POINT(46.712541 13.675658)',4326), 1153, 527);

SELECT geopoint.Long FROM dbo.discoveries;




