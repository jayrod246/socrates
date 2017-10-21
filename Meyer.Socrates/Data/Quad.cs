namespace Meyer.Socrates.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Size = 4)]
    public struct Quad: IEnumerable<Char>, IComparable
    {
        internal byte q0;
        internal byte q1;
        internal byte q2;
        internal byte q3;

        public Quad(string str)
        {
            if (str == null) str = "    ";
            str = str.PadRight(4).Substring(0, 4).ToUpperInvariant();
            q0 = (byte)str[3];
            q1 = (byte)str[2];
            q2 = (byte)str[1];
            q3 = (byte)str[0];
        }

        public Quad(char[] chrs) : this(new string(chrs))
        { }

        public static implicit operator string(Quad quad)
        {
            return quad.ToString();
        }

        public static explicit operator Quad(string str)
        {
            return new Quad(str);
        }

        public static bool operator ==(Quad a, Quad b)
        {
            return a.ToCharArray().SequenceEqual(b.ToCharArray());
        }

        public static bool operator !=(Quad a, Quad b)
        {
            return !a.ToCharArray().SequenceEqual(b.ToCharArray());
        }

        public static bool operator ==(Quad a, string b)
        {
            return string.Equals(a.ToString(), new Quad(b).ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool operator !=(Quad a, string b)
        {
            return !string.Equals(a.ToString(), new Quad(b).ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return (q0, q1, q2, q3).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Quad quad)
                return string.Equals(this.ToString(), quad.ToString(), StringComparison.InvariantCultureIgnoreCase);
            if (obj is string str)
                return string.Equals(this.ToString(), new Quad(str).ToString(), StringComparison.InvariantCultureIgnoreCase);
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return new string(ToCharArray());
        }

        public char[] ToCharArray() => new[] { (char)q3, (char)q2, (char)q1, (char)q0 };

        public IEnumerator<char> GetEnumerator()
        {
            foreach (var c in ToCharArray()) yield return c;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int CompareTo(object obj)
        {
            if (obj is Quad quad)
                return string.Compare(this.ToString(), quad.ToString());
            if (obj is string str)
                return string.Compare(this.ToString(), str);
            if (obj is char c)
                return ((char)q0).CompareTo(c);
            if (obj is byte b)
                return q0.CompareTo(b);
            return -1;
        }
    }
}
