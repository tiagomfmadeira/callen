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

        private String name, id, desc, year, other, collec;

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
