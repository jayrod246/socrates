namespace Meyer.Socrates.Data
{
    public struct InterpretType<TInterpretType, TInterpretAs> where TInterpretType : struct where TInterpretAs : struct
    {
        TInterpretType value;

        public static implicit operator TInterpretAs(InterpretType<TInterpretType, TInterpretAs> x)
        {
            return (TInterpretAs)(object)x.value;
        }

        public static implicit operator InterpretType<TInterpretType, TInterpretAs>(TInterpretAs x)
        {
            return new InterpretType<TInterpretType, TInterpretAs>() { value = (TInterpretType)(object)x };
        }
    }
}
