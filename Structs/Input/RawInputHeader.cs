using CSInputs.Enums;
using System;

namespace CSInputs.Structs.Input
{
    internal struct RawInputHeader
    {
        public RawInputType dwType;
        public int dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;
    }
}
