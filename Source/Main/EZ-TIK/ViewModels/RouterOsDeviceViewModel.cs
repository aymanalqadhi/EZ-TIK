using EZ_TIK.Models;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class RouterOsDeviceViewModel : BindableBase
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="device">Router OS Device Model</param>
        public RouterOsDeviceViewModel(RouterOsDevice device)
        {
            MacAddress = device.MacAddress;
            IpAddress = device.IpAddress.ToString();
            Identity = device.Id;
            Version = device.Version;

        }

        /// <summary>
        /// Constructor to accept a domain name
        /// </summary>
        /// <param name="domain">The domain string</param>
        public RouterOsDeviceViewModel(string domain)
        {
            Domain = domain;
        }

        #region Public Properties

        /// <summary>
        /// Router MacAddress
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// Router Identity
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Router IP Address
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// THe Version of the Mikrotik Firmware
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets the domain name of the routerboard device
        /// </summary>
        public string Domain { get; set; }

        #endregion
    }
}
