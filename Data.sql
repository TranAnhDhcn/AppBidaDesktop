CREATE DATABASE QuanLyQuanBiDa
GO

USE QuanLyQuanBiDa
GO

-- Food
-- Table
-- FoodCategory
-- Account
-- Client
-- Bill
-- BillInfo
-- Calendar

CREATE TABLE TableBida
(
	id INT identity PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa có tên',
	classification NVARCHAR(100) NOT NULL DEFAULT N'Nom',-- Nom || Vip
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống',	-- Trống || Có người
	active INT NOT NULL DEFAULT 1
)
GO

CREATE TABLE PriceBidaHour
(
	tableNom float default 10,
	tableVip float default 10
)
GO

CREATE TABLE Account
(
	id int identity primary key,
	email NVARCHAR(100) not null,
	name NVARCHAR(100) NOT NULL DEFAULT N'chưa có tên',	
	sex int not null default 0, ---0 là Nam , 1 là Nữ
	PassWord NVARCHAR(1000) NOT NULL DEFAULT 0,
	phone int not null default 0,
	type INT NOT NULL  DEFAULT 0, -- 1: admin && 0: staff
	dateW datetime not null,
	status int not null default 0,
	idCard nvarchar(100) default 0000,
	image image NULL,
	active int not null default 1
)
GO


CREATE TABLE Client
(
	id int identity primary key,
	userName NVARCHAR(100) NOT NULL DEFAULT N'chưa có tên',	
	phone int not null default 0,
	point int not null default N'0',
	maLoaiKH NVARCHAR(100) NOT NULL DEFAULT N'NOM'	-- NOM || VIP
)
GO


CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	active int not null default 1
)
GO


CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	timeIn datetime  null,
	salary int null default 0,
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0,
	image image NULL,
	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO



CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATETIME NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATETIME,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0, -- 1: đã thanh toán && 0: chưa thanh toán
	point int,
	discount int default 0,
	totalPrice float,
	idAccount INT NOT NULL,
	FOREIGN KEY (idTable) REFERENCES dbo.TableBida(id),
	FOREIGN KEY (idAccount) REFERENCES dbo.Account(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0
	
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO

create table Calenar
(
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	TimeStart datetime not null,
	TimeEnd datetime not null,
	idAccount int not null,
	FOREIGN KEY (idAccount) REFERENCES dbo.Account(id)
)
GO

create table CoSo
(
	id nvarchar (50) not null,
	Username nvarchar (50) not null,
	Password nvarchar(128) not null,
	Email nvarchar(100) not null,
	TenCoSo nvarchar (50) not null,
	DiaChi nvarchar (100) not null,
	NgayHoatDong datetime not null,
	NgayThanhToan datetime not null,
	SoDienThoai int not null default N'0',
	TenChuCoSo nvarchar (50) not null,
	primary key (id)
)
GO

create table QuanlyCoSo
(
	MaQlyCoSo nvarchar(50) not null,
	IdCoSo nvarchar (50) not null,
	Status nvarchar (100) not null, -- Hoạt động, Không hoạt động
	primary key (MaQlyCoSo),
	foreign key (IdCoSo) references dbo.CoSo(id) 
)
GO

CREATE PROC USP_GetAccountByUserName
@userName nvarchar(100)
AS 
BEGIN
	SELECT * FROM dbo.Account WHERE name = @userName
END
GO

CREATE PROC USP_Login
@passWord nvarchar(100), @email nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE email = @email  AND PassWord = @passWord AND active = 1
END
GO

CREATE PROC USP_LoginAndroid
@passWord nvarchar(100), @email nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE type = 1 AND email = @email  AND PassWord = @passWord AND active = 1
END
GO


CREATE PROC USP_LoginCard
@idCard nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE idCard = @idCard AND active = 1
END
GO


CREATE PROC USP_InsertTable
AS
BEGIN
	DECLARE @i INT = 1
	DECLARE @maxID INT
	-- Get the maximum ID value from TableBida
	SELECT @maxID = MAX(id) FROM TableBida
	BEGIN
		IF @maxID IS NULL
		BEGIN
			INSERT INTO dbo.TableBida (id, name) VALUES (@i, N'Bàn ' + CAST(@i AS NVARCHAR(100)))
			SET @i += 1 
		END
		ELSE
		BEGIN
			INSERT INTO dbo.TableBida (id, name) VALUES (@i + @maxID, N'Bàn ' + CAST(@maxID + @i AS NVARCHAR(100)))
			SET @i += 1 
		END
	END 
END
GO



CREATE PROC USP_GetTableList
AS SELECT * FROM dbo.TableBida where active = 1
GO


CREATE PROC USP_GetTableListNom
AS SELECT * FROM dbo.TableBida where classification = 'Nom' and active = 1
GO

CREATE PROC USP_GetTableListVip
AS SELECT * FROM dbo.TableBida where classification = 'Vip' and active = 1
GO




INSERT dbo.FoodCategory
        ( name )
VALUES  ( N'Hải sản'  -- name - nvarchar(100)
          )
INSERT dbo.FoodCategory
        ( name )
VALUES  ( N'Nông sản' )
INSERT dbo.FoodCategory
        ( name )
VALUES  ( N'Lâm sản' )
INSERT dbo.FoodCategory
        ( name )
VALUES  ( N'Sản sản' )
INSERT dbo.FoodCategory
        ( name )
VALUES  ( N'Nước' )
go

select*from FoodCategory

-- thêm món ăn
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'Mực một nắng nước sa tế', -- name - nvarchar(100)
          1, -- idCategory - int
          120000,
		  10,GETDATE())
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'Nghêu hấp xả', 1, 50000,10,GETDATE())
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'Dú dê nướng sữa', 2, 60000,10,GETDATE())
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'Heo rừng nướng muối ớt', 3, 75000,10,GETDATE())
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'Cơm chiên mushi', 4, 999999,10,GETDATE())
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'7Up', 5, 15000,10,GETDATE())
INSERT dbo.Food
        ( name, idCategory, price, salary, timeIn )
