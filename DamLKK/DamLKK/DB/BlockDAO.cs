using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DamLKK.DB
{
    public class BlockDAO
    {
        private BlockDAO(){}

        private static BlockDAO _MyInstance = null;

        public static BlockDAO GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new BlockDAO();
            }
            return _MyInstance;
        }

        /// <summary>
        /// 获取所有坝段
        /// </summary>
        /// <returns>坝段列表</returns>
        public List<_Model.Block> GetBlocks()
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, "select * from block");
                List<_Model.Block> blocks = new List<_Model.Block>();
                while (reader.Read())
                {
                    _Model.Block block = new DamLKK._Model.Block();

                    block.BlockID = ((int)reader["blockid"]);
                    block.BlockName = ((reader["blockname"].ToString()));
                    blocks.Add(block);
                }
                return blocks;
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
    }
}
