using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Pong
{
    class NativeFontUtils
    {
        public const int FR_PRIVATE = 0x10;
        public const int FR_NOT_ENUM = 0x20;

        [DllImport("gdi32.dll")]
        public static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

        [DllImport("gdi32.dll")]
        public static extern bool RemoveFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);
        
    }
}
