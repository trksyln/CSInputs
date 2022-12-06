using System.Runtime.InteropServices;

namespace CSInputs.Structs.Input
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct RawInput
    {
        [FieldOffset(0)]
        public RawInputHeader Header;
        [FieldOffset(24)]
        public RawMouse Mouse;
        [FieldOffset(24)]
        public RawKeyboard Keyboard;
    }
}
