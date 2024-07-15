namespace CSInputs.ReadInput
{
    public class ModifierKey
    {
        public bool LeftShift, RightShift, Control, Alt, LeftWindows, RightWindows;
        public ModifierKey(ReadInput.InputListener listener)
        {
            listener.KeyboardInputs += Listener_KeyboardInputs;
        }

        private void Listener_KeyboardInputs(Structs.KeyboardData data, ref ModifierKey modifierKey)
        {
            switch (data.Key)
            {
                case Enums.KeyboardKeys.LeftShift:
                    LeftShift = !data.Flags.HasFlag(Enums.KeyFlags.KeyUp);
                    break;
                case Enums.KeyboardKeys.RightShift:
                    RightShift = !data.Flags.HasFlag(Enums.KeyFlags.KeyUp);
                    break;
                case Enums.KeyboardKeys.Ctrl:
                    Control = !data.Flags.HasFlag(Enums.KeyFlags.KeyUp);
                    break;
                case Enums.KeyboardKeys.Alt:
                    Alt = !data.Flags.HasFlag(Enums.KeyFlags.KeyUp);
                    break;
                case Enums.KeyboardKeys.LeftWindows:
                    LeftWindows = !data.Flags.HasFlag(Enums.KeyFlags.KeyUp);
                    break;
                case Enums.KeyboardKeys.RightWindows:
                    RightWindows = !data.Flags.HasFlag(Enums.KeyFlags.KeyUp);
                    break;
            }
        }
    }
}
