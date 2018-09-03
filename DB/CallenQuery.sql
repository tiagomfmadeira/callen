SELECT Favorite,Inst_Number,Item_Name,Item_Descr,Item_Year AS Ano,Theme_Descr,Code,Peer_name,Entity_Name AS sponsor_name
FROM	(SELECT Favorite,Inst_Number,Item_Name,Item_Descr,Item_Year,Theme_Descr,Code,Entity_Name AS Peer_name,Sponsor
		FROM (SELECT Favorite,Inst_Number,Item_Name,Item_Descr,Item_Year,Theme_Descr,Code,Peer,Sponsor
				FROM (SELECT Favorite, Inst_Number,Item_ID,Code,Peer, Theme_Descr
					FROM (SELECT Item_ID, Inst_Number, Favorite, Arquive, Peer
							FROM G_CALLEN.INST) AS IT
							JOIN (SELECT *
								FROM G_CALLEN.ARQUIVE) AS A
							ON IT.Arquive = A.Arquive_ID) AS IA
					JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor
							FROM G_CALLEN.ITEM) AS I
					ON IA.Item_ID = I.Item_ID) AS IAI
				JOIN (SELECT Entity_ID, Entity_Name
					  FROM G_CALLEN.ENTITY) AS E
				ON IAI.Peer=E.Entity_ID) AS IAIE
		 JOIN (SELECT Entity_ID, Entity_Name
			   FROM G_CALLEN.ENTITY) AS EE
		ON IAIE.Sponsor=EE.Entity_ID
WHERE Item_Year LIKE  '%2015%';
