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
                    LeftShift = (data.Flags & Enums.KeyFlags.KeyUp) == 0;
                    break;
                case Enums.KeyboardKeys.RightShift:
                    RightShift = (data.Flags & Enums.KeyFlags.KeyUp) == 0;
                    break;
                case Enums.KeyboardKeys.Ctrl:
                    Control = (data.Flags & Enums.KeyFlags.KeyUp) == 0;
                    break;
                case Enums.KeyboardKeys.Alt:
                    Alt = (data.Flags & Enums.KeyFlags.KeyUp) == 0;
                    break;
                case Enums.KeyboardKeys.LeftWindows:
                    LeftWindows = (data.Flags & Enums.KeyFlags.KeyUp) == 0;
                    break;
                case Enums.KeyboardKeys.RightWindows:
                    RightWindows = (data.Flags & Enums.KeyFlags.KeyUp) == 0;
                    break;
            }
        }
    }
}
