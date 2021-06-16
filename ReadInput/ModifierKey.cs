namespace CSInputs.ReadInput
{
    public class ModifierKey
    {
        public bool Shift, Control, Alt, Windows;
        public ModifierKey(ReadInput.InputListener listener)
        {
            listener.KeyboardInputs += Listener_KeyboardInputs;
        }

        private void Listener_KeyboardInputs(Structs.KeyboardData data, ref ModifierKey modifierKey)
        {
            if (data.Key == Enums.KeyboardKeys.Shift)
            {
                Shift = !data.Flags.HasFlag(Enums.KeyFlags.Up);
            }

            if (data.Key == Enums.KeyboardKeys.Ctrl)
            {
                Control = !data.Flags.HasFlag(Enums.KeyFlags.Up);
            }

            if (data.Key == Enums.KeyboardKeys.Alt)
            {
                Alt = !data.Flags.HasFlag(Enums.KeyFlags.Up);
            }
            if (data.Key == Enums.KeyboardKeys.LeftWindows || data.Key == Enums.KeyboardKeys.RightWindows)
            {
                Windows = !data.Flags.HasFlag(Enums.KeyFlags.Up);
            }
        }

        //public void KeyboardInputs(object sender, EventHandlers.KeyboardDataEventArgs key)
        //{
        //    if (key.Keyboard.Key == Enums.VirtualKeys.Shift)
        //        ShiftPressed = !key.Keyboard.Flags.HasFlag(Enums.KeyFlags.Up);

        //    if (key.Keyboard.Key == Enums.VirtualKeys.Ctrl)
        //        ControlPressed = !key.Keyboard.Flags.HasFlag(Enums.KeyFlags.Up);

        //    if (key.Keyboard.Key == Enums.VirtualKeys.Alt)
        //        AltPressed = !key.Keyboard.Flags.HasFlag(Enums.KeyFlags.Up);
        //}
    }
}
