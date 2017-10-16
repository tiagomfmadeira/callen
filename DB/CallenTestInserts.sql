/* INSERTS IN TABLE */
/*
INSERT INTO() 
VALUES ();
*/


--Tipp de coleção
INSERT INTO G_CALLEN.COLLECTIONTYPE(T_Designation) 
VALUES ('Calendários');

--Coleção
INSERT INTO G_CALLEN.COLLECTION(Collection_Name, Collection_Descr, Collection_Type) 
VALUES ('Teste', 'Coleção de calendários para testes', 1);

--Pasta
INSERT INTO G_CALLEN.ARQUIVE(Code,Theme_Descr) 
VALUES ('A1','Laboratório');

--Entidades - Sponsor e Peers
INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('MEDH','contato@medh.com.br','(11)4513-5067');

INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('John Doe', 'johndoe@anymail.com', '912345678');

INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('SkyMax',NULL,'0800-761-5064');

INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('Coca-Cola', NULL, NULL);

INSERT INTO G_CALLEN.ENTITY(Entity_Name, Email, Phone) 
VALUES ('Dan Cake', NULL, NULL);

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
INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (1,'not real', 0, 'C:\Callen_Pics\Instance_1', '01-01-2017', '01-01-2017', '01-01-2017', 1, 1, 0);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (2,'not real', 0, 'C:\Callen_Pics\Instance_2', '02-02-2017', '02-02-2017', '02-02-2017', 1, 3, 1);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (3,'not real', 1, 'C:\Callen_Pics\Instance_3', '03-03-2017', '03-03-2017', '03-03-2017', 1, 2, 0);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (4,  'not real', 0, 'C:\Callen_Pics\Instance_4', '04-04-2017', '04-04-2017', '04-04-2017', 1, 2, 0);

INSERT INTO G_CALLEN.INST(Item_ID, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Arquive, Peer, State) 
VALUES (5, 'not real', 0, 'C:\Callen_Pics\Instance_5', '05-05-2017', '05-05-2017', '05-05-2017', 1, 2, 0);

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

--Ofertas 
INSERT INTO G_CALLEN.GIFT(Peer, Item, Gift_Date,Offered)
VALUES (2, 2, '03-06-2017',1);

INSERT INTO G_CALLEN.GIFTINST(Gift,Inst)
VALUES(1,2);