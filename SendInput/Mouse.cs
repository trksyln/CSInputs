using CSInputs.Enums;
using CSInputs.NativeMethods;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CSInputs.SendInput
{
    public static class Mouse
    {
        public static void Send(MouseKeys key, short scrollAmount = 60)
        {
            Send(key, KeyFlags.KeyDown, Point.Empty, MousePositioning.Relative, scrollAmount);
            if (key == MouseKeys.MouseWheelForward || key == MouseKeys.MouseWheelRight)
                Send(key, KeyFlags.KeyUp, Point.Empty, MousePositioning.Relative, scrollAmount);
        }
        public static void Send(MouseKeys key, KeyFlags flag, short scrollAmount = 60)
        {
            Send(key, flag, Point.Empty, MousePositioning.Relative, scrollAmount);
        }
        public static void Send(MouseKeys key, Point mousePos, MousePositioning mouseMovement, short scrollAmount = 60)
        {
            Send(key, KeyFlags.KeyDown, mousePos, mouseMovement, scrollAmount);
            if (key != MouseKeys.MouseWheelForward || key != MouseKeys.MouseWheelRight)
                Send(key, KeyFlags.KeyUp, mousePos, mouseMovement, scrollAmount);
        }
        public static void Send(MouseKeys key, KeyFlags flags, Point mousePos, MousePositioning mouseMovement, short scrollAmount = 60)
        {

            if (scrollAmount < 0)
                scrollAmount = Math.Abs(scrollAmount);

            if (key != MouseKeys.MouseWheelForward && key != MouseKeys.MouseWheelRight)
                scrollAmount *= -1;

            if (key == MouseKeys.MouseWheelForward || key == MouseKeys.MouseWheelBackward)
                key = (MouseKeys)2048;
            else if (key == MouseKeys.MouseWheelLeft || key == MouseKeys.MouseWheelRight)
                key = (MouseKeys)4096;
            else
                key = flags == KeyFlags.KeyUp ? (MouseKeys)((int)key * 2) : key;


            Structs.Input.Input input = new Structs.Input.Input();
            Structs.Input.Union inputUnion = new Structs.Input.Union()
            {
                mouseInput = new Structs.MouseInput
                {
                    mouseData = scrollAmount,
                    dx = CalculateAbsoluteCoordinateX(mousePos.X, mouseMovement),
                    dy = CalculateAbsoluteCoordinateY(mousePos.Y, mouseMovement),
                    dwFlags = (uint)(mouseMovement == MousePositioning.Absolute ? 0x0001 | 0x8000 : 0x0001) | (key != (MouseKeys)2048 && key != (MouseKeys)4096 ? (uint)key * 2 : (uint)key),
                    dwExtraInfo = User32.GetCSInputsMessage
                }
            };
            input.Data = inputUnion;
            User32.SendInput(1, new Structs.Input.Input[] { input }, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }

        public static void MoveTo(Point mousePos, MousePositioning mouseMovement)
        {
            Structs.Input.Input input = new Structs.Input.Input();
            Structs.Input.Union inputUnion = new Structs.Input.Union()
            {
                mouseInput = new Structs.MouseInput
                {
                    mouseData = 0,
                    dx = CalculateAbsoluteCoordinateX(mousePos.X, mouseMovement),
                    dy = CalculateAbsoluteCoordinateY(mousePos.Y, mouseMovement),
                    dwFlags = (uint)(mouseMovement == MousePositioning.Absolute ? 0x0001 | 0x8000 : 0x0001),
                    dwExtraInfo = User32.GetCSInputsMessage
                }
            };
            input.Data = inputUnion;
            User32.SendInput(1, new Structs.Input.Input[] { input }, Marshal.SizeOf(typeof(Structs.Input.Input)));
        }

        private static int CalculateAbsoluteCoordinateY(int y, MousePositioning mouseMovement)
        {
            if (mouseMovement == MousePositioning.Absolute)
                return (((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y + y) * 65536) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            return y;
        }

        private static int CalculateAbsoluteCoordinateX(int x, MousePositioning mouseMovement)
        {
            if (mouseMovement == MousePositioning.Absolute)
                return ((System.Windows.Forms.Screen.PrimaryScreen.Bounds.X + x) * 65536) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return x;
        }
    }
}