---------- PROCEDURES ----------

-- Returns Items info (used to fill combo box)
-- The rest of the info is used to auto fill the other parameters if an item is choosen
DROP PROCEDURE G_CALLEN.ITEMS_BOX;
GO
CREATE PROCEDURE G_CALLEN.ITEMS_BOX
AS
	SELECT Item_ID, Item_Name,Item_Descr,Item_Year,Other,Collec 
	FROM G_CALLEN.ITEM AS I
GO


-- Returns Top 25 last visualized items
DROP PROCEDURE G_CALLEN.LastVisItems;
GO
CREATE PROCEDURE G_CALLEN.LastVisItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_CALLEN.ITEM) AS I 
		  JOIN (SELECT Item_ID,Inst_Number, Date_View FROM G_CALLEN.INST WHERE State = '0') AS IT
		  ON I.Item_ID = IT.Item_ID
	ORDER BY Date_View DESC;
GO

-- returns Top 25 list of favourite items
DROP PROCEDURE G_CALLEN.FavouriteItems;
GO
CREATE PROCEDURE G_CALLEN.FavouriteItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_CALLEN.ITEM) AS I
		  JOIN (SELECT Item_ID,Inst_Number 
			   FROM G_CALLEN.INST 
			   WHERE State = '0'
				 AND Favorite = '1') AS IT
		  ON I.Item_ID = IT.Item_ID;
GO

-- Returns Top 25 list of last modified Items (Item ID + Name)
DROP PROCEDURE G_CALLEN.LastModItems;
GO
CREATE PROCEDURE G_CALLEN.LastModItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_CALLEN.ITEM) AS I 
		  JOIN (SELECT Item_ID,Inst_Number, Date_Mod FROM G_CALLEN.INST WHERE State = '0') AS IT
		  ON I.Item_ID = IT.Item_ID
	ORDER BY Date_Mod DESC;
GO

-- Returns the last 25 inserted items 
DROP PROCEDURE G_CALLEN.LastInsertItems;
GO
CREATE PROCEDURE G_CALLEN.LastInsertItems
AS
	SELECT TOP 25 Inst_Number, Item_Name
	FROM (SELECT Item_ID, Item_Name FROM G_CALLEN.ITEM) AS I 
		 JOIN (SELECT Item_ID,Inst_Number, Date_Insert FROM G_CALLEN.INST WHERE State = '0') AS IT
		 ON I.Item_ID = IT.Item_ID
	ORDER BY Date_Insert DESC;
;
go

-- sets an inst to favourite
DROP PROCEDURE G_CALLEN.TOGGLE_FAVOURITE;
go
CREATE PROCEDURE G_CALLEN.TOGGLE_FAVOURITE @ItemID INT
AS
	UPDATE G_CALLEN.INST SET Favorite = ~Favorite WHERE Inst_Number = @ItemID;
GO

-- Returns information about a specific instance (used to fill description)
DROP PROCEDURE G_CALLEN.GET_INST_INFO;
GO
CREATE PROCEDURE G_CALLEN.GET_INST_INFO @InstID INT
AS
	SELECT Favorite, Inst_Number AS ID, Item_Name AS name, Item_Year AS year, Other AS other, Collec AS collec, Item_Descr AS descr, Archive AS folder, Theme_Descr as theme, Note AS note, Inst_PicPath AS img_path
	FROM (SELECT *
		  FROM G_CALLEN.INST
	      WHERE Inst_Number = @InstID) AS INS
		      LEFT JOIN(SELECT * 
					    FROM G_CALLEN.ARCHIVE) AS A
		      ON Archive = A.Archive_ID 
			      LEFT JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Other, Collec
				            FROM G_CALLEN.ITEM) AS I
				  ON I.Item_ID = INS.Item_ID

	UPDATE G_CALLEN.INST SET Date_View = GETDATE() WHERE Inst_Number = @InstID; -- Update visualization time
go

--Return Items Info (Used to fill datagrid)
DROP PROCEDURE G_CALLEN.ITEMS_INFO;
GO
CREATE PROCEDURE G_CALLEN.ITEMS_INFO
AS
	SELECT Favorite, Inst_Number AS ID, Item_Name AS name, Item_Year AS year, Other AS other, Collec AS collec, Item_Descr AS descr, Archive AS folder, Theme_Descr as theme, Note AS note, Inst_PicPath AS img_path
	FROM (SELECT *
		  FROM G_CALLEN.INST
	      WHERE State = '0') AS INS
		      LEFT JOIN(SELECT * 
					    FROM G_CALLEN.ARCHIVE) AS A
		      ON Archive = A.Archive_ID 
			      LEFT JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Other, Collec
				            FROM G_CALLEN.ITEM) AS I
				  ON I.Item_ID = INS.Item_ID
	ORDER BY ID DESC
