namespace Meyer.Socrates.Data
{
    public struct BrLight
    {
        public BrMatrix3x4 TransformMatrix { get; set; }

        public BrScalar Attenuation { get; set; }

        public BrLightType LightType
        {
            get => lightType;
            set => lightType = value;
        }

        public bool IsViewSpace => (LightType & BrLightType.BR_LIGHT_VIEW) == BrLightType.BR_LIGHT_VIEW;

        private InterpretType<uint, BrLightType> lightType;
    }
}
