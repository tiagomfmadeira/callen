using System;
using System.Collections.Generic;

namespace Callen
{
    public class Item  // Used to define an Item in a Collection 
    {
        public String Name { get; set; }
        public String ID { get; set; }

        public String name { get; }
        public String id { get; }
        public String desc { get; }
        public String year { get; }
        public String other { get; }
        public String collec { get; }

        // TODO needed?
        public Item()
        {
        }

        public Item(String name, String ID, String Desc, String year, String other)
        {
            this.id = ID;
            this.name = name;
            this.desc = Desc;
            this.year = year;
            this.other = other;

            this.collec = "";
        }

        // call the previous constructor? TODO
        public Item(String name, String ID, String Desc, String year, String other, String collec) : this(name, ID, Desc, year, other)
        {
            this.collec = collec;
        }
    }

    public class Instance : Item
    {
        public String theme { get; }
        public String folder { get; }
        public String inst_num { get; }
        public String image_path { get; }
        public String note { get; }

        public Instance(String name, String ID, String Inst_num, String Desc, String year, String Theme, String folder,
                            String other, String img, String note, String collec)
                                    : base(name, ID, Desc, year, other, collec)
        {
            this.inst_num = Inst_num;
            this.theme = Theme;
            this.folder = folder;
            this.image_path = img;
            this.note = note;
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
