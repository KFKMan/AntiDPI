using DnsClient;
using NLog;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace AntiDPI
{
	public class LocalWebProxy : ProxyBaseWithId<LocalWebProxySettings>
	{
		private ILogger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		public LocalWebProxy(ProtocolType protocol, LocalWebProxySettings settings) : base(protocol, settings) 
		{ 
		
		}

		public override async Task HandleClientWithIdAsync(Socket client, ulong ClientId)
		{
			try
			{
				NetworkStream ClientStream = client.GetStream();

				Logger.Info($"Client {ClientId} Connected | {client.LocalEndPoint.GetDefaultIfNull().ToString()} - {client.RemoteEndPoint.GetDefaultIfNull().ToString()}");

				if (client.Available <= 0)
				{
					Logger.Warn($"Client {ClientId} No Data Available ?");
					return;
#warning Pooling can be more good
				}

				var firstRequest = await client.ReceiveAvailableAsync();
				if (firstRequest.Length <= 0)
				{
#warning Relook in the end
					Logger.Warn($"Client {ClientId} Connection Closed, No Any First Data Received");
					return;
				}

				string firstRequestAsString = System.Text.Encoding.UTF8.GetString(firstRequest,0,firstRequest.Length);

				Logger.Info($"Client {ClientId} | First Data | {firstRequestAsString}");


				/*
				string[] datas = firstMessage.Split(" ");
				if (datas[0] == "CONNECT" || datas[0] == "GET" || datas[0] == "POST" || datas[0] == "PUT" || datas[0] == "DELETE" || datas[0] == "HEAD")
				{
					string endpoint = datas[1];
					string[] endpointdata = endpoint.Split(":");

					bool IsSecure = datas[0] == "CONNECT";
				}
				else
				{
					Logger.Warn("Unhandled Data Type");
				}
				*/


				bool IsSecure = false;

				string[] firstRequestHeaders = firstRequestAsString.Split(" ");
				if (firstRequestHeaders[0] == "CONNECT")
				{
					Logger.Info($"Client {ClientId} Connection Is Secure (HTTPS)");
					IsSecure = true;
				}

				string endpoint_string = firstRequestHeaders[1];
				string endpoint_host = DataHelper.GetEndpointHost(endpoint_string);
				int endpoint_port = DataHelper.GetEndpointPort(endpoint_string);

				//IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(endpoint_host), endpoint_port);

				if(Settings.DnsServers.Length > 0)
				{
					Logger.Info($"Client {ClientId} Using Custom Dns Servers for {endpoint_host}:{endpoint_port}");

					try
					{
						LookupClient LDnsClient = new LookupClient(new LookupClientOptions(Settings.DnsServers)
						{
							Timeout = TimeSpan.FromSeconds(10)
						});

						var hostEntry = await LDnsClient.GetHostEntryAsync(endpoint_host);
						endpoint_host = hostEntry.AddressList[0].ToString();
					}
					catch (Exception ex)
					{
						Logger.Warn($"Client {ClientId} Custom Dns Server Request Error on 'DnsClient' Library => Trying Other Library | {ex.ToString()}");
						DNS.Client.DnsClient dnsClient = new(Settings.DnsServers[0]);
						var dnsResponse = await dnsClient.Lookup(endpoint_host, DNS.Protocol.RecordType.A);
						endpoint_host = dnsResponse[0].ToString();
					}
					

					Logger.Info($"Client {ClientId} Dns Server Result {endpoint_host}:{endpoint_port}");
				}

				using (TcpClient remote = new TcpClient(endpoint_host,endpoint_port))
				{
					Stream RemoteStream = remote.GetStream();

					async Task SendDataToRemoteWithSize(byte[] data, int length)
					{
						Logger.Info($"Client {ClientId} | Proxy->Server {endpoint_string} | Raw | {length} | {BitConverterX.ToString(data, 0, length)}");
						await RemoteStream.WriteAsync(data, 0, length);
					}

					async Task SendDataToRemote(byte[] data)
					{
						await SendDataToRemoteWithSize(data, data.Length);
					}

					async Task SendDataToRemoteString(string data)
					{
						var strdata = Encoding.UTF8.GetBytes(data);

						Logger.Info($"Client {ClientId} | Proxy->Server {endpoint_string} | {data}");

						await SendDataToRemote(strdata);
					}

					async ValueTask<int> ReadDataFromRemote(byte[] buffer)
					{
						int size = await RemoteStream.ReadAsync(buffer, 0, buffer.Length);

						Logger.Info($"Client {ClientId} | {endpoint_string} Server->Proxy | Raw | {buffer.Length} Requested {size} Getted | {BitConverterX.ToString(buffer, 0, size)}");

						return size;
					}

					async ValueTask<string> ReadDataFromRemoteString(byte[] buffer)
					{
						int size = await ReadDataFromRemote(buffer);
						var data = Encoding.UTF8.GetString(buffer, 0, size);
						Logger.Info($"Client {ClientId} | {endpoint_string} Server->Proxy | {data}");

						return data;
					}

					async Task SendDataToLocalWithSize(byte[] buffer, int length)
					{
						Logger.Info($"Client {ClientId} | Proxy->Local | Raw | {length} | {BitConverterX.ToString(buffer, 0, length)}");

						await ClientStream.WriteAsync(buffer, 0, length);
					}

					async Task SendDataToLocal(byte[] buffer)
					{
						await SendDataToLocalWithSize(buffer, buffer.Length);
					}

					async Task SendDataToLocalString(string data)
					{
						Logger.Info($"Client {ClientId} | Proxy->Local | {data}");
						var buffer = Encoding.UTF8.GetBytes(data);

						await SendDataToLocal(buffer);
					}

					async ValueTask<int> ReadDataFromLocal(byte[] buffer)
					{
						int size = await ClientStream.ReadAsync(buffer);
						Logger.Info($"Client {ClientId} | Local->Proxy | Raw | {buffer.Length} Requested {size} Getted | {BitConverterX.ToString(buffer, 0, size)}");
						return size;
					}

					async ValueTask<string> ReadDataFromLocalString(byte[] buffer)
					{
						int size = await ReadDataFromLocal(buffer);
						string data = Encoding.UTF8.GetString(buffer, 0, size);

						Logger.Info($"Client {ClientId} | Local->Proxy | {data}");

						return data;
					}
					if (IsSecure)
					{
						string secureMSG = $"HTTP/1.1 200 Connection Established\r\n\r\n";

						Logger.Info($"Client {ClientId} Connection Is Https | Sending Proxy OK State");

						await SendDataToLocalString(secureMSG);
					}
					else
					{
						Logger.Info($"Client {ClientId} Connection Is Http | Sending Request Data | Proxy->Server | {firstRequestAsString}");
						await SendDataToRemoteString(firstRequestAsString);
					}

					//Collecting Garbages
					firstRequest = Array.Empty<byte>();
					firstRequestAsString = string.Empty;
					firstRequestHeaders = Array.Empty<string>();

					while (remote.Connected && client.Connected)
					{
						//Poll can be async
						if (remote.Client.Poll(10000, SelectMode.SelectRead))
						{
							byte[] buffer = new byte[remote.Available];
							int size = await ReadDataFromRemote(buffer);
							//Process It
							await SendDataToLocalWithSize(buffer, size);
						}
						if (client.Poll(10000, SelectMode.SelectRead))
						{
							byte[] buffer = new byte[client.Available];
							int size = await ReadDataFromLocal(buffer);
							//Process It
							await SendDataToRemoteWithSize(buffer, size);
						}
					}

					if (!remote.Connected)
					{
						Logger.Info($"Client {ClientId} Remote Connection {endpoint_string} Disconnected");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Client {ClientId} Crashed With {ex.ToString()}");
			}

			Logger.Info($"Client {ClientId} Disconnected");
		}


	}
}
