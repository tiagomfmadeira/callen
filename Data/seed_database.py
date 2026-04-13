import sqlite3
import re
from sqlite3 import Error
from xml.etree import ElementTree
from datetime import datetime

# Print iterations progress
def printProgressBar(iteration, total, prefix = '', suffix = '', decimals = 1, length = 100, fill = '█', printEnd = "\r"):
    """
    Call in a loop to create terminal progress bar
    @params:
        iteration   - Required  : current iteration (Int)
        total       - Required  : total iterations (Int)
        prefix      - Optional  : prefix string (Str)
        suffix      - Optional  : suffix string (Str)
        decimals    - Optional  : positive number of decimals in percent complete (Int)
        length      - Optional  : character length of bar (Int)
        fill        - Optional  : bar fill character (Str)
        printEnd    - Optional  : end character (e.g. "\r", "\r\n") (Str)
    """
    percent = ("{0:." + str(decimals) + "f}").format(100 * (iteration / float(total)))
    filledLength = int(length * iteration // total)
    bar = fill * filledLength + '-' * (length - filledLength)
    print(f'\r{prefix} |{bar}| {percent}% {suffix}', end = printEnd)
    # Print New Line on Complete
    if iteration == total: 
        print()

def create_connection(db_file):
    """ create a database connection to the SQLite database
        specified by db_file
    :param db_file: database file
    :return: Connection object or None
    """
    conn = None
    try:
        conn = sqlite3.connect(db_file)
        return conn
    except Error as e:
        print(e)

    return conn

def create_table(conn, create_table_sql):
    """ create a table from the create_table_sql statement
    :param conn: Connection object
    :param create_table_sql: a CREATE TABLE statement
    :return:
    """
    try:
        c = conn.cursor()
        c.execute(create_table_sql)
    except Error as e:
        print('Error creating table - ' + str(e))

def create_archive(conn, archive):
    """ create a new archive
    :param conn:
    :param archive:
    :return:
    """

    sql = ''' INSERT INTO Archive(code, theme)
              VALUES(?, ?) '''
    cur = conn.cursor()
    cur.execute(sql, archive)
    conn.commit()

    return cur.lastrowid

def create_calendar(conn, item):
    """ create a new item
    :param conn:
    :param item:
    :return:
    """

    sql = ''' INSERT INTO Calendar(id, name, description, year, matrix, collection, date_inserted, date_modified, date_viewed, deleted, pic_path, archive_id)
              VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) '''
    cur = conn.cursor()
    cur.execute(sql, item)
    conn.commit()

    return cur.lastrowid



if __name__ == '__main__':

#################################################################################

	# create or connect to db
	conn = create_connection(r"Callen.db")

	if conn is None:
		print("Could not connect to db! Exiting...")
		exit()

	# create archive table
	archive_table = """ CREATE TABLE IF NOT EXISTS Archive (
						id integer PRIMARY KEY,
						code text,
						theme text
					); """

	create_table(conn, archive_table)

	# create item table
	item_table = """ CREATE TABLE IF NOT EXISTS Calendar (
						id integer PRIMARY KEY,
						name text,
						description text,
						year text,
						matrix text,
						collection text,
						date_inserted text,
						date_modified text,
						date_viewed text,
						deleted integer,
						pic_path text,
						archive_id integer,
						FOREIGN KEY (archive_id) REFERENCES archive (id)
					); """
	
	create_table(conn, item_table)

#################################################################################

	# get archive information from old db

	tree = ElementTree.parse("table2.xml")
	root = tree.getroot()

	themes_by_code = dict()
	for child in root:
		for tmp in child:
			if tmp.tag == "DESIGNAÇÃO":
				theme = tmp.text
			elif tmp.tag == "PASTA":
				code = tmp.text

		if code in themes_by_code:
			themes_by_code[code] = themes_by_code[code] + [theme]
		else:
			themes_by_code[code] = [theme]

	# add generic theme for the folders with multiple themes
	for code in themes_by_code:
		if len(themes_by_code[code]) > 1:
			themes_by_code[code] = ["Tema não especificado"] + themes_by_code[code]

	# create the archive instances and
	# get the key for the first theme on the table for each code
	archive_id_by_code = dict()
	for code in themes_by_code:
		for theme in themes_by_code[code]:
			archive = (code, theme)
			archive_id = create_archive(conn, archive)
			if theme == "Tema não especificado" or len(themes_by_code[code]) == 1:
				archive_id_by_code[code] = archive_id

#################################################################################

	# get item information from old db

	tree = ElementTree.parse("table1.xml")
	root = tree.getroot()

	l = len(root.getchildren())
	i = 0
	printProgressBar(0, l, prefix = 'Progress:', suffix = 'Complete', length = 50)
	for child in root:
		item_info = ["", "", "", "", "", "", datetime.now(), datetime.now(), datetime.now(), 0, "", ""]
		for tmp in child:
			if tmp.tag == "ID":
				item_info[0] = int(tmp.text)
			elif tmp.tag == "DESIGNAÇÃO":
				item_info[1] = tmp.text
			elif tmp.tag == "DESIGNAÇÃO2":
				item_info[2] = tmp.text
			elif tmp.tag == "ANO":
				item_info[3] = tmp.text
			elif tmp.tag == "MATRIZ":
				item_info[4] = tmp.text
			elif tmp.tag == "COLECÇÃO":
				item_info[5] = tmp.text
			elif tmp.tag == "PASTA":
				item_info[11] = archive_id_by_code[tmp.text]

		create_calendar(conn, tuple(item_info))
		i = i + 1
		printProgressBar(i, l, prefix = 'Progress:', suffix = 'Complete', length = 50)
