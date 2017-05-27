-------- VIEWS --------
/* -- Returns Items info
CREATE VIEW G_CALLEN.ITEMS_INFO
AS
SELECT Favorite AS favourite,Inst_Number AS ID,Item_Name AS name,Item_Descr AS descr,Item_Year AS year,Theme_Descr AS theme,Code AS folder,Peer_name AS peer,Entity_Name AS sponsor 
    FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor 
        FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor 
            FROM(SELECT Favorite, Inst_Number, Item_ID, Code, Peer, Theme_Descr 
                FROM(SELECT Item_ID, Inst_Number, Favorite, Arquive, Peer 
                    FROM G_Callen.INST) AS IT 
                    JOIN(SELECT * 
						FROM G_Callen.ARQUIVE) AS A 
                    ON IT.Arquive = A.Arquive_ID) AS IA 
                JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor 
					FROM G_Callen.ITEM) AS I 
                ON IA.Item_ID = I.Item_ID) AS IAI 
            JOIN(SELECT Entity_ID, Entity_Name 
				FROM G_Callen.ENTITY) AS E 
            ON IAI.Peer = E.Entity_ID) AS IAIE 
        JOIN(SELECT Entity_ID, Entity_Name 
            FROM G_Callen.ENTITY) AS EE 
        ON IAIE.Sponsor = EE.Entity_ID;
;
go

-- Returns Item ID + Name by Insert
CREATE VIEW G_Callen.LastInsertItems
AS
	SELECT TOP 50 I.Item_ID, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I 
		 JOIN (SELECT Item_ID, Date_Insert FROM G_Callen.INST) AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_Insert DESC;
;
go

-- Returns Item ID + Name by Modify
CREATE VIEW G_Callen.LastModItems
AS
	SELECT TOP 50 I.Item_ID, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I 
		 JOIN (SELECT Item_ID, Date_Mod FROM G_Callen.INST) AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_Mod DESC;
;
go

-- Returns Item ID + Name by View
CREATE VIEW G_Callen.LastVisItems
AS
	SELECT TOP 50 I.Item_ID, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I 
		 JOIN (SELECT Item_ID, Date_View FROM G_Callen.INST) AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_View DESC;
;
go
-- returns list of favourite items
CREATE VIEW G_Callen.FavouriteItems
AS
	SELECT TOP 50 Item_ID, Item_Name
	FROM G_Callen.ITEM
	WHERE Item_ID IN (SELECT Item_ID FROM G_Callen.INST WHERE Favorite = '1');
;
go

-- returns list of Items Id, name and pic_path
CREATE VIEW G_Callen.ITEMS_PIC_MODE
AS
	SELECT I.Item_ID, Item_Name, Inst_PicPath
	FROM (SELECT Item_Name, Item_ID 
		  FROM G_Callen.ITEM ) AS I
		  JOIN (SELECT Item_ID, Inst_PicPath
				FROM G_Callen.INST
				WHERE NOT ISNULL(Inst_PicPath,'') = '') AS IT
		  ON I.Item_ID = IT.Item_ID;
go

---------- PROCEDURES ----------

-- GETS SPECIFIC ITEM INFO
CREATE PROCEDURE G_Callen.GET_ITEM_INFO @ItemID INT
AS
	SELECT Favorite AS favourite,Inst_Number AS ID,Item_Name AS name,Item_Descr AS descr,Item_Year AS year,Theme_Descr AS theme,Code AS folder,Peer_name AS peer,Entity_Name AS sponsor ,Note AS note, Inst_PicPath AS img_path
    FROM(SELECT Favorite,Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor ,Note, Inst_PicPath
        FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor ,Note, Inst_PicPath
            FROM(SELECT Favorite, Inst_Number, Item_ID, Code, Peer, Theme_Descr ,Note, Inst_PicPath
                FROM(SELECT Item_ID, Inst_Number, Favorite, Arquive, Peer , Note, Inst_PicPath 
                    FROM G_Callen.INST
					WHERE Item_ID = @ItemID) AS IT 
                    JOIN(SELECT * 
						FROM G_Callen.ARQUIVE) AS A 
                    ON IT.Arquive = A.Arquive_ID) AS IA 
                JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor 
					FROM G_Callen.ITEM) AS I 
                ON IA.Item_ID = I.Item_ID) AS IAI 
            JOIN(SELECT Entity_ID, Entity_Name 
				FROM G_Callen.ENTITY) AS E 
            ON IAI.Peer = E.Entity_ID) AS IAIE 
        JOIN(SELECT Entity_ID, Entity_Name 
            FROM G_Callen.ENTITY) AS EE 
        ON IAIE.Sponsor = EE.Entity_ID;

	UPDATE G_Callen.INST SET Date_View = GETDATE() WHERE Inst_Number = @ItemID; -- "Trigger"
go

-- sets an inst to favourite
CREATE PROCEDURE G_Callen.SET_FAVOURITE @ItemID INT
AS
	UPDATE G_Callen.INST SET Favorite = ~Favorite WHERE Inst_Number = @ItemID;
GO

-- Dá update as infos de um item ESTÁ MAL (A DAR UPDATE AO ITEM TAMBEM)
CREATE PROCEDURE G_Callen.UPDATE_INST_INFO @InstID INT, @ItemName VARCHAR(100), @ItemYear VARCHAR(10), @ItemOther VARCHAR(255), @ItemDesc VARCHAR(255)
AS
	DECLARE @itemID as INT;
	SELECT @itemID = Item_ID FROM G_Callen.INST WHERE Inst_Number = @InstID;

	UPDATE G_Callen.ITEM 
	SET Item_Name =  @ItemName , Item_Descr = @ItemDesc, Item_Year = @ItemYear
	WHERE Item_ID = @ItemID;

	UPDATE G_Callen.INST
	SET Note = @ItemOther WHERE Inst_Number = @InstID;

	UPDATE G_Callen.INST SET Date_Mod = GETDATE() WHERE Inst_Number = @ItemID; -- "Trigger"
GO

-- Cria um novo folder
CREATE PROCEDURE G_Callen.CREATE_FOLDER @Code VARCHAR(50), @Theme VARCHAR(50)
AS
	INSERT INTO G_Callen.ARQUIVE(Code, Theme_Descr) VALUES(@Code,@Theme)
GO

-- Create Address
CREATE PROCEDURE G_Callen.CREATE_ADDRESS @Entity INT, @Street VARCHAR(150),	@City VARCHAR(50),@State VARCHAR(50),
													@Country VARCHAR(50), @PostalCode VARCHAR(50)
AS
	DECLARE @ID AS INT;

	INSERT INTO G_CALLEN.ADDRESS(Street, City, State, Country, PostalCode) 
				VALUES (@Street, @City, @State, @Country, @PostalCode) 
	
	SELECT @ID = IDENT_CURRENT('G_CALLEN.ADDRESS')

	INSERT INTO G_Callen.ENTITYADRESS(Entity, Address) VALUES(@Entity,@ID);
GO

-- Create entity - returns Entity id
CREATE PROCEDURE G_Callen.CREATE_ENTITY @Name VARCHAR(50), @Email VARCHAR(150), @Phone VARCHAR(15)
AS
	INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
				VALUES (@Name, @Email, @Phone);

	RETURN IDENT_CURRENT('G_CALLEN.ADDRESS')
GO

CREATE PROCEDURE G_Callen.CREATE_PEER @Name VARCHAR(50), @Email VARCHAR(150), @Phone VARCHAR(15)
AS
	DECLARE @ENTITY_ID AS INT;

	INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
				VALUES (@Name, @Email, @Phone);

	SELECT @ENTITY_ID = IDENT_CURRENT('G_CALLEN.ENTITY');

	INSERT INTO G_Callen.PEER(Peer_ID,QuantityOffered) VALUES (@ENTITY_ID,0);

	RETURN @ENTITY_ID;
GO

CREATE PROCEDURE G_Callen.CREATE_SPONSOR @Name VARCHAR(50), @Email VARCHAR(150), @Phone VARCHAR(15), @WebSite VARCHAR(100)
AS
	DECLARE @ENTITY_ID AS INT;

	INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
				VALUES (@Name, @Email, @Phone);

	SELECT @ENTITY_ID = IDENT_CURRENT('G_CALLEN.ENTITY');

	INSERT INTO G_Callen.SPONSOR(Sponsor_ID,Website) VALUES (@ENTITY_ID,@WebSite);

	RETURN @ENTITY_ID
GO

-- OTHER & NOTE
CREATE PROCEDURE G_Callen.ADD_ITEM @Name VARCHAR(100), @Sponsor INT, @Peer INT, @Desc VARCHAR(255), @Year VARCHAR(10),
										@Folder INT, @Other VARCHAR(150), @Img_Path VARCHAR(255)
AS

	DECLARE @ITEM_ID AS INT;

	INSERT INTO G_Callen.ITEM(Item_Name,Item_Descr,Item_Year,Sponsor,Other,Type)
		VALUES(@Name,@Desc,@Year,@Sponsor,@Other,1);

	SELECT @ITEM_ID = IDENT_CURRENT('G_CALLEN.ITEM');

	INSERT INTO G_Callen.INST(Item_ID,Arquive,Peer,Inst_PicPath,Note,Date_Insert,Favorite)
		VALUES(@ITEM_ID,@Folder,@Peer,@Img_Path,'not real',GETDATE(),0);

	SELECT IDENT_CURRENT('G_Callen.INST');
GO
-------- TRIGGER --------

CREATE TRIGGER G_Callen.SET_IMG_PATH ON G_Callen.INST
AFTER INSERT
AS
	DECLARE @SUB_IMG_PATH AS VARCHAR(255);
	DECLARE @ID AS INT;


	SELECT @SUB_IMG_PATH = Inst_PicPath, @ID = Inst_Number FROM inserted;

	IF ISNULL(@SUB_IMG_PATH,'') = ''
		RETURN;
	ELSE
		UPDATE G_Callen.INST SET Inst_PicPath = CONCAT(@SUB_IMG_PATH,@ID) WHERE Inst_Number = @ID
GO*/