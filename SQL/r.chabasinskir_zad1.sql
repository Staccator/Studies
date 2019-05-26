
--2.1
--select p.ProductName from Products p
--where p.UnitsInStock = '0' and p.UnitsOnOrder = '0' and p.Discontinued = '0'
--order by p.ProductName



--2.2
--select distinct e.EmployeeID, e.LastName, p.ProductName from Employees e
--join Orders o on o.EmployeeID = e.EmployeeID
--join [Order Details] od on od.OrderID = o.OrderID
--join Products p on p.ProductID = od.ProductID
--where p.ProductName = 'Camembert Pierrot'
--and od.Quantity 
-->
--(select avg(Quantity) from [Order Details] od
--join Products p on p.ProductID = od.ProductID
--where p.ProductName = 'Camembert Pierrot');

--2.3
--select * from orders o1
--where o1.OrderID in  (select o.OrderID from orders o
--join [Order Details] od on od.OrderID = o.OrderID
--group by o.OrderID
--having count(od.ProductID) <4)
--order by o1.OrderDate desc

--2.4
--select e.LastName from Employees e
--where e.EmployeeID in (select e.EmployeeID from Employees e
--join Orders o on o.EmployeeID = e.EmployeeID
--join customers c on o.CustomerID = c.CustomerID
--where year(o.ShippedDate) = '1997' and month(o.shippeddate) = 12
--group by e.EmployeeID 
--having count(o.Orderid) > 2)

--and 'France' in (select c1.Country from Orders o1
--join customers c1 on o1.CustomerID = c1.CustomerID
--where year(o1.ShippedDate) = '1997' and month(o1.shippeddate) = 12 and o1.EmployeeID = e.employeeid
--) 
--order by e.LastName

--2.5

--select c.CustomerID  from Customers c
--where (select count(o1.orderid) from orders o1
--join customers  c1 on o1.CustomerID = c1.CustomerID
--where year(o1.OrderDate) = '1997' and c1.CustomerID = c.customerid) 
--> 
--(select count(o1.orderid) from orders o1
--join customers  c1 on o1.CustomerID = c1.CustomerID
--where year(o1.OrderDate) = '1998' and c1.CustomerID = c.customerid)
--and (select count(o1.orderid) from orders o1
--join customers  c1 on o1.CustomerID = c1.CustomerID
--where year(o1.OrderDate) = '1997' and c1.CustomerID = c.customerid) > 0 
--and
--(select count(o1.orderid) from orders o1
--join customers  c1 on o1.CustomerID = c1.CustomerID
--where year(o1.OrderDate) = '1998' and c1.CustomerID = c.customerid) > 0



--(select count(o1.orderid) from orders o1
--join customers  c1 on o1.CustomerID = c1.CustomerID
--where year(o1.OrderID) = '1997' and c1.CustomerID = c.customerid)
