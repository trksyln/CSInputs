using System;
using System.Runtime.InteropServices;

namespace CSInputs.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseInput
    {
        public int dx;
        public int dy;
        public ushort mouseData;
        public uint dwFlags;
        public readonly uint time;
        public IntPtr dwExtraInfo;
    }
}
