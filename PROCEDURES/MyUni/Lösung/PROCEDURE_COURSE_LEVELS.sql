create or replace PROCEDURE PROCEDURE_COURSE_LEVELS(
    student_fn  VARCHAR2,
    student_ln  VARCHAR2,
    course_name VARCHAR2 )
AS
  r_student students%ROWTYPE;
  r_courseCount        INTEGER;
  r_studentCount       INTEGER;
  exception_no_course  EXCEPTION;
  exception_no_student EXCEPTION;
  r_nextlevel          VARCHAR2(20);
BEGIN
  --select coursecount
  SELECT COUNT(*)
  INTO r_courseCount
  FROM COURSES
  WHERE course_name = COURSE;
  IF r_courseCount  = 0 THEN
    RAISE exception_no_course;
  END IF;
  
  
  --Select studentcountersings
  SELECT COUNT(*)
  INTO r_studentCount
  FROM STUDENTS
  WHERE student_fn  = FIRST_NAME
  AND student_ln    = LAST_NAME;
  IF r_studentCount = 0 THEN
    RAISE exception_no_student;
  END IF;
  
  --Select student with highest course
  SELECT *
  INTO r_student
  FROM students
  WHERE FIRST_NAME = student_fn
  AND LAST_NAME    = student_ln
  AND COURSE       = course_name
  AND COURSELEVEL  =
    (SELECT MAX(COURSELEVEL)
    FROM students
    WHERE FIRST_NAME = student_fn
    AND LAST_NAME    = student_ln
    AND COURSE       = course_name
    );
    
    
  SYS.DBMS_OUTPUT.put_line('Selected ' || r_student.first_name || ' ' || r_student.last_name || ' with course ' || r_student.course);
  IF NVL(r_student.grade, 0) >= 50 AND r_student.paid = 'Y' THEN
    CASE r_student.courselevel
    WHEN 'B' THEN
      SELECT 'S' INTO r_nextlevel FROM DUAL;
    WHEN 'S' THEN
      SELECT 'X' INTO r_nextlevel FROM DUAL;
    WHEN 'X' THEN
      SELECT 'finished' INTO r_nextlevel FROM DUAL;
    END CASE;
  ELSE
    SELECT r_student.courselevel into r_nextlevel from DUAL;
  END IF;
  
  SYS.DBMS_OUTPUT.put_Line('student ' || r_student.first_name || ' ' || r_student.last_name || ' ,' || r_student.course || ' => ' || r_nextlevel);
EXCEPTION
WHEN exception_no_student THEN
  SYS.DBMS_OUTPUT.put_Line('Could not find any student with name: ' || student_fn || ' ' || student_ln);
WHEN exception_no_course THEN
  SYS.DBMS_OUTPUT.put_Line('Could not find any course with name: ' || course_name);
END PROCEDURE_COURSE_LEVELS;