using System;
using System.Globalization;

namespace EZ_TIK.Parsers
{
    /// <summary>
    /// Represents a byte size value.
    /// </summary>
    public struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize>
    {
        #region Static Fields

        public static readonly ByteSize MinValue = FromBits(0);
        public static readonly ByteSize MaxValue = FromBits(long.MaxValue);

        #endregion

        #region Public Constants

        public const long BitsInByte = 8;
        public const long BytesInKiloByte = 1024;
        public const long BytesInMegaByte = 1048576;
        public const long BytesInGigaByte = 1073741824;
        public const long BytesInTeraByte = 1099511627776;
        public const long BytesInPetaByte = 1125899906842624;

        public const string BitSymbol = "b";
        public const string ByteSymbol = "B";
        public const string KiloByteSymbol = "KB";
        public const string MegaByteSymbol = "MB";
        public const string GigaByteSymbol = "GB";
        public const string TeraByteSymbol = "TB";
        public const string PetaByteSymbol = "PB";

        #endregion

        #region Public Properties

        public long Bits { get; }
        public double Bytes { get; }
        public double KiloBytes => Bytes / BytesInKiloByte;
        public double MegaBytes => Bytes / BytesInMegaByte;
        public double GigaBytes => Bytes / BytesInGigaByte;
        public double TeraBytes => Bytes / BytesInTeraByte;
        public double PetaBytes => Bytes / BytesInPetaByte; 

        public string LargestWholeNumberSymbol
        {
            get
            {
                // Absolute value is used to deal with negative values
                if (Math.Abs(PetaBytes) >= 1)
                    return PetaByteSymbol;

                if (Math.Abs(TeraBytes) >= 1)
                    return TeraByteSymbol;

                if (Math.Abs(GigaBytes) >= 1)
                    return GigaByteSymbol;

                if (Math.Abs(MegaBytes) >= 1)
                    return MegaByteSymbol;

                if (Math.Abs(KiloBytes) >= 1)
                    return KiloByteSymbol;

                if (Math.Abs(Bytes) >= 1)
                    return ByteSymbol;

                return BitSymbol;
            }
        }

        public double LargestWholeNumberValue
        {
            get
            {
                // Absolute value is used to deal with negative values
                if (Math.Abs(PetaBytes) >= 1)
                    return PetaBytes;

                if (Math.Abs(TeraBytes) >= 1)
                    return TeraBytes;

                if (Math.Abs(GigaBytes) >= 1)
                    return GigaBytes;

                if (Math.Abs(MegaBytes) >= 1)
                    return MegaBytes;

                if (Math.Abs(KiloBytes) >= 1)
                    return KiloBytes;

                if (Math.Abs(Bytes) >= 1)
                    return Bytes;

                return Bits;
            }
        }

        #endregion

        #region Constructors

        public ByteSize(double byteSize)
            : this()
        {
            // Get ceiling because bits are whole units
            Bits = (long)Math.Ceiling(byteSize * BitsInByte);

            Bytes = byteSize;
        }

        #endregion

        #region Static Constructors

        public static ByteSize FromBits(long value) => new ByteSize(value / (double)BitsInByte);

        public static ByteSize FromBytes(double value) => new ByteSize(value);

        public static ByteSize FromKiloBytes(double value) => new ByteSize(value * BytesInKiloByte);

        public static ByteSize FromMegaBytes(double value) => new ByteSize(value * BytesInMegaByte);

        public static ByteSize FromGigaBytes(double value) => new ByteSize(value * BytesInGigaByte);

        public static ByteSize FromTeraBytes(double value) => new ByteSize(value * BytesInTeraByte);

        public static ByteSize FromPetaBytes(double value) => new ByteSize(value * BytesInPetaByte);

        #endregion

        #region Object Overrrides

        /// <summary>
        /// Converts the value of the current ByteSize object to a string.
        /// The metric prefix symbol (bit, byte, kilo, mega, giga, tera) used is
        /// the largest metric prefix such that the corresponding value is greater
        //  than or equal to one.
        /// </summary>
        public override string ToString() => ToString("0.##", CultureInfo.CurrentCulture);

        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);

        public string ToString(string format, IFormatProvider provider)
        {
            if (!format.Contains("#") && !format.Contains("0"))
                format = "0.## " + format;

            if (provider == null) provider = CultureInfo.CurrentCulture;

            Func<string, bool> has = s => format.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) != -1;
            Func<double, string> output = n => n.ToString(format, provider);

            if (has("PB"))
                return output(PetaBytes);
            if (has("TB"))
                return output(TeraBytes);
            if (has("GB"))
                return output(GigaBytes);
            if (has("MB"))
                return output(MegaBytes);
            if (has("KB"))
                return output(KiloBytes);

            // Byte and Bit symbol must be case-sensitive
            if (format.IndexOf(ByteSymbol, StringComparison.Ordinal) != -1)
                return output(Bytes);

            if (format.IndexOf(BitSymbol, StringComparison.Ordinal) != -1)
                return output(Bits);

