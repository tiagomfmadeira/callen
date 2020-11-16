using System.Collections.Generic;

namespace Callen
{
    public class Item // Used to define an Item in a Collection 
    {
        public Item()
        {
        }

        public Item(string name, string id, string desc, string year, string other)
        {
            this.id = id;
            this.name = name;
            this.desc = desc;
            this.year = year;
            this.other = other;

            collec = "";
        }

        public Item(string name, string id, string desc, string year, string other, string collec) : this(name, id,
            desc, year, other)
        {
            this.collec = collec;
        }

        public string Name { get; set; }
        public string ID { get; set; }

        public string name { get; }
        public string id { get; }
        public string desc { get; }
        public string year { get; }
        public string other { get; }
        public string collec { get; }
    }

    public class Instance : Item
    {
        public Instance(string name, string id, string inst_num, string desc, string year, string theme, string folder,
            string other, string img_path, string note, string collec)
            : base(name, id, desc, year, other, collec)
        {
            this.inst_num = inst_num;
            this.theme = theme;
            this.folder = folder;
            image_path = img_path;
            this.note = note;
        }

        public string theme { get; }
        public string folder { get; }
        public string inst_num { get; }
        public string image_path { get; }
        public string note { get; }
    }

    public class ItemsName
    {
        public static IList<Item> CreateNamesList()
        {
            return DBConnect.getItemsBox();
        }
    }
}