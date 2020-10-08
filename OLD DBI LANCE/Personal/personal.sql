DROP TABLE Personal CASCADE CONSTRAINTS;

CREATE TABLE Personal (
  id INTEGER PRIMARY KEY,
  name VARCHAR2(32),
  vorgesetzter INTEGER REFERENCES Personal,
  funktion VARCHAR2 (32) NOT NULL
);


  
INSERT INTO Personal VALUES(11, 'Chefstein', NULL, 'Direktor');
INSERT INTO Personal VALUES(22, 'EDVaustein',11, 'Abteilungsleiter');
INSERT INTO Personal VALUES(33, 'NTstein', 11, 'Abteilungsleiter');
INSERT INTO Personal VALUES(44, 'Softstein', 22 , 'Angestellter');
INSERT INTO Personal VALUES(55, 'Appstein', 22, 'Angestellter');
INSERT INTO Personal VALUES(66, 'Kabelstein', 33, 'Angestellter');
INSERT INTO Personal VALUES(77, 'Schleifstein', 33, 'Hausmeister');
INSERT INTO Personal VALUES(88, 'Schreibstein',11, 'Sekretärin');
INSERT INTO Personal VALUES(99, 'Tippstein', 22, 'Sekretärin');
INSERT INTO Personal VALUES(111, 'Feuerstein', 55 , 'Lehrling');
INSERT INTO Personal VALUES(122, 'Kieselstein', 66, 'Lehrling');



SELECT main.id, main.name, main.funktion, '==>', NVL('' || Personal.id, '--') "id mgr", NVL(Personal.name, '---') "name mgr", NVL(Personal.funktion, '---') "funktion mgr" 
  FROM Personal main
  LEFT JOIN Personal ON main.vorgesetzter = Personal.id
  ORDER BY main.name;
  
SELECT Personal.id "id mgr", Personal.name "name mgr", Personal.funktion "funktion mgr" , '==>', NVL('' || main.id, '--'), NVL(main.name, '---'), NVL(main.funktion, '---') 
  FROM Personal main
  RIGHT JOIN Personal ON Personal.id = main.vorgesetzter
  ORDER BY main.name;
  
SELECT Personal.id, Personal.Name, COUNT(main.id)
  FROM Personal main
  RIGHT JOIN Personal ON Personal.id = main.vorgesetzter
  GROUP BY Personal.id, Personal.Name
  ORDER BY Personal.Name;

--Ausgabe des levels in der Hierarchie
SELECT * FROM Personal
WHERE vorgesetzter IS NULL;

SELECT id, name, funktion, level FROM Personal
START WITH vorgesetzter IS NULL
CONNECT BY PRIOR id = vorgesetzter
ORDER BY level, name;
  
SELECT LPAD(' ', 2 * level) || '-' || name "Ang. -Name", funktion, level FROM Personal
  START WITH id IN (SELECT id FROM Personal 
                      WHERE level = 2
                      START WITH vorgesetzter IS NULL 
                      CONNECT BY PRIOR id = vorgesetzter
                    ) and funktion NOT LIKE 'Sekret%'
  CONNECT BY PRIOR id = vorgesetzter
  ORDER SIBLINGS BY id; 
  
SELECT id, name, funktion, level FROM Personal
  WHERE level = (SELECT level FROM Personal 
                      WHERE name like 'Tippstein'
                      START WITH vorgesetzter IS NULL 
                      CONNECT BY PRIOR id = vorgesetzter)
  START WITH VORGESETZTER IS NULL
  CONNECT BY PRIOR id = vorgesetzter
  ORDER BY name;   

SELECT id, name, funktion, level FROM Personal main
  START WITH funktion LIKE 'Abteilung%' AND (SELECT COUNT(*) FROM Personal 
                                              WHERE id != main.id AND connect_by_root id = main.id 
                                              START WITH funktion LIKE 'Abteilung%'
                                              CONNECT BY PRIOR id = vorgesetzter) >= 4
  CONNECT BY PRIOR id = vorgesetzter;  
  
SELECT id, name, funktion, level FROM Personal main
  START WITH funktion LIKE 'Abteilung%' AND (SELECT COUNT(*) FROM Personal 
                                              START WITH vorgesetzter = main.id
                                              CONNECT BY PRIOR id = vorgesetzter) >= 4
  CONNECT BY PRIOR id = vorgesetzter;   
  
SELECT id, name, funktion, vorgesetzter, level, CASE WHEN level <= 2 THEN 'Manager' ELSE 'Employee' END "LEVELNAME" FROM Personal
  START WITH vorgesetzter IS NULL
  CONNECT BY PRIOR id = vorgesetzter
  ORDER BY levelname DESC, name;
  
SELECT id, name, funktion, level FROM Personal
  START WITH name LIKE 'Feuerstein'
  CONNECT BY id = PRIOR vorgesetzter;


















