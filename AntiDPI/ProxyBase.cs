using NLog;
using System.Net.Sockets;

namespace AntiDPI
{
	public class ProxyBase<T> where T : ISettings
	{
		public Socket? Server = null;

		public Func<Socket> Generator;

		public readonly ProtocolType Protocol;
		private ILogger InternalLogger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		public T Settings;

		public ProxyBase(ProtocolType protocol, T settings)
		{
			Protocol = protocol;
			switch (protocol)
			{
				case ProtocolType.Tcp:
					Generator = () => new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					break;
				case ProtocolType.Udp:
					Generator = () => new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					break;
				default:
					throw new NotSupportedException();
			}
			Settings = settings;
		}

		public bool IsActive
		{
			get
			{
				return Server != null;
			}
		}

		public event EventHandler? AfterClosing;
		public event EventHandler? BeforeClosing;

		public event EventHandler? AfterStarting;
		public event EventHandler? BeforeStarting;

		public void Toggle()
		{
			InternalLogger.Info("Toggle Function Called");

			if (IsActive)
			{
				InternalLogger.Info("Disabling...");

				BeforeClosing?.Invoke(this, EventArgs.Empty);

				//Server!.Shutdown(SocketShutdown.Both);
				Server!.Close();
				Server = null;

				AfterClosing?.Invoke(this, EventArgs.Empty);
			}
			else
			{
				InternalLogger.Info("Enabling...");

				BeforeStarting?.Invoke(this, EventArgs.Empty);

				Server = Generator();
				Server.Bind(Settings.ServerEndPoint);

				if (Protocol == ProtocolType.Tcp)
				{
					Server.Listen(0);
					AcceptController();
				}
				else
				{

				}
				

				InternalLogger.Info("Enabled");

				AfterStarting?.Invoke(this, EventArgs.Empty);
			}
		}

		private void AcceptController()
		{
			Server.BeginAccept(AcceptCallback, null);
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			Socket socket = Server.EndAccept(ar);
			AcceptController();
			Task _ = HandleClientAsync(socket);
		}

		public virtual async Task HandleClientAsync(Socket socket)
		{
			InternalLogger.Info($"New Connection {socket.LocalEndPoint?.ToString()}");
		}
	}
}
