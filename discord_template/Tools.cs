﻿using System.Runtime.Serialization.Formatters.Binary;

namespace voicevox_discord
{
    public static class Tools
    {
        //nullもしくは空の時にtrue
        public static bool IsNullOrEmpty(this string? str)
        {
            if (str == null) { return true; }
            if (str == "") { return true; }
            return false;
        }
        
        //ポート番号出ない時にtrue
        public static bool IsNotPortNumber(this int? port)
        {
            if (port == null) { return true; }
            if(port < 1 || port > 65535) { return true; }

            return false;
        }
    }
}
