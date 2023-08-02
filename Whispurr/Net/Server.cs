using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using WhispurrClient.Net.IO;
using WhispurrServer.Net;

namespace Whispurr.Net
{
	class Server
	{
		private TcpClient _client;
		public PacketReader PacketReader;
		private const string LOCAL_HOST = "127.0.0.1";

		public event Action connectedEvent;
		public event Action messageRecievedEvent;
		public event Action disconnectedEvent;

		public Server()
		{
			_client = new TcpClient();
		}

		public void ConnectToServer(string username)
		{
			if (!_client.Connected)
			{
				_client.Connect(LOCAL_HOST, 4565);
				PacketReader = new PacketReader(_client.GetStream());

				if (!string.IsNullOrEmpty(username))
				{
					var connectPacket = new PacketBuilder();
					connectPacket.WriteOpCode(0);
					connectPacket.WriteMessage(username);
					_client.Client.Send(connectPacket.GetPacketBytes());
				}

				ReadPackets();
			}
		}

		private void ReadPackets()
		{
			Task.Run(() =>
			{
				while (true)
				{
					var opcode = PacketReader.ReadByte();
					switch (opcode)
					{
						case 1:
							connectedEvent?.Invoke();
							break;
						case 5:
							messageRecievedEvent?.Invoke();
							break;
						case 10:
							disconnectedEvent?.Invoke();
							break;
						default:
							Console.WriteLine("yes");
							break;
					}
				}
			});
		}

		public void SendMessageToServer(string message)
		{
			var messagePacket = new PacketBuilder();
			messagePacket.WriteOpCode(5);
			messagePacket.WriteMessage(message);
			_client.Client.Send(messagePacket.GetPacketBytes());
		}
	}
}
