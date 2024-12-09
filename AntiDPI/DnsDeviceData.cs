using NLog;
using System.Management;

namespace AntiDPI
{
	public class DnsDeviceData
	{
		private static Logger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		public string? Caption { get; set; }
		public string? Description { get; set; }
		public string? MACAddress { get; set; }

		public string GetDeviceId()
		{
			return string.Join("--",Caption,Description,MACAddress);
		}

		public static DnsDeviceData? GetDevice(ManagementObject mo)
		{
			var caption = (string?)mo[Win32_NetworkAdapterConsts.Caption];
			var description = (string?)mo[Win32_NetworkAdapterConsts.Description];
			var macaddress = (string?)mo[Win32_NetworkAdapterConsts.MACAddress];
			var dnsServers = (string[]?)mo[Win32_NetworkAdapterConsts.DNSServerSearchOrder];

			List<QualifierData> qualifiers = new List<QualifierData>();
			foreach (var qualifier in mo.Qualifiers)
			{
				qualifiers.Add(qualifier);
			}
			Logger.Warn($"This network device has no identifier, {mo.Path} {string.Join(" | ", qualifiers.Select(x => $"Name {x.Name} - IsLocal {x.IsLocal} - Amended {x.IsAmended} - PropagatesToInstance {x.PropagatesToInstance} - PropagatesToSubclass {x.PropagatesToSubclass} - IsOverridable {x.IsOverridable} - Value {x.Value}"))}");

			if (dnsServers != null && dnsServers.Length > 0 && (caption != null || description != null || macaddress != null))
			{
				return null;
			}
			return new DnsDeviceData()
			{
				Caption = caption,
				Description = description,
				MACAddress = macaddress,
				DnsServers = dnsServers
			};
		}

		public string[] DnsServers { get; set; }
	}
}
