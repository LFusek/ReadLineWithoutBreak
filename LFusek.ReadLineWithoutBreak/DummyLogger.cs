#if DEBUG

using System.IO;

namespace LFusek.ReadLineWithoutBreak
{
	internal static class DummyLogger
	{
		private const string LOG_FILE_PATH = ".log";
		private static readonly StreamWriter StreamWriter = new StreamWriter(LOG_FILE_PATH);

		internal static void WriteLine(string text)
		{
			StreamWriter.WriteLine(text);
			StreamWriter.Flush();
		}
	}

}

#endif
