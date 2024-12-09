using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AntiDPI
{
	public interface ISocketData
	{
		AddressFamily AddressFamily { get; }
		SocketType SocketType { get; }
		ProtocolType ProtocolType { get; }
	}

	public interface ISocketClient : ISocketData
	{
		Task<int> SendAsync(ArraySegment<byte> buffer)
		{
			SendAsync(buffer, SocketFlags.None);
		}
		Task<int> SendAsync(ArraySegment<byte> buffer, SocketFlags socketFlags);

		Task<int> ReceiveAsync(ArraySegment<byte> buffer)
		{
			ReceiveAsync(buffer, SocketFlags.None);
		}
		Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags);
	}

	public interface ISocket : ISocketData
	{
		EndPoint? LocalEndPoint { get; }
		EndPoint? RemoteEndPoint { get; }
		int Handle { get; }

		void Bind(EndPoint localEndPoint);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="backlog">it will not work in the Udp | 0 for infinity</param>
		/// <returns></returns>
		Task<int> Listen(int backlog);

		Task<int> SendToAsync(ArraySegment<byte> buffer, EndPoint remoteEP)
		{
			SendToAsync(buffer, SocketFlags.None, remoteEP);
		}

		Task<int> SendToAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEP);


		Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, EndPoint remoteEndPoint)
		{
			ReceiveFromAsync(buffer, SocketFlags.None, remoteEndPoint);
		}

		Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint);

		void Close();
	}

	public class SocketBase : ISocket
	{
		public Socket Socket { get; private set; }

		public EndPoint? LocalEndPoint => Socket.LocalEndPoint;

		public EndPoint? RemoteEndPoint => Socket.RemoteEndPoint;

		public int Handle => Socket.Handle;

		public AddressFamily AddressFamily => Socket.AddressFamily;

		public SocketType SocketType => Socket.SocketType;

		public ProtocolType ProtocolType => Socket.ProtocolType;

		public SocketBase(Socket socket)
		{
			Socket = socket;
		}

		public virtual void Close()
		{
			Socket.Close();
		}

		public virtual void Bind(EndPoint localEndPoint)
		{
			Socket.Bind(localEndPoint);
		}

		public virtual Task<int> SendToAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEP)
		{
			Socket.SendToAsync(buffer, socketFlags, remoteEP);
		}

		public virtual Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint)
		{
			Socket.ReceiveFromAsync(buffer, socketFlags, remoteEndPoint);
		}

		public virtual Task<int> Listen(int backlog)
		{
			Socket.Listen(backlog);
		}
	}

	public class SocketClientBase : ISocketClient
	{
		public Socket Socket { get; private set; }

		public AddressFamily AddressFamily => Socket.AddressFamily;

		public SocketType SocketType => Socket.SocketType;

		public ProtocolType ProtocolType => Socket.ProtocolType;

		public SocketClientBase(Socket socket)
		{
			Socket = socket;
		}


	}

	public class UdpSocket : SocketBase
	{
		public UdpSocket(Socket socket, int bufferSize = UdpMessageLimit) : base(socket)
		{
			BufferSize = bufferSize;
		}

		/// <summary>
		/// it's like a general limit, there is no limit actually
		/// </summary>
		public const int UdpMessageLimit = 65535;

		public readonly int BufferSize;

		private List<EndPoint> EndPoints { get; set; } = new();

		
	}

	
}
