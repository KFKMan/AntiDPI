//#define DETAILEDLOG

using System.Net.Sockets;

namespace AntiDPI
{
	public static class SocketExtensions
	{
		public static NetworkStream GetStream(this Socket socket)
		{
			return new NetworkStream(socket);
		}

		public static byte[] ReceiveAvailable(this Socket socket)
		{
			byte[] buffer = new byte[socket.Available];
			int readed = socket.Receive(buffer);
			return buffer.Take(readed).ToArray();
		}

		public static async ValueTask<byte[]> ReceiveAvailableAsync(this Socket socket)
		{
			byte[] buffer = new byte[socket.Available];
			int readed = await socket.ReceiveAsync(buffer);
			return buffer.Take(readed).ToArray();
		}
	}
}