GO

-- Used to search the table in pro mode (datagrid mode)
DROP PROCEDURE G_CALLEN.SEARCH_ITEMS_PRO;
go
CREATE PROCEDURE G_CALLEN.SEARCH_ITEMS_PRO @InstID AS INT, @Item_Name AS nvarchar(255), @Item_Year AS VARCHAR(20), @Item_Other AS VARCHAR(255), @Collec AS nvarchar(50) 
												@Item_Desc AS nvarchar(255), @Item_Folder AS VARCHAR(50), @Item_Theme AS VARCHAR(50), @Item_Note AS nvarchar(255)
														
AS
	SELECT Favorite, Inst_Number AS ID, Item_Name AS name, Item_Year AS year, Other AS other, Collec AS collec, Item_Descr AS descr, Archive AS folder, Theme_Descr as theme, Note AS note, Inst_PicPath AS img_path
	FROM (SELECT *
		  FROM G_CALLEN.INST
	      WHERE State = '0'
		  AND (ISNULL (@InstID, '') = '' OR Inst_Number = @InstID)
		  AND (ISNULL (@Item_Note, '') = '' OR note LIKE '%'+@Item_Note+'%')) AS INS
		      LEFT JOIN(SELECT * 
					    FROM G_CALLEN.ARCHIVE
						WHERE (ISNULL (@Item_Folder, '') = '' OR Code = @Item_Folder)
						  AND (ISNULL (@Item_Theme, '') = '' OR Theme_Descr  = @Item_Theme)) AS A
		      ON Archive = A.Archive_ID 
			      LEFT JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Other, Collec
				            FROM G_CALLEN.ITEM
							WHERE (ISNULL (@Item_Name, '') = '' OR Item_Name LIKE  '%'+@Item_Name+'%')
							  AND (ISNULL (@Item_Other, '') = '' OR Other  LIKE '%'+@Item_Other+'%')
							  AND (ISNULL (@Item_Desc, '') = '' OR Item_Descr  LIKE '%'+@Item_Desc+'%')
							  AND (ISNULL (@Collec, '') = '' OR Collec  LIKE '%'+@Collec+'%')
							  AND (ISNULL (@Item_Year, '') = '' OR Item_Year = @Item_Year)) AS I
				  ON I.Item_ID = INS.Item_ID
	ORDER BY ID DESC
GO

-- Returns folder info (used to fill combo box)
DROP PROCEDURE G_CALLEN.FOLDER_INFO;
GO
CREATE PROCEDURE G_CALLEN.FOLDER_INFO
AS
	SELECT * FROM G_CALLEN.ARCHIVE ORDER BY Code;
GO

-- Returns folder names
DROP PROCEDURE G_CALLEN.FOLDERS_NAMES;
GO
CREATE PROCEDURE G_CALLEN.FOLDERS_NAMES
AS
	SELECT Code
	FROM G_CALLEN.ARCHIVE
	GROUP BY Code
	ORDER BY Code;
GO

-- Returns folders themes and ID's
DROP PROCEDURE G_CALLEN.FOLDERS_THEMES;
GO
CREATE PROCEDURE G_CALLEN.FOLDERS_THEMES @Code VARCHAR(50)
AS 
	SELECT Archive_ID, Theme_Descr
	FROM G_CALLEN.ARCHIVE
	WHERE Code = @Code
	ORDER BY Theme_Descr;
GO

-- Creates a new folder (returns inserted row)
DROP PROCEDURE G_CALLEN.CREATE_FOLDER;
go
CREATE PROCEDURE G_CALLEN.CREATE_FOLDER @Code VARCHAR(50), @Theme VARCHAR(50)
AS
	DECLARE @out TABLE (id INT);
	DECLARE @folderId INT;

	INSERT INTO G_CALLEN.ARCHIVE(Code, Theme_Descr)
	OUTPUT inserted.Archive_ID into @out(id)
	VALUES(@Code,@Theme)

	SELECT @folderId = id FROM @out;

	SELECT * FROM G_CALLEN.ARCHIVE WHERE Archive_ID = @folderId;
GO

-- returns list of Inst Number, Item name and pic_path of inst with pics only
DROP PROCEDURE G_CALLEN.ITEMS_PIC_MODE;
GO
CREATE PROCEDURE G_CALLEN.ITEMS_PIC_MODE
AS
	SELECT Inst_Number, Item_Name, Inst_PicPath
	FROM (SELECT Item_ID,Inst_Number, Inst_PicPath
				FROM G_CALLEN.INST
				WHERE NOT ISNULL(Inst_PicPath,'') = ''
				  AND State = '0') AS IT
		  LEFT OUTER JOIN (SELECT Item_Name, Item_ID 
						   FROM G_CALLEN.ITEM ) AS I
		  ON I.Item_ID = IT.Item_ID;
