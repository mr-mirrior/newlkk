using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DamLKK.DB
{
    public class UnitDAO
    {
        private UnitDAO()
        {

        }

        private static UnitDAO _MyInstance = null;

        public static UnitDAO GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new UnitDAO();
            }

            return _MyInstance;
        }

        /// <添加一个单元>
        /// 添加一个单元
        /// </添加一个单元>
        public bool AddUnit(_Model.Unit p_Unit)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            string CmdText = null, Blocks = string.Empty;

            foreach (_Model.Block b in p_Unit.Blocks)
            {
                Blocks += (","+b.BlockID.ToString() + ",");
            }

            //Blocks = Blocks.Substring(0, Blocks.Length - 1);

            try
            {
                conn = DBConnection.getSqlConnection();
                CmdText = "insert into unit Values('" + Blocks + "','" + p_Unit.Name + "','" + p_Unit.StartZ + "','" + p_Unit.EndZ + "','" + p_Unit.Vertex + "')";
                reader = DBConnection.executeQuery(conn, CmdText);

                return true;
            }
            catch (System.Exception e)
            {
                Utils.DebugUtil.log(e);
                return false;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


        /// <获得指定单元>
        /// 获得指定单元最小斜层标识  -1没有 -2报错
        /// </获得指定单元>
        public double GetMinTag(DamLKK._Model.Unit p_unit)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                string sqltxt = "select max(designz)+1 from segment where unitid=" + p_unit.ID + " and designz<100";
                reader = DBConnection.executeQuery(conn, sqltxt);

                while (reader.Read())
                {
                    if (reader[0] == DBNull.Value)
                        return -1;
                    return Convert.ToDouble(reader[0]);
                }
                return -2;
            }
            catch (System.Exception e)
            {
                Utils.DebugUtil.log(e);
                return -2;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        /// <获得指定单元>
        /// 获得指定单元
        /// </获得指定单元>
        public _Model.Unit GetOneUnit(string p_UnitName)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, "select * from unit where unitname='" + p_UnitName + "'order by ID desc");
                _Model.Unit unit = new _Model.Unit();
                while (reader.Read())
                {
                    unit.ID = (int)reader["ID"];
                    unit.Name = reader["UnitName"].ToString();
                    unit.StartZ = Convert.ToSingle(reader["StartZ"]);
                    unit.EndZ = Convert.ToSingle(reader["EndZ"]);
                    if (reader["Vertex"] != DBNull.Value)
                    unit.Vertex = reader["Vertex"].ToString();
                    unit.Blocks = GetBlockIDs(reader["BlockID"].ToString());
                }
                return unit;
            }
            catch (System.Exception e)
            {
                Utils.DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        /// <获取所有单元>
        /// 获取所有单元
        /// </获取所有单元>
        /// <returns>单元列表</returns>
        public List<_Model.Unit> GetUnits()
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, "select * from unit");
                List<_Model.Unit> units = new List<_Model.Unit>();
                while (reader.Read())
                {
                    _Model.Unit unit = new _Model.Unit();

                    unit.ID = (int)reader["ID"];
                    unit.Name = reader["UnitName"].ToString();
                    unit.StartZ = Convert.ToSingle(reader["StartZ"]);
                    unit.EndZ = Convert.ToSingle(reader["EndZ"]);
                    if (reader["Vertex"]!=DBNull.Value)
                    unit.Vertex = reader["Vertex"].ToString();
                    unit.Blocks=GetBlockIDs(reader["BlockID"].ToString());
                    units.Add(unit);
                }
                return units;
            }
            catch (System.Exception e)
            {
                Utils.DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        /// <获取有工作仓面的坝段>
        /// 获取有工作仓面的坝段
        /// </获取有工作仓面的坝段>
        /// <returns>坝段列表</returns>
        public List<_Model.Unit> GetWorkingUnits()
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                string sqltxt="select * from unit where ID in (select UnitID from segment where Workstate=1)";
                reader = DBConnection.executeQuery(conn,sqltxt );
                List<_Model.Unit> units = new List<_Model.Unit>();
                while (reader.Read())
                {
                    _Model.Unit unit = new _Model.Unit();

                    unit.ID = (int)reader["ID"];
                    unit.Name = reader["UnitName"].ToString();
                    unit.StartZ = Convert.ToSingle(reader["StartZ"]);
                    unit.EndZ = Convert.ToSingle(reader["EndZ"]);
                    unit.Vertex = reader["Vertex"].ToString();
                    unit.Blocks = GetBlockIDs(reader["BlockID"].ToString());
                    units.Add(unit);
                }
                return units;
            }
            catch (System.Exception e)
            {
                Utils.DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


        /// <获取有工作仓面的坝段>
        /// 获取指定单元上工作的仓面tag
        /// </获取有工作仓面的坝段>
        public List<double> GetTagsInUnit(int unitid,bool onlyWorking)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                string sqltxt = "select designz from segment where unitid=" + unitid.ToString();

                if(onlyWorking)
                    sqltxt += " and workstate=1";

                sqltxt += " order by designz asc";

                reader = DBConnection.executeQuery(conn, sqltxt);
                List<double> tags = new List<double>();
                while (reader.Read())
                {
                    tags.Add(Convert.ToDouble(reader[0]));
                }
                return tags;
            }
            catch (System.Exception e)
            {
                Utils.DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }



        private List<_Model.Block> GetBlockIDs(string p_Blocks)
        {
            List<_Model.Block> Blocks = new List<_Model.Block>();

            string[] blockIDs=p_Blocks.Trim().Split(',');

            foreach (string i in blockIDs)
            {
                if (i == string.Empty)
                    continue;
                Blocks.Add(new _Model.Block(Convert.ToInt32(i), Convert.ToInt32(i).ToString("0号坝段")));
            }

            return Blocks;
        }
    }
}
