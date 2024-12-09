//#define DETAILEDLOG

using NLog;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AntiDPI
{
	public class CertificateHelper
	{
		public static string GenerateRandomPassword(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
			char[] password = new char[length];

			using (var rng = RandomNumberGenerator.Create())
			{
				byte[] data = new byte[length];
				rng.GetBytes(data);

				for (int i = 0; i < length; i++)
				{
					password[i] = chars[data[i] % chars.Length];
				}
			}

			return new string(password);
		}

		public Logger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		public (string certPath, string PWD) CreateOrReturnCertificate()
		{
			const string pwdPath = "certpwd.txt";
			string certName = "CN=MyLocalCert";
			string certFilePath = "localcert.pfx"; // Path to save the certificate
			string certPassword = GenerateRandomPassword(12);

			if (File.Exists(certFilePath) && File.Exists(pwdPath))
			{
				return (certFilePath, File.ReadAllText(pwdPath));
			}

			var rsa = RSA.Create(2048); // Generate a new RSA key
			var request = new CertificateRequest(certName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

			// Set the certificate validity period
			DateTimeOffset notBefore = DateTimeOffset.UtcNow;
			DateTimeOffset notAfter = notBefore.AddYears(1);
			var certificate = request.CreateSelfSigned(notBefore, notAfter);

			// Save the certificate to a .pfx file
			var export = certificate.Export(X509ContentType.Pkcs12, certPassword);
			System.IO.File.WriteAllBytes(certFilePath, export);
			File.CreateText(pwdPath).Dispose();
			File.WriteAllText(pwdPath, certPassword);

			Console.WriteLine($"Certificate created and saved to {certFilePath}");

			// Load the certificate to import it into the Windows Certificate Store
			var cert = new X509Certificate2(certFilePath, certPassword, X509KeyStorageFlags.PersistKeySet);

			// Add the certificate to the Local Machine's Trusted Root Certification Authorities store
			using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
			{
				store.Open(OpenFlags.ReadWrite);
				store.Add(cert);
				store.Close();
			}

			Logger.Info("Certificate added to the Windows Certificate Store.");

			

			return (certFilePath, certPassword);
		}
	}
}
