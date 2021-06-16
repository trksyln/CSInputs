using CSInputs.NativeMethods;
using System.Runtime.InteropServices;

namespace CSInputs.SendInput
{
    public static class Keyboard
    {
        public static void Send(Enums.KeyboardKeys key)
        {
            Send(key, Enums.KeyFlags.Down);
            System.Threading.Thread.Sleep(1);
            Send(key, Enums.KeyFlags.Up);
        }
        public static void Send(Enums.KeyboardKeys key, Enums.KeyFlags flags)
        {
            uint keyFlags = 0;
            switch (flags)
            {
                case Enums.KeyFlags.Down:
                    keyFlags = 0x0000 | 0x0008;
                    break;
                case Enums.KeyFlags.Up:
                    keyFlags = 0x0002 | 0x0008;
                    break;
            }
            if (key == Enums.KeyboardKeys.LeftWindows || key == Enums.KeyboardKeys.RightWindows)
            {
                keyFlags |= 0x0001;
            }

            Structs.Input.Input input = new Structs.Input.Input();
            Structs.Input.Union inputUnion = new Structs.Input.Union()
            {
                keyboardInput = new Structs.KeyboardInput
                {
                    wVk = 0,
                    wScan = (ushort)User32.MapVirtualKey((uint)key, 0x00),
                    dwFlags = keyFlags,
                    dwExtraInfo = User32.GetMessageExtraInfo()
                }
            };
            input.type = 1;
            input.Data = inputUnion;
            User32.SendInput(1, new Structs.Input.Input[] { input }, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }

        public static void SendString(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                SendChar(text[i]);
            }
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
                    dwExtraInfo = User32.GetMessageExtraInfo()
                }
            };
            Structs.Input.Union up = new Structs.Input.Union()
            {
                keyboardInput = new Structs.KeyboardInput
                {
                    wVk = 0,
                    wScan = character,
                    dwFlags = 0x0004 | 0x0002,
                    dwExtraInfo = User32.GetMessageExtraInfo()
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
