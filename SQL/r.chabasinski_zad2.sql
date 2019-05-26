---------------------------- ZADANIE 1 --------------------
--1)
create table biegi(
id_biegu int not null identity(1,1) primary key,
dystans int,
liczba_miejsc int
)

create table zespoly(
id int not null identity(1,1) primary key,
Nazwa varchar(100) 
)

create table zgloszenie(
id_zapisu int not null identity(1,1) primary key,
Imie_biegacza varchar(100),
Nazwisko_biegacza varchar(100),
PESEL_biegacza char(13),
Data_urodzenia_biegacza datetime,
Punkty_uzyskane_w_poprzednich_biegach int,
Bieg_1 int FOREIGN KEY REFERENCES biegi(id_biegu),
Bieg_2 int FOREIGN KEY REFERENCES biegi(id_biegu),
Bieg_3 int FOREIGN KEY REFERENCES biegi(id_biegu),
Przyznany_bieg int null FOREIGN KEY REFERENCES biegi(id_biegu),
Zespol int FOREIGN KEY REFERENCES zespoly(id),
Preferowany_dystas_biegu int
)
--2)
alter table biegi
add liczba_zapisanych int
--3)
insert into biegi values
(10,8,4),
(9,6,5)
----4)
insert into zespoly values
('mini'),
('eiti')

insert into zgloszenie values
('Andrzej','Stachowski' , '1234512345123' , DATEFROMPARTS(1997,3,3),3,1,2,null,1, 1 ,10  ),
('Tomasz','Skladowy' , '5432154321543' , DATEFROMPARTS(1998,4,4),3,1,2,null,2, 2 ,9  )
--5)
alter table zespoly
add liczba_zawodnikow int

update zespoly set
liczba_zawodnikow = 
(select count(id_zapisu) from zgloszenie z
where z.zespol = zespoly.id
group by z.id_zapisu
)

--6)
update biegi set
liczba_zapisanych = 
(select count(id_zapisu) from zgloszenie z
where z.przyznany_bieg = biegi.id_biegu
group by z.id_zapisu
)
---------------------------- ZADANIE 2--------------------
create procedure procDodajBieg @preferowany_dystans_biegu int  as
begin
	DECLARE @id_zapisu int
	declare @emp_id int
	declare @bieg1 int
	declare @bieg2 int
	declare @bieg3 int
	declare @liczba_miejsc int
	declare @liczba_zapisanych int

	DECLARE zgloszenieiter CURSOR LOCAL FOR SELECT id_zapisu,Bieg_1,Bieg_2,Bieg_3 from zgloszenie where Przyznany_bieg is null and Preferowany_dystas_biegu = @preferowany_dystans_biegu order by Punkty_uzyskane_w_poprzednich_biegach desc
	open zgloszenieiter
	FETCH NEXT FROM zgloszenieiter INTO @id_zapisu,@bieg1,@bieg2,@bieg3
	WHILE @@FETCH_STATUS=0
	BEGIN
		set @liczba_zapisanych=(select liczba_zapisanych from biegi where id_biegu = @bieg1)
		set @liczba_miejsc=(select liczba_miejsc from biegi where id_biegu = @bieg1)
		if(@liczba_zapisanych < @liczba_miejsc)
		begin
			update zgloszenie set
			Przyznany_bieg = @bieg1
			where  zgloszenie.id_zapisu = @id_zapisu

			update biegi set
			liczba_zapisanych = @liczba_zapisanych + 1
			where biegi.id_biegu = @bieg1
		end

		else
		begin
			set @liczba_zapisanych=(select liczba_zapisanych from biegi where id_biegu = @bieg2)
			set @liczba_miejsc=(select liczba_miejsc from biegi where id_biegu = @bieg2)
			if(@liczba_zapisanych < @liczba_miejsc)
			begin
				update zgloszenie set
				Przyznany_bieg = @bieg2
				where  zgloszenie.id_zapisu = @id_zapisu

				update biegi set
				liczba_zapisanych = @liczba_zapisanych + 1
				where biegi.id_biegu = @bieg2
			end
		else 
		begin 
			set @liczba_zapisanych=(select liczba_zapisanych from biegi where id_biegu = @bieg3)
			set @liczba_miejsc=(select liczba_miejsc from biegi where id_biegu = @bieg3)
			if(@liczba_zapisanych < @liczba_miejsc)
			begin
			update zgloszenie set
			Przyznany_bieg = @bieg3
			where  zgloszenie.id_zapisu = @id_zapisu

			update biegi set
			liczba_zapisanych = @liczba_zapisanych + 1
			where biegi.id_biegu = @bieg3
			end
		end

		end


	FETCH NEXT FROM zgloszenieiter INTO @id_zapisu,@bieg1,@bieg2,@bieg3
	END
close zgloszenieiter
deallocate zgloszenieiter
end
go  
------------ TEST ETAPU 2 ----------
insert into zgloszenie values
('Andrj','Stachski' , '1234512345122' , DATEFROMPARTS(1997,3,3),3,1,2,1,null, 1 ,10  ),
('Tomasz','Skladowy' , '5432154321542' , DATEFROMPARTS(1998,4,4),3,1,2,1,null, 2 ,9  )
exec procdodajbieg 9
select * from biegi
select * from zgloszenie
----------ZADANIE 3-----------------

create trigger trigDodajBieg on biegi after insert as
begin
	declare @id_biegu int
	declare @dystans int
	declare @liczba_miejsc int
	declare @liczba_zapisanych int
	declare @id_zapisu int

	DECLARE biegiter CURSOR LOCAL FOR SELECT id_biegu,dystans,liczba_miejsc,liczba_zapisanych FROM inserted
	open biegiter
	FETCH NEXT FROM biegiter INTO @id_biegu,@dystans,@liczba_miejsc,@liczba_zapisanych
	WHILE @@FETCH_STATUS=0
	BEGIN
			DECLARE zgloszenieiter CURSOR LOCAL FOR SELECT id_zapisu from zgloszenie where Przyznany_bieg is null and Preferowany_dystas_biegu = @dystans order by Punkty_uzyskane_w_poprzednich_biegach desc
			open zgloszenieiter
			FETCH NEXT FROM zgloszenieiter INTO @id_zapisu
			WHILE @@FETCH_STATUS=0
			BEGIN
				if(@liczba_zapisanych < @liczba_miejsc) -- daj bieg
				begin
					update zgloszenie set
					Przyznany_bieg = @id_biegu
					where  zgloszenie.id_zapisu = @id_zapisu

					set @liczba_zapisanych = @liczba_zapisanych + 1

					update biegi set
					liczba_zapisanych = @liczba_zapisanych
					where biegi.id_biegu = @id_biegu
					
				end

			FETCH NEXT FROM zgloszenieiter INTO @id_zapisu
			END
			close zgloszenieiter
			deallocate zgloszenieiter
		
	FETCH NEXT FROM biegiter INTO @id_biegu,@dystans,@liczba_miejsc,@liczba_zapisanych
	END
close biegiter
deallocate biegiter
end
go


------------ TEST ETAPU 3 ----------
insert into zgloszenie values
('Anddfrj','Stachski' , '1234512345122' , DATEFROMPARTS(1997,3,3),3,1,2,1,null, 1 ,8  ),
('Tomdfsfasz','Skladowy' , '5432154321542' , DATEFROMPARTS(1998,4,4),3,1,2,1,null, 2 ,8  ),
('Tomdfsfasz','Skladowy' , '5432154321542' , DATEFROMPARTS(1998,4,4),3,1,2,1,null, 2 ,8  )
insert into biegi values (8,2,0)
select * from zgloszenie
select * from biegi
