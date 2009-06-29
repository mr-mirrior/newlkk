using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DamLKK.Utils.Sys
{
    public static class SysUtils
    {
        public static bool StartProgram(string path, string param)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = param;
            proc.Start();
            return true;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct NONCLIENTMETRICS
        {
            public int cbSize;
            public int iBorderWidth;
            public int iScrollWidth;
            public int iScrollHeight;
            public int iCaptionWidth;
            public int iCaptionHeight;
            public LOGFONT lfCaptionFont;
            public int iSmCaptionWidth;
            public int iSmCaptionHeight;
            public LOGFONT lfSmCaptionFont;
            public int iMenuWidth;
            public int iMenuHeight;
            public LOGFONT lfMenuFont;
            public LOGFONT lfStatusFont;
            public LOGFONT lfMessageFont;
        }

        const int SPI_GETNONCLIENTMETRICS = 0x0029;
        const int SPI_SETNONCLIENTMETRICS = 0x002A;
        const int SPI_GETFONTSMOOTHING  = 74;
        const int SPI_SETFONTSMOOTHING = 75;

        [DllImport("user32.dll ", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uiAction, int uiParam,
        ref   NONCLIENTMETRICS ncMetrics, int fWinIni);

//         private static NONCLIENTMETRICS GetSysParam(int options, ref object x)
//         {
//             NONCLIENTMETRICS nm = new NONCLIENTMETRICS();
//             nm.cbSize = Marshal.SizeOf(nm);
//             SystemParametersInfo(SPI_GETNONCLIENTMETRICS, nm.cbSize, ref nm, 0);
//             return nm;
//         }
//         private static void SetSysParam(int options, NONCLIENTMETRICS nm)
//         {
// //             NONCLIENTMETRICS nm = new NONCLIENTMETRICS();
//             int nSize = Marshal.SizeOf(nm);
//             nm.cbSize = nSize;
//             //SystemParametersInfo(SPI_GETNONCLIENTMETRICS, nSize, ref   nm, 0);
//             //nm.iCaptionHeight = 20;
//             SystemParametersInfo(SPI_SETNONCLIENTMETRICS, nSize, ref   nm, 0);
//             //GetCaptionFont(); 
//         }
// 
        public static void SetClearType()
        {
//             NONCLIENTMETRICS nm = new NONCLIENTMETRICS();
//             nm.cbSize = Marshal.SizeOf(nm);
//             SystemParametersInfo(SPI_GETNONCLIENTMETRICS)
        }

    }
}