            return $"{LargestWholeNumberValue.ToString(format, provider)} {LargestWholeNumberSymbol}";
        }

        public override bool Equals(object value)
        {
            if (value == null)
                return false;

            ByteSize other;
            if (value is ByteSize)
                other = (ByteSize)value;
            else
                return false;

            return Equals(other);
        }

        public bool Equals(ByteSize value) => Bits == value.Bits;

        public override int GetHashCode() => Bits.GetHashCode();

        public int CompareTo(ByteSize other) => Bits.CompareTo(other.Bits);

        #endregion

        #region Public Methods

        public ByteSize Add(ByteSize bs) => new ByteSize(Bytes + bs.Bytes);

        public ByteSize AddBits(long value) => this + FromBits(value);

        public ByteSize AddBytes(double value) => this + FromBytes(value);

        public ByteSize AddKiloBytes(double value) => this + FromKiloBytes(value);

        public ByteSize AddMegaBytes(double value) => this + FromMegaBytes(value);

        public ByteSize AddGigaBytes(double value) => this + FromGigaBytes(value);

        public ByteSize AddTeraBytes(double value) => this + FromTeraBytes(value);

        public ByteSize AddPetaBytes(double value) => this + FromPetaBytes(value);

        public ByteSize Subtract(ByteSize bs) => new ByteSize(Bytes - bs.Bytes);

        #endregion

        #region Operator Overloads

        public static ByteSize operator +(ByteSize b1, ByteSize b2) => new ByteSize(b1.Bytes + b2.Bytes);

        public static ByteSize operator ++(ByteSize b) => new ByteSize(b.Bytes + 1);

        public static ByteSize operator -(ByteSize b) => new ByteSize(-b.Bytes);

        public static ByteSize operator -(ByteSize b1, ByteSize b2) => new ByteSize(b1.Bytes - b2.Bytes);

        public static ByteSize operator --(ByteSize b) => new ByteSize(b.Bytes - 1);

        public static bool operator ==(ByteSize b1, ByteSize b2) => b1.Bits == b2.Bits;

        public static bool operator !=(ByteSize b1, ByteSize b2) => b1.Bits != b2.Bits;

        public static bool operator <(ByteSize b1, ByteSize b2) => b1.Bits < b2.Bits;

        public static bool operator <=(ByteSize b1, ByteSize b2) => b1.Bits <= b2.Bits;

        public static bool operator >(ByteSize b1, ByteSize b2) => b1.Bits > b2.Bits;

        public static bool operator >=(ByteSize b1, ByteSize b2) => b1.Bits >= b2.Bits;

        #endregion

        #region Static Methods

        public static ByteSize Parse(string s)
        {
            // Arg checking
#if NET35
            if (string.IsNullOrEmpty(s) || s.Trim() == "")
                throw new ArgumentNullException("s", "String is null or whitespace");
#else
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(nameof(s), "String is null or whitespace");
#endif

            // Get the index of the first non-digit character
            s = s.TrimStart(); // Protect against leading spaces

            int num;
            var found = false;

            var decimalSeparator = Convert.ToChar(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            var groupSeparator = Convert.ToChar(NumberFormatInfo.CurrentInfo.NumberGroupSeparator);

            // Pick first non-digit number
            for (num = 0; num < s.Length; num++)
                if (!(char.IsDigit(s[num]) || s[num] == decimalSeparator || s[num] == groupSeparator))
                {
                    found = true;
                    break;
                }

            if (found == false)
                throw new FormatException($"No byte indicator found in value '{ s }'.");

            int lastNumber = num;

            // Cut the input string in half
            string numberPart = s.Substring(0, lastNumber).Trim();
            string sizePart = s.Substring(lastNumber, s.Length - lastNumber).Trim();

            // Get the numeric part
            double number;
            if (!double.TryParse(numberPart, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.CurrentInfo, out number))
                throw new FormatException($"No number found in value '{ s }'.");

            // Get the magnitude part
            switch (sizePart)
            {
                case "b":
                    if (number % 1 != 0) // Can't have partial bits
                        throw new FormatException($"Can't have partial bits for value '{ s }'.");

                    return FromBits((long)number);

                case "B":
                    return FromBytes(number);

                case "KB":
                case "kB":
                case "kb":
                    return FromKiloBytes(number);

                case "MB":
                case "mB":
                case "mb":
                    return FromMegaBytes(number);

                case "GB":
                case "gB":
                case "gb":
                    return FromGigaBytes(number);

                case "TB":
                case "tB":
                case "tb":
                    return FromTeraBytes(number);

                case "PB":
                case "pB":
                case "pb":
                    return FromPetaBytes(number);

                default:
                    throw new FormatException($"Bytes of magnitude '{ sizePart }' is not supported.");
            }
        }

        public static bool TryParse(string s, out ByteSize result)
        {
            try
            {
                result = Parse(s);
                return true;
            }
            catch
            {
                result = new ByteSize();
                return false;
            }
        } 

        #endregion
    }
}