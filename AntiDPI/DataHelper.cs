//#define DETAILEDLOG

namespace AntiDPI
{
	public class DataHelper
	{
		public static string GetEndpointHost(string endpoint)
		{
			if (endpoint.Contains("://"))
			{
				return endpoint.Split("://")[1].Split("/")[0].Split(":")[0];
			}
			else if(endpoint.Contains(":"))
			{
				return endpoint.Split(":")[0];
			}
			else if(endpoint.Contains("/"))
			{
				return endpoint.Split("/")[0];
			}

			return endpoint;
		}

		public static int GetEndpointPort(string endpoint)
		{
			bool StartPrefix = false;
			int Index = 1;
			if (endpoint.Contains("://"))
			{
				StartPrefix = true;
				Index++;
			}

			var points = endpoint.Split(":");

			if(points.Length > Index && int.TryParse(points[Index],out int port))
			{
				return port;
			}

			if (StartPrefix)
			{
				var prefix = endpoint.Split("://")[0].Trim();
				switch (prefix)
				{
					case "http":
						return 80;
					case "https":
						return 443;
				}
			}

			return 80;
		}
	}
}
