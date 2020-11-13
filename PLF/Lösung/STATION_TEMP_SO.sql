CREATE OR REPLACE PROCEDURE STATION_TEMP_SO
AS
  CURSOR c_stationen
  IS
    SELECT gebiet.gnr gnr, gname, station.stnr stnr, stname, c, so 
    FROM station INNER JOIN gebiet
    ON gebiet.gnr = station.gnr
    INNER JOIN aufzeichnungen
    ON aufzeichnungen.stnr = station.stnr
    ORDER BY gname, stname, datum;
  gnr     NUMBER := -1;
  stnr    NUMBER := -1;
  maxTemp NUMBER := -1;
  avgSO   NUMBER := -1;
  tage    NUMBER := -1;
BEGIN
  FOR r_station IN c_stationen
  LOOP
    IF r_station.gnr <> gnr THEN
      dbms_output.put_line('Gebiet: ' || r_station.gname);
      gnr := r_station.gnr;
    END IF;
    IF r_station.stnr <> stnr THEN
      -- Is there a better solution to this?
      SELECT MAX(datum) - MIN(datum)
      INTO tage
      FROM aufzeichnungen
      WHERE stnr = r_station.stnr;
      
      -- Would like to format this bette with lpads and r pads so there aren't that mcuh hard coded spaces everywhere but no time to do so
      dbms_output.put_line('    Station: ' || r_station.stname || ', Azahl der Tage zw erster und letzter Aufzeichnung: ' || tage);
      dbms_output.put_line('        ' || rpad('Temp.', 12) || 'SO');
      dbms_output.put_line(LPAD(' ', 8)||RPAD('-',15,'-'));
      stnr := r_station.stnr;
      SELECT MAX(c) INTO maxTemp FROM aufzeichnungen WHERE stnr = r_station.stnr;
      SELECT AVG(so) INTO avgSO FROM aufzeichnungen WHERE stnr = r_station.stnr;
    END IF;
    IF r_station.c = maxTemp THEN
      dbms_output.put('        ' || rpad(r_station.c || '(*)', 12));
    ELSE
      dbms_output.put('        ' || rpad(r_station.c, 12));
    END IF;
    IF r_station.so > avgSO THEN
      dbms_output.put_line('        ' || rpad(r_station.so || '(*)', 12));
    ELSE
      dbms_output.put_line('        ' || rpad(r_station.so, 12));
    END IF;
  END LOOP;
END STATION_TEMP_SO;
