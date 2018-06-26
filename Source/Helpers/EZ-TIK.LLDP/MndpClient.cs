using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EZ_TIK.LLDP
{
    /// <summary>
    /// MikroTik Nighbor Discovery Protocol CLient
    /// </summary>
    public class MndpClient : IDisposable
    {
        #region Private Members

        /// <summary>
        /// The Cancellation source to cancel the scan task
        /// </summary>
        private readonly CancellationTokenSource _source;

        /// <summary>
        /// The token of <see cref="_source"/>
        /// </summary>
        private CancellationToken _token;

        /// <summary>
        /// The Udp Server
        /// </summary>
        private readonly UdpClient _server;

        #endregion

        #region Ctors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MndpClient()
        {
            _source = new CancellationTokenSource();
            _server = new UdpClient(5678) { EnableBroadcast = true };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start the scan task
        /// </summary>
        /// <param name="onUpdate">Invokes when there is new device detected</param>
        /// <param name="token">The cancellation token to cancel te task</param>
        /// <returns></returns>
        private async Task StartScanTaskAsync(Action<MndpPacket> onUpdate, CancellationToken token)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (_source.IsCancellationRequested) return;
                    var clientEp = new IPEndPoint(IPAddress.Any, 0);
                    var clientRequestData = _server.Receive(ref clientEp);

                    if(clientRequestData.Length < 20) continue;

                    var mndp = new MndpPacket(clientRequestData) { IpAddress = clientEp.Address };
                    onUpdate(mndp);
                }

            }, token);
        }

        #endregion

        #region Public Methods

        public async Task Refresh()
        {
            var dg = new byte[] { 0x0, 0x0, 0x0, 0x0 };

            for (var i = 0; i < 3; i++)
                await _server.SendAsync(dg, dg.Length, new IPEndPoint(IPAddress.Broadcast, 5678));
        }

        /// <summary>
        /// Start the scan task async
        /// </summary>
        /// <param name="onUpdate">Invokes when a new device detected</param>
        /// <returns>awaitable task</returns>
        public async Task StartAsync(Action<MndpPacket> onUpdate)
        {
            // sets the cancellation token
            _token = _source.Token;

            // start the scan task
            await StartScanTaskAsync(onUpdate, _token);
        }

        /// <summary>
        /// Start the scan task sync
        /// </summary>
        /// <param name="onUpdate">Invokes when a new device detected</param>
        public void Start(Action<MndpPacket> onUpdate)
        {
            _token = _source.Token;
            StartAsync(onUpdate).Wait(_token);
        }

        /// <summary>
        /// Stop the current scan task
        /// </summary>
        public void Stop() => _source.Cancel();

        #region IDisposable Implemention

        /// <summary>
        /// Dispose the object fields
        /// </summary>
        public void Dispose()
        {
            _source?.Dispose();
            ((IDisposable)_server)?.Dispose();
        }

        #endregion

        #endregion
    }
}