VALUES  ( N'Cafe', 5, 12000,12,GETDATE())
GO

Create PROC USP_InsertBill
@dateCheckIn DateTime, @idTable INT , @idAccount INT 
AS
BEGIN
	INSERT Bill(DateCheckIn,DateCheckOut, idTable, status, point, discount, totalPrice, idAccount)
	VALUES (@DateCheckIn,NULL, @idTable, 0, 0, 0, 0, @idAccount)
END
GO


CREATE PROC USP_InsertBillInfo
@idBill INT, @idFood INT, @count INT
AS
BEGIN

	DECLARE @isExitsBillInfo INT
	DECLARE @foodCount INT = 1
	
	SELECT @isExitsBillInfo = id, @foodCount = b.count 
	FROM dbo.BillInfo AS b 
	WHERE idBill = @idBill AND idFood = @idFood

	IF (@isExitsBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF (@newCount > 0)
			UPDATE dbo.BillInfo	SET count = @foodCount + @count WHERE idFood = @idFood
		ELSE
			DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	END
	ELSE
	BEGIN
		INSERT	dbo.BillInfo
        ( idBill, idFood, count )
		VALUES  ( @idBill, -- idBill - int
          @idFood, -- idFood - int
          @count  -- count - int
          )
	END
END
GO

CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = idBill FROM Inserted
	
	DECLARE @idTable INT
	
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status = 0	
	
	DECLARE @count INT
	SELECT @count = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idBill
	
	IF (@count > 0)
	BEGIN
	
		PRINT @idTable
		PRINT @idBill
		PRINT @count
		
		UPDATE dbo.TableBida SET status = N'Có người' WHERE id = @idTable		
		
	END		
	ELSE
	BEGIN
	PRINT @idTable
		PRINT @idBill
		PRINT @count
	UPDATE dbo.TableBida SET status = N'Trống' WHERE id = @idTable	
	end
	
END
GO

CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = id FROM Inserted	
	
	DECLARE @idTable INT
	
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	
	DECLARE @count int = 0
	
	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0
	
	IF (@count = 0)
		UPDATE dbo.TableBida SET status = N'Trống' WHERE id = @idTable
END
GO

CREATE PROC USP_SwitchTabel
@idTable1 INT, @idTable2 int
AS BEGIN

	DECLARE @idFirstBill int
	DECLARE @idSeconrdBill INT
	
	DECLARE @isFirstTablEmty INT = 1
	DECLARE @isSecondTablEmty INT = 1
	
	
	SELECT @idSeconrdBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0
	
	PRINT @idFirstBill
	PRINT @idSeconrdBill
	PRINT '-----------'
	
	IF (@idFirstBill IS NULL)
	BEGIN
		PRINT '0000001'
		INSERT dbo.Bill
		        ( DateCheckIn ,
		          DateCheckOut ,
		          idTable ,
		          status
		        )
		VALUES  ( GETDATE() , -- DateCheckIn - date
		          NULL , -- DateCheckOut - date
		          @idTable1 , -- idTable - int
		          0  -- status - int
		        )
		        
		SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0
		
	END
	
	SELECT @isFirstTablEmty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idFirstBill
	
	PRINT @idFirstBill
	PRINT @idSeconrdBill
	PRINT '-----------'
	
	IF (@idSeconrdBill IS NULL)
	BEGIN
		PRINT '0000002'
		INSERT dbo.Bill
		        ( DateCheckIn ,
		          DateCheckOut ,
		          idTable ,
		          status
		        )
		VALUES  ( GETDATE() , -- DateCheckIn - date
		          NULL , -- DateCheckOut - date
		          @idTable2 , -- idTable - int
		          0  -- status - int
		        )
		SELECT @idSeconrdBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
		
	END
	
	SELECT @isSecondTablEmty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idSeconrdBill
	
	PRINT @idFirstBill
	PRINT @idSeconrdBill
	PRINT '-----------'

	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSeconrdBill
	
	UPDATE dbo.BillInfo SET idBill = @idSeconrdBill WHERE idBill = @idFirstBill
	
	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id IN (SELECT * FROM IDBillInfoTable)
	
	DROP TABLE IDBillInfoTable
	
	IF (@isFirstTablEmty = 0)
		UPDATE dbo.TableBida SET status = N'Trống' WHERE id = @idTable2
		
	IF (@isSecondTablEmty= 0)
		UPDATE dbo.TableBida SET status = N'Trống' WHERE id = @idTable1
END
GO

CREATE PROC USP_SwapTable 
@Table1 INT, @Table2 int
AS BEGIN

    -- Swap table numbers
    UPDATE dbo.Bill SET idTable = CASE 
        WHEN idTable = @Table1 THEN @Table2 
        WHEN idTable = @Table2 THEN @Table1
        ELSE idTable
    END

    -- Swap status
    UPDATE dbo.TableBida SET status = CASE 
        WHEN id = @Table1 THEN (SELECT Status FROM dbo.TableBida WHERE id = @Table2)
        WHEN id = @Table2 THEN (SELECT Status FROM dbo.TableBida WHERE id = @Table1)
        ELSE status
    END
END
go

CREATE PROC USP_GetListBillByDate
@checkIn date, @checkOut date
AS 
BEGIN
	SELECT t.name AS [Bàn],  b.DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá], totalPrice AS [Doanh thu], a.name AS [Nhân viên]
	FROM Bill AS b, TableBida AS t, Account as a
	WHERE DateCheckIn >= @checkIn AND (DateCheckOut < DATEADD(day, 1, @checkOut) OR DateCheckOut IS NULL) AND b.status = 1
	AND t.id = b.idTable
	AND a.id = b.idAccount
