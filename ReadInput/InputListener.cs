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
        private System.Drawing.Point LastCursorPos;
        public InputListener(bool notifyKeyDownsOnce = true)
        {
            NotifyKeyDownsOnce = notifyKeyDownsOnce;
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
                {
                    return;
                }

                if (rw.Header.dwType == Enums.RawInputType.Keyboard && Enum.IsDefined(typeof(Enums.KeyboardKeys), rw.Keyboard.VirtualKey))
                {
                    if (rw.Keyboard.Flags.HasFlag(Enums.KeyFlags.Up))
                    {
                        rw.Keyboard.Flags = Enums.KeyFlags.Up;
                    }
                    else
                    {
                        rw.Keyboard.Flags = Enums.KeyFlags.Down;
                    }

                    if (NotifyKeyDownsOnce && rw.Keyboard.Flags == Enums.KeyFlags.Down && keysAreDown.Contains(rw.Keyboard.VirtualKey))
                    {
                        return;
                    }

                    if (NotifyKeyDownsOnce && rw.Keyboard.Flags == Enums.KeyFlags.Down && !keysAreDown.Contains(rw.Keyboard.VirtualKey))
                    {
                        keysAreDown.Add(rw.Keyboard.VirtualKey);
                    }

                    if (NotifyKeyDownsOnce && rw.Keyboard.Flags == Enums.KeyFlags.Up && keysAreDown.Contains(rw.Keyboard.VirtualKey))
                    {
                        keysAreDown.Remove(rw.Keyboard.VirtualKey);
                    }

                    KeyboardInputs?.Invoke(new Structs.KeyboardData()
                    {
                        Key = rw.Keyboard.VirtualKey,
                        Flags = rw.Keyboard.Flags
                    }, ref ModifierKeys);

                }
                if (rw.Header.dwType == Enums.RawInputType.Mouse)
                {
                    Structs.MouseData data = new Structs.MouseData
                    {
                        Flags = Enum.IsDefined(typeof(Enums.MouseKeys), rw.Mouse.Buttons) ? Enums.KeyFlags.Down : Enums.KeyFlags.Up,
                        PositionRelative = new System.Drawing.Point(rw.Mouse.LastX, rw.Mouse.LastY),
                        PositionAbsolute = Cursor.Position
                    };
                    if (rw.Mouse.Buttons != Enums.MouseKeys.None)
                    {
                        if (data.Flags == Enums.KeyFlags.Up)
                        {
                            data.Key = (Enums.MouseKeys)((int)rw.Mouse.Buttons / 2);
                        }
                        else
                        {
                            data.Key = rw.Mouse.Buttons;
                        }

                        MouseInputs?.Invoke(data, ref ModifierKeys);
                    }
                    else if (rw.Mouse.Buttons == Enums.MouseKeys.None && LastCursorPos != data.PositionAbsolute)
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
