using System.Net;

namespace AntiDPI
{
	public class LocalDNSProxySettings() : SocketSettingsBase("127.0.0.1",80)
	{
		public bool RandomizeHostname = false;
		public IPEndPoint[] TargetServers = new IPEndPoint[]
		{
			new IPEndPoint(IPAddress.Parse("1.1.1.1"),53), //Cloudflare
			new IPEndPoint(IPAddress.Parse("1.0.0.1"),53), //Cloudflare
			new IPEndPoint(IPAddress.Parse("1.1.1.2"),53), //Cloudflare Sec
			new IPEndPoint(IPAddress.Parse("1.0.0.2"),53), //Cloudflare Sec
			new IPEndPoint(IPAddress.Parse("8.8.8.8"),53), //Google
			new IPEndPoint(IPAddress.Parse("8.8.4.4"),53), //Google
			new IPEndPoint(IPAddress.Parse("9.9.9.9"),5053) //Quad9
		};
	}
}
