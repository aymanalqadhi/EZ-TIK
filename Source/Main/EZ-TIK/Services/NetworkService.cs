using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EZ_TIK.Models;
using EZ_TIK.ViewModels;

//using PrimS.Telnet;

namespace EZ_TIK
{
    public class NetworkService
    {
        public static List<MacIpPair> GetAllNetworkDevices()
        {
            var mip = new List<MacIpPair>();
            var ps = new Process
            {
                StartInfo =
                {
                    FileName = "arp",
                    Arguments = "-a ",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            ps.Start();
            var cmdOutput = ps.StandardOutput.ReadToEnd();
            const string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
                mip.Add(new MacIpPair
                {
                    MacAddress = m.Groups["mac"].Value.Replace('-', ':'),
                    IpAddress = m.Groups["ip"].Value
                });


            return mip;
        }

        public static async Task<IEnumerable<MacIpPair>> GetAllNetworkDevicesAsync()
            => await Task.Run(() => GetAllNetworkDevices());

        public static IEnumerable<RouterOsDevice> GetMikrotikRouters()
        {
            var devices = GetAllNetworkDevices();
            var routers = new List<RouterOsDevice>();

            devices.ForEach(d =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        var client = new TelnetConnection(d.IpAddress, 23);
                        if (!client.IsConnected) return;

                        var output = client.Read().Trim();
                        var match = Regex.Match(output, @"^(\w+)\s+([^\s]+)");

                        if (!match.Success) return;

                        var router = new RouterOsDevice(match.Groups[1].Value, match.Groups[2].Value,
                            d.MacAddress, d.IpAddress);

                        routers.Add(router);
                    }
                    catch
                    {
                    }

                }).Wait(500);
            });

            return routers;
        }

        public static async Task<IEnumerable<RouterOsDevice>> GetMikrotikRoutersAsync()
            => await Task.Run(() => GetMikrotikRouters());

        public static IEnumerable<string> GetDefaultGateways()
        {
            foreach (var nics in NetworkInterface.GetAllNetworkInterfaces())
            foreach (var props in nics.GetIPProperties().GatewayAddresses)
                if (nics.OperationalStatus == OperationalStatus.Up) yield return props.Address.ToString();
        }

        #region Structs

        public struct MacIpPair
        {
            public string MacAddress, IpAddress;
        }

        #endregion
    }
}