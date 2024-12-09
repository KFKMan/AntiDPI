using System.Net;

namespace AntiDPI
{
	public interface ISettings
	{
		public IPEndPoint ServerEndPoint { get; }
	}
}
