namespace CSInputs.Structs
{
    public struct MouseData
    {
        public Enums.MouseKeys Key { get; set; }
        public Enums.KeyFlags Flags { get; set; }
        public System.Drawing.Point PositionAbsolute { get; set; }
        public System.Drawing.Point PositionRelative { get; set; }
    }
}
