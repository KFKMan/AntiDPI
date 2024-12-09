//#define DETAILEDLOG

namespace AntiDPI
{
	public class BitConverterX
	{
		public static string ToString(byte[] buffer, int offset, int length)
		{
#if DETAILEDLOG
			return BitConverter.ToString(buffer, offset, length);
#else
			return "Detailed Log Disabled";
#endif
		}
	}
}
