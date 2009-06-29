using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DamLKK.DB
{
    public class LoginDAO
    {
        private static LoginDAO _MyInstance = null;

        private LoginDAO(){}

        public static LoginDAO GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new LoginDAO();
            }
            return _MyInstance;
        }

       
        public _Control.UserInfo Login(string p_username,string p_password)
        {
            try
            {
                _Control.UserInfo user = new _Control.UserInfo();
                SqlConnection conn = DBConnection.getSqlConnection();
                SqlDataReader reader = DBConnection.executeQuery(conn, "select userpassword,userclass from userlist where loginname='" + p_username + "'");
                if (reader.Read())
                {
                    user.LoginPassword=reader["userpassword"].ToString();
                    user.Authority=(_Control.LoginResult)GetIntAuth(reader["userclass"]);
                }
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
                return user;
            }
            catch (System.Exception e)
            {
                DamLKK.Utils.DebugUtil.log(e);
                throw e;
            }
        }

        private int GetIntAuth(object p_obj)
        {
            if (p_obj.ToString().Equals("浏览"))
            {
                return 4;
            }
            else if (p_obj.ToString().Equals("操作"))
            {
                return 1;
            }
            else if (p_obj.ToString().Equals("管理员"))
            {
                return 2;
            }
            else if (p_obj.ToString().Equals("不报警"))
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
    }
}
