using System;
using System.Net;
using System.Net.Sockets;
using WhispurrClient.Net.IO;

namespace WhispurrServer
{
	class Program
	{
		private static List<Client> _userList;
		private static TcpListener _listener;

		static void Main(string[] args)
		{
			_userList= new List<Client>();
			_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4565);
			_listener.Start();

			while(true)
			{
				var client = new Client(_listener.AcceptTcpClient());
				_userList.Add(client);

				BroadCastConnection();
			}
		}

		private static void BroadCastConnection()
		{
			foreach (var user1 in _userList)
			{
				foreach (var user2 in _userList)
				{
					var broadcastPacket = new PacketBuilder();
					broadcastPacket.WriteOpCode(1);
					broadcastPacket.WriteMessage(user2.Username);
					broadcastPacket.WriteMessage(user2.UID.ToString());
					user1.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
				}
			}
		}

		public static void BroadCastMessage(string message)
		{
			foreach (var user in _userList)
			{
				var msgPacket = new PacketBuilder();
				msgPacket.WriteOpCode(5);
				msgPacket.WriteMessage(message);
				user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
			}
		}

		public static void BroadCastDisconnect(string uid)
		{
			var disconnectedUser = _userList.Where(x => x.UID.ToString() == uid).FirstOrDefault();
			_userList.Remove(disconnectedUser);

			foreach (var user in _userList)
			{
				var broadcastPacket = new PacketBuilder();
				broadcastPacket.WriteOpCode(10);
				broadcastPacket.WriteMessage(uid);
				user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
			}

			BroadCastMessage($"{disconnectedUser.Username} has left the server.");
		}
	}
}