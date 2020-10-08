create or replace PROCEDURE NOT_PAID
AS
  CURSOR students_with_dept
  IS
    SELECT first_name,
      last_name,
      SUM(COURSES.COSTS) AS total_debt
    FROM STUDENTS
    INNER JOIN COURSES
    ON STUDENTS.COURSE       = COURSES.COURSE
    AND STUDENTS.COURSELEVEL = COURSES.COURSELEVEL
    WHERE STUDENTS.PAID      = 'N'
    GROUP BY first_name,
      last_name
    ORDER BY STUDENTS.LAST_NAME,
      STUDENTS.FIRST_NAME;
  r_student_before students%ROWTYPE;
BEGIN
  FOR student IN students_with_dept
  LOOP
    SYS.DBMS_OUTPUT.put_Line(' ');
    SYS.DBMS_OUTPUT.put_Line(student.FIRST_NAME || ' ' || student.LAST_NAME || ' : € ' || student.total_debt);
    SYS.DBMS_OUTPUT.put_Line('------------------------------------------------------');
    FOR COURSE IN
    (SELECT students.course,
      STUDENTS.COURSELEVEL,
      COURSES.COSTS
    FROM students
    INNER JOIN COURSES
    ON students.COURSE        = COURSES.COURSE
    AND students.COURSELEVEL  = COURSES.COURSELEVEL
    WHERE students.first_name = student.first_name
    AND student.last_name     = students.last_name
    AND students.paid         = 'N'
    )
    LOOP
      SYS.DBMS_OUTPUT.put_Line(course.course || ' ' || course.courselevel || ' ' || course.costs || '€');
    END LOOP;
  END LOOP;
END NOT_PAID;