using NLog;
using System.Management;

namespace AntiDPI
{
	public class DnsHelper
	{
		private Logger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		private List<DnsDeviceData> DnsDeviceDatas = new();

		public void SetDns(params string[] dnsServers)
		{
			if(DnsDeviceDatas.Count > 0)
			{
				RevertChanges();
			}
			DnsDeviceDatas = GetCurrentDns();
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = true");
			foreach (ManagementObject mo in searcher.Get())
			{
				try
				{
					var device = DnsDeviceData.GetDevice(mo);

					if(device != null)
					{
						
						mo[Win32_NetworkAdapterConsts.DNSServerSearchOrder] = dnsServers;
						mo.Put();
						Logger.Info($"DNS Servers [{string.Join(" | ", dnsServers)}] Setted for {device.GetDeviceId()}");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"DNS ayarları değiştirilemedi: {ex.Message}");
				}
			}
		}

		public void RevertChanges()
		{

		}

		private List<DnsDeviceData> GetCurrentDns()
		{
			List<DnsDeviceData> devices = new();
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = true");
			foreach (ManagementObject mo in searcher.Get())
			{
				var device = DnsDeviceData.GetDevice(mo);
				if (device != null)
				{
					devices.Add(device);
				}
			}
			return new();
		}
	}
}
