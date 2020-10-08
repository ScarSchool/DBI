DROP TABLE Kandidaten CASCADE CONSTRAINTS;
DROP TABLE Quiz CASCADE CONSTRAINTS;
DROP TABLE Fragen CASCADE CONSTRAINTS;
DROP TABLE Antworten CASCADE CONSTRAINTS;
DROP TABLE KmachtQ CASCADE CONSTRAINTS;
DROP TABLE KwaehltA CASCADE CONSTRAINTS;



CREATE TABLE Kandidaten (
  K# VARCHAR2(16),
  Name VARCHAR2(64),
  Schultyp VARCHAR2(16),
  
  CONSTRAINT pk_kandidat PRIMARY KEY (K#),
  CONSTRAINT ckSchType CHECK (UPPER(Schultyp) IN ('NMS', 'AHS', 'SONSTIGE'))
);

CREATE TABLE Quiz (
  Q# VARCHAR2(16),
  bezeichnung VARCHAR2(128),
  
  CONSTRAINT pk_quiz PRIMARY KEY (Q#)
);

CREATE TABLE Fragen (
  F# VARCHAR2(16),
  Q# VARCHAR2(16) NOT NULL,
  text VARCHAR(128) NOT NULL,

  CONSTRAINT pkIdFrage PRIMARY KEY(F#, Q#),
  CONSTRAINT fkIdQuiz FOREIGN KEY (Q#) REFERENCES Quiz
);

CREATE TABLE Antworten (
  A# VARCHAR2(16),
  Q# VARCHAR2(16) NOT NULL,
  F# VARCHAR2(16) NOT NULL,
  text VARCHAR(128) NOT NULL,
  istRichtig VARCHAR(4) NOT NULL,
  
  CONSTRAINT pkIdAntwort PRIMARY KEY(A#, F#, Q#),
  CONSTRAINT fkIdQuizFrage FOREIGN KEY (F#, Q#) REFERENCES Fragen,
  CONSTRAINT ckRichtig CHECK (UPPER(istRichtig) IN ('JA', 'NEIN'))
);



CREATE TABLE KmachtQ (
  Q# VARCHAR2(16),
  K# VARCHAR2(16),
  Datum DATE,
  
  CONSTRAINT pk_KmachtQ PRIMARY KEY (Q#, K#),
  CONSTRAINT fk_quiz FOREIGN KEY (Q#) REFERENCES Quiz,
  CONSTRAINT fk_kandidat FOREIGN KEY (K#) REFERENCES Kandidaten
);

CREATE TABLE KwaehltA (
  K# VARCHAR2(16),
  Q# VARCHAR2(16),
  A# VARCHAR2(16),
  F# VARCHAR2(16),
  
  CONSTRAINT pk_KwaehltA PRIMARY KEY (K#, Q#, F#),
  CONSTRAINT fk_KwaehltAntwort FOREIGN KEY (A#, F#, Q#) REFERENCES Antworten,
  CONSTRAINT fk_KwaehltKamchtQ FOREIGN KEY (Q#, K#) REFERENCES KmachtQ
);



INSERT INTO QUIZ VALUES('Q1', 'Österreich');
INSERT INTO QUIZ VALUES('Q2', 'Europa');

INSERT INTO FRAGEN VALUES('F1', 'Q1', 'Die Hauptstadt von Österreich?');
INSERT INTO FRAGEN VALUES('F2', 'Q1', 'Die Hauptstadt von NÖ?');
INSERT INTO FRAGEN VALUES('F3', 'Q1', 'Was ist das Wahrzeichen von Klagenfurt?');
INSERT INTO FRAGEN VALUES('F1', 'Q2', 'Die Hauptstadt von Frankreich?');
INSERT INTO FRAGEN VALUES('F2', 'Q2', 'Was ist das Wahrzeichen von Brüssel?');

INSERT INTO FRAGEN VALUES('F3', 'Q2', 'Wieso keine richtige antwort?');
INSERT INTO FRAGEN VALUES('F4', 'Q2', 'Wieso zwei richtige antwort?');

INSERT INTO ANTWORTEN VALUES('A1', 'Q1', 'F1', 'Wien', 'JA');
INSERT INTO ANTWORTEN VALUES('A2', 'Q1', 'F1', 'Klagenfurt', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A3', 'Q1', 'F1', 'Graz', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A1', 'Q1', 'F2', 'Wien', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A2', 'Q1', 'F2', 'Klagenfurt', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A3', 'Q1', 'F2', 'St. Pölten', 'JA');
INSERT INTO ANTWORTEN VALUES('A1', 'Q1', 'F3', 'Lindwurm', 'JA');
INSERT INTO ANTWORTEN VALUES('A2', 'Q1', 'F3', 'See', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A3', 'Q1', 'F3', 'Stadion', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A1', 'Q2', 'F1', 'Wien', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A2', 'Q2', 'F1', 'Paris', 'JA');
INSERT INTO ANTWORTEN VALUES('A3', 'Q2', 'F1', 'Graz', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A4', 'Q2', 'F1', 'Klagenfurt', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A1', 'Q2', 'F2', 'Manneken Pis', 'JA');
INSERT INTO ANTWORTEN VALUES('A2', 'Q2', 'F2', 'EuGH', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A3', 'Q2', 'F2', 'Atomium', 'NEIN');

INSERT INTO ANTWORTEN VALUES('A1', 'Q2', 'F3', 'Antwort 1', 'NEIN');
INSERT INTO ANTWORTEN VALUES('A1', 'Q2', 'F4', 'Antwort 1', 'JA');
INSERT INTO ANTWORTEN VALUES('A2', 'Q2', 'F4', 'Antwort 2', 'JA');
INSERT INTO ANTWORTEN VALUES('A3', 'Q2', 'F4', 'Antwort 3', 'NEIN');

INSERT INTO KANDIDATEN VALUES('K1', 'Kernstein Karli', 'NMS');
INSERT INTO KANDIDATEN VALUES('K2', 'Weinstein Werner', 'NMS');
INSERT INTO KANDIDATEN VALUES('K3', 'Steiner Susi', 'AHS');
INSERT INTO KANDIDATEN VALUES('K4', 'Steinwender Sandra', 'AHS');

INSERT INTO KMACHTQ VALUES('Q1', 'K1', TO_DATE('20.09.2019', 'DD.MM.YYYY'));
INSERT INTO KMACHTQ VALUES('Q2', 'K1', TO_DATE('20.09.2019', 'DD.MM.YYYY'));
INSERT INTO KMACHTQ VALUES('Q1', 'K3', TO_DATE('20.09.2019', 'DD.MM.YYYY'));
INSERT INTO KMACHTQ VALUES('Q2', 'K4', TO_DATE('19.09.2019', 'DD.MM.YYYY'));

INSERT INTO KWAEHLTA VALUES('K1', 'Q1', 'A1', 'F1');
INSERT INTO KWAEHLTA VALUES('K1', 'Q1', 'A2', 'F2');
INSERT INTO KWAEHLTA VALUES('K1', 'Q1', 'A2', 'F3');
INSERT INTO KWAEHLTA VALUES('K1', 'Q2', 'A4', 'F1');
INSERT INTO KWAEHLTA VALUES('K1', 'Q2', 'A1', 'F2');
INSERT INTO KWAEHLTA VALUES('K3', 'Q1', 'A3', 'F1');
INSERT INTO KWAEHLTA VALUES('K3', 'Q1', 'A2', 'F2');
INSERT INTO KWAEHLTA VALUES('K3', 'Q1', 'A1', 'F3');
INSERT INTO KWAEHLTA VALUES('K4', 'Q2', 'A3', 'F1');
INSERT INTO KWAEHLTA VALUES('K4', 'Q2', 'A1', 'F2');



-- FAIL --
-- INSERT INTO ANTWORTEN VALUES('A1', 'Q11', 'F1', 'Quiz gibt es nicht', 'JA');
-- INSERT INTO ANTWORTEN VALUES('A2', 'Q1', 'F4', 'Frage gibt es nicht', 'NEIN');
-- INSERT INTO ANTWORTEN VALUES('A3', 'Q1', 'F1', 'Flag gibt es nicht', 'korr');

-- INSERT INTO FRAGEN VALUES('F1', 'Q11', 'Quiz gibt es nicht');

-- INSERT INTO KMACHTQ VALUES('Q11', 'K1', TO_DATE('20.09.2019', 'DD.MM.YYYY'));
-- INSERT INTO KMACHTQ VALUES('Q2', 'K11', TO_DATE('20.09.2019', 'DD.MM.YYYY'));

-- INSERT INTO KWAEHLTA VALUES('K5', 'Q1', 'A1', 'F1');
-- INSERT INTO KWAEHLTA VALUES('K1', 'Q11', 'A2', 'F2');
-- INSERT INTO KWAEHLTA VALUES('K1', 'Q1', 'A7', 'F3');
-- INSERT INTO KWAEHLTA VALUES('K1', 'Q2', 'A4', 'F11');
-- INSERT INTO KWAEHLTA VALUES('K1', 'Q1', 'A2', 'F1');


SELECT quiz.bezeichnung, COUNT(*) FROM fragen 
  INNER JOIN quiz on fragen.Q# = quiz.Q# 
  GROUP BY fragen.Q#, quiz.bezeichnung;
  

SELECT quiz.bezeichnung, antworten.f#, fragen.text, COUNT(*) FROM ANTWORTEN
  inner join fragen on fragen.F# = ANTWORTEN.F# and ANTWORTEN.Q# = fragen.q#
  inner join quiz on quiz.Q# = antworten.Q#
  GROUP BY antworten.F#, quiz.bezeichnung, fragen.text;

SELECT quiz.bezeichnung, antworten.f#, fragen.text, COUNT(*) FROM ANTWORTEN
  inner join fragen on fragen.F# = ANTWORTEN.F# and ANTWORTEN.Q# = fragen.q#
  inner join quiz on quiz.Q# = antworten.Q#
  GROUP BY antworten.F#, quiz.bezeichnung, fragen.text having count(*) != 4;
  
select quiz.q#, quiz.bezeichnung, main.f#, main.text FROM FRAGEN main
  inner join quiz on quiz.Q# = main.Q#
  where
    (SELECT COUNT(*) FROM ANTWORTEN sub WHERE main.f# = sub.f# and main.q# = sub.q# AND ISTRICHTIG = 'JA') 
    != 1;
