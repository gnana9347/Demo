create table order_archived 
select * from orders

select *
from order_archived

insert into order_archived
select *
from orders
where order_date<'2020-01-01'