GO

-- Updates instance only information (returns 1 some info was updated) TODO
DROP PROCEDURE G_CALLEN.UPDATE_INST_INFO;
go
CREATE PROCEDURE G_CALLEN.UPDATE_INST_INFO @InstID INT, @InstNote VARCHAR(150), @InstPeer INT, @InstFolder INT
AS

	DECLARE @oldPeer INT;
	DECLARE @oldNote VARCHAR(150);
	DECLARE @oldFolder INT;
	DECLARE @updated BIT;

	SET @updated = 0;

	SELECT @oldPeer = Peer, @oldNote = Note, @oldFolder = Archive FROM G_CALLEN.INST WHERE Inst_Number = @InstID;

	IF @oldNote != @InstNote
	BEGIN
		UPDATE G_CALLEN.INST
			SET Note = @InstNote WHERE Inst_Number = @InstID;
		
		SET @updated = 1;
	END
	IF(@oldPeer != @InstPeer)
	BEGIN
		UPDATE G_CALLEN.PEER SET QuantityOffered -= 1 WHERE Peer_ID = @oldPeer;
		UPDATE G_CALLEN.PEER SET QuantityOffered += 1 WHERE Peer_ID = @InstPeer;

		UPDATE G_CALLEN.INST
			SET Peer = @InstPeer WHERE Inst_Number = @InstID;

		SET @updated = 1;
	END
	
	IF(@oldFolder != @InstFolder) 
	BEGIN
		UPDATE G_CALLEN.INST
			SET Archive = @InstFolder WHERE Inst_Number = @InstID;

		SET @updated = 1;
	END

	IF(@updated = 1)
		UPDATE G_CALLEN.INST SET Date_Mod = GETDATE() WHERE Inst_Number = @InstID; -- "Trigger"

	SELECT @updated;
GO

-- Adds a new Inst (returns inserted row)
DROP PROCEDURE G_CALLEN.ADD_INST;
go
CREATE PROCEDURE G_CALLEN.ADD_INST @Name VARCHAR(100), @Sponsor INT, @Peer INT, @Desc VARCHAR(255), @Year VARCHAR(10),
										@Series INT, @SeriesNum INT,@Folder INT, @Other VARCHAR(150),@Note VARCHAR(150), @Img_Path VARCHAR(255)
AS

	DECLARE @ITEM_ID AS INT;
	DECLARE @out TABLE(id INT);

	BEGIN TRAN
		INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Sponsor,Other,Type)
			OUTPUT INSERTED.Item_ID INTO @out(id)
			VALUES(@Name,@Desc,@Year,@Sponsor,@Other,1);

		SELECT @ITEM_ID = id FROM @out;

		IF(@Series > 0)
			INSERT INTO G_CALLEN.SERIESITEMS(Series,Item,NumberInSeries) VALUES (@Series,@ITEM_ID,@SeriesNum);

		IF @Peer > 0
			INSERT INTO G_CALLEN.INST(Item_ID,Archive,Peer,Inst_PicPath,Note,Date_Insert,Favorite,State)
				VALUES(@ITEM_ID,@Folder,@Peer,@Img_Path,@Note,GETDATE(),0,0);
		ELSE
			INSERT INTO G_CALLEN.INST(Item_ID,Archive,Inst_PicPath,Note,Date_Insert,Favorite,State)
				VALUES(@ITEM_ID,@Folder,@Img_Path,@Note,GETDATE(),0,0);
	COMMIT TRAN

	SELECT * FROM G_CALLEN.INST WHERE Inst_Number = IDENT_CURRENT('G_CALLEN.INST');
GO

-- Adds a new Inst with a old item
DROP PROCEDURE G_CALLEN.ADD_INST_WITH_ITEM;
go
CREATE PROCEDURE G_CALLEN.ADD_INST_WITH_ITEM @ItemID INT, @Peer INT,@Folder INT,
												@Note VARCHAR(150), @Img_Path VARCHAR(255)
AS
	IF(@Peer > 0)
		INSERT INTO G_CALLEN.INST(Item_ID,Archive,Peer,Inst_PicPath,Note,Date_Insert,Favorite,State)
			VALUES(@ItemID,@Folder,@Peer,@Img_Path,@Note,GETDATE(),0,0);
	ELSE
		INSERT INTO G_CALLEN.INST(Item_ID,Archive,Inst_PicPath,Note,Date_Insert,Favorite,State)
			VALUES(@ItemID,@Folder,@Img_Path,@Note,GETDATE(),0,0);

	SELECT * FROM G_CALLEN.INST WHERE Inst_Number = IDENT_CURRENT('G_CALLEN.INST');
