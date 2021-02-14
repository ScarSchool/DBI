DROP TABLE angebote CASCADE CONSTRAINTS;
DROP TABLE angebotsartikel CASCADE CONSTRAINTS;
DROP TABLE warenlager CASCADE CONSTRAINTS;
DROP TABLE warenkorb CASCADE CONSTRAINTS;

create table angebote (
  anr integer primary key,
  angebotstitel varchar2(50),
  rabatt integer
);

create table angebotsartikel (
  anr integer,
  besteht_aus_nr integer,
  menge integer,
  primary key (anr,besteht_aus_nr),
  foreign key (besteht_aus_nr) references angebote (anr),
  foreign key (anr) references angebote (anr)
);

create table warenlager (
  anr integer primary key,
  lmenge integer,
  epreis number(8,2),
  foreign key (anr) references angebote (anr)  -- ,lagerstandort SDO_GEOMETRY,
);

create table warenkorb (
  kundenid varchar2(20) primary key,
  anr integer,
  bestellmenge integer,
  bestellpreis number(10,2),
  foreign key (anr) references angebote (anr)
);



insert into angebote values(1,'Kettensäge DUC 306',0);
insert into angebote values(2,'Kettensäge DUC 540',0);
insert into angebote values(3,'Heckenschere H10',0);
insert into angebote values(4,'Rasentrimmer TUP18',0);
insert into angebote values(5,'Ladegerät P18',0);
insert into angebote values(6,'Ladegerät PD36',0);
insert into angebote values(7,'Akku 5Ah',10);
insert into angebote values(8,'Akku 3Ah',0);
insert into angebote values(9,'Akku 6Ah',0);
insert into angebote values(10,'Schlagbohrmaschine',0);
insert into angebote values(11,'Schlagschrauber',0);
insert into angebote values(12,'Winkelschleifer',0);

insert into warenlager values(1, 10, 355.00);
insert into warenlager values(2, 10, 280.50);
insert into warenlager values(3, 10, 150.00);
insert into warenlager values(4, 10, 255.00);
insert into warenlager values(5, 10, 55.00);
insert into warenlager values(6, 10, 75.00);
insert into warenlager values(7, 20, 80.00);
insert into warenlager values(8, 20, 60.00);
insert into warenlager values(9, 20, 90.00);
insert into warenlager values(10, 10, 190.00);
insert into warenlager values(11, 10, 290.00);
insert into warenlager values(12, 20, 150.50);

-- sets
insert into angebote values(13,'Schlagbohrerset Dounle-Akku',10);
insert into angebote values(14,'Rasentrimmerset Single-Akku',10);
insert into angebote values(15,'Maschinenset Basic',10);
insert into angebote values(16,'Maschinenset Maxi',20);
insert into angebote values(17,'Kettensäge Double-Charger + 4x5Ah',10);

insert into angebotsartikel values (13,10,1);
insert into angebotsartikel values (13,6,1);
insert into angebotsartikel values (13,7,4);
insert into angebotsartikel values (14,4,1);
insert into angebotsartikel values (14,5,1);
insert into angebotsartikel values (14,8,2);
insert into angebotsartikel values (17,2,1);
insert into angebotsartikel values (17,6,1);
insert into angebotsartikel values (17,7,4);
insert into angebotsartikel values (15,10,1);
insert into angebotsartikel values (15,11,1);
insert into angebotsartikel values (16,13,1);
insert into angebotsartikel values (16,11,1);
insert into angebotsartikel values (16,12,1);
commit;



