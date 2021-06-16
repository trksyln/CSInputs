using System;
using System.Runtime.InteropServices;

namespace CSInputs.Structs.Input
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDevice
    {
        public ushort UsagePage;
        public ushort Usage;
        public uint Flags;
        public IntPtr HwndTarget;
    }
}
