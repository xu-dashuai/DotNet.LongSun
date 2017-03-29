using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace LonsunTicket
{
    #region 播放Mp3
    public class Mp3Player
    {
        //// <summary>
        ///// 使用API
        ///// </summary>
        //static uint SND_ASYNC = 0x0001; // play asynchronously  
        //static uint SND_FILENAME = 0x00020000; // name is file name
        //[DllImport("winmm.dll")]
        //static extern int mciSendString(string m_strCmd, string m_strReceive, int m_v1, int m_v2);

        //[DllImport("Kernel32", CharSet = CharSet.Auto)]
        //static extern Int32 GetShortPathName(String path, StringBuilder shortPath, Int32 shortPathLength);

        public static void Play(string MusicFile)
        {
            //if (!System.IO.File.Exists(MusicFile)) return;
            //StringBuilder shortpath = new StringBuilder(80);
            //int result = GetShortPathName(MusicFile, shortpath, shortpath.Capacity);
            //MusicFile = shortpath.ToString();
            //mciSendString(@"close all", null, 0, 0);
            //mciSendString(@"open " + MusicFile + " alias song", null, 0, 0); //打开
            //mciSendString("play song", null, 0, 0); //播放

            System.Media.SoundPlayer player = new SoundPlayer(MusicFile);
            player.Play();
        }
    }
    #endregion
}
