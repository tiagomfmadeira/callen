using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace Callen
{
    public class GiftsSupp
    {
        public static Instance getInstInfo(String id) // Gets Selected item info 
        {
            Instance inst = null;
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
                    Instance it = new Instance(rdr["name"].ToString(), rdr["ID"].ToString(), rdr["descr"].ToString(), rdr["year"].ToString(),
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["peer"].ToString(), rdr["sponsor"].ToString(), rdr["other"].ToString(),
                                        rdr["img_path"].ToString(), rdr["note"].ToString(), rdr["Series_Name"].ToString(), rdr["NumberInSeries"].ToString());

                    inst =  it;
                    break;
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                //MessageBox.Show(ee.ToString());
            }

            return inst;
        }

        public static Item getItemInfo(String id)
        {
            Item iteme = null;
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.GET_ITEM_INFO @ItemID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@ItemID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Item it = new Item(rdr["Item_Name"].ToString(), rdr["Item_ID"].ToString(),
                        rdr["Item_Descr"].ToString(), rdr["Item_Year"].ToString(), rdr["Sponsor"].ToString(),
                        rdr["Other"].ToString(), rdr["Series"].ToString(), rdr["NumberInSeries"].ToString());

                    iteme = it;
                    break;
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                //MessageBox.Show(ee.ToString());
            }
            return iteme;
        }
    }
}
