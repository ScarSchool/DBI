create or replace PROCEDURE CAR_RACE AS 
  r_win_1 VARCHAR2(1) := '';
  r_win_2 VARCHAR2(1) := '';
  r_sec_diff VARCHAR2(2) := '';
  r_min_diff VARCHAR2(2) := '';
  
  r_loser_1 VARCHAR2(5) := '';
  r_loser_2 VARCHAR2(5) := '';
  
  CURSOR losers IS
    SELECT id FROM VEHICLES veh1 INNER JOIN races on veh1.id = races.vid1
      WHERE time1 = (SELECT min(time1) FROM VEHICLES veh2
                    INNER JOIN races rac1 ON veh2.id = rac1.vid1)
      OR time2 = (SELECT min(time2) FROM VEHICLES veh3
                INNER JOIN races rac2 ON veh3.id = rac2.vid1);
  
  CURSOR slowestVehicles IS
    SELECT id, time from vehicles veh1
      INNER JOIN races on races.vid1 = veh1.id or races.vid2 = veh1.id
      INNER JOIN ( 
      select v1.VTYPE, 
      MAX(CASE v1.id
        WHEN rac1.vid1 THEN
          rac1.time1
        WHEN rac1.vid2 THEN
          rac1.time2
        END) as time
      from VEHICLES v1 INNER JOIN RACES rac1 ON v1.ID = rac1.VID1 OR v1.ID = rac1.VID2 
      group by v1.VTYPE
    ) maxTime ON (CASE veh1.id
            WHEN races.vid1 THEN
              races.time1
            WHEN races.vid2 THEN
              races.time2
            END) = maxTime.time AND veh1.VTYPE = maxTime.vtype;

BEGIN
  SYS.DBMS_OUTPUT.put_Line(RPAD('vehicle-1' , 36, ' ') || RPAD('difference', 36, ' ') || RPAD('vehicle-2' , 36, ' ') );
  SYS.DBMS_OUTPUT.put_Line(RPAD('\' , 108, '='));

  FOR race IN (select 
VEHICLES.NAME NAME1,
VEHICLES.VTYPE VTYPE,
RACES.TIME1 TIME1,
RACES.TIME2 TIME2,
races.VID1,
races.VID2,
v2.NAME NAME2 from VEHICLES INNER JOIN RACES ON VEHICLES.ID = RACES.VID1 INNER JOIN vehicles v2 ON v2.ID = RACES.VID2)
  LOOP
    -- Reset the winners/losers
    r_win_1 := '';
    r_win_2 := '';
    r_loser_1 := '';
    r_loser_2 := '';
  
    FOR slowestVehicle in slowestVehicles
    LOOP
    IF (race.vid1 = slowestVehicle.id AND race.time1 = slowestVehicle.time) THEN
      r_loser_1 := 'LOSER';
    END IF;
    IF (race.vid2 = slowestVehicle.id AND race.time2 = slowestVehicle.time) THEN
      r_loser_2 := 'LOSER';
    END IF;
    END LOOP;
    
    
    -- Declare winner indicators
    if(race.time1 <= race.time2) THEN
     r_win_1 := '*';
    END IF;
    if(race.time1 >= race.time2) THEN
      r_win_2 := '*';
    END IF;
    
    -- Calculate differences
    r_min_diff := LPAD(FLOOR(abs(race.time1- race.time2) * 1440), 2, '0');
    r_sec_diff := LPAD(MOD(ROUND(abs(race.time1- race.time2) * 86400), 60), 2, '0');
    
    -- Output the races
    SYS.DBMS_OUTPUT.put_Line(RPAD(race.name1 || ' (' || TO_CHAR(race.time1, 'MI:SS') || ') ' || r_win_1 || r_loser_1 , 36, ' ') ||
    RPAD(r_min_diff || ':' || r_sec_diff, 36, ' ') ||
    RPAD(race.name2 || ' (' || TO_CHAR(race.time2, 'MI:SS') || ') ' || r_win_2 || r_loser_2 , 36, ' '));
  END LOOP;
END CAR_RACE;