GO


-------- TRIGGER --------

-- Updates Image path to have correct Inst ID And updates peer quantity offered
DROP TRIGGER G_CALLEN.INST_TRIGGER;
go
CREATE TRIGGER G_CALLEN.INST_TRIGGER ON G_CALLEN.INST
AFTER INSERT
AS
	DECLARE @SUB_IMG_PATH AS VARCHAR(255);
	DECLARE @ID AS INT;
	DECLARE @peerID INT;

	SELECT @SUB_IMG_PATH = Inst_PicPath, @ID = Inst_Number, @peerID = Peer FROM inserted;

	IF NOT (ISNULL(@peerID,'') = '')
		UPDATE G_CALLEN.PEER SET QuantityOffered += 1 WHERE Peer_ID = @peerID;

	IF ISNULL(@SUB_IMG_PATH,'') = ''
		RETURN;
	ELSE
		UPDATE G_CALLEN.INST SET Inst_PicPath = CONCAT(@SUB_IMG_PATH,@ID) WHERE Inst_Number = @ID;
GO

-- Used to search the table in pic mode TODO
DROP PROCEDURE G_CALLEN.SEARCH_ITEMS_PIC;
GO
CREATE PROCEDURE G_CALLEN.SEARCH_ITEMS_PIC @InstID AS INT, @Item_Name AS VARCHAR(100), @Item_Desc AS VARCHAR(255),
													@Item_Year AS VARCHAR(10), @Item_Note AS VARCHAR(150), @Item_Theme AS VARCHAR(50),
														@Item_Folder AS VARCHAR(50), @Item_Peer AS VARCHAR(50), @Item_Sponsor AS VARCHAR(50),
															@Item_Other AS VARCHAR(255)
AS
SELECT Inst_Number,Item_Name,Inst_PicPath
    FROM(SELECT Inst_Number,Item_Name,Inst_PicPath,Sponsor
        FROM(SELECT Inst_Number,Item_Name, Peer,Inst_PicPath,Sponsor
            FROM(SELECT Inst_Number,Peer, Archive , Item_Name,Inst_PicPath,Sponsor
                FROM(SELECT Item_ID,Inst_Number, Archive, Peer,Inst_PicPath
                     FROM G_CALLEN.INST
					 WHERE State = '0'
						   AND NOT ISNULL(Inst_PicPath,'') = ''
						   AND (ISNULL (@InstID, '') = '' OR Inst_Number = @InstID)
						   AND (ISNULL (@Item_Note, '') = '' OR Note LIKE '%'+@Item_Note+'%')) AS INST
				INNER JOIN (SELECT Item_ID, Item_Name,Sponsor
								 FROM G_CALLEN.ITEM
								 WHERE (ISNULL (@Item_Name, '') = '' OR Item_Name LIKE  '%'+@Item_Name+'%')
								   AND (ISNULL (@Item_Other, '') = '' OR Other  LIKE '%'+@Item_Other+'%')
								   AND (ISNULL (@Item_Desc, '') = '' OR Item_Descr LIKE '%'+@Item_Desc+'%')
								   AND (ISNULL (@Item_Year, '') = '' OR Item_Year = @Item_Year)) AS IT
				ON INST.Item_ID = IT.Item_ID) AS ITEMS
			INNER JOIN(SELECT *
							FROM G_CALLEN.ARCHIVE
							WHERE (ISNULL (@Item_Folder, '') = '' OR Code = @Item_Folder)
							  AND (ISNULL (@Item_Theme, '') = '' OR Theme_Descr = @Item_Theme)) AS A
			ON ITEMS.Archive = A.Archive_ID) AS ITEMS_A 
        INNER JOIN(SELECT Entity_ID, Entity_Name 
						FROM G_CALLEN.ENTITY
						WHERE (ISNULL (@Item_Peer, '') = '' OR Entity_Name LIKE  '%'+@Item_Peer+'%')) AS E 
        ON ITEMS_A.Peer = E.Entity_ID) AS ITEMS_E 
    INNER JOIN(SELECT Entity_ID, Entity_Name 
					FROM G_CALLEN.ENTITY
					WHERE (ISNULL (@Item_Sponsor, '') = '' OR Entity_Name LIKE '%'+@Item_Sponsor+'%')) AS EE 
    ON ITEMS_E.Sponsor = EE.Entity_ID;
go