using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Security;
using System.Threading;
using DamLKK._Control;

namespace DamLKK.Forms
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        List<string> _RetainedUsers;
     
        private void loginbtn_Click(object sender, EventArgs e)
        {
            //账号密码不能为空
            if (cbuserName.Text==string.Empty || tbPassWord.Text==string.Empty)
            {
                lbregister.Text = "用户名密码不能为空！";
                return;
            }

            //用户名密码输入错误
            _Control.LoginResult loginResult = _Control.LoginControl.Login(cbuserName.Text, tbPassWord.Text);

            //登录判断
            if (loginResult == DamLKK._Control.LoginResult.INVALID_PASSWORD || loginResult == DamLKK._Control.LoginResult.INVALID_USER)
                lbregister.Text = "用户名密码输入错误！请重试。";
            else if (loginResult== DamLKK._Control.LoginResult.ERROR)
                lbregister.Text = "数据库访问错误！请检查网络连接或服务器状态。";
            else
                DialogResult = DialogResult.OK;

            if (DialogResult == DialogResult.OK)
                this.Close();
           
            //失败沉睡1秒
            if (!LoginOKorNO(loginResult))
            {
                Thread.Sleep(1000);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void DMLogin_Load(object sender, EventArgs e)
        {
            _RetainedUsers = Utils.Xml.XMLUtil<List<string>>.LoadXml("UserInfo.xml");

            if (_RetainedUsers == null)
                return;

            foreach (string ui in _RetainedUsers)
            {
                cbuserName.Items.Add(ui);
            }
        }
      
        /// <summary>
        /// 判断登录是否成功
        /// </summary>
        /// <param name="LResult"></param>
        /// <returns></returns>
        private bool LoginOKorNO(_Control.LoginResult LResult)
        {
            if (LResult == DamLKK._Control.LoginResult.ERROR || LResult == DamLKK._Control.LoginResult.INVALID_PASSWORD || LResult == DamLKK._Control.LoginResult.INVALID_USER)
                return false;
            else
                return true;
        }

        //成功登录保存用户名
        private void DMLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (LoginControl.User.Authority == LoginResult.ADMIN || LoginControl.User.Authority == LoginResult.OPERATOR)
            {
                if (cbRetainMe.Checked)
                {
                    foreach (string s in _RetainedUsers)
                    {
                        if (cbuserName.Text.Equals(s))
                            return;
                    }
                    _RetainedUsers.Add(cbuserName.Text);
                }

                Utils.Xml.XMLUtil<List<string>>.SaveXml("UserInfo.xml", _RetainedUsers);
            }
            if (LoginControl.User.Authority == LoginResult.ERROR || LoginControl.User.Authority == LoginResult.INVALID_PASSWORD || LoginControl.User.Authority == LoginResult.INVALID_USER)
            {
                if (e.CloseReason != CloseReason.UserClosing)
                    e.Cancel = true;
            }
        }
    }
}
