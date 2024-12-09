using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DnsClient;
using NLog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UtfUnknown;

namespace AntiDPI
{
	public class LocalDnsProxy : ProxyBaseWithId<LocalDNSProxySettings>
	{
		public const int DNSTimeout = 40000;

		private ILogger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		public LocalDnsProxy(ProtocolType protocol, LocalDNSProxySettings settings) : base(protocol, settings)
		{

		}

		private readonly Encoding DefaultEncoding = Encoding.UTF8; //it can't be const

		private string Randomize(string text)
		{
			int i = 0;
			List<char> modifiedString = new();
			foreach (char c in text)
			{
				if (!char.IsLetter(c))
				{
					modifiedString.Add(c);
					continue;
				}

				char modified;
				if(i % 2 == 0)
				{
					modified = char.ToUpper(c);
				}
				else
				{
					modified = char.ToLower(c);
				}

				modifiedString.Add(modified);
				i++;
			}

			return new string(modifiedString.ToArray());
		}

		public override async Task HandleClientWithIdAsync(Socket client, ulong ClientId)
		{
			try
			{
				NetworkStream ClientStream = client.GetStream();
				LookupClient DnsClient = new();

				Logger.Info($"Client {ClientId} Connected | {client.LocalEndPoint.GetDefaultIfNull().ToString()} - {client.RemoteEndPoint.GetDefaultIfNull().ToString()}");

				async Task ProcessRequest(Request dnsRequest)
				{
					Header modifiedHeader = new();
					modifiedHeader.Id = dnsRequest.Id;
					modifiedHeader.RecursionDesired = dnsRequest.RecursionDesired;
					modifiedHeader.OperationCode = dnsRequest.OperationCode;

					List<Question> modifiedQuestions = dnsRequest.Questions.ToList();
					List<IResourceRecord> modifiedAdditionalRecords = dnsRequest.AdditionalRecords.ToList();
					if (Settings.RandomizeHostname)
					{
						List<Question> modifiedQuestionsCache = new();
						foreach (var dnsRequestQuestion in modifiedQuestions)
						{
							var domain = dnsRequestQuestion.Name;

							var detectedEncoding = CharsetDetector.DetectFromBytes(domain.ToArray());
							var targetEncoding = detectedEncoding.Details.Count > 0 ? (detectedEncoding.Details[0].Encoding ?? DefaultEncoding) : DefaultEncoding;

							var domainStr = domain.ToString(targetEncoding);
							var modifiedDomainStr = Randomize(domainStr);

							var modifiedDomain = new Domain(modifiedDomainStr.Split("."), targetEncoding);

							var modifiedQuestion = new Question(modifiedDomain, dnsRequestQuestion.Type, dnsRequestQuestion.Class);
							modifiedQuestionsCache.Add(modifiedQuestion);
						}
						modifiedQuestions = modifiedQuestionsCache;
					}


					modifiedHeader.QuestionCount = modifiedQuestions.Count;
					modifiedHeader.AdditionalRecordCount = modifiedAdditionalRecords.Count;

					var modifiedDnsRequest = new Request(modifiedHeader, modifiedQuestions, modifiedAdditionalRecords);

					// TODO: Implement DoH, TLS and others...
					// TODO: Add Logging
					// TODO: Use Pool, Don't Block The CPU (Send/Receive using so much CPU, it's async yes but very high CPU usage will block work)

					using (UdpClient DNSServer = new UdpClient(Settings.TargetServers[0]))
					{
						var modifiedDnsRequestRaw = modifiedDnsRequest.ToArray();

						Logger.Info($"Client {ClientId} Sending DNS dnsRequest | Proxy -> Server | {Encoding.UTF8.GetString(modifiedDnsRequestRaw)}");
						await DNSServer.SendAsync(modifiedDnsRequestRaw, modifiedDnsRequestRaw.Length);
						Logger.Info($"Client {ClientId} Waiting DNS Response | Server -> Proxy");

						if (DNSServer.Client.Poll(DNSTimeout * TimeSpan.FromMilliseconds(1).Microseconds, SelectMode.SelectRead))
						{
							var responseRaw = await DNSServer.ReceiveAsync();

							//There is no need process, the data came is Raw (no DoH, TLS...) and the Local Proxy is using Raw Protocol too (no DoH, TLS...)
							Logger.Info($"Client {ClientId} Sending DNS Response | Proxy -> Local | {Encoding.UTF8.GetString(responseRaw.Buffer, 0, responseRaw.Buffer.Length)}");
							await client.SendAsync(responseRaw.Buffer);
						}
						// TODO: Don't close the client it's not a good way so change it with Error Response
						DNSServer.Close();
						client.Close();
					}
				}

				while (client.Connected)
				{
					if (client.Poll(10000, SelectMode.SelectRead))
					{
						var requestRaw = await client.ReceiveAvailableAsync();
						var request = DNS.Protocol.Request.FromArray(requestRaw);
						Logger.Info($"Client {ClientId} New Dns Request | Local->Proxy");
						await ProcessRequest(request);
						Logger.Info($"Client {ClientId} Dns Request Handled");
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
