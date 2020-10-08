DROP TABLE students CASCADE CONSTRAINTS;
DROP TABLE courses CASCADE CONSTRAINTS;

CREATE TABLE students (
  first_name       VARCHAR2(20),
  last_name        VARCHAR2(20),
  course           VARCHAR2(20),
  courselevel	   VARCHAR2(1),	/*B...beginner, S...standard, X...expert */
  grade            INTEGER,		/*reached percentage in examination */
  paid			   VARCHAR2(1),	/*Y, N */
  discount    NUMBER(7,2)
  );

INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Scott', 'Smith','Computer Science', 'B', null, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Tina', 'Turner','Computer Science', 'B', null, 'N');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','Computer Science', 'B', 55, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Scott', 'Smith','German', 'B', 45, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Tina', 'Turner','German', 'B', 90, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Tina', 'Turner','German', 'S', null, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','German', 'B', 65, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Scott', 'Smith','History', 'B', 50, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Scott', 'Smith','History', 'S', 50, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Scott', 'Smith','History', 'X', null, 'N');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Tina', 'Turner','History', 'B', 84, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','History', 'B', 55, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','History', 'S', null, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','Mathematics', 'S', 65, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','Mathematics', 'B', 75, 'Y');
  INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Ben', 'Bluemchen','Mathematics', 'X', 50, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Bibi', 'Block','History', 'B', 55, 'Y');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Bibi', 'Block','History', 'S', null, 'N');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Bibi', 'Block','Mathematics', 'S', 65, 'N');
INSERT INTO students (first_name, last_name, course, courselevel, grade, paid)
  VALUES ( 'Bibi', 'Block','Mathematics', 'B', 75, 'Y');


CREATE TABLE courses (
  course           VARCHAR2(20),
  courselevel	   VARCHAR2(1),	/*B...beginner, S...standard, E...expert */
  costs			   INTEGER		/* per student */
);

INSERT INTO courses(course, courselevel, costs)
	VALUES ('Mathematics', 'B', 75);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('Mathematics', 'X', 85);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('Mathematics', 'S', 80);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('History', 'B', 45);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('History', 'X', 55);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('History', 'S', 50);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('German', 'B', 25);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('German', 'X', 85);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('German', 'S', 70);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('Computer Science', 'B', 175);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('Computer Science', 'X', 185);
INSERT INTO courses(course, courselevel, costs)
	VALUES ('Computer Science', 'S', 180);

COMMIT;