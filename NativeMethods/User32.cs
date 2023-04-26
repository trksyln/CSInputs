using System;
using System.Runtime.InteropServices;

namespace CSInputs.NativeMethods
{
    internal class User32
    {
        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

        internal static IntPtr GetCSInputsMessage = (IntPtr)new Random().Next(int.MaxValue / 2, int.MaxValue);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, Structs.Input.Input[] pInputs, int cbSize);

        [DllImport("user32", SetLastError = true)]
        internal static extern bool RegisterRawInputDevices(Structs.Input.RawInputDevice[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        internal static extern int GetRawInputData(IntPtr hRawInput, uint uiCommand, out Structs.Input.RawInput pData, ref int pcbSize, int cbSizeHeader);

    }
}
