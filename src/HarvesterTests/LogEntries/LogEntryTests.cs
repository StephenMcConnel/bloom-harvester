using BloomHarvester.LogEntries;
using NUnit.Framework;

namespace BloomHarvesterTests.LogEntries
{
	[TestFixture]
	class LogEntryTests
	{
		[Test]
		public void LogEntry_ToString_MissingFontError_SerializedInCorrectFormat()
		{
			var logEntry = new BloomHarvester.LogEntries.LogEntry(LogLevel.Error, LogType.MissingFont, "madeUpFontName");
			var result = logEntry.ToString();

			Assert.That(result, Is.EqualTo("Error: MissingFont - madeUpFontName"));
		}

		[Test]
		public void LogEntry_Parse_MissingFontErrorString_DeserializedCorrectly()
		{
			var result = BloomHarvester.LogEntries.LogEntry.Parse("Error: MissingFont - madeUpFontName");

			Assert.That(result.Level, Is.EqualTo(LogLevel.Error), "Level should match");
			Assert.That(result.Type, Is.EqualTo(LogType.MissingFont), "Type should match");
			Assert.That(result.Message, Is.EqualTo("madeUpFontName"), "Message should match");
		}
	}
}