/*
Aufgabe: Bestellvorgang in einem Warenhaus

Ein Warenhaus hat Angebote mit Produkten und gewährt darauf einen Rabatt.
Stellen sie sich vor, sie machen zu einem Angebot eine Bestellung. Sie sind Kunde mit einer eindeutigen KundenID. 
Die Produkte dieses Angebots werden für diese Bestellung in den Warenkorb gelegt aber nur, wenn sie im Warenlager vollständig zur Verfügung stehen.

Falls der Lagerbestand für ein bestelltes Produkt nicht ausreicht, wird der gesamte Bestellvorgang 
mit einer entsprechenden Meldung abgebrochen. Der Bestellvorgang kann dann nicht mit kostenpflichtigem Kauf
abgeschlossen werden, da der Warenkorb für diese Bestellung keine Produkte enthält bzw. die Bestellung abgebrochen worden ist. 

Wenn alle Produkte einer Bestellung geliefert werden können, also ausreichend auf Lager sind, kann die Bestellung
mit einem kostenpflichtigen Kauf abgeschlossen werden oder doch noch storniert werden. 

Der Warenkorb ist nach einer kostenpflichtigen Bestellung oder einer Stornierung für den aktuellen Kunden wieder zu leeren.
Für einen Kunden gibt es jeweils nur höchstens eine Bestellung im Warenkorb.

Der Ablauf muss transaktionssicher sein, d.h. zwichen Warenkorb und kostenpflichtiger Bestellung 
darf keine andere Transaktion den Lagerstand der betroffenen Produkte ändern.

Bei Stornierungen müssen Änderungen in den Tabellen rückgängig gemacht werden. Bei erfolgreichen Bestellungen 
müssen Änderungen festgeschrieben werden. Locks sind in jedem Fall aufzuheben.
 

Procedure 1: Bestellung eines Angebots und Produkte in Warenkorb legen
proc_warenkorb(kundenid IN varchar2, angebotnr IN integer, bestellmenge IN integer, bestell_flag OUT varchar2)

Bestellt wird ein Angebot in einer bestimmten Menge. Das Bestellflag zeigt den Status der Bestellung an:
  'ok', wenn alle Produkte des Angebots geliefert werden können oder 'derzeit nicht lieferbar', 
   wenn zumindest ein Produkt nicht ausreichend auf Lager liegt.

Das Füllen des Warenkorbs geht über INSERT-Kommandos. Die Produkte des Angebots werden der Reihe nach 
in den Warenkorb eingefügt. Die aktuellen Preise inklusive Rabatte sind für die Warenkorbeinträge zu berechnen.

Trigger:
Ein Trigger, der auf das INSERT-Ereignis reagieren soll, macht alle Überprüfungen und Änderungen im Warenlager.
Der Trigger erkennt einen zu niedrigen Lagerbestand und bricht die Bestellung ab.

Procedure 2: kostenpflichtige Bestellung oder Stornierung
proc_order(kundenid IN varchar2, order_status IN boolean)

- Bestellung durchführen/Änderungen festschreiben: Status = true
- Bestellung stornieren:  Status = false



Zu erstellen sind 2 Prozeduren und ein Trigger. Den Benutzern der PL/SQL-Routinen soll eine ausreichende Info geboten werden.
Eine umfangreiche, aussagekräftige Protokollierung der Ereignisse soll erfolgen. 
Es gibt mehrere Benutzer, die diese Routinen starten können. Ein kontrollierter Transaktionsablauf ist zu planen.
*/



-- Function - calculate price
CREATE OR REPLACE FUNCTION CALCULATE_PRICE (
  p_bestellmenge IN INTEGER, 
  p_anr IN INTEGER 
) 
RETURN NUMBER 
AS 
  CURSOR c_angebotinfos IS
    SELECT rabatt, epreis preis FROM angebote
    LEFT JOIN warenlager ON warenlager.anr = angebote.anr
    WHERE angebote.anr = p_anr;
    
  CURSOR c_angebotItems IS
    SELECT menge, besteht_aus_nr anr FROM angebotsartikel
    WHERE anr = p_anr;
  
  angebotInfo c_angebotinfos%ROWTYPE;
  bestellpreis NUMBER(10,2) := 0.0;
BEGIN
  OPEN c_angebotinfos;
  FETCH c_angebotinfos INTO angebotInfo;
  
  IF angebotInfo.preis IS NOT NULL THEN
    bestellpreis := p_bestellmenge * angebotInfo.preis * (100 - angebotInfo.rabatt) / 100;
  ELSE
    FOR item IN c_angebotItems LOOP
      bestellpreis := bestellpreis + calculate_price(p_bestellmenge * item.menge, item.anr) * (100 - angebotInfo.rabatt) / 100;
    END LOOP;
  END IF;
  
  RETURN bestellpreis;
END CALCULATE_PRICE;

-- Procedure - check lagerbestand
CREATE OR REPLACE PROCEDURE CHECK_LAGERBESTAND(
  p_bestellmenge IN INTEGER, 
  p_anr IN INTEGER 
)
AS 
  CURSOR c_angebotLager IS
    SELECT lmenge menge, angebote.anr anr, angebotstitel FROM warenlager
    INNER JOIN angebote ON angebote.anr = warenlager.anr
    WHERE warenlager.anr = p_anr;
    
  CURSOR c_angebotItems IS
    SELECT menge, besteht_aus_nr anr FROM angebotsartikel
    WHERE anr = p_anr;
  
  lagerbestand c_angebotLager%ROWTYPE;
