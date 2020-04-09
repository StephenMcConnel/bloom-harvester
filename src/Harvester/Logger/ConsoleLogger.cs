using System;
using System.Collections.Generic;
using System.Text;

namespace BloomHarvester.Logger
{
	/// <summary>
	/// An implementation of IMonitorLogger that writes the logs to Standard Error
	/// </summary>
	class ConsoleLogger : IMonitorLogger
	{
		public void Dispose()
		{
		}

		public void LogCritical(string messageFormat, params object[] args)
		{
			if (args != null && args.Length > 0)
				Console.Error.WriteLine("Log Critical: " + messageFormat, args);
			else
				Console.Error.WriteLine("Log Critical: " + messageFormat);
		}

		public void LogError(string messageFormat, params object[] args)
		{
			if (args != null && args.Length > 0)
				Console.Error.WriteLine("Log Error: " + messageFormat, args);
			else
				Console.Error.WriteLine("Log Error: " + messageFormat);
		}

		public void LogInfo(string messageFormat, params object[] args)
		{
			if (args != null && args.Length > 0)
				Console.Error.WriteLine("Log Info: " + messageFormat, args);
			else
				Console.Error.WriteLine("Log Info: " + messageFormat);
		}

		public void LogVerbose(string messageFormat, params object[] args)
		{
			if (args != null && args.Length > 0)
				// FYI, this will interpret messageFormat as a format string...
				// which means that any braces that happen to be in the string will get interpreted that way.
				Console.Error.WriteLine("Log Verbose: " + messageFormat, args);
			else
				Console.Error.WriteLine("Log Verbose: " + messageFormat);
		}

		public void LogWarn(string messageFormat, params object[] args)
		{
			if (args != null && args.Length > 0)
				Console.Error.WriteLine("Log Warn: " + messageFormat, args);
			else
				Console.Error.WriteLine("Log Warn: " + messageFormat);
		}

		public void TrackEvent(string eventName)
		{
			Console.Error.WriteLine($"Event: {eventName}");
		}
	}
}
