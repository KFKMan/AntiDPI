using Microsoft.Win32;
using NLog;
using System.Net;
using System.Net.Sockets;

namespace AntiDPI
{

	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			checkBox1.Checked = AppSettings.AutoProxySettings;

			textBox1.Text = WebProxySettings.IP.ToString();
			numericUpDown1.Value = WebProxySettings.PORT;

			textBox2.Text = DNSProxySettings.IP.ToString();
			numericUpDown2.Value = DNSProxySettings.PORT;
			checkBox2.Checked = DNSProxySettings.RandomizeHostname;

			richTextBox1.Text = string.Join(Environment.NewLine, DNSProxySettings.TargetServers.Select(x => x.ToString()));
			richTextBox2.Text = string.Join(Environment.NewLine, WebProxySettings.DnsServers.Select(x => x.ToString()));
			checkBox3.Checked = AppSettings.UseDnsProxyAsADns;

			WebProxy = new LocalWebProxy(System.Net.Sockets.ProtocolType.Tcp, WebProxySettings);
			DNSProxy = new LocalDnsProxy(ProtocolType.Udp, DNSProxySettings);
		}

		private LocalWebProxy WebProxy;
		private LocalWebProxySettings WebProxySettings = new();

		private LocalDnsProxy DNSProxy;
		private LocalDNSProxySettings DNSProxySettings = new();

		private AppSettings AppSettings = new();

		private ILogger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		private void Form1_Load(object sender, EventArgs e)
		{
			Logger.Info("App Started");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (AppSettings.AutoProxySettings)
			{
				Logger.Info("Auto Proxy...");
				RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);

				if (key == null)
				{
					Logger.Warn("Auto Proxy Cancelled, Regedit Key Is Null");
				}
				else
				{
					key.SetValue("ProxyEnable", WebProxy.IsActive ? 1 : 0);
					key.SetValue("ProxyServer", WebProxy.IsActive ? $"{WebProxySettings.IP}:{WebProxySettings.PORT}" : "");
					key.SetValue("ProxyOverride", "<local>"); // Disable Proxy for Local Addresses
					key.Close();

					System.Diagnostics.Process.Start("rundll32.exe", "url.dll,FileProtocolHandler http://localhost"); //Update Settings

					Logger.Info("Auto Proxy Completed");
				}
			}

			if (AppSettings.UseDnsProxyAsADns)
			{
				WebProxySettings.DnsServers = new IPEndPoint[]
				{
					DNSProxySettings.ServerEndPoint
				};
				Logger.Info($"Web Proxy Dns Servers setted as Local Dns Proxy {string.Join(" | ",WebProxySettings.DnsServers.Select(x => x.ToString()))}");
			}

			try
			{
				WebProxy.Toggle();
			}
			catch (SocketException ex)
			{
				Logger.Warn($"It's looks like a Web Proxy Toggling error for event close, {ex.ToString()}");
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (int.TryParse(numericUpDown1.Value.ToString(), out int port))
			{
				WebProxySettings.PORT = port;
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (IPAddress.TryParse(textBox1.Text, out IPAddress ipAddress))
			{
				WebProxySettings.IP = ipAddress.ToString();
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			AppSettings.AutoProxySettings = checkBox1.Checked;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			button1.Text = WebProxy.IsActive ? "Stop Web Proxy" : "Start Web Proxy";
			button2.Text = DNSProxy.IsActive ? "Stop Dns Proxy" : "Start Dns Proxy";
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			if (int.TryParse(numericUpDown2.Value.ToString(), out int port))
			{
				DNSProxySettings.PORT = port;
			}
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			if (IPAddress.TryParse(textBox2.Text, out IPAddress ipAddress))
			{
				DNSProxySettings.IP = ipAddress.ToString();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				DNSProxy.Toggle();
			}
			catch (SocketException ex)
			{
				Logger.Warn($"It's looks like a Dns Proxy Toggling error for event close, {ex.ToString()}");
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			richTextBox2.Enabled = !checkBox3.Checked;
			AppSettings.UseDnsProxyAsADns = checkBox3.Checked;
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			List<IPEndPoint> targetDNSServers = new();
			foreach (var line in richTextBox1.Lines)
			{
				if (IPEndPoint.TryParse(line, out var result))
				{
					targetDNSServers.Add(result);
				}
			}

			DNSProxySettings.TargetServers = targetDNSServers.ToArray();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			List<string> filesToRemove = new();
			foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.log"))
			{
				filesToRemove.Add(file);
			}

			foreach (string file in filesToRemove)
			{
				try
				{
					File.Delete(file);
				}
				catch
				{

				}
			}
		}
	}
}
