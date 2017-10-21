namespace Meyer.Socrates.Data
{
    public struct BrLight
    {
        public BrScalar Unk1 { get; set; }
        public BrScalar Unk2 { get; set; }
        public BrScalar Unk3 { get; set; }
        public BrScalar Unk4 { get; set; }
        public BrScalar Unk5 { get; set; }
        public BrScalar Unk6 { get; set; }
        public BrScalar Unk7 { get; set; }
        public BrScalar Unk8 { get; set; }
        public BrScalar Unk9 { get; set; }
        public BrScalar X { get; set; }
        public BrScalar Y { get; set; }
        public BrScalar Z { get; set; }
        public BrScalar Attenuation { get; set; }

        public BrLightType LightType { get; set; }

        public bool IsViewSpace => (LightType & BrLightType.BR_LIGHT_VIEW) == BrLightType.BR_LIGHT_VIEW;
    }

    public enum BrLightType: uint
    {
        /// <summary>
        /// Mask used for getting the light type.
        /// </summary>
        BR_LIGHT_TYPE = 0x0003,

        /// <summary>
        /// Point light.
        /// </summary>
        BR_LIGHT_POINT = 0x0000,
        /// <summary>
        /// Directional light.
        /// </summary>
        BR_LIGHT_DIRECT = 0x0001,

        /// <summary>
        /// Spot light.
        /// </summary>
        BR_LIGHT_SPOT = 0x0002,

        /// <summary>
        /// Flag indicating that calculations are to be done in view space.
        /// </summary>
        BR_LIGHT_VIEW = 0x0004
    };
}
