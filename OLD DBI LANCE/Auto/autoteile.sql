DROP TABLE teile CASCADE CONSTRAINTS;
DROP TABLE bauplan CASCADE CONSTRAINTS;
DROP TABLE einzelteile CASCADE CONSTRAINTS;

CREATE TABLE teile (
  id INTEGER,
  bezeichnung VARCHAR2(50),
  type VARCHAR2(20),
  CONSTRAINT pkiTeile PRIMARY KEY (id),
  CONSTRAINT uqiTeilType UNIQUE (id, type),
  CONSTRAINT ckiType CHECK (UPPER(type) IN ('PRODUKT', 'ZWISCHENPRODUKT', 'EINZELTEIL'))
  );

CREATE TABLE bauplan (
  product# INTEGER,
  bestandteil# INTEGER,
  menge INTEGER,
  fertigungskosten INTEGER,
  CONSTRAINT uqiBauplan UNIQUE (product#, bestandteil#),
  CONSTRAINT fkiBestehtAus_Produkt2 FOREIGN KEY (product#) REFERENCES Teile(id),
  CONSTRAINT fkiBestehtAus_Bestandteil2 FOREIGN KEY (bestandteil#) REFERENCES Teile(id),
  CONSTRAINT ckiMEngePReis CHECK (product# is not null AND menge is not null AND fertigungskosten is not null 
                                  OR product# is null AND menge is null AND fertigungskosten is null),
  CONSTRAINT ckiIds2 CHECK ( product# <> bestandteil# )
);

CREATE TABLE einzelteile (
  id INTEGER,
  gewicht INTEGER,
  einkaufspreis INTEGER,
  CONSTRAINT pkiEinzelteile2 PRIMARY KEY (id),
  CONSTRAINT fkiETTeile FOREIGN KEY (id) REFERENCES Teile(id)
);

INSERT INTO teile VALUES(10, 'Isetta', 'PRODUKT');
INSERT INTO teile VALUES(20, 'Doppel Corsa A', 'PRODUKT');
INSERT INTO teile VALUES(30, 'Motor-I', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(40, 'Karosserie-I', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(50, 'Inneneinrichtung-I', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(60, 'Rahmen-I', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(330, 'Motor-C', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(340, 'Karosserie-C', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(350, 'Inneneinrichtung-C', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(360, 'Rahmen-C', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(370, 'Überrollbügel-C', 'ZWISCHENPRODUKT');
INSERT INTO teile VALUES(80, 'Blech', 'EINZELTEIL');
INSERT INTO teile VALUES(90, 'Fenster', 'EINZELTEIL');
INSERT INTO teile VALUES(100, 'Tür', 'EINZELTEIL');
INSERT INTO teile VALUES(110, 'Kabel', 'EINZELTEIL');
INSERT INTO teile VALUES(120, 'Zylinder', 'EINZELTEIL');
INSERT INTO teile VALUES(130, 'Kunststoff', 'EINZELTEIL');
INSERT INTO teile VALUES(140, 'Lenkrad', 'EINZELTEIL');
INSERT INTO teile VALUES(150, 'Sitz', 'EINZELTEIL');
INSERT INTO teile VALUES(160, 'Pedal', 'EINZELTEIL');
INSERT INTO teile VALUES(170, 'Stoff', 'EINZELTEIL');
INSERT INTO teile VALUES(180, 'Spoiler', 'EINZELTEIL');

INSERT INTO einzelteile VALUES(80, 20, 5);
INSERT INTO einzelteile VALUES(90, 120, 20);
INSERT INTO einzelteile VALUES(100, 220, 50);
INSERT INTO einzelteile VALUES(110, 2, 1);
INSERT INTO einzelteile VALUES(120, 520, 50);
INSERT INTO einzelteile VALUES(130, 5, 1);
INSERT INTO einzelteile VALUES(140, 210, 5);
INSERT INTO einzelteile VALUES(150, 420, 35);
INSERT INTO einzelteile VALUES(160, 20, 5);
INSERT INTO einzelteile VALUES(170, 5, 1);
INSERT INTO einzelteile VALUES(180, 30, 15);

INSERT INTO bauplan VALUES(NULL,10,NULL,NULL);
INSERT INTO bauplan VALUES(NULL,20,NULL,NULL);
INSERT INTO bauplan VALUES(10,30,1,1000);
INSERT INTO bauplan VALUES(10,40,1,400);
INSERT INTO bauplan VALUES(10,50,1,300);
INSERT INTO bauplan VALUES(10,170,3,100);
INSERT INTO bauplan VALUES(30,120,2,20);
INSERT INTO bauplan VALUES(30,110,10,5);
INSERT INTO bauplan VALUES(40,100,1,99);
INSERT INTO bauplan VALUES(40,90,4,20);
INSERT INTO bauplan VALUES(40,60,1,110);
INSERT INTO bauplan VALUES(50,140,1,15);
INSERT INTO bauplan VALUES(50,150,2,10);
INSERT INTO bauplan VALUES(50,160,3,5);
INSERT INTO bauplan VALUES(60,80,10,1200);
INSERT INTO bauplan VALUES(60,130,2,500);

INSERT INTO bauplan VALUES(20,330,2,1200);
INSERT INTO bauplan VALUES(20,340,1,500);
INSERT INTO bauplan VALUES(20,350,1,500);
INSERT INTO bauplan VALUES(330,120,4,30);
INSERT INTO bauplan VALUES(330,110,30,5);
INSERT INTO bauplan VALUES(340,360,1,220);
INSERT INTO bauplan VALUES(340,100,2,110);
INSERT INTO bauplan VALUES(340,90,6,20);
INSERT INTO bauplan VALUES(340,180,2,5);
INSERT INTO bauplan VALUES(350,140,1,15);
INSERT INTO bauplan VALUES(350,150,2,5);
INSERT INTO bauplan VALUES(350,160,3,5);
INSERT INTO bauplan VALUES(350,370,2,5);
INSERT INTO bauplan VALUES(360,80,20,1100);
INSERT INTO bauplan VALUES(360,130,12,200);
INSERT INTO bauplan VALUES(370,80,1,20);
INSERT INTO bauplan VALUES(370,170,4,10);
/*
INSERT INTO bauplan VALUES(NULL,101010,1,1); -- parent NULL => Menge u Kosten müssen auch NULL sein
INSERT INTO bauplan VALUES(NULL,101010,NULL,1); -- parent NULL => Menge u Kosten müssen auch NULL sein
INSERT INTO bauplan VALUES(NULL,101010,1,NULL); -- parent NULL => Menge u Kosten müssen auch NULL sein
INSERT INTO bauplan VALUES(370,10,1,NULL); -- parent NOT NULL => Menge u Kosten dürfen nicht NULL sein
*/



SELECT teile.BEZEICHNUNG "Teil", '==>' "    ", teile2.BEZEICHNUNG || ' (' || teile2.type || ')' "besteht aus", menge, fertigungskosten "KOSTEN" FROM bauplan
  INNER JOIN teile ON teile.ID = bauplan.PRODUCT#
  INNER JOIN teile teile2 ON teile2.ID = bauplan.BESTANDTEIL#
  ORDER BY teile.type, teile.BEZEICHNUNG;

SELECT level, LPAD(' ', 2 * level) || '-> ' || teile.BEZEICHNUNG || ' (' || teile.type || ')' "Teil", NVL('' || bauplan.MENGE, '---') "Menge", NVL('' || bauplan.FERTIGUNGSKOSTEN, '---') "Kosten" FROM bauplan
  INNER JOIN teile ON bauplan.BESTANDTEIL# = teile.ID
  START WITH teile.BEZEICHNUNG LIKE 'Isetta'
  CONNECT BY PRIOR BESTANDTEIL# = PRODUCT#;
  
  