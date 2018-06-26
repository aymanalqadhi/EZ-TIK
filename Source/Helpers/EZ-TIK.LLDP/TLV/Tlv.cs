namespace EZ_TIK.LLDP.TLV
{
    /// <summary>
    /// Type-Length-Value Object
    /// </summary>
    public class Tlv
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the type of the tlv object
        /// </summary>
        public byte[] Type;

        /// <summary>
        /// Gets or sets the length of the tlv object
        /// </summary>
        public byte[] Length;

        /// <summary>
        /// Gets or sets the value of the tlv object
        /// </summary>
        public byte[] Value;

        /// <summary>
        /// Gets the length vlaue of the tlv object in integer number
        /// </summary>
        public int LengthNumber { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">The type field</param>
        /// <param name="length">The length field</param>
        /// <param name="value">The value field</param>
        public Tlv(byte[] type, byte[] length, byte[] value)
        {
            Type = type;
            Length = length;
            Value = value;

            LengthNumber = TlvHelper.GetNumber(Length);
        } 

        #endregion
    }
}
