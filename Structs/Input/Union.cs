using System.Runtime.InteropServices;

namespace CSInputs.Structs.Input
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Union
    {
        [FieldOffset(0)] public MouseInput mouseInput;
        [FieldOffset(0)] public KeyboardInput keyboardInput;
    }
}
