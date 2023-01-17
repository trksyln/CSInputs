using CSInputs.Enums;
using System.Runtime.InteropServices;

namespace CSInputs.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct RawMouse
    {
        [FieldOffset(0)] public ushort Flags;
        [FieldOffset(4)] public MouseKeys Buttons;
        [FieldOffset(6)] public readonly short ButtonData;
        [FieldOffset(8)] public readonly uint RawButtons;
        [FieldOffset(12)] public int LastX;
        [FieldOffset(16)] public int LastY;
        [FieldOffset(20)] public readonly uint ExtraInformation;
    }
}
