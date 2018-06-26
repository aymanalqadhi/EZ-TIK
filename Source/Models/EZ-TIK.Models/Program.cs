using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using EZ_TIK.Models;
using tik4net;
using tik4net.Objects;

namespace EZ_TIK.Models
{
    public class Program
    {
        private static void Main(string[] args)
        {

            var conn = ConnectionFactory.CreateConnection(TikConnectionType.Api);
            conn.Open("192.168.56.2", 8728, "admin", string.Empty);

            var user = conn.LoadList<UserManagerCustomer>();

            Console.Read();
        }
    }
}
