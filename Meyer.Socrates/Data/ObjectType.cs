namespace Meyer.Socrates.Data
{
    using System;

    public enum ObjectType : Int32
    {
        /// <summary>
        /// The type of object is an actor.
        /// </summary>
        Actor = 1,

        /// <summary>
        /// The type of object is a 3D word.
        /// </summary>
        ThreeDWord = 2,

        // TODO: Check that this is REALLY 2D word box type.
        /// <summary>
        /// The type of object is a 2D word box.
        /// </summary>
        WordBox = 3,

        /// <summary>
        /// The type of object is a prop.
        /// </summary>
        Prop = 4,
    }
}