BEGIN
  OPEN c_angebotLager;
  FETCH c_angebotLager INTO lagerbestand;
  
  IF c_angebotLager%FOUND THEN
    IF p_bestellmenge > lagerbestand.menge THEN
      raise_application_error(-20010, 'Lagerbestand von ' || lagerbestand.anr || ' ' || lagerbestand.angebotstitel || ' zu niedrig für bestellmenge von ' || p_bestellmenge || '!');
    END IF;
  ELSE
    FOR item IN c_angebotItems LOOP
      check_lagerbestand(p_bestellmenge * item.menge, item.anr);
    END LOOP;
  END IF;
END CHECK_LAGERBESTAND;

-- Procedure - update lagerbestand
CREATE OR REPLACE PROCEDURE UPDATE_LAGERBESTAND(
  p_bestellmenge IN INTEGER, 
  p_anr IN INTEGER,
  p_op IN VARCHAR2
)
AS 
  CURSOR c_angebotLager IS
    SELECT lmenge menge, angebote.anr anr, angebotstitel FROM warenlager
    INNER JOIN angebote ON angebote.anr = warenlager.anr
    WHERE warenlager.anr = p_anr;
    
  CURSOR c_angebotItems IS
    SELECT menge, besteht_aus_nr anr FROM angebotsartikel
    WHERE anr = p_anr;
  
  lagerbestand c_angebotLager%ROWTYPE;
BEGIN
  OPEN c_angebotLager;
  FETCH c_angebotLager INTO lagerbestand;
  
  IF c_angebotLager%FOUND THEN
    IF p_op = '-' THEN
      UPDATE warenlager 
      SET lmenge = lagerbestand.menge - p_bestellmenge 
      WHERE anr = p_anr;
    ELSE
      UPDATE warenlager 
      SET lmenge = lagerbestand.menge + p_bestellmenge 
      WHERE anr = p_anr;
    END IF;
  ELSE
    FOR item IN c_angebotItems LOOP
      update_lagerbestand(p_bestellmenge * item.menge, item.anr, p_op);
    END LOOP;
  END IF;
END UPDATE_LAGERBESTAND;



-- Proc 1
CREATE OR REPLACE PROCEDURE PROC_WARENKORB (
  p_kundenid IN VARCHAR2, 
  p_anr IN VARCHAR2, 
  p_bestellmenge IN INTEGER, 
  p_flag OUT VARCHAR2
) 
IS 
BEGIN  
  INSERT INTO warenkorb VALUES(p_kundenid, p_anr, p_bestellmenge, calculate_price(p_bestellmenge, p_anr));
  p_flag := 'ok';
  
  EXCEPTION WHEN OTHERS THEN
    p_flag := 'derzeit nicht lieferbar';
      
END PROC_WARENKORB;

-- Proc 2
CREATE OR REPLACE PROCEDURE PROC_ORDER (
  p_kundenid IN VARCHAR2, 
  p_order_status IN INTEGER
) 
IS 
  CURSOR c_bestellungen IS
    SELECT anr, bestellmenge
    FROM warenkorb
    WHERE kundenid = p_kundenid;
    
  bestellung c_bestellungen%ROWTYPE;
BEGIN
  OPEN c_bestellungen;
  FETCH c_bestellungen INTO bestellung;
  DELETE FROM warenkorb WHERE kundenid = p_kundenid;

  IF p_order_status = 0 THEN
    update_lagerbestand(bestellung.bestellmenge, bestellung.anr, '+'); 
  END IF;
END PROC_ORDER;

-- Trigger
CREATE OR REPLACE TRIGGER WARENLAGER 
BEFORE INSERT ON WARENKORB 
FOR EACH ROW 
BEGIN
  IF :NEW.bestellpreis <> calculate_price(:NEW.bestellmenge, :NEW.anr) THEN
      raise_application_error(-20020, 'Bestellpreis stimmt nich mit Angebot überein!');
  END IF;
  
  check_lagerbestand(:NEW.bestellmenge, :NEW.anr);
  update_lagerbestand(:NEW.bestellmenge, :NEW.anr, '-');
END;
