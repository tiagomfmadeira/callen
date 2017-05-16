/* Tiago Madeira 76321 - Diogo Duarte 77645 */

--Callen

/* TABLE CREATION */
/*
CREATE SCHEMA G_CALLEN;
go

CREATE TABLE G_CALLEN.COLLECTIONTYPE(
	T_ID INT IDENTITY(1,1) PRIMARY KEY,
	T_Designation VARCHAR(50) NOT NULL
);
go

CREATE TABLE G_CALLEN.COLLECTION(
	Collection_ID INT IDENTITY(1,1) PRIMARY KEY,
	Collection_Name VARCHAR(50) NOT NULL,
	Collection_Descr VARCHAR(150) NOT NULL,
	Collection_PicPath VARCHAR(255),
	Collection_Type INT REFERENCES G_CALLEN.COLLECTIONTYPE(T_ID) NOT NULL
);
go

CREATE TABLE G_CALLEN.ARQUIVE(
	Arquive_ID INT IDENTITY(1,1) PRIMARY KEY, 
	Code VARCHAR(50) NOT NULL, 	--codigo na pasta fisica, primary key??
	Theme_Descr VARCHAR(50),
);
go


CREATE TABLE G_CALLEN.ENTITY(
	Entity_ID INT IDENTITY(1,1) PRIMARY KEY,
	Entity_Name VARCHAR(50) NOT NULL,
	Entity_PicPath VARCHAR(255),
	Email VARCHAR(150),
	Phone VARCHAR(15)
);
go

CREATE TABLE G_CALLEN.ADDRESS(
	Address_ID INT IDENTITY(1,1) PRIMARY KEY,
	Street VARCHAR(150),
	City VARCHAR(50),
	State VARCHAR(50),
	Country VARCHAR(50),
	PostalCode VARCHAR(50)
);
go

CREATE TABLE G_CALLEN.ENTITYADRESS(
	Entity INT REFERENCES G_CALLEN.ENTITY(Entity_ID),
	Address INT REFERENCES G_CALLEN.ADDRESS(Address_ID)
	PRIMARY KEY (Entity, Address)
);
go

CREATE TABLE G_CALLEN.SPONSOR(
	Sponsor_ID INT REFERENCES G_CALLEN.ENTITY(Entity_ID) PRIMARY KEY,
	Website VARCHAR(100)
);
go

CREATE TABLE G_CALLEN.PEER(
	Peer_ID INT REFERENCES G_CALLEN.ENTITY(Entity_ID) PRIMARY KEY,
	QuantityOffered INT
);
go

--Theme atribute - folder
CREATE TABLE G_CALLEN.ITEM(
	Item_ID INT IDENTITY(1,1) PRIMARY KEY,
	Item_Name VARCHAR(50) NOT NULL,
	Item_Descr VARCHAR(150) NOT NULL,
	Item_Year VARCHAR(10) NOT NULL,
	Other VARCHAR(255),
	Type VARCHAR(50) NOT NULL, --callendar, coin...
	Sponsor INT REFERENCES G_CALLEN.SPONSOR(Sponsor_ID)
);
go


CREATE TABLE G_CALLEN.SERIES(
	Series_ID INT IDENTITY(1,1) PRIMARY KEY,
	Series_Name VARCHAR(50) NOT NULL,
	Descr VARCHAR(255) NOT NULL,
);
go

CREATE TABLE G_CALLEN.SERIESITEMS(
	Series INT REFERENCES G_CALLEN.SERIES(Series_ID),
	Item INT REFERENCES G_CALLEN.ITEM(Item_ID),
	NumberInSeries INT IDENTITY(1,1),
	PRIMARY KEY(Series, Item)
);
go

CREATE TABLE G_CALLEN.INST(
	Item_ID INT REFERENCES G_CALLEN.ITEM(Item_ID),
	Inst_Number INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Note VARCHAR(150) NOT NULL,
	Favorite Bit,
	Inst_PicPath VARCHAR(255),
	Date_Insert DATETIME,
	Date_Mod DATETIME,
	Date_View DATETIME,
	Arquive INT REFERENCES G_CALLEN.ARQUIVE(Arquive_ID),
	Peer INT REFERENCES G_CALLEN.PEER(Peer_ID)
);
go

CREATE TABLE G_CALLEN.INSTINCOLLECTION(
	Inst INT REFERENCES G_CALLEN.INST(Inst_number),
	Collection INT REFERENCES G_CALLEN.COLLECTION(Collection_ID),
	PRIMARY KEY (Inst, Collection)
);
go

CREATE TABLE G_CALLEN.GIFTPLAN(
	Peer INT REFERENCES G_CALLEN.PEER(Peer_ID) PRIMARY KEY,
	Item INT REFERENCES G_CALLEN.ITEM(Item_ID),
);
go

CREATE TABLE G_CALLEN.GIFTPLANINST(
	Peer INT REFERENCES G_CALLEN.PEER(Peer_ID),
	Item INT REFERENCES G_CALLEN.ITEM(Item_ID),
	Inst INT REFERENCES G_CALLEN.INST(Inst_number),
	PRIMARY KEY(Peer,Item)
);
go

CREATE TABLE G_CALLEN.GIFT(
	Peer INT REFERENCES G_CALLEN.PEER(Peer_ID) PRIMARY KEY,
	Inst INT REFERENCES G_CALLEN.INST(Inst_number),
	Gift_Date DATE
);
go
*/

