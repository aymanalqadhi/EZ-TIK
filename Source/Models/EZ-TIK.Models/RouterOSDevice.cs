using System.Net;

namespace EZ_TIK.Models
{
    public class RouterOsDevice
    {
        #region Constructors

        /// <summary>
        ///     Default Constructor
        /// </summary>
        /// <param name="id">The id of the router board device</param>
        /// <param name="version">The version of the router board device</param>
        /// <param name="macAddress">The mac address of the router board device</param>
        /// <param name="ipAddress">the ip address of the router board device</param>
        public RouterOsDevice(string id, string version, string macAddress, string ipAddress)
        {
            Id = id;
            Version = version;
            MacAddress = macAddress;
            IpAddress = IPAddress.Parse(ipAddress);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the id of the routerboard device
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        ///     Gets the id of the routerboard device
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        ///     Gets the mac address of the routerboard device
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        ///     Gets the ip address of the routerboard device
        /// </summary>
        public IPAddress IpAddress { get; set; }


        /// <summary>
        ///     The router board
        /// </summary>
        public string Board { get; set; }

        /// <summary>
        ///     THe platofrm of the router
        /// </summary>
        public string Platform { get; set; }

        #endregion
    }
}