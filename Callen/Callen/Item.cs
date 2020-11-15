using System;
using System.Collections.Generic;

namespace Callen
{
    public class Item  // Used to define an Item in a Collection 
    {
        public String Name { set; get; }
        public String ID { set; get; }

        // TODO get/set
        private String name, id, desc, year, other, collec;

        // TODO needed?
        public Item()
        {
        }

        public Item(String name, String ID, String Desc, String year,  String other)
        {
            this.Name = name;
            this.ID = ID;

            this.id = ID;
            this.name = name;
            this.desc = Desc;
            this.year = year;
            this.other = other;

            this.collec = "";
        }

        // call the previous constructor? TODO
        public Item(String name, String ID, String Desc, String year, String other, String collec)
        {
            this.Name = name;
            this.ID = ID;

            this.id = ID;
            this.name = name;
            this.desc = Desc;
            this.year = year;
            this.other = other;
            this.collec = collec;
        }

        public String getName()
        {
            return name;
        }

        public String getID()
        {
            return id;
        }

        public String getDesc()
        {
            return desc;
        }

        public String getYear()
        {
            return year;
        }

        public String getOther()
        {
            return other;
        }

        public String getCollec()
        {
            return collec;
        }
    }

    public class Instance : Item
    {
        private String theme, folder, inst_num, image_path, note;

        public Instance(String name, String ID, String Inst_num, String Desc, String year, String Theme, String folder,
                            String other, String img, String note, String collec)
                                    : base (name, ID, Desc, year, other, collec)
        {
            this.inst_num = Inst_num;
            this.theme = Theme;
            this.folder = folder;
            this.image_path = img;
            this.note = note;
        }

        public String getTheme()
        {
            return theme;
        }

        public String getFolder()
        {
            return folder;
        }

        public String getImagePath()
        {
            return image_path;
        }

        public String getNote()
        {
            return note;
        }

        public String getInstID()
        {
            return inst_num;
        }
    }

    public class ItemsName
    {
        public static IList<Item> CreateNamesList()
        {
            return DBConnect.getItemsBox();
        }
    }
}
