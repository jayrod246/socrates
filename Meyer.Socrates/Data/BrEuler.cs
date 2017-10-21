namespace Meyer.Socrates.Data
{
    public struct BrEuler
    {
        public const int Size = 8;
        public BrAngle A { get; set; }
        public BrAngle B { get; set; }
        public BrAngle C { get; set; }
        public BrEulerFlags Order { get; set; }
    }
}
