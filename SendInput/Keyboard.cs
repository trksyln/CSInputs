using CSInputs.NativeMethods;
using System.Runtime.InteropServices;

namespace CSInputs.SendInput
{
    public static class Keyboard
    {
        public static void Send(Enums.KeyboardKeys key)
        {
            Send(key, Enums.KeyFlags.KeyDown);
            System.Threading.Thread.Sleep(1);
            Send(key, Enums.KeyFlags.KeyUp);
        }
        public static void Send(Enums.KeyboardKeys key, Enums.KeyFlags flags)
        {
            uint keyFlags = 0x0008; // KEYEVENTF_SCANCODE;
            switch (flags)
            {
                case Enums.KeyFlags.KeyDown:
                    keyFlags |= 0x0000; // KEYEVENTF_KEYDOWN
                    break;
                case Enums.KeyFlags.KeyUp:
                    keyFlags |= 0x0002; // KEYEVENTF_KEYUP
                    break;
            }


            if (key == Enums.KeyboardKeys.LeftWindows || key == Enums.KeyboardKeys.RightWindows ||
        key == Enums.KeyboardKeys.Left || key == Enums.KeyboardKeys.Right ||
        key == Enums.KeyboardKeys.Up || key == Enums.KeyboardKeys.Down ||
        key == Enums.KeyboardKeys.Insert || key == Enums.KeyboardKeys.Delete ||
        key == Enums.KeyboardKeys.Home || key == Enums.KeyboardKeys.End ||
        key == Enums.KeyboardKeys.PageUp || key == Enums.KeyboardKeys.PageDown ||
        key == Enums.KeyboardKeys.NumEnter || key == Enums.KeyboardKeys.RightControl ||
        key == Enums.KeyboardKeys.Alt)
                keyFlags |= 0x0001; // KEYEVENTF_EXTENDEDKEY


            Structs.Input.Input input = new Structs.Input.Input();
            Structs.Input.Union inputUnion = new Structs.Input.Union()
            {
                keyboardInput = new Structs.KeyboardInput
                {
                    wVk = 0,
                    wScan = (ushort)User32.MapVirtualKey((uint)key, 0x00),
                    dwFlags = keyFlags,
                    dwExtraInfo = User32.GetCSInputsMessage
                }
            };
            input.type = 1;
            input.Data = inputUnion;
            User32.SendInput(1, new Structs.Input.Input[] { input }, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }

        public static void SendString(string text)
        {
            for (int i = 0; i < text.Length; i++)
                SendChar(text[i]);
        }
        public static void SendChar(char character)
        {
            Structs.Input.Input[] inputs = new Structs.Input.Input[2];
            Structs.Input.Union down = new Structs.Input.Union()
            {
                keyboardInput = new Structs.KeyboardInput
                {
                    wVk = 0,
                    wScan = character,
                    dwFlags = 0x0004,
                    dwExtraInfo = User32.GetCSInputsMessage
                }
            };
            Structs.Input.Union up = new Structs.Input.Union()
            {
                keyboardInput = new Structs.KeyboardInput
                {
                    wVk = 0,
                    wScan = character,
                    dwFlags = 0x0004 | 0x0002,
                    dwExtraInfo = User32.GetCSInputsMessage
                }
            };
            inputs[0].type = 1;
            inputs[0].Data = down;
            inputs[1].type = 1;
            inputs[1].Data = up;
            User32.SendInput(2, inputs, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }
    }
}
