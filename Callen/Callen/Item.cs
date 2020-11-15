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

        public Item()
        {
        }

        public Item(String name, String id, String desc, String year, String other)
        {
            this.id = id;
            this.name = name;
            this.desc = desc;
            this.year = year;
            this.other = other;

            this.collec = "";
        }

        public Item(String name, String id, String desc, String year, String other, String collec) : this(name, id, desc, year, other)
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

        public Instance(String name, String id, String inst_num, String desc, String year, String theme, String folder,
                            String other, String img_path, String note, String collec)
                                    : base(name, id, desc, year, other, collec)
        {
            this.inst_num = inst_num;
            this.theme = theme;
            this.folder = folder;
            this.image_path = img_path;
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
