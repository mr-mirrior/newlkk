using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DamLKK.Utils;

namespace DamLKK.DB
{
    public class DBConnection
    {
        public static SqlConnection  getSqlConnection()
        {
            string dbname = /*"DAMGPS"*/DBconfig.GetInstance().Dbname;
           

            string server = /*"172.23.225.215"*/DBconfig.GetInstance().Server;
            string username = /*"sa"*/DBconfig.GetInstance().Username;
            string password = /*"s"*/DBconfig.GetInstance().Password;
            dbname = "lkk";
            username = "sa";
            password = "123456";
            server = "localhost";

            //             string sqlConnectionCommand = "Data Source="+server+";Initial Catalog="+dbname+";User ID="+username+";password="+"'"+password+"'";
            string sqlConnectionCommand = string.Format("Data Source={0}; Initial Catalog={1};User ID='{2}';password='{3}'",
                server,
                dbname,
                username,
                password
                );
            try
            {
                SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
                return sqlConnection;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }

        }

        public static void closeSqlConnection(SqlConnection sqlConnection)
        {
            try
            {
                sqlConnection.Close();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
            }
        }

        public static void closeDataReader(SqlDataReader dataReader)
        {
            try
            {
                dataReader.Close();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
            }
        }

        /// 
        /// 执行更新SQL语句，返回影响的记录数,

        public static int executeUpdate(string SQLString)
        {

            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = getSqlConnection();
                cmd = new SqlCommand(SQLString, conn);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                throw E;
            }
            finally
            {
                closeSqlConnection(conn);
            }

        }

        ///
        /// 执行查询sql语句,返回SqlDataReader

        public static SqlDataReader executeQuery(SqlConnection sqlConnection, string sqlString)
        {

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = sqlString;
            sqlConnection.Open();
            try
            {
                SqlDataReader reader = sqlCommand.ExecuteReader();
                return reader;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }

        }

    }
}