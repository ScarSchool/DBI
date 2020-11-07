# Normalformen Übung
## Bootsvermietung
Vermietung(**V#**, B#, Typ, Segelfläche, Länge, K#, KName, TelNr, Tag)
*1NF ist gegeben*

B# -> Typ, Segelfläche, Länge \
K# -> KName, TelNr \
B#, Tag -> K# \
K#, Tag -> B# 

SK1: B#, Tag \
SK2: K#, Tag

Boot(**B#**, Typ, Segelfläche, Länge) \
Kunde(**K#**, KName, TelNr) \
Vermietung(**V#**, B#, K#, Tag) \
*2NF, 3NF, BCNF ist gegeben*



## Formel 1 Saison
Rennen(**R#**, Ort, F1#, H#, HName, HNation, Modell, KW, PID, Startplatz, Ergebnis) \
*1NF ist gegeben*

F1# -> HName, HNation, Modell, KW \
Ort, F1# -> Startplatz, Ergebnis, PID \
Ort, PID -> Startplatz, Ergebnis, F1# \
Ort, Startplatz -> Ergebnis, PID, F1# \
Ort, Ergebnis -> Startplatz, PID, F1# 

SK1: Ort, F1# \
SK2: Ort, PID \
SK3: Ort, Startplatz \
SK4: Ort, Ergebnis \

F1-Auto(**F1#**, H#, HName, HNation, Modell, KW) \
Rennen(**R#**, Ort, F1#, PID, Startplatz, Ergebnis) \
*2NF ist gegeben*

H# -> HName, HNation \
Modell -> H#, KW

Hersteller(**H#**, HName, HNation) \
Modell(**Modell**, H#, KW) \
F1-Auto(**F1#**, Modell) \
Rennen(**R#**, Ort, F1#, PID, Startplatz, Ergebnis) \
*3NF, BCNF ist gegeben*



## Telefonkostenverfolgung
Kosten(**K#**, H#, TelNr, Anbieter, Tarif, Kosten/Minute, Telefonminuten) \
*1NF ist gegeben*

TelNr -> Anbieter, Tarif, Kosten/Minute \
TelNr, H# -> Telefonminuten

SK: TelNr, H#

SimKarte(**TelNr**, Anbieter, Tarif, Kosten/Minute) \
Kosten(**K#**, TelNr, H#, Telefonminuten) \
*2NF ist gegeben*

Tarif -> Anbieter, Kosten/Minute

SimKarte(**TelNr**, Tarif) \
Tarif(**Tarif**, Anbieter, Kosten/Minute) \
Kosten(**K#**, H#, TelNr, Telefonminuten) \
*3NF, BCNF ist gegeben*



## Beliebiges Beispiel [R(a,b,c,d,e,f)]
Zeitaufwand(**Z#**, ProjNr, PersNr, ProjId, AbtNr, AbtStandort, Zeit) \
*1NF ist gegeben*

ProjNr -> ProjId \
ProjId -> ProjNr \
PersNr -> AbtNr, AbtStandort \
ProjId, PersNr -> Zeit \
ProjNr, PersNr -> Zeit

SK1: ProjNr, PersNr \
SK2: ProjId, PersNr

Person(**PersNr**, AbtNr, AbtStandort) \
Projekt(**ProjNr**, ProjId) \
Zeitaufwand(**Z#**, ProjNr, PersNr, Zeit) \
*2NF ist gegeben*

AbtNr -> AbtStandort

Person(**PersNr**, AbtNr) \
Abteilung(**AbtNr**, AbtStandort) \
Projekt(**ProjNr**, ProjId) \
Zeitaufwand(**Z#**, ProjNr, PersNr, Zeit) \
*3NF, BCNF ist gegeben*



## Spedition mit Fahrer, LKW’s und Lieferungen
LieferungsNr -> Lieferung, Fahrt \
Fahrt -> LKW, Fahrer

SK: LieferungsNr, Fahrt

Lieferung(**LieferungsNr**, Lieferung, Fahrt) \
Fahrt(**Fahrt**, LKW, Fahrer) \
*1NF, 2NF, 3NF, BCNF ist gegeben*
