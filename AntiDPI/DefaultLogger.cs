using NLog;
using NLog.Config;
using NLog.Targets;

namespace AntiDPI
{

	public static class DefaultLogger
	{
		public static LogFactory LogFactory = CreateFactory();

		private static LogFactory CreateFactory()
		{
			#region We want to delete main log files (not archived, archived logs will be deleted automatically by NLog)
			List<string> filesToRemove = new();
			foreach(string file in Directory.GetFiles(Directory.GetCurrentDirectory(),"*.log"))
			{
				string[] loots = file.Split("/");
				string lastLoot = loots.Last();

				string[] reverseLoots = lastLoot.Split(@"\");
				string lastReverseLoot = reverseLoots.Last();

				string[] primes = lastReverseLoot.Split(".");
				if(primes.Length > 2)
				{
					string number = primes[primes.Length - 2]; //a.1.log we want 1
					if(!int.TryParse(number,out var num))
					{
						filesToRemove.Add(file);
					}
				}
			}

			foreach(string file in filesToRemove)
			{
				try
				{
					File.Delete(file);
				}
				catch
				{

				}
			}
			#endregion

			var factory = LogManager.Setup((opt) =>
			{
				opt.LoadConfiguration((lg) =>
				{
					lg.ForLogger().WriteToFile("${logger}.log", "${time}|${level:uppercase=true}|${message:withexception=true}", archiveAboveSize: 1024 * 1024,maxArchiveFiles: 150, maxArchiveDays: 1).WithAutoFlush((x) => true, false);
				});

				
			});

			return factory;
		}
	}
}
