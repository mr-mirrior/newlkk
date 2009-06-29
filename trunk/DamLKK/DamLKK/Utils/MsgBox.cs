using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DamLKK.Utils
{
    public static class MB
    {
        private static bool OKCancel(string cap, string title, MessageBoxIcon i)
        {
            return DialogResult.OK ==
                MessageBox.Show(cap, title, MessageBoxButtons.OKCancel, i);
        }
        public static bool OKCancelQ(string cap)
        {
            return OKCancel(cap,"确认", MessageBoxIcon.Question);
        }
        public static void Info(string cap, string title, MessageBoxIcon i)
        {
            MessageBox.Show(cap, title, MessageBoxButtons.OK, i);
        }
        public static void OKI(string cap)
        {
            Info(cap, "成功", MessageBoxIcon.Information);
        }
        public static void OK(string cap)
        {
            Info(cap, "信息", MessageBoxIcon.Information);
        }
        public static bool OKCancelW(string cap, string title)
        {
            return OKCancel(cap, title, MessageBoxIcon.Warning);
        }
        public static void Error(string cap)
        {
            MessageBox.Show(cap, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        public static void Warning(string cap)
        {
            MessageBox.Show(cap, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private static bool YesNo(string cap, string title, MessageBoxIcon i)
        {
            return DialogResult.Yes ==
                MessageBox.Show(cap, title, MessageBoxButtons.YesNo, i);
        }
        public static DialogResult YesNoCancelQ(string cap)
        {
            return MessageBox.Show(cap, "", MessageBoxButtons.YesNoCancel, 
                MessageBoxIcon.Question);
        }
        public static bool YesNoQ(string cap)
        {
            return YesNo(cap, "确认", MessageBoxIcon.Question);
        }
        public static bool YesNoW(string cap, string title)
        {
            return YesNo(cap, title, MessageBoxIcon.Exclamation);
        }
    }
}
