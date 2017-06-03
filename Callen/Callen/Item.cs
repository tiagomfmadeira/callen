using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callen
{
    public class Item // Used to define an Item in a Collection 
    {
        String name, desc, theme, folder, peer, sponsor, id, year, other, image_path, note;

        public Item(String name, String ID, String Desc, String year, String Theme, String folder, String peer, String sponsor)
        {
            this.name = name;
            this.id = ID;
            this.desc = Desc;
            this.year = year;
            this.theme = Theme;
            this.folder = folder;
            this.peer = peer;
            this.sponsor = sponsor;
            this.other = "";
            this.note = "";
            this.image_path = "";
        }

        public Item(String name, String ID, String Desc, String year, String Theme, String folder, String peer, String sponsor,String other, String img, String note)
        {
            this.name = name;
            this.id = ID;
            this.desc = Desc;
            this.year = year;
            this.theme = Theme;
            this.folder = folder;
            this.peer = peer;
            this.sponsor = sponsor;
            this.other = other;
            this.image_path = img;
            this.note = note;
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

        public String getSponsor()
        {
            return sponsor;
        }

        public void setOther(String o)
        {
            other = o;
        }

        public String getOther()
        {
            return other;
        }

        public void setImagePath(String path)
        {
            image_path = path;
        }

        public String getImagePath()
        {
            return image_path;
        }

        public void setNote(String note)
        {
            this.note = note;
        }

        public String getNote()
        {
            return note;
        }
    }
}
