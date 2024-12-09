using System.Net;

namespace AntiDPI
{
	public class LocalWebProxySettings() : SocketSettingsBase("127.0.0.1",8085)
	{

		public IPEndPoint[] DnsServers { get; set; } = Array.Empty<IPEndPoint>();
	}
}
