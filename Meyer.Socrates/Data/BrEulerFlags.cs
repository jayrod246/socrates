namespace Meyer.Socrates.Data
{
    public enum BrEulerFlags: ushort
    {
        BR_EULER_FIRST = 0x03,
        BR_EULER_FIRST_X = 0x00,
        BR_EULER_FIRST_Y = 0x01,
        BR_EULER_FIRST_Z = 0x02,

        BR_EULER_PARITY = 0x04,
        BR_EULER_PARITY_EVEN = 0x00,
        BR_EULER_PARITY_ODD = 0x04,

        BR_EULER_REPEAT = 0x08,
        BR_EULER_REPEAT_NO = 0x00,
        BR_EULER_REPEAT_YES = 0x08,

        BR_EULER_FRAME = 0x10,
        BR_EULER_FRAME_STATIC = 0x00,
        BR_EULER_FRAME_ROTATING = 0x10,

        // Euler Order
        BR_EULER_XYZ_S = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_XYX_S = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_XZY_S = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_XZX_S = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_YZX_S = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_YZY_S = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_YXZ_S = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_YXY_S = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_ZXY_S = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_ZXZ_S = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_ZYX_S = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_ZYZ_S = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_STATIC),

        BR_EULER_ZYX_R = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_XYX_R = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_YZX_R = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_XZX_R = (BR_EULER_FIRST_X |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_XZY_R = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_YZY_R = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_ZXY_R = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_YXY_R = (BR_EULER_FIRST_Y |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_YXZ_R = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_ZXZ_R = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_EVEN |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_XYZ_R = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_NO |
                          BR_EULER_FRAME_ROTATING),

        BR_EULER_ZYZ_R = (BR_EULER_FIRST_Z |
                          BR_EULER_PARITY_ODD |
                          BR_EULER_REPEAT_YES |
                          BR_EULER_FRAME_ROTATING)
    };
}
