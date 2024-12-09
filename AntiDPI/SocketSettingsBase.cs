using System.Net;

namespace AntiDPI
{
	public class SocketSettingsBase : ISettings
	{
		public SocketSettingsBase() { }

		public SocketSettingsBase(string defaultIP, int defaultPort)
		{
			IP = defaultIP;
			PORT = defaultPort;
		}

		public string IP = "127.0.0.1";
		public int PORT = 8085;

		private IPEndPoint? Point = null;

		private bool PointNullOrChanged()
		{
			return Point == null || Point.Address.ToString() != IP || Point.Port != PORT;
		}

		public IPEndPoint ServerEndPoint
		{
			get
			{
				if(PointNullOrChanged())
				{
					Point = new IPEndPoint(IPAddress.Parse(IP), PORT);
				}
				return Point!;
			}
		}
	}
}
