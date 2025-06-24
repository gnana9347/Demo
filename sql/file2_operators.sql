-- use sql_invoicing;
/*SELECT client_id*2+1
FROM invoices
where client_id=5;  

select *
from invoices
where invoice_total<>100;

select *
from invoices
where invoice_total>100;

select *
from invoices
where invoice_total<100; 

select *
from invoices
where invoice_total between 100 and 150;

select *
from clients
where city like 's%';

select *
from clients
where city like '%o';

select *
from clients
where city like 's%o';

select *
from clients
where city like 'w___';

select *
from clients
where city like '___o';

select *
from clients
where city like '__C_';

select *
from clients
where city regexp '^s';

select *
from clients
where city regexp 'o$';

select *
from clients
where city regexp 'o$'  

select *
from clients 
where city regexp '[a-z]s';

select * 
from invoices 
where payment_date is null;

select *
from invoices 
where payment_date is not null;  

use sql_store;
select address
from customers
where address regexp 'an|en';*/

use sql_store;
select * 
from customers
order by customer_id 
limit 3 offset 10;