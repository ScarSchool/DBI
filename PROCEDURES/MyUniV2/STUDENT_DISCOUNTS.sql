CREATE OR REPLACE PROCEDURE STUDENT_DISCOUNTS(
    r_10perc_boundary COURSES.COSTS%TYPE := 50,
    r_5perc_boundary COURSES.COSTS%TYPE  := 20 )
AS
  r_discount_amount INTEGER;
BEGIN
  FOR student IN
  -- Selects only students that are eligible to even get a discount
  (SELECT first_name, last_name, STUDENTS.COURSE, SUM(COURSES.COSTS) costsSum
  FROM STUDENTS 
  INNER JOIN COURSES ON students.course = courses.course AND STUDENTS.COURSELEVEL = courses.courselevel
  WHERE 
  paid = 'Y'
  and grade >= 50
  GROUP BY STUDENTS.COURSE, first_name, last_name
  ORDER BY costsSum DESC,
    last_name,
    first_name
  )
  LOOP
  IF student.costsSum >= r_5perc_boundary THEN
    IF student.costsSum    >= r_10perc_boundary THEN
      r_discount_amount := 10;
    ELSE
      if( r_discount_amount = 10) THEN
        SYS.DBMS_OUTPUT.put_Line(LPAD(' ' || r_10perc_boundary, 20, '=') || RPAD(' ', 20, '='));
      END IF;
      r_discount_amount := 5;
    END IF;
    
    --Update students with according discount amount
    UPDATE students
    SET discount              = r_discount_amount
    WHERE students.first_name = student.first_name
    AND students.last_name    = student.last_name
    AND students.course       = student.course;
   
    SYS.DBMS_OUTPUT.put_Line(RPAD(student.FIRST_NAME || ' ' || 
    student.LAST_NAME , 20, '.') || ' ' || 
    RPAD(student.course, 20, '.') || ' ' || 
    LPAD('€ ' || student.costsSum,12, '.') || ' =>' || 
    LPAD(r_discount_amount || '%', 5, ' '));
    END IF;
  END LOOP;
  SYS.DBMS_OUTPUT.put_Line(LPAD(' ' || r_5perc_boundary, 20, '=') || RPAD(' ', 20, '='));
END STUDENT_DISCOUNTS;