drop table buecher;
drop table leser;
drop table gelesen;
drop table beruf;

create table buecher(
  bno int primary key,
  btitel varchar2(30),
  anzahlseiten int
);
  
create table leser(
  lno int primary key,
  lname varchar2(30),
  jobno int
);

create table gelesen(
  lno int,
  bno int,
  von date,
  bis date
);

create table beruf(
  jno int primary key,
  jname varchar2(30)
);

insert into buecher values(1,'Winnetou I',101);
insert into buecher values(2,'Winnetou II',2101);
insert into buecher values(3,'Lederstrumpf',301);
insert into buecher values(4,'Schlafes Bruder',404);
insert into buecher values(5,'C# fuer Anfänger',1);
insert into buecher values(6,'C++ fuer Profis',1001);
insert into buecher values(7,'Maerchen',543);

insert into beruf values(8,'Schueler');
insert into beruf values(9,'Lehrer');
insert into beruf values(10,'Sonstige');

insert into leser values(11,'Ameise',8);
insert into leser values(12,'Bmeise',8);
insert into leser values(13,'Cmeise',8);
insert into leser values(14,'Einstein',9);
insert into leser values(15,'Zweistein',9);
insert into leser values(16,'Dreistein',10);

insert into gelesen values(11,1,'11/12/2005','14/12/2005');
insert into gelesen values(11,2,'11/12/2005','15/12/2005');
insert into gelesen values(11,3,'11/11/2005','14/12/2005');
insert into gelesen values(11,4,'11/10/2005','14/12/2005');
insert into gelesen values(11,1,'21/01/2006','22/02/2006');
insert into gelesen values(11,2,'21/01/2006','22/03/2006');
insert into gelesen values(11,3,'11/04/2006','14/12/2006');
insert into gelesen values(12,6,'04/05/2006','14/06/2006');
insert into gelesen values(12,5,'11/12/2006','14/12/2006');
insert into gelesen values(12,6,'11/12/2005','14/12/2005');
insert into gelesen values(12,5,'10/10/2005','14/12/2005');
insert into gelesen values(12,4,'11/12/2006','14/12/2006');
insert into gelesen values(12,1,'11/12/2005','14/12/2005');
insert into gelesen values(13,1,'11/12/2005','14/12/2005');
insert into gelesen values(13,2,'10/02/2005','14/02/2005');
insert into gelesen values(14,4,'11/12/2005','14/12/2005');
insert into gelesen values(14,5,'11/12/2006','14/12/2006');
insert into gelesen values(14,3,'10/02/2005','04/03/2005');
insert into gelesen values(15,4,'11/12/2005','14/12/2005');
insert into gelesen values(15,6,'11/12/2005','14/12/2005');
insert into gelesen values(16,5,'11/12/2005','14/12/2005');
insert into gelesen values(16,6,'11/09/2005','14/12/2005');
insert into gelesen values(16,7,'11/08/2005','14/12/2005');
commit;

SELECT * FROM buecher;
SELECT * FROM leser;
SELECT * FROM gelesen;
SELECT * FROM beruf;

SELECT 
  beruf.jname, 
  NVL(to_char(gelesen.VON, 'YYYY'), 'alle Jahre') jahr, 
  COUNT(*) leseranzahl, 
  to_char(AVG(months_between(gelesen.BIS, gelesen.VON)), '9999.0') avg_mon 
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO
GROUP BY GROUPING SETS ( 
  (beruf.JNO, beruf.jname), 
  (beruf.JNO, beruf.jname, to_char(gelesen.VON, 'YYYY'))
)
ORDER BY beruf.JNAME, to_char(gelesen.VON, 'YYYY');

SELECT 
  buecher.BTITEL, 
  NVL(to_char(EXTRACT(YEAR FROM gelesen.VON)), 'alle') jahr,
  NVL(beruf.JNAME, 'alle') job, 
  to_char(AVG(MONTHS_BETWEEN(gelesen.BIS, gelesen.VON)), '9999.0') avg_mon,
  GROUPING_ID(buecher.BTITEL, beruf.JNAME, EXTRACT(YEAR FROM gelesen.VON)) gid
FROM buecher
INNER JOIN gelesen ON gelesen.BNO = buecher.BNO
INNER JOIN leser ON leser.LNO = gelesen.LNO
INNER JOIN beruf ON beruf.JNO = leser.JOBNO
GROUP BY GROUPING SETS (
   (buecher.BTITEL),
   (buecher.BTITEL, beruf.JNAME),
   (buecher.BTITEL, EXTRACT(YEAR FROM gelesen.VON))
)
HAVING COUNT(gelesen.von) > 1
ORDER BY gid, beruf.JNAME DESC, avg_mon;

SELECT 
  buecher.BTITEL, 
  NVL(to_char(EXTRACT(YEAR FROM gelesen.VON)), 'alle') jahr,
  NVL(beruf.JNAME, 'alle') job, 
  to_char(AVG(MONTHS_BETWEEN(gelesen.BIS, gelesen.VON)), '9999.0') avg_mon,
  GROUPING_ID(buecher.BTITEL, beruf.JNAME, EXTRACT(YEAR FROM gelesen.VON)) gid
