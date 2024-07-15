using System;

namespace CSInputs.Enums
{
    [Flags]
    public enum KeyFlags : ushort
    {
        KeyDown = 0,
        KeyUp = 1,
        Movement = 2
    }
}
