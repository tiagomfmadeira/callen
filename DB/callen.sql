
/* Tiago Madeira 76321 - Diogo Duarte 77645 */

--Callen

/* TABLE CREATION */

CREATE SCHEMA CALLEN;
go


CREATE TABLE G_CALLEN.ARCHIVE(
	Archive_ID INT IDENTITY(1,1) PRIMARY KEY, 
	Code VARCHAR(50) NOT NULL, 	--codigo na pasta fisica, primary key??
	Theme_Descr VARCHAR(50),
);
go

--Theme atribute - folder
CREATE TABLE G_CALLEN.ITEM(
	Item_ID INT IDENTITY(1,1) PRIMARY KEY,
	Item_Name nvarchar(255) NOT NULL,
	Item_Descr nvarchar(255) NOT NULL,
	Item_Year VARCHAR(20) NOT NULL,
	Other nvarchar(255),
    Collec nvarchar(50)
);
go

CREATE TABLE G_CALLEN.INST(
	Inst_Number INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Item_ID INT REFERENCES G_CALLEN.ITEM(Item_ID),
	Note nvarchar(255) NOT NULL,
	Favorite Bit,
	Inst_PicPath VARCHAR(255),
	Date_Insert DATETIME,
	Date_Mod DATETIME,
	Date_View DATETIME,
	State Bit, -- 0 - Belongs to Colection | 1 - Gifted
	Archive INT REFERENCES G_CALLEN.ARCHIVE(Archive_ID)
);
go