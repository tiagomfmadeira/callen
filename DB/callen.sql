/* Tiago Madeira 76321 - Diogo Duarte 77645 */

--Callen

/* TABLE CREATION */
/*
CREATE SCHEMA CALLEN;
go
*/

CREATE TABLE CALLEN.COLLECTIONTYPE(
	T_ID INT IDENTITY(1,1) PRIMARY KEY,
	T_Designation VARCHAR(50) NOT NULL
);
go

CREATE TABLE CALLEN.COLLECTION(
	Collection_ID INT IDENTITY(1,1) PRIMARY KEY,
	Collection_Name VARCHAR(50) NOT NULL,
	Collection_Descr VARCHAR(150) NOT NULL,
	Collection_Type INT REFERENCES CALLEN.COLLECTIONTYPE(T_ID) NOT NULL
);
go

CREATE TABLE CALLEN.ARQUIVE(
	Arquive_ID INT IDENTITY(1,1) PRIMARY KEY, 
	Code VARCHAR(50) NOT NULL, 	--codigo na pasta fisica, primary key??
	Theme_Descr VARCHAR(50),
);
go

CREATE TABLE CALLEN.ENTITY(
	Entity_ID INT IDENTITY(1,1) PRIMARY KEY,
	Entity_Name VARCHAR(50) NOT NULL,
	Email VARCHAR(150),
	Phone VARCHAR(15)
);
go

CREATE TABLE CALLEN.ADDRESS(
	Address_ID INT IDENTITY(1,1) PRIMARY KEY,
	Street VARCHAR(150),
	City VARCHAR(50),
	State VARCHAR(50),
	Country VARCHAR(50),
	PostalCode VARCHAR(50)
);
go

CREATE TABLE CALLEN.ENTITYADRESS(
	Entity INT REFERENCES CALLEN.ENTITY(Entity_ID),
	Address INT REFERENCES CALLEN.ADDRESS(Address_ID)
	PRIMARY KEY (Entity, Address)
);
go

CREATE TABLE CALLEN.SPONSOR(
	Sponsor_ID INT REFERENCES CALLEN.ENTITY(Entity_ID) PRIMARY KEY,
	Website VARCHAR(100)
);
go

CREATE TABLE CALLEN.PEER(
	Peer_ID INT REFERENCES CALLEN.ENTITY(Entity_ID) PRIMARY KEY,
	QuantityOffered INT
);
go

CREATE TABLE CALLEN.SERIES(
	Series_ID INT IDENTITY(1,1) PRIMARY KEY,
	Series_Name VARCHAR(50) NOT NULL,
	Descr VARCHAR(255) NOT NULL,
);
go

--Theme atribute - folder
CREATE TABLE CALLEN.ITEM(
	Item_ID INT IDENTITY(1,1) PRIMARY KEY,
	Item_Name VARCHAR(100) NOT NULL,
	Item_Descr VARCHAR(255) NOT NULL,
	Item_Year VARCHAR(10) NOT NULL,
	Other VARCHAR(255),
	Type VARCHAR(50) NOT NULL, --callendar, coin...
	Sponsor INT REFERENCES CALLEN.SPONSOR(Sponsor_ID)
);
go

CREATE TABLE CALLEN.SERIESITEMS(
	Series INT REFERENCES CALLEN.SERIES(Series_ID),	
	Item INT REFERENCES CALLEN.ITEM(Item_ID),
	NumberInSeries INT,
	PRIMARY KEY(Series, Item)
);
go

CREATE TABLE CALLEN.INST(
	Inst_Number INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Item_ID INT REFERENCES CALLEN.ITEM(Item_ID),
	Note VARCHAR(150) NOT NULL,
	Favorite Bit,
	Inst_PicPath VARCHAR(255),
	Date_Insert DATETIME,
	Date_Mod DATETIME,
	Date_View DATETIME,
	State Bit, -- 0 - Belongs to Colection | 1 - Gifted
	Arquive INT REFERENCES CALLEN.ARQUIVE(Arquive_ID),
	Peer INT REFERENCES CALLEN.PEER(Peer_ID)
);
go

CREATE TABLE CALLEN.INSTINCOLLECTION(
	Inst INT REFERENCES CALLEN.INST(Inst_number),
	Collection INT REFERENCES CALLEN.COLLECTION(Collection_ID),
	PRIMARY KEY (Inst, Collection)
);
go

CREATE TABLE CALLEN.GIFT(
	Gift_ID INT IDENTITY(1,1) PRIMARY KEY,
	Peer INT REFERENCES CALLEN.PEER(Peer_ID),
	Item INT REFERENCES CALLEN.ITEM(Item_ID),
	Gift_Date DATE,
	Offered bit 
);
go

CREATE TABLE CALLEN.GIFTINST(
	Gift INT REFERENCES CALLEN.GIFT(Gift_ID) PRIMARY KEY,
	Inst INT REFERENCES CALLEN.INST(Inst_number)
);
go

