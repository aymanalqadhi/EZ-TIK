using System;
using System.Text;
using System.Linq;
using System.Net;
using EZ_TIK.LLDP.TLV;

namespace EZ_TIK.LLDP
{
    /// <summary>
    /// MikroTik Nighbor Discovery Protocol Parsed Packet object
    /// </summary>
    public class MndpPacket
    {
        #region Public Properties

        /// <summary>
        /// Gets the packet header
        /// </summary>
        public int Header { get; private set; }

        /// <summary>
        /// The packet header seq no
        /// </summary>
        public long SeqNo { get; private set; }

        /// <summary>
        /// Gets or sets the mac-address of the router
        /// </summary>
        public byte[] MacAddress { get; set; }

        /// <summary>
        /// Gets or sets the identity of the router
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the version of the router firmare
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the platform that the router runs on
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets time from the last power on
        /// </summary>
        public double UpTime { get; set; }

        /// <summary>
        /// Gets or sets the software id of the router
        /// </summary>
        public string SoftwareId { get; set; }

        /// <summary>
        /// Gets or sets the board of the router
        /// </summary>
        public string Board { get; set; }

        /// <summary>
        /// Gets or sets the unpack option on the router
        /// </summary>
        public byte Unpack { get; set; }

        /// <summary>
        /// Gets or sets the interface that this packet is found in
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// Gets or sets the ip of the router
        /// </summary>
        public IPAddress IpAddress { get; set; }

        #endregion

        #region Ctors

        public MndpPacket(byte[] receiveBytes)
        {
            if (receiveBytes.Length < 14) return;

            MacAddress = new byte[6];

            MacAddress[0] = receiveBytes[8];
            MacAddress[1] = receiveBytes[9];
            MacAddress[2] = receiveBytes[10];
            MacAddress[3] = receiveBytes[11];
            MacAddress[4] = receiveBytes[12];
            MacAddress[5] = receiveBytes[13];

            var pointer = 17;
            var len = receiveBytes[pointer];

            Identity = Encoding.ASCII.GetString(receiveBytes, pointer + 1, len);

            pointer += len + 4;
            len = receiveBytes[pointer];

            Version = Encoding.ASCII.GetString(receiveBytes, pointer + 1, len);

            pointer += len + 4;
            len = receiveBytes[pointer];

            Platform = Encoding.ASCII.GetString(receiveBytes, pointer + 1, len);

            pointer += len + 4;
            len = receiveBytes[pointer];

            // TODO:
            // Parse uptime
            // Encoding.ASCII.GetString(receiveBytes, pointer + 1, len));

            pointer += len + 4;
            len = receiveBytes[pointer];

            SoftwareId = Encoding.ASCII.GetString(receiveBytes, pointer + 1, len);

            pointer += len + 4;
            len = receiveBytes[pointer];

            Board = Encoding.ASCII.GetString(receiveBytes, pointer + 1, len);

            pointer += len + 4;
            len = receiveBytes[pointer];

            // TODO:
            // SET UPTIME
            //Console.WriteLine(Encoding.ASCII.GetString(receiveBytes, pointer + 1, len));

            pointer += len + 4;
            len = receiveBytes[pointer];


            InterfaceName = Encoding.ASCII.GetString(receiveBytes, pointer + 1, len);

            #region TMP
            //// Parse the packet into TLV list
            //var parsed = TlvHelper.ParseTlv(packet).ToList();

            //// Extract the header
            //var header = parsed.GetFirst();

            //// Set the header & seq no properties
            //Header = TlvHelper.GetNumber(header.Type);
            //SeqNo = TlvHelper.GetNumber(header.Value);

            //// Loop through the TLV list
            //foreach (var tlv in parsed)
            //{
            //    // Switch the type of the tlv object
            //    switch (TlvHelper.GetNumber(tlv.Type))
            //    {
            //        case 0x1:
            //            MacAddress = tlv.Value;
            //            break;

            //        case 0x5:
            //            Identity = Encoding.ASCII.GetString(tlv.Value);
            //            break;

            //        case 0x7:
            //            Version = Encoding.ASCII.GetString(tlv.Value);
            //            break;

            //        case 0x8:
            //            Platform = Encoding.ASCII.GetString(tlv.Value);
            //            break;

            //        case 0xA:
            //            UpTime = TlvHelper.GetDouble(tlv.Value);
            //            break;

            //        case 0xB:
            //            SoftwareId = Encoding.ASCII.GetString(tlv.Value);
            //            break;

            //        case 0xC:
            //            Board = Encoding.ASCII.GetString(tlv.Value);
            //            break;

            //        case 0xE:
            //            Unpack = (byte)TlvHelper.GetNumber(tlv.Value);
            //            break;

            //        case 0x10:
            //            InterfaceName = Encoding.ASCII.GetString(tlv.Value);
            //            break;
            //    } 
            #endregion
        }
    }
    #endregion
}