END
GO 


--khong xai
CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS 
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT
	SELECT @idBillInfo = id, @idBill = Deleted.idBill FROM Deleted
	
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	
	DECLARE @count INT = 0
	
	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id = @idBill AND b.status = 0
	
END
GO 

--CREATE PROC USP_GetListFood
--AS 
--BEGIN
--	Select id  AS [Mã hàng], name AS [Tên hàng],CONVERT(date, timeIn, 101) AS [Ngày nhập kho] ,salary AS [Số lượng],idCategory AS [Danh mục], price AS [Giá thành]
--	from food
--END
--GO

--tim kiếm chữ có dấu

if exists (select * from sys.objects where name = 'Search')
	drop function Search
go
create FUNCTION [dbo].[Search] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput End
go

--CREATE PROC USP_SearchGetListFood
--@searchName NVARCHAR(100)
--AS 
--BEGIN
--	Select id  AS [Mã hàng], name AS [Tên hàng],CONVERT(date, timeIn, 101) AS [Ngày nhập kho] ,salary AS [Số lượng],idCategory AS [Danh mục], price AS [Giá thành]
--	FROM dbo.Food WHERE dbo.fuConvertToUnsign1(name) LIKE N'%' + dbo.fuConvertToUnsign1(@searchName) + '%'
--END
--GO

CREATE PROC USP_InsertAdminAcc
 @email NVARCHAR(100),
    @name NVARCHAR(100),
    @sex INT,
    @password NVARCHAR(1000),
    @phone INT,
	@type int,
	@image image,
	@idCard nvarchar(100)
AS
BEGIN
    INSERT INTO Account (email, name, sex, PassWord, phone, Type ,dateW, image, idCard)
    VALUES (@email, @name, @sex, @password, @phone, @type, GETDATE(), @image, @idCard)
END
GO


CREATE PROC USP_UpdateAccountStaff
    @email NVARCHAR(100),
    @name NVARCHAR(100),
    @phone INT,
	@id INT
AS
BEGIN
    UPDATE Account SET name = @name, phone = @phone , email = @email where id = @id;
END
GO




CREATE PROC USP_UpdatePassAccount
    @email NVARCHAR(100),
    @pass NVARCHAR(1000)
AS
BEGIN
    UPDATE Account SET PassWord = @pass where email = @email;
END
GO

CREATE PROC USP_UpdateAccount
    @name NVARCHAR(100),
    @email NVARCHAR(100),
    @phone INT,
    @pass NVARCHAR(1000),
    @image IMAGE,
    @id INT
AS
BEGIN
    UPDATE Account
    SET name = @name, email = @email,  phone = @phone,  PassWord = @pass, image = @image  WHERE id = @id;
END
GO

CREATE PROC USP_UpdateInFoAccount
    @name NVARCHAR(100),
    @email NVARCHAR(100),
    @phone INT,
    @image IMAGE,
    @id INT
AS
BEGIN
    UPDATE Account
    SET name = @name, email = @email,  phone = @phone, image = @image WHERE id = @id;
END
GO

CREATE PROC USP_UpdateInFoPassAccount
    @pass NVARCHAR(1000),
    @id INT
AS
BEGIN
    UPDATE Account SET PassWord = @pass WHERE id = @id;
END
GO




CREATE PROC USP_UpdateInFoIdCard
    @idCard NVARCHAR(100),
    @id INT
AS
BEGIN
    UPDATE Account SET idCard = @idCard WHERE id = @id;
END
GO