using System;
using System.Net;
using System.Threading.Tasks;
using tik4net;

namespace EZ_TIK.Models
{
    /// <summary>
    /// A Class for auth to server
    /// </summary>
    public static class AuthManager
    {
        public static Task<ITikConnection> Login(IPAddress host, string user, string password)
        {
            return Task.Run(() =>
            {
                try
                {
                    var conn = ConnectionFactory.CreateConnection(TikConnectionType.Api);
                    conn.Open(host.ToString(), user, password);

                    return conn.IsOpened ? conn : null;
                }
                catch
                {
                    return null;
                }
            });
        }

    }
}
