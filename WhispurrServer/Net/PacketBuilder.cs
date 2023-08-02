﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WhispurrClient.Net.IO
{
	class PacketBuilder
	{
		private MemoryStream _ms;

		public PacketBuilder()
		{
			_ms = new MemoryStream();
		}

		public void WriteOpCode(byte opcode)
		{
			_ms.WriteByte(opcode);
		}

		public void WriteMessage(string msg)
		{
			var msgLength = msg.Length;
			_ms.Write(BitConverter.GetBytes(msgLength));
			_ms.Write(Encoding.ASCII.GetBytes(msg));
		}

		public byte[] GetPacketBytes()
		{
			return _ms.ToArray();
		}
	}
}
