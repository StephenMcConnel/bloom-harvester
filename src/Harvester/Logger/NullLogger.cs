using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloomHarvester.Logger
{
	// This class is used when you don't want to actually log anything and just want to send it to /dev/null instead.
	class NullLogger : IMonitorLogger
	{
		// A semi-singleton pattern, except not enforcing it via private constructor
		private static NullLogger _instance;
		public static NullLogger Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new NullLogger();
				}
				return _instance;
			}
		}

		public void Dispose()
		{
		}

		public void LogCritical(string messageFormat, params object[] args)
		{
		}

		public void LogError(string messageFormat, params object[] args)
		{
		}

		public void LogInfo(string messageFormat, params object[] args)
		{
		}

		public void LogVerbose(string messageFormat, params object[] args)
		{
		}

		public void LogWarn(string messageFormat, params object[] args)
		{
		}

		public void TrackEvent(string eventName)
		{
		}
	}
}
