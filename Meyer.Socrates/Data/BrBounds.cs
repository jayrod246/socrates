namespace Meyer.Socrates.Data
{
    /// <summary>
    /// Axis aligned bounding box.
    /// </summary>
    public struct BrBounds
    {
        /// <summary>
        /// Minimum corner of bounds.
        /// </summary>
        public BrVector3 Minimum { get; set; }
        /// <summary>
        /// Maximum corner of bounds.
        /// </summary>
        public BrVector3 Maximum { get; set; }
    }
}
