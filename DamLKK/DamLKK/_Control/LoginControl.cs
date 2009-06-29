using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK._Control
{
    public struct UserInfo
    {
        string _LoginName;
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName
        {
            get { return _LoginName; }
            set { _LoginName = value; }
        }
        string _LoginPassword;
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword
        {
            get { return _LoginPassword; }
            set { _LoginPassword = value; }
        }
        LoginResult _Authority;
        /// <summary>
        /// 权限
        /// </summary>
        public LoginResult Authority
        {
            get { return _Authority; }
            set { _Authority = value; }
        }
    }

    /// <summary>
    /// 登录返回枚举
    /// </summary>
    public enum LoginResult
    {
        INVALID_USER = -2,
        INVALID_PASSWORD,
        /*{"浏览","操作","管理员","不报警"};*/
        ERROR, OPERATOR, ADMIN, DISWARNING, VIEW
    }

    /// <summary>
    /// 登录控制
    /// </summary>
    public static class LoginControl
    {
        private static UserInfo _User;
        /// <summary>
        /// 软件使用者
        /// </summary>
        public static UserInfo User
        {
            get { return LoginControl._User; }
            set { LoginControl._User = value; }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="p_username"></param>
        /// <param name="p_password"></param>
        /// <returns></returns>
        public static LoginResult Login(string p_username, string p_password)
        {
            try
            {
                _User = DB.LoginDAO.GetInstance().Login(p_username,p_password);
            }
            catch (System.Exception e)
            {
                DamLKK.Utils.DebugUtil.log(e);
                return LoginResult.ERROR;
            }

            if (_User.LoginName==string.Empty)
            {
                return LoginResult.INVALID_USER;
            }
            else if (!p_password.Equals(_User.LoginPassword))
            {
                return LoginResult.INVALID_PASSWORD;
            }

            return _User.Authority;
        }
    }
}
