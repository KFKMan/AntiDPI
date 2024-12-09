using NLog;
using System.Net.Sockets;

namespace AntiDPI
{
	public class ProxyBaseWithId<T> : ProxyBase<T> where T : ISettings
	{
		private ILogger InternalLogger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		public ProxyBaseWithId(ProtocolType protocol, T settings) : base(protocol, settings)
		{
		}

		private ulong LastId = ulong.MinValue;

		public override async Task HandleClientAsync(Socket socket)
		{
			LastId++;
			ulong ClientId = LastId;

			if (LastId == ulong.MaxValue)
			{
				LastId = ulong.MinValue;
			}

			InternalLogger.Info($"Client {ClientId} Connected");

			await HandleClientWithIdAsync(socket, ClientId);
		}

		public virtual Task HandleClientWithIdAsync(Socket socket, ulong Id)
		{
			return Task.CompletedTask;
		}
	}
}
