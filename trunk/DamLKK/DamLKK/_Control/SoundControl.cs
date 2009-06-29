using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace DamLKK._Control
{
    public enum SoundType
    {
        SIMPLEBEEP = -1,          // Simple beep. If the sound card is not available, the sound is generated using the speaker.
                                            // Note that this value is resolved to 0xFFFFFFFF within the function.
        MB_ICONASTERISK = 0x00000040, // SystemAsterisk
        MB_ICONEXCLAMATION = 0x00000030, // SystemExclamation
        MB_ICONHAND = 0x00000010, // SystemHand
        MB_ICONQUESTION = 0x00000020, //SystemQuestion
        MB_OK = 0x00000000 //SystemDefault
    }
    public static class SoundControl
    {
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint uType);
        public static void Beep()
        {
            _Control.SoundControl.MessageBeep((uint)0xffffffff);
        }

        [DllImport("winmm.dll")]   
        private static extern int sndPlaySoundA(byte[] lpszSoundName, int uFlags);

        public static void Init()
        {
            try
            {
                FileStream fs = new FileStream("TrackPoint.wav", FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                wavefile = new byte[fs.Length];
                br.Read(wavefile, 0, (int)fs.Length);
                br.Close();
                fs.Close();

                BeepII();
            }
            catch (System.Exception e)
            {
                Utils.MB.OK(e.Message);
            }
        }
        static byte[] wavefile = null;
        //API定义   
        private const int SND_ASYNC = 0x1;
        private const int SND_MEMORY = 0x4;
        public static void BeepII()
        {
            //将K2.wav添加入工程并设置为嵌入的资源   
            //现在是将它读入内存备用   
            if( wavefile != null )
                //播放缓存   
                sndPlaySoundA(wavefile, SND_ASYNC);

            //这个方法只能播放wav,首先一定要把wav文件添加到工程中,然后点击属性,在生成操作的选项中,选择嵌入的资源.

        }

        [DllImport("winmm.dll")]
        public static extern long PlaySound(string lpszName, int hModule, int dwFlags);  
        public static void BeepIII()
        {
            PlaySound("TrackPoint.wav", 0, SND_ASYNC);
        }
    }
}
