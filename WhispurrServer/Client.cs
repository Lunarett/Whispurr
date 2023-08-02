using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using WhispurrServer.Net;

namespace WhispurrServer
{
	class Client
	{
		public string Username { get; set; }
		public Guid UID { get; set; }
		public TcpClient ClientSocket { get; set; }

		private PacketReader _packetReader;

		public Client(TcpClient client)
		{
			ClientSocket = client;
			UID = Guid.NewGuid();
			_packetReader = new PacketReader(ClientSocket.GetStream());
			var opcode = _packetReader.ReadByte();
			Username = _packetReader.ReadMessage();

			Console.WriteLine($"[{DateTime.Now}] {Username}: Client connected!");

			Task.Run(() => Process());
		}

		private void Process()
		{
			while (true)
			{
				try
				{
					var opcode = _packetReader.ReadByte();
					switch (opcode)
					{
						case 5:
							var msg = _packetReader.ReadMessage();
							Console.WriteLine($"[{DateTime.Now}] {Username}: {msg}");
							Program.BroadCastMessage($"[{DateTime.Now}] {Username}: {msg}");
							break;
						default:
							break;
					}
				}
				catch (Exception)
				{
					Console.WriteLine($"[{UID}]: Disconected");
					Program.BroadCastDisconnect(UID.ToString());
					ClientSocket.Close();
					break;
				}
			}
		}
	}
}
