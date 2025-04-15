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
        /// <param name="notifyKeyDownsOnce">When key is down, notify once or repeat until key up is sent</param>
        /// <param name="ignoreSelf">Ignore keys sent from this CSInputs instance.</param>
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

                    if ((rw.Data.Keyboard.Flags & Enums.KeyFlags.KeyUp) != 0)
                        rw.Data.Keyboard.Flags = Enums.KeyFlags.KeyUp;
                    else
                        rw.Data.Keyboard.Flags = Enums.KeyFlags.KeyDown;

                    if (NotifyKeyDownsOnce && rw.Data.Keyboard.Flags == Enums.KeyFlags.KeyDown && keysAreDown.Contains(rw.Data.Keyboard.VirtualKey))
                        return;

                    if (NotifyKeyDownsOnce && rw.Data.Keyboard.Flags == Enums.KeyFlags.KeyDown && !keysAreDown.Contains(rw.Data.Keyboard.VirtualKey))
                        keysAreDown.Add(rw.Data.Keyboard.VirtualKey);

                    if (NotifyKeyDownsOnce && rw.Data.Keyboard.Flags == Enums.KeyFlags.KeyUp && keysAreDown.Contains(rw.Data.Keyboard.VirtualKey))
                        keysAreDown.Remove(rw.Data.Keyboard.VirtualKey);


                    #region Shift key left right translation
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

                    Structs.MouseData baseData = new Structs.MouseData
                    {
                        PositionRelative = new System.Drawing.Point(rw.Data.Mouse.LastX, rw.Data.Mouse.LastY),
                        PositionAbsolute = Cursor.Position
                    };
                    if (rw.Data.Mouse.Buttons != Enums.MouseKeys.None)
                    {
                        int[,] mouseButtonFlags = new int[,]
                        {
                            { 0x0001, 0x0002, (int)Enums.MouseKeys.MouseLeft },
                            { 0x0004, 0x0008, (int)Enums.MouseKeys.MouseRight },
                            { 0x0010, 0x0020, (int)Enums.MouseKeys.MouseMiddle },
                            { 0x0040, 0x0080, (int)Enums.MouseKeys.XButton1 },
                            { 0x0100, 0x0200, (int)Enums.MouseKeys.XButton2 }
                        };

                        // vertical wheel
                        if (rw.Data.Mouse.Buttons == Enums.MouseKeys.RI_MOUSE_WHEEL)
                        {
                            var data = baseData;
                            short delta = (short)rw.Data.Mouse.ButtonData;
                            data.Key = delta > 0 ? Enums.MouseKeys.MouseWheelForward : Enums.MouseKeys.MouseWheelBackward;
                            data.Flags = Enums.KeyFlags.KeyDown;
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                            data.Flags = Enums.KeyFlags.KeyUp;
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                        }

                        // horizontal wheel
                        if (rw.Data.Mouse.Buttons == Enums.MouseKeys.RI_MOUSE_HWHEEL)
                        {
                            var data = baseData;
                            short delta = (short)rw.Data.Mouse.ButtonData;
                            data.Key = delta > 0 ? Enums.MouseKeys.MouseWheelRight : Enums.MouseKeys.MouseWheelLeft;
                            data.Flags = Enums.KeyFlags.KeyDown;
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                            data.Flags = Enums.KeyFlags.KeyUp;
                            MouseInputs?.Invoke(data, ref ModifierKeys);
                        }

                        for (int i = 0; i < mouseButtonFlags.GetLength(0); i++)
                        {
                            if (((int)rw.Data.Mouse.Buttons & mouseButtonFlags[i, 0]) != 0)
                            {
                                var data = baseData;
                                data.Key = (Enums.MouseKeys)mouseButtonFlags[i, 2];
                                data.Flags = Enums.KeyFlags.KeyDown;
                                MouseInputs?.Invoke(data, ref ModifierKeys);
                            }

                            if (((int)rw.Data.Mouse.Buttons & mouseButtonFlags[i, 1]) != 0)
                            {
                                var data = baseData;
                                data.Key = (Enums.MouseKeys)mouseButtonFlags[i, 2];
                                data.Flags = Enums.KeyFlags.KeyUp;
                                MouseInputs?.Invoke(data, ref ModifierKeys);
                            }
                        }

                    }
                    else if (rw.Data.Mouse.Buttons == Enums.MouseKeys.None && LastCursorPos != baseData.PositionAbsolute)
                    {
                        LastCursorPos = baseData.PositionAbsolute;
                        MouseMovements?.Invoke(baseData, ref ModifierKeys);
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