/* INSERTS IN TABLE */
/*
INSERT INTO  () 
VALUES ();
*/


--Tipp de coleção
INSERT INTO G_CALLEN.COLLECTIONTYPE(T_Designation) 
VALUES ('Calendários');

--Coleção
INSERT INTO G_CALLEN.COLLECTION(Collection_Name, Collection_Descr, Collection_PicPath, Collection_Type) 
VALUES ('Teste', 'Coleção de calendários para testes', 'C:\Callen_Pics\Collec_1', 1);

--Pasta
INSERT INTO G_CALLEN.ARQUIVE(Code,Theme_Descr) 
VALUES ('A1','Laboratório');

--Entidades - Sponsor e Peers
INSERT INTO G_CALLEN.ENTITY(Entity_Name,Entity_PicPath, Email, Phone) 
VALUES ('MEDH','C:\Callen_Pics\Entity_1','contato@medh.com.br','(11)4513-5067');

INSERT INTO G_CALLEN.ENTITY(Entity_Name,Entity_PicPath, Email, Phone) 
VALUES ('John Doe','C:\Callen_Pics\Entity_2', 'johndoe@anymail.com', '912345678');

INSERT INTO G_CALLEN.ENTITY(Entity_Name,Entity_PicPath, Email, Phone) 
VALUES ('SkyMax','C:\Callen_Pics\Entity_3',NULL,'0800-761-5064');

INSERT INTO G_CALLEN.ENTITY(Entity_Name,Entity_PicPath, Email, Phone) 
VALUES ('Coca-Cola','C:\Callen_Pics\Entity_4', NULL, NULL);

INSERT INTO G_CALLEN.ENTITY(Entity_Name,Entity_PicPath, Email, Phone) 
VALUES ('Dan Cake','C:\Callen_Pics\Entity_5', NULL, NULL);

--Morada
INSERT INTO G_CALLEN.ADDRESS(Street, City, State, Country, PostalCode) 
VALUES ('123 Main St', 'Anytown', 'AS', 'USA', '12345');

INSERT INTO G_CALLEN.ADDRESS(Street, City, State, Country, PostalCode) 
VALUES ('Horta dos Bacelos', 'Santa Iria de Azoia', 'Sacavém', 'Portugal', '2685');

--Atribuir morada a entidade
INSERT INTO G_CALLEN.ENTITYADRESS(Entity, Address) 
VALUES (2, 1);

INSERT INTO G_CALLEN.ENTITYADRESS(Entity, Address) 
VALUES (5, 2);

--Patrocinadores
INSERT INTO G_CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (1, 'www.medh.com.br');

INSERT INTO G_CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (3, 'www.skymaxtelecom.com.br');

INSERT INTO G_CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (4, 'www.coca-cola.com');

INSERT INTO G_CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (5, 'www.dancakes.com');

--Peers
INSERT INTO G_CALLEN.PEER(Peer_ID, QuantityOffered) 
VALUES (2, 3);

INSERT INTO G_CALLEN.PEER(Peer_ID, QuantityOffered) 
VALUES (1, 1);

INSERT INTO G_CALLEN.PEER(Peer_ID, QuantityOffered) 
VALUES (3, 1);

--Items
INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Instrumentação analítica','Calendário azul vertical de bolso sobre empresa de instrumentação analítica', 2014, NULL, 1, 1);

INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Internet Banda Larga','Calendário vermelho e cinzento vertical de bolso sobre empresa de internet banda larga', 2016, NULL, 1, 3);

INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Delicious and Refreshing','Calendário esverdeado com mulher sentada a sorrir e com uma garrafa de coca-cola na mão. Vertical de bolso.', 2015, NULL, 1, 4);

INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Drive safely...drive refreshed','Calendário esverdeado com cara de uma mulher a sorrir e com uma garrafa de coca-cola na mão. Vertical de bolso.', 2015, NULL, 1, 4);

INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('o que eu gosto','Calendário castanho e amarelo com logo da Dan Cake. Vertical de bolso.', 1983, NULL, 1, 5);

--Instancias
INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer) 
VALUES (1,'not real', 0, 'C:\Callen_Pics\Instance_1', '01-01-2017', '01-01-2017', '01-01-2017', 1, 1);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer) 
VALUES (2,'not real', 0, 'C:\Callen_Pics\Instance_2', '02-02-2017', '02-02-2017', '02-02-2017', 1, 3);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer) 
VALUES (3,'not real', 1, 'C:\Callen_Pics\Instance_3', '03-03-2017', '03-03-2017', '03-03-2017', 1, 2);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer) 
VALUES (4,  'not real', 0, 'C:\Callen_Pics\Instance_4', '04-04-2017', '04-04-2017', '04-04-2017', 1, 2);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer) 
VALUES (5, 'not real', 0, 'C:\Callen_Pics\Instance_5', '05-05-2017', '05-05-2017', '05-05-2017', 1, 2);

--Relação entre instancias e coleção
INSERT INTO G_CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (1,1);

INSERT INTO G_CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (2,1);

INSERT INTO G_CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (3,1);

INSERT INTO G_CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (4,1);

INSERT INTO G_CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (5,1); 
