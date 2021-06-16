using CSInputs.Enums;
using System.Runtime.InteropServices;

namespace CSInputs.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawKeyboard
    {
        public short MakeCode;
        public KeyFlags Flags;
        public short Reserved;
        public KeyboardKeys VirtualKey;
        public int Message;
        public int ExtraInformation;
    }
}
