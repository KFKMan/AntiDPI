using NLog;

namespace AntiDPI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.

			File.CreateText("fxlog.txt").Dispose();

			ApplicationConfiguration.Initialize();
			Application.ThreadException += Application_ThreadException;
            Application.Run(new Form1());
        }

        private static ILogger Logger = DefaultLogger.LogFactory.GetCurrentClassLogger();

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
            Logger.Error($"FX Error Accoured {e.Exception.ToString()}");
            
            File.AppendAllText("fxlog.txt", e.Exception.ToString() + Environment.NewLine);
		}
	}
}