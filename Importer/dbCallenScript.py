from xml.etree import ElementTree

def getFolders():
	print("Getting the folders!")

	folderTree = ElementTree.parse("table2.xml")   
	folderRoot = folderTree.getroot()

	folderInfo = [0,1,2]

	fileOut = open('foldersScript.txt','w', encoding='utf8')

	#identity insert
	fileOut.write("SET IDENTITY_INSERT Callen.G_CALLEN.ARCHIVE ON\n")

	i = 0
	d = dict()
	cheat = []

	for child in folderRoot:
		for tmp in child:
			tmpTag = tmp.tag
			tmpText = tmp.text
			
			# some special char replacement
			tmpText = tmpText.replace("\\", "\\\\")
			tmpText = tmpText.replace("'", "''")

			# ID 0
			if tmpTag == "ID":
				folderInfo[0] = tmpText
        	# DESIGNAÇÃO
			elif tmpTag == "DESIGNAÇÃO":
				folderInfo[1] = tmpText
			# PASTA
			elif tmpTag == "PASTA":
				folderInfo[2] = tmpText

		fileOut.write('INSERT INTO G_CALLEN.ARCHIVE(Archive_ID, Code, Theme_Descr)\n') 
		fileOut.write("VALUES(" + str(i) + ", '" + folderInfo[2] + "', '" + folderInfo[1] + "');\n")
		#once?
		if folderInfo[2] in d:
			#second time?
			if folderInfo[2] not in cheat:
				#add dummy to dict (replace any existing folder)
				i = i+1
				d.update({folderInfo[2]:i})
				#mark it 
				cheat.append(folderInfo[2])
				#write dummy line
				fileOut.write('INSERT INTO G_CALLEN.ARCHIVE(Archive_ID, Code, Theme_Descr)\n') 
				fileOut.write("VALUES(" + str(i) + ",'" + folderInfo[2] + "', 'Não especificado');\n")
			#third time, already wrote dummy line and added to dict 
		#never seen it
		else:
			d.update({folderInfo[2]:i})
		i = i+1

	fileOut.close()

	print("Done!")

	#for x in d:
	#	print (x)

	return d

def getTheRest(d):
	print("Getting the rest!")

	tree = ElementTree.parse("table1.xml")   
	root = tree.getroot()

	fileOut = open('itemsScript.txt','w', encoding='utf8')

	folderInfo = ["", "", "", "", "", "", ""]

	for child in root:
		i = 0
		for tmp in child:
			tmpTag = tmp.tag
			tmpText = tmp.text

			tmpText = str(tmpText)
			
			# some special char replacement
			tmpText = tmpText.replace("\\", "\\\\")
			tmpText = tmpText.replace("'", "''")

			# ID 0
			if tmpTag == "ID":
				folderInfo[0] = tmpText
        	# Item_Name 1
			elif tmpTag == "DESIGNAÇÃO":
				folderInfo[1] = tmpText
			# Other 2
			elif tmpTag == "MATRIZ":
				folderInfo[2] = tmpText
			# Series_Name 3
			elif tmpTag == "COLECÇÃO":
				folderInfo[3] = tmpText
			# Item_Year 4
			elif tmpTag == "ANO":
				folderInfo[4] = tmpText
			# Item_Descr 5
			elif tmpTag == "DESIGNAÇÃO2":
				folderInfo[5] = tmpText
			# Archive 6
			elif tmpTag == "PASTA":
				folderInfo[6] = tmpText

		#ID,Item_Name,Other,Series_Name,Item_Year,Item_Descr,Archive
		#0   1         2     3           4         5          6    

		# identity insert
		fileOut.write("SET IDENTITY_INSERT Callen.G_CALLEN.INST ON\n")
		fileOut.write('DECLARE @Item_ID INT;\n\n')

		# Items   					
		fileOut.write('INSERT INTO G_CALLEN.ITEM(Item_Name,Item_Descr,Item_Year,Other,Collec)\n') 
		fileOut.write('VALUES (' + "N'" + folderInfo[1] + "'" + ','+ "N'" + folderInfo[5]+ "'" +','+"'"+ folderInfo[4] +"'"+','+"N'"+ folderInfo[2] +"'"+','+"N'"+ folderInfo[3] +"'"+');\n\n')

		# get ID last inserted item ID
		fileOut.write('SET @Item_ID = SCOPE_IDENTITY();\n\n')

		# instances						
		fileOut.write('INSERT INTO G_CALLEN.INST(Item_ID, Inst_Number, Note, Favorite, Inst_PicPath, Date_Insert, Date_Mod, Date_View, Archive, State)\n') 
		fileOut.write("VALUES (@Item_ID, " + folderInfo[0] + ", '' , 0, '', '', '', ''," + str(d[folderInfo[6]]) + ", 0);\n")

		fileOut.write('GO\n\n')


	fileOut.close()

	print("Done!")

def main():
	getTheRest(getFolders())

main()