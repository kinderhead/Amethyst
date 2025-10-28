using System.Net.Sockets;
using System.Text;

namespace Amethyst.Daemon
{
    public enum MessageType
    {
        Login = 3,
        Command = 2,
        Response = 0
    }

    public readonly record struct Message(int Length, int ID, MessageType Type, string Body)
    {
        public byte[] Serialize()
        {
            return [
                ..BitConverter.GetBytes(Length),
                ..BitConverter.GetBytes(ID),
                ..BitConverter.GetBytes((int)Type),
                ..Encoding.ASCII.GetBytes(Body),
                0, 0 // Add null and 1 byte padding
            ];
        }

        public const int HeaderLength = 10;
    }

    public class Rcon : IDisposable
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly byte[] buffer = new byte[4096 + 14];

        public Rcon(string url, int port)
        {
            client = new(url, port);
            stream = client.GetStream();
        }

        public bool Login(string password) => Send(new(password.Length + Message.HeaderLength, 1, MessageType.Login, password));
        public bool SendCommand(string cmd) => Send(new(cmd.Length + Message.HeaderLength, 1, MessageType.Command, cmd));

        public bool Send(Message msg)
        {
            stream.Write(msg.Serialize());

            int read = stream.Read(buffer);

            // TODO: responses
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            stream.Close();
            client.Close();
        }

        public static bool IsServerRunning()
        {
            try
            {
                using var server = new Rcon("localhost", GetPort());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void StopServer()
        {
            try
            {
                using var server = new Rcon("localhost", GetPort());
                server.Login(Password);
                server.SendCommand("stop");
            }
            catch
            {
                // If this fails then the server is already stopped
            }
        }

        public static int GetPort()
        {
            var data = File.ReadAllLines(Server.ServerPropertiesLocation);

            foreach (var i in data)
            {
                if (i.StartsWith("rcon.port"))
                {
                    return int.Parse(i.Split('=')[1]);
                }
            }

            throw new FileNotFoundException("No rcon port available");
        }

		public const string Password = "amethyst";
    }
}
