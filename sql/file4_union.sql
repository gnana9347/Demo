/*select first_name from customers
union 
select name from shippers

select customer_id from customers
union all
select customer_id from orders 

select  order_id,order_date,'active' as status
from orders
where order_date<='2024-01-01'  

select customer_id,first_name,points,'Bronze' as medal
from customers
where points<2000
union
select customer_id,first_name,points,'Silver' as medal
from customers
where points between 2000 and 3000
union
select customer_id,first_name,points,'Gold' as medal
from customers
where points>3000
order by points desc;

select *
from customers 

insert into customers(customer_id,first_name,last_name,birth_date,phone,address,city,state,points)
values(11,'tharun','tripurala','2001-01-01',904433909334,'tirupati','tirupathi','Na',4344)  */

insert into customers(first_name,last_name,address,city,state)
values('sai','tripurala','tirupathi','tirupati','Ap')

insert into customers
values(13,'prasad','arthala','1995-01-01',9044339334,'jayanthi nagar','karnataka','ka',5344)