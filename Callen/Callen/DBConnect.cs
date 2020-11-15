using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Callen
{
    public class DBConnect // Used to create a connection with the data base
    {
        public static SqlConnection getConnection()
        {
            //Local server
            return new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString);

            //UA server
            //return new SqlConnection("Data Source = tcp: 193.136.175.33\\SQLSERVER2012,8293;Initial Catalog = p1g10; uid = ;password = ");
        }

        #region GETTERS

        // TODO Check where this is used
        public static List<Item> getItemsBox()
        {
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

        // Return info instead of filling the table? TODO
        public static void getPicItems(DataTable dt)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.ITEMS_PIC_MODE";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("PicItems");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        public static Instance getInstanceInfo(String id)
        {
            Instance instance = null;
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.GET_INST_INFO @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@InstID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    instance = new Instance(rdr["name"].ToString(), rdr["item_id"].ToString(), id, rdr["descr"].ToString(), rdr["year"].ToString(),
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["other"].ToString(),
                    rdr["img_path"].ToString(), rdr["note"].ToString(), rdr["collec"].ToString());
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            return instance;
        }

        // TODO change return value?
        public static DataView getItemsInfo()
        {
            DataView itemsView = null;
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.ITEMS_INFO";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable items = new DataTable("Items");
                sda.Fill(items);

                itemsView = items.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            return itemsView;
        }

        public static List<Folders> getFolders()
        {
            List<Folders> folders = new List<Folders>();

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.FOLDERS_NAMES";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Folder");
                sda.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    folders.Add(new Folders { folder = row["Code"].ToString() });
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            return folders;
        }

        // TODO code?
        public static List<Folders> getFoldersThemes(String code)
        {
            List<Folders> folders_themes = new List<Folders>();

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.FOLDERS_THEMES @Code";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Code";
                paramFolder.Value = code;
                cmd.Parameters.Add(paramFolder);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Folder");
                sda.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    folders_themes.Add(new Folders { theme = row["Theme_Descr"].ToString(), id = row["Archive_ID"].ToString() });
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            return folders_themes;
        }

        #endregion

        public static void toggleFavourite(String id)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.TOGGLE_FAVOURITE @ItemID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@ItemID";
                param.Value = id;
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        // TODO change return value?
        public static DataTable searchInstances(Instance inst, bool pic_search)
        {
            DataTable dt = null;

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";
                if (pic_search == false)
                    Get_Data = "EXEC G_CALLEN.SEARCH_ITEMS_PRO @InstID, @Item_Name, @Item_Year, @Other, @Collec, @Item_Desc, @Item_Folder, @Item_Theme, @Item_Note;";
                else
                    Get_Data = "EXEC G_CALLEN.SEARCH_ITEMS_PIC @InstID, @Item_Name, @Item_Desc, @Item_Year, @Item_Note, @Item_Theme, @Item_Folder, @Collec;";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@InstID";
                paramID.Value = inst.id;
                cmd.Parameters.Add(paramID);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Item_Name";
                paramName.Value = inst.name;
                cmd.Parameters.Add(paramName);

                SqlParameter paramDesc = new SqlParameter();
                paramDesc.ParameterName = "@Item_Desc";
                paramDesc.Value = inst.desc;
                cmd.Parameters.Add(paramDesc);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@Item_Year";
                paramYear.Value = inst.year;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramNote = new SqlParameter();
                paramNote.ParameterName = "@Item_Note";
                paramNote.Value = inst.note;
                cmd.Parameters.Add(paramNote);

                SqlParameter paramTheme = new SqlParameter();
                paramTheme.ParameterName = "@Item_Theme";
                paramTheme.Value = inst.theme;
                cmd.Parameters.Add(paramTheme);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Item_Folder";
                paramFolder.Value = inst.folder;
                cmd.Parameters.Add(paramFolder);

                SqlParameter paramCollec = new SqlParameter();
                paramCollec.ParameterName = "@Collec";
                paramCollec.Value = inst.collec;
                cmd.Parameters.Add(paramCollec);

                SqlParameter paramOther = new SqlParameter();
                paramOther.ParameterName = "@Other";
                paramOther.Value = inst.other;
                cmd.Parameters.Add(paramOther);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("INST");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());

            }

            return dt;
        }

        public static void createFolder(String folder_name, String theme_value)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.CREATE_FOLDER @Code, @Theme";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Code";
                param.Value = folder_name;
                cmd.Parameters.Add(param);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@Theme";
                param2.Value = theme_value;
                cmd.Parameters.Add(param2);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        public static void addInstance(Instance inst, ImageSource image, String image_base_path)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";

                Get_Data = "EXEC G_CALLEN.ADD_INST @Name, @Desc, @Year, @Collec, @Folder, @Other, @Note, @Img_Path";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = inst.name;
                cmd.Parameters.Add(paramName);

                SqlParameter paramDesc = new SqlParameter();
                paramDesc.ParameterName = "@Desc";
                paramDesc.Value = inst.desc;
                cmd.Parameters.Add(paramDesc);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@Year";
                paramYear.Value = inst.year;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramOther = new SqlParameter();
                paramOther.ParameterName = "@Other";
                paramOther.Value = inst.other;
                cmd.Parameters.Add(paramOther);

                SqlParameter paramCollec = new SqlParameter();
                paramCollec.ParameterName = "@Collec";
                paramCollec.Value = inst.collec;
                cmd.Parameters.Add(paramCollec);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Folder";
                paramFolder.Value = inst.folder;
                cmd.Parameters.Add(paramFolder);

                SqlParameter paramNote = new SqlParameter();
                paramNote.ParameterName = "@Note";
                paramNote.Value = inst.note;
                cmd.Parameters.Add(paramNote);

                // Image Path
                SqlParameter paramImg = new SqlParameter();
                paramImg.ParameterName = "@Img_Path";
                if (image != null) // Theres an img
                {
                    var img_path = image_base_path + "\\Instance_"; // gets filled in database trigger
                    paramImg.Value = img_path;
                }
                else
                {
                    paramImg.Value = "";
                }
                cmd.Parameters.Add(paramImg);

                SqlDataReader rdr = cmd.ExecuteReader();

                // TODO remove this from here?
                while (rdr.Read())
                {
                    if (image != null) // Theres an image
                    {
                        var filename = image.ToString().Substring(image.ToString().LastIndexOf("///") + 3);
                        System.IO.File.Copy(filename, image_base_path + "\\Instance_" + rdr["Inst_Number"].ToString() + ".jpeg");
                    }
                    break;
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        public static void removeInstance(String instance_id)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.REMOVE_INST @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = instance_id;
                cmd.Parameters.Add(paramInst);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        // TODO Join this function with the next
        public static bool updateItemInfo(Instance updated_instance)
        {
            bool updated = false;

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.UPDATE_ITEM_INFO @ItemID, @ItemName, @ItemDescr, @ItemYear, @ItemOther, @ItemCollec, @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@ItemID";
                paramID.Value = updated_instance.id;
                cmd.Parameters.Add(paramID);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@ItemName";
                paramName.Value = updated_instance.name;
                cmd.Parameters.Add(paramName);

                SqlParameter paramDescr = new SqlParameter();
                paramDescr.ParameterName = "@ItemDescr";
                paramDescr.Value = updated_instance.desc;
                cmd.Parameters.Add(paramDescr);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@ItemYear";
                paramYear.Value = updated_instance.year;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramOther = new SqlParameter();
                paramOther.ParameterName = "@ItemOther";
                paramOther.Value = updated_instance.other;
                cmd.Parameters.Add(paramOther);

                SqlParameter paramCollec = new SqlParameter();
                paramCollec.ParameterName = "@ItemCollec";
                paramCollec.Value = updated_instance.collec;
                cmd.Parameters.Add(paramCollec);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = updated_instance.inst_num;
                cmd.Parameters.Add(paramInst);

                updated = (bool)cmd.ExecuteScalar();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            return updated;
        }

        public static bool updateInstanceInfo(Instance updated_instance)
        {
            bool updated = false;

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.UPDATE_INST_INFO @InstID, @InstNote, @InstFolder";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = updated_instance.inst_num;
                cmd.Parameters.Add(paramInst);

                SqlParameter paramNote = new SqlParameter();
                paramNote.ParameterName = "@InstNote";
                paramNote.Value = updated_instance.note;
                cmd.Parameters.Add(paramNote);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@InstFolder";
                paramFolder.Value = updated_instance.folder;
                cmd.Parameters.Add(paramFolder);

                updated = (bool)cmd.ExecuteScalar();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            return updated;
        }
    }
}
