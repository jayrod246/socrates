using System;

namespace Meyer.Socrates.Data
{
    public struct Fraction
    {
        internal int top;
        internal int bottom;
        public static readonly Fraction Zero = new Fraction(0, 0);

        public int Numerator => top;
        public int Denominator => bottom;

        public Fraction(int top, int bottom)
        {
            this.top = top;
            this.bottom = bottom;
        }

        public Fraction(float value)
        {
            this = SingleToFraction(value);
        }

        public Fraction(decimal value)
        {
            this = SingleToFraction((float)value);
        }

        public Fraction(double value)
        {
            this = SingleToFraction((float)value);
        }

        public float AsSingle()
        {
            if (bottom == 0)
            {
                if (top == 0) return 0f;
                throw new DivideByZeroException();
            }

            return (float)top / bottom;
        }

        public Fraction Simplified()
        {
            int i = GCD(top, bottom);
            return new Fraction(top / i, bottom / i);
        }

        private static int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        public static Fraction operator /(Fraction frac, int n)
        {
            if (n == 0) throw new DivideByZeroException();
            return new Fraction(frac.top, frac.bottom * n);
        }

        public static Fraction operator *(Fraction frac, int n)
        {
            if (n == 0) return Zero;
            return new Fraction(frac.top, frac.bottom / n);
        }

        public static Fraction operator +(Fraction frac, int n)
        {
            return new Fraction(frac.top + (n * frac.bottom), frac.bottom);
        }

        public static Fraction operator -(Fraction frac, int n)
        {
            return new Fraction(frac.top - (n * frac.bottom), frac.bottom);
        }

        #region HAX
        // HACK: Should find a better approach for adding/subtracting fractions.
        public static Fraction operator +(Fraction a, Fraction b)
        {
            return new Fraction(a.AsSingle() + b.AsSingle());
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            return new Fraction(a.AsSingle() - b.AsSingle());
        }
        #endregion

        public static implicit operator float(Fraction frac)
        {
            return frac.AsSingle();
        }

        public static explicit operator Fraction(float x)
        {
            return SingleToFraction(x);
        }

        public override string ToString()
        {
            return Simplified().ToStringCore();
        }

        private string ToStringCore()
        {
            if (top == 0) return "0";
            if (top == bottom) return "1";
            if (bottom == 1) return top.ToString();
            if (bottom == -1) return (-top).ToString();
            return $"{top}/{bottom}";
        }

        private static Fraction SingleToFraction(float x, float error = 0.000001f)
        {
            int n = (int)Math.Floor(x);
            x -= n;
            if (x < error)
                return new Fraction(n, 1);
            else if (1 - error < x)
                return new Fraction(n + 1, 1);

            // The lower fraction is 0/1
            int lower_n = 0;
            int lower_d = 1;
            // The upper fraction is 1/1
            int upper_n = 1;
            int upper_d = 1;
            while (true)
            {
                // The middle fraction is (lower_n + upper_n) / (lower_d + upper_d)
                int middle_n = lower_n + upper_n;
                int middle_d = lower_d + upper_d;
                // If x + error < middle
                if (middle_d * (x + error) < middle_n)
                {
                    // middle is our new upper
                    upper_n = middle_n;
                    upper_d = middle_d;
                }
                // Else If middle < x - error
                else if (middle_n < (x - error) * middle_d)
                {
                    // middle is our new lower
                    lower_n = middle_n;
                    lower_d = middle_d;
                }
                // Else middle is our best fraction
                else
                    return new Fraction(n * middle_d + middle_n, middle_d);
            }
        }
    }
}
