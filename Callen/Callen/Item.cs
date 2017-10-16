using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace Callen
{
    public class Item  // Used to define an Item in a Collection 
    {
        public String Name { set; get; }
        public String ID { set; get; }

        private String name, id, desc, year, sponsor, other, series, seriesNumber;

        public Item()
        {
        }

        public Item(String name, String ID, String Desc, String year, String sponsor, String other)
        {
            this.Name = name;
            this.ID = ID;

            this.id = ID;
            this.name = name;
            this.desc = Desc;
            this.year = year;
            this.other = other;
            this.sponsor = sponsor;

            this.series = "";
            this.seriesNumber = "";
        }

        public Item(String name, String ID, String Desc, String year, String sponsor, String other, String series, String seriesNum)
        {
            this.Name = name;
            this.ID = ID;

            this.id = ID;
            this.name = name;
            this.desc = Desc;
            this.year = year;
            this.other = other;
            this.series = series;
            this.sponsor = sponsor;
            this.seriesNumber = seriesNum;
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

        public String getSeries()
        {
            return series;
        }

        public String getSeriesNumber()
        {
            return seriesNumber;
        }
    }

    public class Instance : Item
    {
        private String theme, folder, peer, inst_num, image_path, note;

        public Instance(String name, String ID, String Desc, String year, String Theme, String folder, String peer,
                            String sponsor, String other, String img, String note, String series, String seriesNum)
                                    : base (name, "0", Desc, year, sponsor, other, series, seriesNum)
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

    public class ItemsName
    {
        public static IList<Item> CreateNamesList()
        {
            List<Item> names = new List<Item>();

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.ITEMS_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Items");
                sda.Fill(dt);

                List<Item> items = new List<Item>();
                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new Item() { Name = row["Item_Name"].ToString(), ID = row["Item_ID"].ToString() });
                }

                thisConnection.Close();

                return items.ToList();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }

            return new List<Item>().ToList();
        }
    }
}
