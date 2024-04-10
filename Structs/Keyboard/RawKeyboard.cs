using CSInputs.Enums;
using System.Runtime.InteropServices;

namespace CSInputs.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawKeyboard
    {
        public ushort MakeCode;
        public KeyFlags Flags;
        public ushort Reserved;
        public KeyboardKeys VirtualKey;
        public int Message;
        public int ExtraInformation;
    }
}
