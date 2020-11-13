SELECT * from Einzelteile RIGHT JOIN Teile oleTeile ON oleTeile.ID = Einzelteile.ID 
INNER JOIN Bauplan ON Bauplan.PRODUCT# = oleTeile.ID;

-- Kreg anfoch olles in geiler hierachie
SELECT
oleTeile.BEZEICHNUNG BEZEICHNUNG,
oleTeile.TYPE TYPE,
Bauplan.PRODUCT# PRODUCT#,
Bauplan.BESTANDTEIL# BESTANDTEIL#,
Bauplan.MENGE MENGE,
Bauplan.FERTIGUNGSKOSTEN FERTIGUNGSKOSTEN,
level from Einzelteile RIGHT JOIN Teile oleTeile ON oleTeile.ID = Einzelteile.ID 
INNER JOIN Bauplan ON Bauplan.BESTANDTEIL# = oleTeile.ID
START WITH oleTeile.TYPE = 'PRODUKT'
CONNECT BY PRIOR oleTeile.ID = Bauplan.PRODUCT#
ORDER BY level, oleTeile.BEZEICHNUNG;

SELECT
oleTeile.BEZEICHNUNG BEZEICHNUNG,
oleTeile.TYPE TYPE,
Bauplan.PRODUCT# PRODUCT#,
Bauplan.BESTANDTEIL# BESTANDTEIL#,
Bauplan.MENGE MENGE,
Bauplan.FERTIGUNGSKOSTEN FERTIGUNGSKOSTEN,
level from Einzelteile RIGHT JOIN Teile oleTeile ON oleTeile.ID = Einzelteile.ID 
INNER JOIN Bauplan ON Bauplan.BESTANDTEIL# = oleTeile.ID
START WITH oleTeile.BEZEICHNUNG = 'Isetta'
CONNECT BY PRIOR oleTeile.ID = Bauplan.PRODUCT#
ORDER BY level, oleTeile.BEZEICHNUNG;

-- rechne de summe aus gewicht und göld
SELECT
(NVL(SUM(Einzelteile.EINKAUFSPREIS * Bauplan.MENGE), 0) + NVL(SUM(Bauplan.FERTIGUNGSKOSTEN * Bauplan.MENGE), 0)) GESAMTPREIS
 from Einzelteile RIGHT JOIN Teile oleTeile ON oleTeile.ID = Einzelteile.ID 
INNER JOIN Bauplan ON Bauplan.BESTANDTEIL# = oleTeile.ID 
START WITH oleTeile.BEZEICHNUNG = 'Isetta'
CONNECT BY PRIOR oleTeile.ID = Bauplan.PRODUCT#
ORDER BY level, oleTeile.BEZEICHNUNG;

SELECT
oleTeile.BEZEICHNUNG BEZEICHNUNG,
level, SYS_CONNECT_BY_PATH(BEZEICHNUNG, '/') PATH from Einzelteile RIGHT JOIN Teile oleTeile ON oleTeile.ID = Einzelteile.ID 
INNER JOIN Bauplan ON Bauplan.BESTANDTEIL# = oleTeile.ID
START WITH oleTeile.BEZEICHNUNG = 'Isetta'
CONNECT BY PRIOR oleTeile.ID = Bauplan.PRODUCT#
ORDER BY level, oleTeile.BEZEICHNUNG;

SELECT last_name "Employee",
   LEVEL, SYS_CONNECT_BY_PATH(last_name, '/') "Path"
   FROM employees
   WHERE level <= 3 AND department_id = 80
   START WITH last_name = 'King'
   CONNECT BY NOCYCLE PRIOR employee_id = manager_id AND LEVEL <= 4;

DROP VIEW summe;
CREATE VIEW summe as SELECT
oleTeile.BEZEICHNUNG BEZEICHNUNG,
oleTeile.ID ID,
(NVL(SUM(Einzelteile.EINKAUFSPREIS * Bauplan.MENGE), 0) + NVL(SUM(Bauplan.FERTIGUNGSKOSTEN * Bauplan.MENGE), 0)) GESAMTPREIS
 from Einzelteile RIGHT JOIN Teile oleTeile ON oleTeile.ID = Einzelteile.ID 
INNER JOIN Bauplan ON Bauplan.BESTANDTEIL# = oleTeile.ID 
START WITH oleTeile.BEZEICHNUNG = 'Isetta'
CONNECT BY PRIOR oleTeile.ID = Bauplan.PRODUCT#
group by oleTeile.BEZEICHNUNG, oleTeile.ID;

SELECT teile.BEZEICHNUNG, SUM(summe.GESAMTPREIS) Gesamtkosten from teile inner join summe on teile.ID = (select id from summe where BEZEICHNUNG = 'Isetta')
Group by teile.BEZEICHNUNG;
