using System.Runtime.InteropServices;

namespace CSInputs.Structs.Input
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInput
    {
        internal RawInputHeader Header;
        internal Union Data;

        [StructLayout(LayoutKind.Explicit)]
        internal struct Union
        {
            [FieldOffset(0)]
            internal RawMouse Mouse;
            [FieldOffset(0)]
            internal RawKeyboard Keyboard;
        }
    }
}

