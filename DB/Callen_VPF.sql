---------- PROCEDURES ----------
/*
-- Returns Items info (used to fill combo box)
DROP PROCEDURE G_Callen.ITEMS_BOX;
GO
CREATE PROCEDURE G_Callen.ITEMS_BOX
AS
	SELECT Item_ID, Item_Name,Item_Descr,Item_Year,Sponsor,Other,Series,NumberInSeries 
	FROM G_Callen.ITEM AS I
	LEFT JOIN G_Callen.SERIESITEMS AS S
	ON I.Item_ID = S.Item;
GO

-- Returns Series info (used to fill combo box)
DROP PROCEDURE G_Callen.FILL_SERIES_BOX;
GO
CREATE PROCEDURE G_Callen.FILL_SERIES_BOX
AS
	SELECT Series_ID AS ID,Series_Name AS name FROM G_Callen.SERIES;
GO

-- Returns list of gifted items
DROP PROCEDURE G_CALLEN.GIFT_INST
GO
CREATE PROCEDURE G_CALLEN.GIFT_INST @Offer INT
AS
SELECT dest, CONVERT(VARCHAR(10),Gift_Date,110) AS date, Item_Name AS name, Item_Year AS year, sponsor,Item_Descr AS descr,Inst,Item,Gift_ID
		FROM(SELECT dest,Gift_Date,  Item_Name, Item_Descr, Item_Year, EE.Entity_name as Sponsor,Inst,Item,Gift_ID
				FROM(SELECT Gift_Date,Item_Name, Item_Descr, Item_Year, Sponsor,Inst,Item,Peer,Gift_ID
					FROM(SELECT Offered.Item, Offered.Peer, Gift_Date,Inst,Gift_ID
						FROM(SELECT Gift_ID, Item, Peer, Gift_Date
							 FROM G_Callen.GIFT
							 WHERE Offered = @Offer) AS Offered
						LEFT OUTER JOIN G_Callen.GIFTINST AS OfferInst
						ON Offered.Gift_ID = OfferInst.Gift) AS Offer
					LEFT OUTER JOIN (SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor
									FROM G_Callen.ITEM) AS IT
					ON Offer.Item = IT.Item_ID) AS ITEMS
			LEFT OUTER JOIN(SELECT Entity_ID, Entity_Name 
							FROM G_Callen.ENTITY) AS EE 
			ON ITEMS.Sponsor = EE.Entity_ID
	LEFT OUTER JOIN(SELECT Entity_ID, Entity_Name as dest
					FROM G_Callen.ENTITY) AS Offer_E 
	ON ITEMS.Peer = Offer_E.Entity_ID) as smth;
go

-- changes gift plan to gifted
DROP PROCEDURE G_CALLEN.ADD_PLAN_TO_GIFTS;
GO
CREATE PROCEDURE G_CALLEN.ADD_PLAN_TO_GIFTS @GiftPlan INT
AS
	UPDATE G_Callen.GIFT SET Offered = 1 WHERE Gift_ID = @GiftPlan;

	DECLARE @instID INT;
	EXEC @instID = G_Callen.fInstGift @GiftPlan;
	
	IF @instID > -1
		UPDATE G_Callen.INST SET State = 1 WHERE Inst_Number = @instID;
GO

-- Removes plan (doesnt change instance/item in any way)
DROP PROCEDURE G_CALLEN.REMOVE_PLAN;
GO
CREATE PROCEDURE G_CALLEN.REMOVE_PLAN @GiftPlan INT
AS
	DELETE FROM G_Callen.GIFTINST WHERE Gift = @GiftPlan;
	DELETE FROM G_Callen.GIFT WHERE Gift_ID = @GiftPlan;
GO

-- Returns Item ID + Name by View
DROP PROCEDURE G_Callen.LastVisItems;
GO
CREATE PROCEDURE G_Callen.LastVisItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I 
		 JOIN (SELECT Item_ID,Inst_Number, Date_View FROM G_Callen.INST WHERE State = '0') AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_View DESC;
;
go

-- GETS SPECIFIC ITEM INFO
DROP PROCEDURE G_Callen.GET_INST_INFO;
go
CREATE PROCEDURE G_Callen.GET_INST_INFO @InstID INT
AS
	SELECT favourite,ID,name,other, descr, year, theme,folder, peer,sponsor ,note, img_path,Series_Name,NumberInSeries
	FROM(SELECT favourite,ID,name,other,descr,year,theme,folder,peer,sponsor ,note,img_path,Series,NumberInSeries
		FROM(SELECT Favorite AS favourite,Inst_Number AS ID,Item_Name AS name,Other AS other,Item_Descr AS descr,Item_Year AS year,Theme_Descr AS theme,Code AS folder,Peer_name AS peer,Entity_Name AS sponsor ,Note AS note, Inst_PicPath AS img_path,Item_ID
			FROM(SELECT Favorite,Inst_Number, Item_Name, Item_Descr,Other, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor ,Note, Inst_PicPath,Item_ID
				FROM(SELECT Favorite, Inst_Number, Item_Name,Other, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor ,Note, Inst_PicPath,I.Item_ID
					FROM(SELECT Favorite, Inst_Number, Item_ID, Code, Peer, Theme_Descr ,Note, Inst_PicPath
						FROM(SELECT Item_ID, Inst_Number, Favorite, Arquive, Peer , Note, Inst_PicPath 
							FROM G_Callen.INST
							WHERE Inst_Number = @InstID) AS IT 
							LEFT JOIN(SELECT * 
								FROM G_Callen.ARQUIVE) AS A 
							ON IT.Arquive = A.Arquive_ID) AS IA 
						LEFT JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor, Other
							FROM G_Callen.ITEM) AS I 
						ON IA.Item_ID = I.Item_ID) AS IAI
					LEFT JOIN(SELECT Entity_ID, Entity_Name 
						FROM G_Callen.ENTITY) AS E 
					ON IAI.Peer = E.Entity_ID) AS IAIE 
				LEFT JOIN(SELECT Entity_ID, Entity_Name 
					FROM G_Callen.ENTITY) AS EE 
				ON IAIE.Sponsor = EE.Entity_ID) AS IAIES
			LEFT JOIN G_Callen.SERIESITEMS AS S
			ON S.Item = IAIES.Item_ID) AS SS
	LEFT JOIN (SELECT Series_ID,Series_Name
			   FROM G_Callen.SERIES) AS SE
	ON SE.Series_ID = SS.Series;

	UPDATE G_Callen.INST SET Date_View = GETDATE() WHERE Inst_Number = @InstID; -- "Trigger"
go

-- GETS SPECIFIC ITEM INFO
DROP PROCEDURE G_Callen.GET_ITEM_INFO;
go
CREATE PROCEDURE G_Callen.GET_ITEM_INFO @ItemID INT
AS
SELECT Item_ID, Item_Name,Item_Descr,Item_Year,Entity_Name AS Sponsor,Other,Series_Name AS Series,NumberInSeries
FROM(SELECT Item_ID, Item_Name,Item_Descr,Item_Year,Sponsor,Other,Series_Name,NumberInSeries
	 FROM(SELECT Item_ID, Item_Name,Item_Descr,Item_Year,Sponsor,Other,Series,NumberInSeries 
		 FROM (SELECT *
			   FROM G_Callen.ITEM 
	 		   WHERE Item_ID = @ItemID)AS I
		 LEFT JOIN G_Callen.SERIESITEMS AS SI
		 ON I.Item_ID = SI.Item) AS SS
	 LEFT JOIN G_Callen.SERIES AS S
	 ON S.Series_ID = SS.Series) AS SSS
LEFT JOIN (SELECT Entity_ID,Entity_Name
		   FROM G_Callen.ENTITY) AS E
ON E.Entity_ID = SSS.Sponsor;
go

--Return Items Info
DROP PROCEDURE G_Callen.ITEMS_INFO;
GO
CREATE PROCEDURE G_Callen.ITEMS_INFO
AS
	SELECT Favorite AS favourite,Inst_Number AS ID,Item_Name AS name,Item_Descr AS descr,Item_Year AS year,Note AS note,Theme_Descr AS theme,Code AS folder,Peer_name AS peer,Entity_Name AS sponsor
    FROM(SELECT Favorite, Inst_Number, Item_Name,Note, Item_Descr, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor
        FROM(SELECT Favorite, Inst_Number,Note, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor
            FROM(SELECT Favorite, Inst_Number,Note, Peer, Arquive , Item_Name, Item_Descr, Item_Year, Sponsor
                FROM(SELECT Item_ID, Inst_Number,Note, Favorite, Arquive, Peer
                     FROM G_Callen.INST
					 WHERE State = '0') AS INST
				LEFT OUTER JOIN (SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor
							FROM G_Callen.ITEM) AS IT
				ON INST.Item_ID = IT.Item_ID) AS ITEMS
			LEFT OUTER JOIN(SELECT *
							FROM G_Callen.ARQUIVE) AS A
			ON ITEMS.Arquive = A.Arquive_ID) AS ITEMS_A 
        LEFT OUTER JOIN(SELECT Entity_ID, Entity_Name 
						FROM G_Callen.ENTITY) AS E 
        ON ITEMS_A.Peer = E.Entity_ID) AS ITEMS_E 
    LEFT OUTER JOIN(SELECT Entity_ID, Entity_Name 
					FROM G_Callen.ENTITY) AS EE 
    ON ITEMS_E.Sponsor = EE.Entity_ID;
GO

-- Used to search the table in pro mode
DROP PROCEDURE G_Callen.SEARCH_ITEMS_PRO_VIEW;
go
CREATE PROCEDURE G_Callen.SEARCH_ITEMS_PRO_VIEW @InstID AS INT, @Item_Name AS VARCHAR(100), @Item_Desc AS VARCHAR(255),
											@Item_Year AS VARCHAR(10), @Item_Note AS VARCHAR(150), @Item_Theme AS VARCHAR(50),
												@Item_Folder AS VARCHAR(50), @Item_Peer AS VARCHAR(50), @Item_Sponsor AS VARCHAR(50),
													@Item_Other AS VARCHAR(255)
AS
SELECT Favorite AS favourite,Inst_Number AS ID,Item_Name AS name,Item_Descr AS descr,Item_Year AS year,Note AS note,Theme_Descr AS theme,Code AS folder,Peer_name AS peer,Entity_Name AS sponsor
    FROM(SELECT Favorite, Inst_Number, Item_Name,Note, Item_Descr, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor
        FROM(SELECT Favorite, Inst_Number,Note, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor
            FROM(SELECT Favorite, Inst_Number,Note, Peer, Arquive , Item_Name, Item_Descr, Item_Year, Sponsor
                FROM(SELECT Item_ID, Inst_Number,Note, Favorite, Arquive, Peer
                     FROM G_Callen.INST
					 WHERE State = '0'
					   AND (ISNULL (@InstID, '') = '' OR Inst_Number = @InstID)
					   AND	(ISNULL (@Item_Note, '') = '' OR note   LIKE '%'+@Item_Note+'%')) AS INST
				INNER JOIN (SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor
							FROM G_Callen.ITEM
							WHERE (ISNULL (@Item_Name, '') = '' OR Item_Name LIKE  '%'+@Item_Name+'%')
							  AND (ISNULL (@Item_Other, '') = '' OR Other  LIKE '%'+@Item_Other+'%')
							  AND (ISNULL (@Item_Desc, '') = '' OR Item_Descr  LIKE '%'+@Item_Desc+'%')
							  AND (ISNULL (@Item_Year, '') = '' OR Item_Year = @Item_Year)) AS IT
				ON INST.Item_ID = IT.Item_ID) AS ITEMS
			INNER JOIN(SELECT *
					   FROM G_Callen.ARQUIVE
					   WHERE (ISNULL (@Item_Folder, '') = '' OR Code = @Item_Folder)
						 AND (ISNULL (@Item_Theme, '') = '' OR Theme_Descr  = @Item_Theme)) AS A
			ON ITEMS.Arquive = A.Arquive_ID) AS ITEMS_A 
        INNER JOIN(SELECT Entity_ID, Entity_Name 
				   FROM G_Callen.ENTITY
				   WHERE (ISNULL (@Item_Sponsor, '') = '' OR 
							Entity_Name LIKE '%'+@Item_Sponsor+'%')) AS E 
        ON ITEMS_A.Peer = E.Entity_ID) AS ITEMS_E 
    INNER JOIN(SELECT Entity_ID, Entity_Name 
			   FROM G_Callen.ENTITY
			   WHERE (ISNULL (@Item_Peer, '') = '' OR Entity_Name LIKE  '%'+@Item_Peer+'%')) AS EE 
    ON ITEMS_E.Sponsor = EE.Entity_ID;
GO

-- Used to search the table in pic mode
DROP PROCEDURE G_Callen.SEARCH_ITEMS_PIC;
GO
CREATE PROCEDURE G_Callen.SEARCH_ITEMS_PIC @InstID AS INT, @Item_Name AS VARCHAR(100), @Item_Desc AS VARCHAR(255),
													@Item_Year AS VARCHAR(10), @Item_Note AS VARCHAR(150), @Item_Theme AS VARCHAR(50),
														@Item_Folder AS VARCHAR(50), @Item_Peer AS VARCHAR(50), @Item_Sponsor AS VARCHAR(50),
															@Item_Other AS VARCHAR(255)
AS
SELECT Item_ID, Item_Name,Inst_PicPath
    FROM(SELECT Item_ID,Item_Name,Inst_PicPath,Sponsor
        FROM(SELECT Item_ID,Item_Name, Peer,Inst_PicPath,Sponsor
            FROM(SELECT INST.Item_ID,Peer, Arquive , Item_Name,Inst_PicPath,Sponsor
                FROM(SELECT Item_ID, Arquive, Peer,Inst_PicPath
                     FROM G_Callen.INST
					 WHERE State = '0'
						   AND NOT ISNULL(Inst_PicPath,'') = ''
						   AND (ISNULL (@InstID, '') = '' OR Inst_Number = @InstID)
						   AND (ISNULL (@Item_Note, '') = '' OR Note LIKE '%'+@Item_Note+'%')) AS INST
				INNER JOIN (SELECT Item_ID, Item_Name,Sponsor
								 FROM G_Callen.ITEM
								 WHERE (ISNULL (@Item_Name, '') = '' OR Item_Name LIKE  '%'+@Item_Name+'%')
								   AND (ISNULL (@Item_Other, '') = '' OR Other  LIKE '%'+@Item_Other+'%')
								   AND (ISNULL (@Item_Desc, '') = '' OR Item_Descr LIKE '%'+@Item_Desc+'%')
								   AND (ISNULL (@Item_Year, '') = '' OR Item_Year = @Item_Year)) AS IT
				ON INST.Item_ID = IT.Item_ID) AS ITEMS
			INNER JOIN(SELECT *
							FROM G_Callen.ARQUIVE
							WHERE (ISNULL (@Item_Folder, '') = '' OR Code = @Item_Folder)
							  AND (ISNULL (@Item_Theme, '') = '' OR Theme_Descr = @Item_Theme)) AS A
			ON ITEMS.Arquive = A.Arquive_ID) AS ITEMS_A 
        INNER JOIN(SELECT Entity_ID, Entity_Name 
						FROM G_Callen.ENTITY
						WHERE (ISNULL (@Item_Peer, '') = '' OR Entity_Name LIKE  '%'+@Item_Peer+'%')) AS E 
        ON ITEMS_A.Peer = E.Entity_ID) AS ITEMS_E 
    INNER JOIN(SELECT Entity_ID, Entity_Name 
					FROM G_Callen.ENTITY
					WHERE (ISNULL (@Item_Sponsor, '') = '' OR Entity_Name LIKE '%'+@Item_Sponsor+'%')) AS EE 
    ON ITEMS_E.Sponsor = EE.Entity_ID;
go

-- returns list of Items Id, name and pic_path of inst with pics only
DROP PROCEDURE G_Callen.ITEMS_PIC_MODE;
GO
CREATE PROCEDURE G_Callen.ITEMS_PIC_MODE
AS
	SELECT I.Item_ID, Item_Name, Inst_PicPath
	FROM (SELECT Item_ID, Inst_PicPath
				FROM G_Callen.INST
				WHERE NOT ISNULL(Inst_PicPath,'') = ''
				   AND State = '0') AS IT
		  LEFT OUTER JOIN (SELECT Item_Name, Item_ID 
						   FROM G_Callen.ITEM ) AS I
		  ON I.Item_ID = IT.Item_ID;
go

-- returns list of favourite items
DROP PROCEDURE G_Callen.FavouriteItems;
GO
CREATE PROCEDURE G_Callen.FavouriteItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I
		 JOIN (SELECT Item_ID,Inst_Number 
			   FROM G_Callen.INST 
			   WHERE State = '0'
				 AND Favorite = '1') AS IT
		 ON I.Item_ID = IT.Item_ID;
go

-- sets an inst to favourite
DROP PROCEDURE G_Callen.SET_FAVOURITE;
go
CREATE PROCEDURE G_Callen.SET_FAVOURITE @ItemID INT
AS
	UPDATE G_Callen.INST SET Favorite = ~Favorite WHERE Inst_Number = @ItemID;
GO

-- Returns Item ID + Name by Modify
DROP PROCEDURE G_Callen.LastModItems;
GO
CREATE PROCEDURE G_Callen.LastModItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I 
		 JOIN (SELECT Item_ID,Inst_Number, Date_Mod FROM G_Callen.INST WHERE State = '0') AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_Mod DESC;
;
go

-- Dá update as infos de uma inst
DROP PROCEDURE G_Callen.UPDATE_INST_INFO;
go
CREATE PROCEDURE G_Callen.UPDATE_INST_INFO @InstID INT, @InstNote VARCHAR(150), @InstPeer INT, @InstFolder INT
AS

	DECLARE @oldPeer INT;
	DECLARE @oldNote VARCHAR(150);
	DECLARE @oldFolder INT;
	DECLARE @updated BIT;

	SET @updated = 0;

	SELECT @oldPeer = Peer, @oldNote = Note, @oldFolder = Arquive FROM G_Callen.INST WHERE Inst_Number = @InstID;

	IF @oldNote != @InstNote
	BEGIN
		UPDATE G_Callen.INST
			SET Note = @InstNote WHERE Inst_Number = @InstID;
		
		SET @updated = 1;
	END
	IF(@oldPeer != @InstPeer)
	BEGIN
		UPDATE G_Callen.PEER SET QuantityOffered -= 1 WHERE Peer_ID = @oldPeer;

		UPDATE G_Callen.INST
			SET Peer = @InstPeer WHERE Inst_Number = @InstID;

		SET @updated = 1;
	END
	
	IF(@oldFolder != @InstFolder) 
	BEGIN
		UPDATE G_Callen.INST
			SET Arquive = @InstFolder WHERE Inst_Number = @InstID;

		SET @updated = 1;
	END

	IF(@updated = 1)
		UPDATE G_Callen.INST SET Date_Mod = GETDATE() WHERE Inst_Number = @InstID; -- "Trigger"

	SELECT @updated;
GO

-- Returns folder info (used to fill combo box)
DROP PROCEDURE G_Callen.FOLDER_INFO;
GO
CREATE PROCEDURE G_Callen.FOLDER_INFO
AS
	SELECT * FROM G_Callen.ARQUIVE;
GO

-- Cria um novo folder
DROP PROCEDURE G_Callen.CREATE_FOLDER;
go
CREATE PROCEDURE G_Callen.CREATE_FOLDER @Code VARCHAR(50), @Theme VARCHAR(50)
AS
	INSERT INTO G_Callen.ARQUIVE(Code, Theme_Descr) VALUES(@Code,@Theme)
GO

-- Cria uma nova serie
DROP PROCEDURE G_Callen.CREATE_SERIES;
go
CREATE PROCEDURE G_Callen.CREATE_SERIES @Name VARCHAR(50), @Desc VARCHAR(50)
AS
	INSERT INTO G_Callen.SERIES(Series_Name, Descr) VALUES(@Name,@Desc);
GO

-- Create Address
DROP PROCEDURE G_Callen.CREATE_ADDRESS;
go
CREATE PROCEDURE G_Callen.CREATE_ADDRESS @Street VARCHAR(150),	@City VARCHAR(50),@State VARCHAR(50),
													@Country VARCHAR(50), @PostalCode VARCHAR(50)
AS
	DECLARE @address_id INT;
	DECLARE @Out TABLE(id INT);

	IF(@Street = '' AND @City = '' AND @State = '' AND @Country = '' AND @PostalCode = '')
		RETURN -1;

	INSERT INTO G_CALLEN.ADDRESS(Street, City, State, Country, PostalCode) 
				OUTPUT INSERTED.Address_ID INTO @Out(id)
				VALUES (@Street, @City, @State, @Country, @PostalCode) 
	
	SELECT @address_id = id FROM @Out;

	RETURN @address_id;
GO

-- Add Peer and Address
DROP PROCEDURE G_Callen.ADD_PEER;
GO
CREATE PROCEDURE G_Callen.ADD_PEER @Name VARCHAR(50), @Email VARCHAR(150), @Phone VARCHAR(15),
									 @Street VARCHAR(150),	@City VARCHAR(50),@State VARCHAR(50),
										@Country VARCHAR(50), @PostalCode VARCHAR(50)
AS
	DECLARE @ENTITY_ID AS INT;
	DECLARE @out TABLE(id INT);

	DECLARE @address_id INT;
	DECLARE @AddOut TABLE(id INT);

	INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone)
				OUTPUT INSERTED.Entity_ID INTO @out(id) 
				VALUES (@Name, @Email, @Phone);

	SELECT @ENTITY_ID = id FROM @out;

	INSERT INTO G_Callen.PEER(Peer_ID,QuantityOffered) VALUES (@ENTITY_ID,0);

	EXEC @address_id = G_Callen.CREATE_ADDRESS @Street, @City, @State, @Country, @PostalCode;

	IF(@address_id > -1)
		INSERT INTO G_Callen.ENTITYADRESS(Entity, Address) VALUES(@ENTITY_ID,@address_id);
GO

-- Returns Item ID + Name by Insert
DROP PROCEDURE G_Callen.COUNT_PEERS;
GO
CREATE PROCEDURE G_Callen.COUNT_PEERS
AS
	SELECT Entity_Name AS peer,QuantityOffered AS count
	FROM (SELECT TOP 25 *
		  FROM G_Callen.PEER
		  ORDER BY QuantityOffered DESC) AS P
	LEFT JOIN (SELECT Entity_ID,Entity_Name 
			   FROM G_Callen.ENTITY) AS E
	ON P.Peer_ID = E.Entity_ID;
;
go

-- Creates a Sponsor entiry
DROP PROCEDURE G_Callen.ADD_SPONSOR;
go
CREATE PROCEDURE G_Callen.ADD_SPONSOR @Name VARCHAR(50), @Email VARCHAR(150), @Phone VARCHAR(15), 
										@WebSite VARCHAR(100), @Street VARCHAR(150), @City VARCHAR(50),
											@State VARCHAR(50), @Country VARCHAR(50), @PostalCode VARCHAR(50)
											
AS
	DECLARE @ENTITY_ID AS INT;
	DECLARE @out TABLE (id INT);

	DECLARE @address_id INT;
	DECLARE @AddOut TABLE(id INT);

	INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
				OUTPUT INSERTED.Entity_ID INTO @out(id)
				VALUES (@Name, @Email, @Phone);

	SELECT @ENTITY_ID = id FROM @out;

	INSERT INTO G_Callen.SPONSOR(Sponsor_ID,Website) VALUES (@ENTITY_ID,@WebSite);

	EXEC @address_id = G_Callen.CREATE_ADDRESS @Street, @City, @State, @Country, @PostalCode;
	
	IF(@address_id > -1)
		INSERT INTO G_Callen.ENTITYADRESS(Entity, Address) VALUES(@ENTITY_ID,@address_id);
GO

-- Returns Item ID + Name by Insert
DROP PROCEDURE G_Callen.LastInsertItems;
GO
CREATE PROCEDURE G_Callen.LastInsertItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_Callen.ITEM) AS I 
		 JOIN (SELECT Item_ID,Inst_Number, Date_Insert FROM G_Callen.INST WHERE State = '0') AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_Insert DESC;
;
go

-- Adds a new Inst
DROP PROCEDURE G_Callen.ADD_INST;
go
CREATE PROCEDURE G_Callen.ADD_INST @Name VARCHAR(100), @Sponsor INT, @Peer INT, @Desc VARCHAR(255), @Year VARCHAR(10),
										@Series INT, @SeriesNum INT,@Folder INT, @Other VARCHAR(150),@Note VARCHAR(150), @Img_Path VARCHAR(255)
AS

	DECLARE @ITEM_ID AS INT;
	DECLARE @out TABLE(id INT);

	INSERT INTO G_Callen.ITEM(Item_Name,Item_Descr,Item_Year,Sponsor,Other,Type)
		OUTPUT INSERTED.Item_ID INTO @out(id)
		VALUES(@Name,@Desc,@Year,@Sponsor,@Other,1);

	SELECT @ITEM_ID = id FROM @out;

	IF(@Series > 0)
		INSERT INTO G_Callen.SERIESITEMS(Series,Item,NumberInSeries) VALUES (@Series,@ITEM_ID,@SeriesNum);

	IF @Peer > 0
		INSERT INTO G_Callen.INST(Item_ID,Arquive,Peer,Inst_PicPath,Note,Date_Insert,Favorite,State)
			VALUES(@ITEM_ID,@Folder,@Peer,@Img_Path,@Note,GETDATE(),0,0);
	ELSE
		INSERT INTO G_Callen.INST(Item_ID,Arquive,Inst_PicPath,Note,Date_Insert,Favorite,State)
			VALUES(@ITEM_ID,@Folder,@Img_Path,@Note,GETDATE(),0,0);

	SELECT IDENT_CURRENT('G_Callen.INST');
GO

-- Adds a new Inst with a old item
DROP PROCEDURE G_Callen.ADD_INST_WITH_ITEM;
go
CREATE PROCEDURE G_Callen.ADD_INST_WITH_ITEM @ItemID INT, @Peer INT,@Folder INT,
												@Note VARCHAR(150), @Img_Path VARCHAR(255)
AS
	IF(@Peer > 0)
		INSERT INTO G_Callen.INST(Item_ID,Arquive,Peer,Inst_PicPath,Note,Date_Insert,Favorite,State)
			VALUES(@ItemID,@Folder,@Peer,@Img_Path,@Note,GETDATE(),0,0);
	ELSE
		INSERT INTO G_Callen.INST(Item_ID,Arquive,Inst_PicPath,Note,Date_Insert,Favorite,State)
			VALUES(@ItemID,@Folder,@Img_Path,@Note,GETDATE(),0,0);

	SELECT IDENT_CURRENT('G_Callen.INST');
GO

-- Adds a new gift
DROP PROCEDURE G_Callen.ADD_GIFT;
go
CREATE PROCEDURE G_Callen.ADD_GIFT @Name VARCHAR(100), @Sponsor INT, @Desc VARCHAR(255), @Year VARCHAR(10),
										 @Other VARCHAR(150), @Series INT, @SeriesNum INT, @Dest INT, @Offered INT
AS

	DECLARE @ITEM_ID AS INT;
	DECLARE @out TABLE(id INT);

	INSERT INTO G_Callen.ITEM(Item_Name,Item_Descr,Item_Year,Sponsor,Other,Type)
		OUTPUT INSERTED.Item_ID INTO @out(id)
		VALUES(@Name,@Desc,@Year,@Sponsor,@Other,1);

	SELECT @ITEM_ID = id FROM @out;

	IF(@Series > 0)
		INSERT INTO G_Callen.SERIESITEMS(Series,Item,NumberInSeries) VALUES (@Series,@ITEM_ID,@SeriesNum);

	INSERT INTO G_Callen.GIFT(Peer,Item,Gift_Date,Offered) VALUES (@Dest,@ITEM_ID,GETDATE(),@Offered);

	SELECT IDENT_CURRENT('G_Callen.GIFT');
GO

-- Adds a new gift
DROP PROCEDURE G_Callen.ADD_GIFT_WITH_ITEM;
go
CREATE PROCEDURE G_Callen.ADD_GIFT_WITH_ITEM @ItemID INT, @Dest INT, @Offered INT
AS
	INSERT INTO G_Callen.GIFT(Peer,Item,Gift_Date,Offered) VALUES (@Dest,@ItemID,GETDATE(),@Offered);

	SELECT IDENT_CURRENT('G_Callen.GIFT');
GO

-- Adds a new gift
DROP PROCEDURE G_Callen.ADD_GIFT_WITH_INST;
go
CREATE PROCEDURE G_Callen.ADD_GIFT_WITH_INST @InstID INT, @Dest INT, @Offered INT
AS
	DECLARE @ItemID INT;
	DECLARE @out TABLE (id INT);
	DECLARE @Gift INT;
	SELECT @ItemID = Item_ID FROM G_Callen.INST WHERE Inst_Number = @InstID;

	INSERT INTO G_Callen.GIFT(Peer,Item,Gift_Date,Offered)
				OUTPUT INSERTED.Gift_ID INTO @Out(id)
				VALUES (@Dest,@ItemID,GETDATE(),@Offered);

	SELECT @Gift = id FROM @out;

	INSERT INTO G_Callen.GIFTINST(Gift,Inst) VALUES (@Gift,@InstID);

	IF @Offered = 1
		UPDATE G_Callen.INST SET State = 1 WHERE Inst_Number = @InstID;

	SELECT IDENT_CURRENT('G_Callen.GIFT');
GO

-- Used to search the table in pro mode
DROP PROCEDURE G_Callen.SEARCH_GIFTS;
go
CREATE PROCEDURE G_Callen.SEARCH_GIFTS @InstID AS INT, @Item_Name AS VARCHAR(100), @Item_Desc AS VARCHAR(255),
											@Item_Year AS VARCHAR(10), @Item_Note AS VARCHAR(150), @Item_Theme AS VARCHAR(50),
												@Item_Folder AS VARCHAR(50), @Item_Dest AS VARCHAR(50), @Item_Sponsor AS VARCHAR(50),
													@Item_Other AS VARCHAR(255), @Offer INT
AS
	SELECT name, descr, year, sponsor, Inst, Item,Gift_ID, date,destName AS dest
	FROM(SELECT Item_Name AS name,Item_Descr AS descr,Item_Year AS year,Entity_Name AS sponsor,Inst_Number AS Inst,Item,Gift_ID,CONVERT(VARCHAR(10),Gift_Date,110) AS date,dest,SponsorID
		 FROM(SELECT Inst_Number, Item_Name, Item_Descr, Item_Year, SponsorID,Item,dest,Gift_ID,Gift_Date
			FROM(SELECT  Inst_Number, Item_Name, Item_Descr, Item_Year,dest ,Item,Arquive,Gift_ID,Gift_Date,IT.Sponsor AS SponsorID
				FROM(SELECT Inst_Number,dest, Arquive, Item,Gift_ID,Gift_Date
					FROM(SELECT Offered.Item, dest, Gift_Date,Inst,Gift_ID
						FROM(SELECT Gift_ID, Item, Peer AS dest, Gift_Date
								FROM G_Callen.GIFT
								WHERE Offered = @Offer) AS Offered
						LEFT OUTER JOIN (SELECT * 
									FROM G_Callen.GIFTINST
									WHERE (ISNULL (@InstID, '') = '' OR Inst = @InstID)) AS OfferInst
						ON Offered.Gift_ID = OfferInst.Gift) AS Offer
					LEFT OUTER JOIN(SELECT Item_ID, Inst_Number, Arquive, Peer
								FROM G_Callen.INST
								WHERE State = '1'
								AND	(ISNULL (@Item_Note, '') = '' OR note   LIKE '%'+@Item_Note+'%')) AS I
					ON Offer.Inst = I.Inst_Number) AS INST
			INNER JOIN (SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor
						FROM G_Callen.ITEM
						WHERE (ISNULL (@Item_Name, '') = '' OR Item_Name LIKE  '%'+@Item_Name+'%')
						  AND (ISNULL (@Item_Other, '') = '' OR Other  LIKE '%'+@Item_Other+'%')
						  AND (ISNULL (@Item_Desc, '') = '' OR Item_Descr  LIKE '%'+@Item_Desc+'%')
						  AND (ISNULL (@Item_Year, '') = '' OR Item_Year = @Item_Year)) AS IT
			ON INST.Item = IT.Item_ID) AS ITEMS
		LEFT OUTER JOIN(SELECT *
					FROM G_Callen.ARQUIVE
					WHERE (ISNULL (@Item_Folder, '') = '' OR Code = @Item_Folder)
					  AND (ISNULL (@Item_Theme, '') = '' OR Theme_Descr  LIKE '%'+@Item_Theme+'%')) AS A
		ON ITEMS.Arquive = A.Arquive_ID) AS ITEMS_A 
	INNER JOIN(SELECT Entity_ID, Entity_Name 
				FROM G_Callen.ENTITY
				WHERE (ISNULL (@Item_Sponsor, '') = '' OR 
						Entity_Name LIKE '%'+@Item_Sponsor+'%')) AS E 
	ON ITEMS_A.SponsorID = E.Entity_ID) AS ITEMS_E 
INNER JOIN(SELECT Entity_ID, Entity_Name AS destName
			FROM G_Callen.ENTITY
			WHERE (ISNULL (@Item_Dest, '') = '' OR Entity_Name LIKE  '%'+@Item_Dest+'%')) AS EE 
ON ITEMS_E.dest = EE.Entity_ID;
GO

-- Returns Item ID + Name by Insert
DROP PROCEDURE G_Callen.RECENT_GIFTS;
GO
CREATE PROCEDURE G_Callen.RECENT_GIFTS
AS
	SELECT Entity_Name AS Entity, Item_Name,Item, Inst
	FROM(	SELECT Entity_Name,Item,Inst
			FROM(	SELECT Peer, Item, Inst
					FROM(	SELECT DISTINCT TOP 25  * 
							FROM G_Callen.GIFT
							WHERE Offered = 1
							ORDER BY Gift_Date DESC) AS G
					LEFT JOIN G_Callen.GIFTINST AS GI
					ON G.Gift_ID = GI.Gift) AS GIF
			LEFT JOIN (SELECT Entity_ID, Entity_Name
					   FROM G_Callen.ENTITY) AS E
			ON GIF.Peer = E.Entity_ID) AS ENT
	LEFT JOIN (SELECT *
			   FROM G_Callen.ITEM) AS I
	ON ENT.Item = I.Item_ID;
;
go

-- Returns Sponsor info (used to fill combo box)
DROP PROCEDURE G_Callen.FILL_SPONSOR_BOX;
GO
CREATE PROCEDURE G_Callen.FILL_SPONSOR_BOX
AS
	SELECT Entity_Name AS name, Sponsor_ID as ID
	FROM G_Callen.ENTITY 
	INNER JOIN G_Callen.SPONSOR 
	on Entity_ID = Sponsor_ID;
GO

-- Returns peer info (used to fill combo box)
DROP PROCEDURE G_Callen.FILL_PEER_BOX;
GO
CREATE PROCEDURE G_Callen.FILL_PEER_BOX
AS
	SELECT Entity_Name AS name, Peer_ID AS ID
	FROM G_Callen.ENTITY
	INNER JOIN G_Callen.PEER
	on Entity_ID = Peer_ID;
GO

-- Returns full list of peers
DROP PROCEDURE FILL_PEER 
GO
CREATE PROCEDURE FILL_PEER 
AS
SELECT ID,Nome,Email,Telefone,Ofertas,Street AS Rua, City AS Cidade, State AS Estado, Country AS País, PostalCode As CódPostal
FROM(SELECT ID,Nome,Email,Telefone,Ofertas,Address
	 FROM (SELECT Entity_ID AS ID, Entity_Name AS Nome, Email, Phone AS Telefone, QuantityOffered as Ofertas
		  FROM G_Callen.ENTITY 
		  INNER JOIN G_Callen.PEER 
		  on Entity_ID = Peer_ID) AS ENT
 	 LEFT JOIN G_Callen.ENTITYADRESS AS EA
	 ON ENT.ID = EA.Entity) AS ENTA
LEFT JOIN G_Callen.ADDRESS AS A
ON ENTA.Address = A.Address_ID;
GO

-- Returns searched list of peers
DROP PROCEDURE SEARCH_PEER 
GO
CREATE PROCEDURE SEARCH_PEER @PeerID INT, @PeerName VARCHAR(50), @PeerEmail VARCHAR(150),@PeerPhone VARCHAR(15),@PeerStreet VARCHAR(150),
								@PeerCity VARCHAR(50),@PeerState VARCHAR(50),@PeerCountry VARCHAR(50),@PeerPostalCode VARCHAR(50)
AS
	SELECT ID,Nome,Email,Telefone,Ofertas,Street AS Rua, City AS Cidade, State AS Estado, Country AS País, PostalCode As CódPostal
	FROM(SELECT ID,Nome,Email,Telefone,Ofertas,Address
		 FROM (SELECT Entity_ID AS ID, Entity_Name AS Nome, Email, Phone AS Telefone, QuantityOffered as Ofertas
			  FROM (SELECT *  
					FROM G_Callen.ENTITY
					WHERE (ISNULL (@PeerID, '') = '' OR Entity_ID = @PeerID)
					  AND (ISNULL (@PeerName, '') = '' OR Entity_Name LIKE '%'+@PeerName+'%')
					  AND (ISNULL (@PeerEmail, '') = '' OR Email LIKE '%'+@PeerEmail+'%')
					  AND (ISNULL (@PeerPhone, '') = '' OR Phone LIKE '%'+@PeerPhone+'%')) AS E
			  INNER JOIN G_Callen.PEER 
			  on E.Entity_ID = Peer_ID) AS ENT
 		 LEFT JOIN G_Callen.ENTITYADRESS AS EA
		 ON ENT.ID = EA.Entity) AS ENTA
	INNER JOIN (SELECT *
			   FROM G_Callen.ADDRESS
			   WHERE (ISNULL (@PeerPostalCode, '') = '' OR PostalCode LIKE '%'+@PeerPostalCode+'%')
			     AND (ISNULL (@PeerStreet, '') = '' OR Street LIKE '%'+@PeerStreet+'%')
				 AND (ISNULL (@PeerCity, '') = '' OR City LIKE '%'+@PeerCity+'%')
				 AND (ISNULL (@PeerState, '') = '' OR State LIKE '%'+@PeerState+'%')
				 AND (ISNULL (@PeerCountry, '') = '' OR Country LIKE '%'+@PeerCountry+'%')) AS A
	ON ENTA.Address = A.Address_ID;
GO

-- Returns full list of sponsors
DROP PROCEDURE FILL_SPONSOR 
GO
CREATE PROCEDURE FILL_SPONSOR
AS	
SELECT ID,Nome,Email,Telefone,Website,Street AS Rua, City AS Cidade, State AS Estado, Country AS País, PostalCode As CódPostal
FROM(SELECT ID,Nome,Email,Telefone,Website,Address
	 FROM (SELECT Entity_ID AS ID, Entity_Name AS Nome, Email, Phone AS Telefone, Website
		   FROM G_Callen.ENTITY
		   INNER JOIN G_Callen.SPONSOR
		   on Entity_ID = Sponsor_ID) AS ENT
	 LEFT JOIN G_Callen.ENTITYADRESS AS EA
	 ON ENT.ID = EA.Entity) AS ENTA
LEFT JOIN G_Callen.ADDRESS AS A
ON ENTA.Address = A.Address_ID;
GO

-- Returns searched list of peers
DROP PROCEDURE G_Callen.SEARCH_SPONSOR
GO
CREATE PROCEDURE G_Callen.SEARCH_SPONSOR @SponsorID INT, @SponsorName VARCHAR(50), @SponsorEmail VARCHAR(150),@SponsorPhone VARCHAR(15),@SponsorStreet VARCHAR(150),
								@SponsorCity VARCHAR(50),@SponsorState VARCHAR(50),@SponsorCountry VARCHAR(50),@SponsorPostalCode VARCHAR(50)
AS
	SELECT ID,Nome,Email,Telefone,Website,Street AS Rua, City AS Cidade, State AS Estado, Country AS País, PostalCode As CódPostal
	FROM(SELECT ID,Nome,Email,Telefone,Website,Address
		 FROM (SELECT Entity_ID AS ID, Entity_Name AS Nome, Email, Phone AS Telefone, Website
			  FROM (SELECT *  
					FROM G_Callen.ENTITY
					WHERE (ISNULL (@SponsorID, '') = '' OR Entity_ID = @SponsorID)
					  AND (ISNULL (@SponsorName, '') = '' OR Entity_Name LIKE '%'+@SponsorName+'%')
					  AND (ISNULL (@SponsorEmail, '') = '' OR Email LIKE '%'+@SponsorEmail+'%')
					  AND (ISNULL (@SponsorPhone, '') = '' OR Phone LIKE '%'+@SponsorPhone+'%')) AS E
			  INNER JOIN G_Callen.SPONSOR 
			  on E.Entity_ID = Sponsor_ID) AS ENT
 		 LEFT JOIN G_Callen.ENTITYADRESS AS EA
		 ON ENT.ID = EA.Entity) AS ENTA
	INNER JOIN (SELECT *
			   FROM G_Callen.ADDRESS
			   WHERE (ISNULL (@SponsorPostalCode, '') = '' OR PostalCode LIKE '%'+@SponsorPostalCode+'%')
			     AND (ISNULL (@SponsorStreet, '') = '' OR Street LIKE '%'+@SponsorStreet+'%')
				 AND (ISNULL (@SponsorCity, '') = '' OR City LIKE '%'+@SponsorCity+'%')
				 AND (ISNULL (@SponsorState, '') = '' OR State LIKE '%'+@SponsorState+'%')
				 AND (ISNULL (@SponsorCountry, '') = '' OR Country LIKE '%'+@SponsorCountry+'%')) AS A
	ON ENTA.Address = A.Address_ID;
GO

-------- TRIGGER --------
-- Updates Image path to have correct Inst ID
DROP TRIGGER G_Callen.SET_IMG_PATH;
go
CREATE TRIGGER G_Callen.SET_IMG_PATH ON G_Callen.INST
AFTER INSERT
AS
	DECLARE @SUB_IMG_PATH AS VARCHAR(255);
	DECLARE @ID AS INT;
	DECLARE @peerID INT;

	SELECT @SUB_IMG_PATH = Inst_PicPath, @ID = Inst_Number, @peerID = Peer FROM inserted;

	UPDATE G_Callen.PEER SET QuantityOffered += 1 WHERE Peer_ID = @peerID;

	IF ISNULL(@SUB_IMG_PATH,'') = ''
		RETURN;
	ELSE
		UPDATE G_Callen.INST SET Inst_PicPath = CONCAT(@SUB_IMG_PATH,@ID) WHERE Inst_Number = @ID
GO

------- FUNCTIONS ---------
DROP FUNCTION G_Callen.fInstGift
GO
CREATE FUNCTION G_Callen.fInstGift (@GiftPlan INT) 
RETURNS INT
AS
BEGIN
	DECLARE @instID INT;
	SELECT @instID = Inst FROM G_Callen.GIFTINST WHERE Gift = @GiftPlan;

	IF @instID IS NOT NULL
		RETURN @instID;
	
	RETURN -1;
END
GO
*/