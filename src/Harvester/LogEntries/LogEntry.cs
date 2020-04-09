using System;

namespace BloomHarvester.LogEntries
{
	class LogEntry
	{
		public LogEntry()
		{
		}

		public LogEntry(LogLevel level, LogType type, string message)
		{
			Level = level;
			Type = type;
			Message = message;
		}

		public LogLevel Level { get; set; }
		public LogType Type { get; set; }
		public virtual string Message { get; set; }

		public override string ToString()
		{
			return $"{this.Level}: {this.Type.ToString()} - {this.Message}";
		}

		/// <summary>
		/// Reads an entry from harvestLog field and attempts to parse it
		/// </summary>
		/// <param name="logEntry">An entry from the harvestLog field in Parse's books class</param>
		/// <returns>An object which is some type of LogEntry if the string could be parsed, or null otherwise</returns>
		public static LogEntry Parse(string logEntry)
		{
			if (String.IsNullOrEmpty(logEntry))
			{
				return null;
			}

			int endOfLevelIndex = logEntry.IndexOf(":");
			if (endOfLevelIndex < 0)
				return null;
			string levelStr = logEntry.Substring(0, endOfLevelIndex);
			if (!Enum.TryParse(levelStr, out LogLevel level))
				return null;

			int startOfTypeIndex = endOfLevelIndex + 1;	// ok for this to be whitespace
			int endOfTypeIndex = logEntry.IndexOf("-", startOfTypeIndex);
			if (endOfTypeIndex < 0)
				return null;
			string typeStr = logEntry.Substring(startOfTypeIndex, endOfTypeIndex - startOfTypeIndex).Trim();
			if (!Enum.TryParse(typeStr, out LogType type))
				return null;

			string message = logEntry.Substring(endOfTypeIndex + 1).TrimStart();

			var parsedValue = new LogEntry(level, type, message);
			return parsedValue;
		}
	}
}
