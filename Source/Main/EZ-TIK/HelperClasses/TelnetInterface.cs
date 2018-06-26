using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EZ_TIK
{
    internal enum Verbs
    {
        Will = 251,
        Wont = 252,
        Do = 253,
        Dont = 254,
        Iac = 255
    }

    internal enum Options
    {
        Sga = 3
    }

    public class TelnetConnection
    {
        private readonly TcpClient _tcpSocket;

        private int _timeOutMs = 100;

        public TelnetConnection(string hostname, int port)
        {
            _tcpSocket = new TcpClient(hostname, port);
        }

        public bool IsConnected => _tcpSocket.Connected;

        public string Login(string username, string password, int loginTimeOutMs)
        {
            var oldTimeOutMs = _timeOutMs;
            _timeOutMs = loginTimeOutMs;
            var s = Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no login prompt");
            WriteLine(username);

            s += Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no password prompt");
            WriteLine(password);

            s += Read();
            _timeOutMs = oldTimeOutMs;
            return s;
        }

        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        public void Write(string cmd)
        {
            if (!_tcpSocket.Connected) return;
            var buf = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            _tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            if (!_tcpSocket.Connected) return null;
            var sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                Thread.Sleep(_timeOutMs);
            } while (_tcpSocket.Available > 0);
            return sb.ToString();
        }

        private void ParseTelnet(StringBuilder sb)
        {
            while (_tcpSocket.Available > 0)
            {
                var input = _tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int) Verbs.Iac:
                        // interpret as command
                        var inputverb = _tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int) Verbs.Iac:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int) Verbs.Do:
                            case (int) Verbs.Dont:
                            case (int) Verbs.Will:
                            case (int) Verbs.Wont:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                var inputoption = _tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                _tcpSocket.GetStream().WriteByte((byte) Verbs.Iac);
                                if (inputoption == (int) Options.Sga)
                                    _tcpSocket.GetStream()
                                        .WriteByte(inputverb == (int) Verbs.Do ? (byte) Verbs.Will : (byte) Verbs.Do);
                                else
                                    _tcpSocket.GetStream()
                                        .WriteByte(inputverb == (int) Verbs.Do ? (byte) Verbs.Wont : (byte) Verbs.Dont);
                                _tcpSocket.GetStream().WriteByte((byte) inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char) input);
                        break;
                }
            }
        }
    }
}