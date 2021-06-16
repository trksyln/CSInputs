using CSInputs.NativeMethods;
using System.Runtime.InteropServices;

namespace CSInputs.SendInput
{
    public static class Mouse
    {
        public static void Send(Enums.MouseKeys key)
        {
            Send(key, Enums.KeyFlags.Down, System.Drawing.Point.Empty, Enums.MousePositioning.Relative);
            Send(key, Enums.KeyFlags.Up, System.Drawing.Point.Empty, Enums.MousePositioning.Relative);
        }
        public static void Send(Enums.MouseKeys key, Enums.KeyFlags flag)
        {
            Send(key, flag, System.Drawing.Point.Empty, Enums.MousePositioning.Relative);
        }
        public static void Send(Enums.MouseKeys key, System.Drawing.Point mousePos, Enums.MousePositioning mouseMovement)
        {
            Send(key, Enums.KeyFlags.Down, mousePos, mouseMovement);
            Send(key, Enums.KeyFlags.Up, mousePos, mouseMovement);
        }
        public static void Send(Enums.MouseKeys key, Enums.KeyFlags flags, System.Drawing.Point mousePos, Enums.MousePositioning mouseMovement)
        {
            key = flags == Enums.KeyFlags.Up ? (Enums.MouseKeys)((int)key * 2) : key;
            Structs.Input.Input input = new Structs.Input.Input();
            Structs.Input.Union inputUnion = new Structs.Input.Union()
            {
                mouseInput = new Structs.MouseInput
                {
                    mouseData = 0,
                    dx = CalculateAbsoluteCoordinateX(mousePos.X, mouseMovement),
                    dy = CalculateAbsoluteCoordinateY(mousePos.Y, mouseMovement),
                    dwFlags = (uint)(mouseMovement == Enums.MousePositioning.Absolute ? 0x0001 | 0x8000 : 0x0001) | ((uint)key * 2),
                    dwExtraInfo = User32.GetMessageExtraInfo()
                }
            };
            input.Data = inputUnion;
            User32.SendInput(1, new Structs.Input.Input[] { input }, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }

        public static void MoveTo(System.Drawing.Point mousePos, Enums.MousePositioning mouseMovement)
        {
            Structs.Input.Input input = new Structs.Input.Input();
            Structs.Input.Union inputUnion = new Structs.Input.Union()
            {
                mouseInput = new Structs.MouseInput
                {
                    mouseData = 0,
                    dx = CalculateAbsoluteCoordinateX(mousePos.X, mouseMovement),
                    dy = CalculateAbsoluteCoordinateY(mousePos.Y, mouseMovement),
                    dwFlags = (uint)(mouseMovement == Enums.MousePositioning.Absolute ? 0x0001 | 0x8000 : 0x0001),
                    dwExtraInfo = User32.GetMessageExtraInfo()
                }
            };
            input.Data = inputUnion;
            User32.SendInput(1, new Structs.Input.Input[] { input }, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }

        private static int CalculateAbsoluteCoordinateY(int y, Enums.MousePositioning mouseMovement)
        {
            if (mouseMovement == Enums.MousePositioning.Absolute)
            {
                return (((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y + y) * 65536) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            }
            else
            {
                return y;
            }
        }

        private static int CalculateAbsoluteCoordinateX(int x, Enums.MousePositioning mouseMovement)
        {
            if (mouseMovement == Enums.MousePositioning.Absolute)
            {
                return ((System.Windows.Forms.Screen.PrimaryScreen.Bounds.X + x) * 65536) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            }
            else
            {
                return x;
            }
        }
    }
}