FROM buecher
INNER JOIN gelesen ON gelesen.BNO = buecher.BNO
INNER JOIN leser ON leser.LNO = gelesen.LNO
INNER JOIN beruf ON beruf.JNO = leser.JOBNO
GROUP BY CUBE (buecher.BTITEL, EXTRACT(YEAR FROM gelesen.VON), beruf.JNAME)
HAVING COUNT(gelesen.von) > 1 AND GROUPING_ID(buecher.BTITEL, EXTRACT(YEAR FROM gelesen.VON), beruf.JNAME) BETWEEN 1 AND 3
ORDER BY gid, beruf.JNAME DESC, avg_mon;

SELECT 
  COUNT(CASE WHEN beruf.JNAME = 'Schueler' THEN 1 END) Schueler,
  COUNT(CASE WHEN beruf.JNAME = 'Lehrer' THEN 1 END) Lehrer,
  COUNT(CASE WHEN beruf.JNAME = 'Sonstige' THEN 1 END) Sonstige
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO;

SELECT 
  to_char(to_date(EXTRACT(MONTH FROM gelesen.VON), 'MM'), 'MON') mon,
  COUNT(CASE WHEN beruf.JNAME = 'Schueler' THEN 1 END) Schueler,
  COUNT(CASE WHEN beruf.JNAME = 'Lehrer' THEN 1 END) Lehrer,
  COUNT(CASE WHEN beruf.JNAME = 'Sonstige' THEN 1 END) Sonstige
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO
GROUP BY EXTRACT(MONTH FROM gelesen.VON)
ORDER BY EXTRACT(MONTH FROM gelesen.VON);

SELECT 
  to_char(gelesen.VON, 'Q') || '. Qu.'  quartal,
  COUNT(CASE WHEN beruf.JNAME = 'Schueler' THEN 1 END) Schueler,
  COUNT(CASE WHEN beruf.JNAME = 'Lehrer' THEN 1 END) Lehrer,
  COUNT(CASE WHEN beruf.JNAME = 'Sonstige' THEN 1 END) Sonstige
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO
GROUP BY to_char(gelesen.VON, 'Q') 
ORDER BY to_char(gelesen.VON, 'Q');


SELECT 
  beruf.JNAME,
  COUNT(CASE WHEN to_char(gelesen.VON, 'Q') = '1' THEN 1 END) "1. Qu",
  COUNT(CASE WHEN to_char(gelesen.VON, 'Q') = '2' THEN 1 END) "2. Qu",
  COUNT(CASE WHEN to_char(gelesen.VON, 'Q') = '3' THEN 1 END) "3. Qu",
  COUNT(CASE WHEN to_char(gelesen.VON, 'Q') = '4' THEN 1 END) "4. Qu"
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO
GROUP BY beruf.JNAME
ORDER BY beruf.JNAME;

SELECT 
  NVL(buecher.BTITEL, 'alle Buecher') buch, 
  NVL(to_char(EXTRACT(YEAR FROM gelesen.VON)), 'ueber alle jahre') jahr,
  COUNT(*) anzahl,
  to_char(AVG(gelesen.bis - gelesen.von), '99999') avg_day
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO
INNER JOIN buecher ON buecher.BNO = gelesen.BNO
WHERE beruf.JNAME = 'Lehrer'
GROUP BY ROLLUP (EXTRACT(YEAR FROM gelesen.VON), (buecher.BNO, buecher.BTITEL))
ORDER BY GROUPING_ID(buecher.BTITEL, EXTRACT(YEAR FROM gelesen.VON)), EXTRACT(YEAR FROM gelesen.VON), buecher.BTITEL;

SELECT 
  beruf.jname,
  buecher.BTITEL buch, 
  COUNT(CASE WHEN EXTRACT(YEAR FROM gelesen.VON) = 2005 THEN 1 END) "Anzahl 2005",
  COUNT(CASE WHEN EXTRACT(YEAR FROM gelesen.VON) = 2006 THEN 1 END) "Anzahl 2006",
  to_char(-100 + COUNT(CASE WHEN EXTRACT(YEAR FROM gelesen.VON) = 2006 THEN 1 END) / COUNT(CASE WHEN EXTRACT(YEAR FROM gelesen.VON) = 2005 THEN 1 END) * 100, '99999999.0') "Veränderung in %"
FROM beruf
INNER JOIN leser ON leser.JOBNO = beruf.JNO
INNER JOIN gelesen ON gelesen.LNO = leser.LNO
INNER JOIN buecher ON buecher.BNO = gelesen.BNO
GROUP BY beruf.jname, buecher.BTITEL
HAVING COUNT(CASE WHEN EXTRACT(YEAR FROM gelesen.VON) = 2005 THEN 1 END) > 0
ORDER BY beruf.JNAME, buch;
