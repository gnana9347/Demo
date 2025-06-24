/*select *
from orders

insert into orders(customer_id,order_date,shipper_id)
values(11,'2024-02-09',5)

select last_insert_id() */

insert into orders(customer_id,order_date,shipper_id)
values(1,'2023-06-29',1);
insert into order_items
values(last_insert_id(),3,2,1.500);