/* INSERTS IN TABLE */
/*
INSERT INTO() 
VALUES ();
*/


--Tipp de coleção
INSERT INTO CALLEN.COLLECTIONTYPE(T_Designation) 
VALUES ('Calendários');

--Coleção
INSERT INTO CALLEN.COLLECTION(Collection_Name, Collection_Descr, Collection_Type) 
VALUES ('Teste', 'Coleção de calendários para testes', 1);

--Pasta
INSERT INTO CALLEN.ARQUIVE(Code,Theme_Descr) 
VALUES ('A1','Laboratório');

--Entidades - Sponsor e Peers
INSERT INTO CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('MEDH','contato@medh.com.br','(11)4513-5067');

INSERT INTO CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('John Doe', 'johndoe@anymail.com', '912345678');

INSERT INTO CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('SkyMax',NULL,'0800-761-5064');

INSERT INTO CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('Coca-Cola', NULL, NULL);

INSERT INTO CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('Dan Cake', NULL, NULL);

--Morada
INSERT INTO CALLEN.ADDRESS(Street, City, State, Country, PostalCode) 
VALUES ('123 Main St', 'Anytown', 'AS', 'USA', '12345');

INSERT INTO CALLEN.ADDRESS(Street, City, State, Country, PostalCode) 
VALUES ('Horta dos Bacelos', 'Santa Iria de Azoia', 'Sacavém', 'Portugal', '2685');

--Atribuir morada a entidade
INSERT INTO CALLEN.ENTITYADRESS(Entity, Address) 
VALUES (2, 1);

INSERT INTO CALLEN.ENTITYADRESS(Entity, Address) 
VALUES (5, 2);

--Patrocinadores
INSERT INTO CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (1, 'www.medh.com.br');

INSERT INTO CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (3, 'www.skymaxtelecom.com.br');

INSERT INTO CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (4, 'www.coca-cola.com');

INSERT INTO CALLEN.SPONSOR(Sponsor_ID, Website) 
VALUES (5, 'www.dancakes.com');

--Peers
INSERT INTO CALLEN.PEER(Peer_ID, QuantityOffered) 
VALUES (2, 3);

INSERT INTO CALLEN.PEER(Peer_ID, QuantityOffered) 
VALUES (1, 1);

INSERT INTO CALLEN.PEER(Peer_ID, QuantityOffered) 
VALUES (3, 1);

--Items
INSERT INTO CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Instrumentação analítica','Calendário azul vertical de bolso sobre empresa de instrumentação analítica', 2014, NULL, 1, 1);

INSERT INTO CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Internet Banda Larga','Calendário vermelho e cinzento vertical de bolso sobre empresa de internet banda larga', 2016, NULL, 1, 3);

INSERT INTO CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Delicious and Refreshing','Calendário esverdeado com mulher sentada a sorrir e com uma garrafa de coca-cola na mão. Vertical de bolso.', 2015, NULL, 1, 4);

INSERT INTO CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('Drive safely...drive refreshed','Calendário esverdeado com cara de uma mulher a sorrir e com uma garrafa de coca-cola na mão. Vertical de bolso.', 2015, NULL, 1, 4);

INSERT INTO CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Type,Sponsor) 
VALUES ('o que eu gosto','Calendário castanho e amarelo com logo da Dan Cake. Vertical de bolso.', 1983, NULL, 1, 5);

--Instancias
INSERT INTO CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (1,'not real', 0, 'C:\Callen_Pics\Instance_1', '01-01-2017', '01-01-2017', '01-01-2017', 1, 1, 0);

INSERT INTO CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (2,'not real', 0, 'C:\Callen_Pics\Instance_2', '02-02-2017', '02-02-2017', '02-02-2017', 1, 3, 1);

INSERT INTO CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (3,'not real', 1, 'C:\Callen_Pics\Instance_3', '03-03-2017', '03-03-2017', '03-03-2017', 1, 2, 0);

INSERT INTO CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (4,  'not real', 0, 'C:\Callen_Pics\Instance_4', '04-04-2017', '04-04-2017', '04-04-2017', 1, 2, 0);

INSERT INTO CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (5, 'not real', 0, 'C:\Callen_Pics\Instance_5', '05-05-2017', '05-05-2017', '05-05-2017', 1, 2, 0);

--Relação entre instancias e coleção
INSERT INTO CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (1,1);

INSERT INTO CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (2,1);

INSERT INTO CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (3,1);

INSERT INTO CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (4,1);

INSERT INTO CALLEN.INSTINCOLLECTION(Inst, Collection) 
VALUES (5,1);

--Ofertas 
INSERT INTO CALLEN.GIFT(Peer, Item, Gift_Date,Offered)
VALUES (2, 2, '03-06-2017',1);

INSERT INTO CALLEN.GIFTINST(Gift,Inst)
VALUES(1,2);
