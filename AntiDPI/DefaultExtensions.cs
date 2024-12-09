#define LOG

using System.Net;

namespace AntiDPI
{
	public static class DefaultExtensions
	{
		public static EndPoint GetDefaultIfNull(this EndPoint? endpoint)
		{
			if(endpoint != null)
			{
				return endpoint;
			}
			return new IPEndPoint(IPAddress.Any, 0);
		}
	}
}
