create database k7IT;
use k7it;
create table employees(employee_id int(10) not null unique primary key, emp_name varchar(33),subject varchar(33),students_id int(10) not null unique);
drop table employees


insert into employees
value(01,'keshavulu','java',201),
(02,'dhanyavani','ui',202),
(03,'vinod','database',203),
(04,'thulasi','testing',204)

create table student(employee_id int not null unique,student_name varchar(20) not null)
insert into student
value(01,'tharun'),
(02,'sai'),
(03,'kumar'),
(04,'mouni');


