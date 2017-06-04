using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callen
{
    public class Item  // Used to define an Item in a Collection 
    {
        public String Name { set; get; }
        public String ID { set; get; }

        private String name, id, desc, year, sponsor, other;

        public Item(String name, String ID, String Desc, String year, String sponsor, String other)
        {
            this.Name = name;
            this.ID = ID;

            this.name = name;
            this.id = ID;
            this.desc = Desc;
            this.year = year;
            this.sponsor = sponsor;
            this.other = other;
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

        public String getSponsor()
        {
            return sponsor;
        }

        public String getOther()
        {
            return other;
        }
    }

    public class Instance : Item
    {
        private String theme, folder, peer, inst_num, image_path, note;

        public Instance(String name, String ID, String Desc, String year, String Theme, String folder, String peer, String sponsor, String other, String img, String note)
                                    : base (name, "0", Desc, year, sponsor, other)
        {
            this.inst_num = ID;
            this.theme = Theme;
            this.folder = folder;
            this.peer = peer;
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

        public String getPeer()
        {
            return peer;
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
}
