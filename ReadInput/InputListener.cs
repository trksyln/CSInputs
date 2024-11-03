using CSInputs.Extensions;
using CSInputs.NativeMethods;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CSInputs.ReadInput
{
    public class InputListener : NativeWindow
    {
        public event KeyboardDataDel KeyboardInputs;
        public event MouseDataDel MouseInputs;
        public event MouseDataDel MouseMovements;
        public delegate void KeyboardDataDel(Structs.KeyboardData data, ref ModifierKey modifierKey);
        public delegate void MouseDataDel(Structs.MouseData data, ref ModifierKey modifierKey);

        private ModifierKey ModifierKeys;
        private readonly List<Enums.KeyboardKeys> keysAreDown = new List<Enums.KeyboardKeys>();
        private readonly bool NotifyKeyDownsOnce = true;
        private readonly bool IgnoreSelf = true;
        private System.Drawing.Point LastCursorPos;

        /// <summary>
        /// Input listener for mouse and keyboard
        /// </summary>
        /// <param name="notifyKeyDownsOnce">When key down, notify once or until key up</param>
        /// <param name="ignoreSelf">Ignore keys, sends from this listener</param>
        public InputListener(bool notifyKeyDownsOnce = true, bool ignoreSelf = true)
        {
            //this.AssignHandle(Handle);
            NotifyKeyDownsOnce = notifyKeyDownsOnce;
            IgnoreSelf = ignoreSelf;
            CreateHandle(new CreateParams
            {
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Style = 0x800000,
            });
            RegisterDevices(Handle);
            ModifierKeys = new ModifierKey(this);
        }

        private static void RegisterDevices(IntPtr handle)
        {
            Structs.Input.RawInputDevice[] devs = new Structs.Input.RawInputDevice[]
                {
                            new Structs.Input.RawInputDevice()
                            {
                                Usage = (ushort)Enums.HIDUsage.Mouse,
                                Flags = 0x00000100,
                                UsagePage =1,
                                HwndTarget = handle
                            },
                            new Structs.Input.RawInputDevice()
                            {
                                Usage = (ushort)Enums.HIDUsage.Keyboard,
                                Flags = 0x00000100,
                                UsagePage =1,
                                HwndTarget = handle
                            }
            };
            User32.RegisterRawInputDevices(devs, (uint)devs.Length, (uint)Marshal.SizeOf(typeof(Structs.Input.RawInputDevice)));
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_INPUT = 0x00FF;
            if (m.Msg == WM_INPUT)
            {
                int rwSize = Marshal.SizeOf(typeof(Structs.Input.RawInput));
                int hdSize = Marshal.SizeOf(typeof(Structs.Input.RawInputHeader));
                int res = User32.GetRawInputData(m.LParam, 0x10000003, out Structs.Input.RawInput rw, ref rwSize, hdSize);
                if (res < 0)
                    return;

                if (KeyboardInputs != null && rw.Header.dwType == Enums.RawInputType.Keyboard && Enum.IsDefined(typeof(Enums.KeyboardKeys), rw.Data.Keyboard.VirtualKey))
                {
                    if (IgnoreSelf && rw.Data.Keyboard.ExtraInformation == (int)User32.GetCSInputsMessage)
                        return;

                    if (rw.Data.Keyboard.Flags.HasFlag(Enums.KeyFlags.KeyUp))
                        rw.Data.Keyboard.Flags = Enums.KeyFlags.KeyUp;
                    else
                        rw.Data.Keyboard.Flags = Enums.KeyFlags.KeyDown;

                    if (NotifyKeyDownsOnce && rw.Data.Keyboard.Flags == Enums.KeyFlags.KeyDown && keysAreDown.Contains(rw.Data.Keyboard.VirtualKey))
                        return;

                    if (NotifyKeyDownsOnce && rw.Data.Keyboard.Flags == Enums.KeyFlags.KeyDown && !keysAreDown.Contains(rw.Data.Keyboard.VirtualKey))
                        keysAreDown.Add(rw.Data.Keyboard.VirtualKey);

                    if (NotifyKeyDownsOnce && rw.Data.Keyboard.Flags == Enums.KeyFlags.KeyUp && keysAreDown.Contains(rw.Data.Keyboard.VirtualKey))
                        keysAreDown.Remove(rw.Data.Keyboard.VirtualKey);


                    #region Shieft key left right translation
                    if (rw.Data.Keyboard.VirtualKey == Enums.KeyboardKeys.Shift)
                        rw.Data.Keyboard.VirtualKey = rw.Data.Keyboard.MakeCode == 0x2A ? Enums.KeyboardKeys.LeftShift : Enums.KeyboardKeys.RightShift;
                    #endregion

                    KeyboardInputs?.Invoke(new Structs.KeyboardData()
                    {
                        Key = rw.Data.Keyboard.VirtualKey,
                        Flags = rw.Data.Keyboard.Flags
                    }, ref ModifierKeys);

                }
                if ((MouseMovements != null || MouseInputs != null) && rw.Header.dwType == Enums.RawInputType.Mouse)
                {

                    if (IgnoreSelf && rw.Data.Mouse.ExtraInformation == (int)User32.GetCSInputsMessage)
                        return;

                    Structs.MouseData data = new Structs.MouseData
                    {
                        Flags = Enum.IsDefined(typeof(Enums.MouseKeys), rw.Data.Mouse.Buttons) ? Enums.KeyFlags.KeyDown : Enums.KeyFlags.KeyUp,
                        PositionRelative = new System.Drawing.Point(rw.Data.Mouse.LastX, rw.Data.Mouse.LastY),
                        PositionAbsolute = Cursor.Position
                    };
                    if (rw.Data.Mouse.Buttons != Enums.MouseKeys.None)
                    {
                        if ((ushort)rw.Data.Mouse.Buttons == 1024 && rw.Data.Mouse.ButtonData > 0)
                            data.Key = Enums.MouseKeys.MouseWheelForward;
                        else if ((ushort)rw.Data.Mouse.Buttons == 1024 && rw.Data.Mouse.ButtonData < 0)
                            data.Key = Enums.MouseKeys.MouseWheelBackward;
                        else if ((ushort)rw.Data.Mouse.Buttons == 2048 && rw.Data.Mouse.ButtonData > 0)
                            data.Key = Enums.MouseKeys.MouseWheelRight;
                        else if ((ushort)rw.Data.Mouse.Buttons == 2048 && rw.Data.Mouse.ButtonData < 0)
                            data.Key = Enums.MouseKeys.MouseWheelLeft;
                        else
                        {
                            if (data.Flags == Enums.KeyFlags.KeyUp)
                                data.Key = (Enums.MouseKeys)((int)rw.Data.Mouse.Buttons / 2);
                            else
                                data.Key = rw.Data.Mouse.Buttons;
                        }
                        if ((ushort)rw.Data.Mouse.Buttons == 1024 || (ushort)rw.Data.Mouse.Buttons == 2048)
                        {
                            data.Flags = Enums.KeyFlags.KeyDown;
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                            data.Flags = Enums.KeyFlags.KeyUp;
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                        }
                        else
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                    }
                    else if (rw.Data.Mouse.Buttons == Enums.MouseKeys.None && LastCursorPos != data.PositionAbsolute)
                    {
                        LastCursorPos = data.PositionAbsolute;
                        MouseMovements?.Invoke(data, ref ModifierKeys);
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
