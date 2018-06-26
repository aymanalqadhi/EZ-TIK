using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EZ_TIK.LLDP.TLV
{
    public static class TlvHelper
    {
        #region Main Methods

        /// <summary>
        /// Parse Plain Packet Into TLV Objects
        /// </summary>
        /// <param name="packet">The byte array packet</param>
        /// <param name="fixedLen">THe fixed length of the type and the length fields</param>
        /// <returns>A list of collection the tlv objects</returns>
        public static IEnumerable<Tlv> ParseTlv(byte[] packet, int fixedLen = 2)
        {
            // Get Packet list
            var packetList = packet.ToList();

            // Parse the packet
            while (packet.Length >= fixedLen * 2)
            {
                // return if the list is in invalid size
                if (packetList.Count < 2) yield break;

                // Extract the type field
                var type = new byte[fixedLen];
                for (var i = 0; i < fixedLen; i++)
                    type[i] = packetList.GetFirst();
                

                // Extract the length field
                var length = new byte[fixedLen];
                for (var i = 0; i < fixedLen; i++)
                    length[i] = packetList.GetFirst();

                // Extract the value field
                var value = new byte[GetNumber(length)];
                for (var i = 0; i < value.Length; i++)
                    value[i] = packetList.GetFirst();
                
                // return the tlv object
                yield return new Tlv(type, length, value);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert a byte array to an integer number
        /// </summary>
        /// <param name="value">the byte array of the number</param>
        /// <returns>an integer value</returns>
        public static int GetNumber(byte[] value)
        {
            if (value == null || value.Length == 0) return 0;

            var merged = string.Join(string.Empty, value.ToList().Select(b => b.ToString("X2")));
            return int.Parse(merged, NumberStyles.HexNumber);
        }

        /// <summary>
        /// Convert a byte array to a floating-point number
        /// </summary>
        /// <param name="value">the byte array of the number</param>
        /// <returns>a floating-point value</returns>
        public static double GetDouble(byte[] value)
        {
            if (value == null || value.Length == 0) return 0;

            var merged = string.Join(string.Empty, value.ToList().Select(b => b.ToString("X2")));
            return int.Parse(merged, NumberStyles.HexNumber);
        }

        #endregion

        #region Extenstion Methods

        /// <summary>
        /// Extension method to list object to get the first element an remove it after returning it
        /// </summary>
        /// <typeparam name="T">The List type</typeparam>
        /// <param name="lst">the list</param>
        /// <returns>the first item</returns>
        public static T GetFirst<T>(this List<T> lst)
        {
            // throw exception if the list is empty
            if(lst.Count == 0) throw new IndexOutOfRangeException("The list is empty");

            // Get and delete the first item
            var value = lst[0];
            lst.RemoveAt(0);

            // return the first item
            return value;
        }

    #endregion
}
}
