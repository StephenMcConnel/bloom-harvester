using System;
using System.Collections.Generic;
using System.Linq;
using BloomHarvester;
using NUnit.Framework;

namespace BloomHarvesterTests
{
	[TestFixture]
	class FontCheckerTests
	{
		[Test]
		public void GetMissingFonts_BookContainsEmptyStringFont_NotMarkedAsMissing()
		{
			IEnumerable<string> fontNamesUsedInBook = new string[] { "Arial", "" }; // Assumes that Arial is on the machine running the test

			List<string> missingFontsResult = FontChecker.GetMissingFonts(fontNamesUsedInBook);

			Assert.That(missingFontsResult.Count, Is.EqualTo(0), "No missing fonts were expected.");
		}

		[Test]
		public void GetMissingFonts_BookIsMissingFont_MarkedAsMissing()
		{
			string fontName = "qwertuiopasdfghjk";	// a gibberish font name
			IEnumerable<string> fontNamesUsedInBook = new string[] { fontName };

			List<string> missingFontsResult = FontChecker.GetMissingFonts(fontNamesUsedInBook);

			Assert.That(missingFontsResult.Count, Is.EqualTo(1));
			Assert.That(missingFontsResult.First(), Is.EqualTo(fontName));
		}

		// Officially defined generic fallbacks
		[TestCase("serif")]
		[TestCase("sans-serif")]
		[TestCase("monospace")]
		[TestCase("cursive")]
		[TestCase("fantasy")]
		[TestCase("system-ui")]
		[TestCase("ui-serif")]
		[TestCase("ui-sans-serif")]
		[TestCase("ui-monospace")]
		[TestCase("ui-rounded")]
		[TestCase("math")]
		[TestCase("emoji")]
		[TestCase("fangsong")]
		// Unofficial generic fallbacks
		[TestCase("宋体")]
		[TestCase("黑体")]
		[TestCase("楷体")]
		public void GetMissingFonts_BookContainsGenericFallback_NotMarkedAsMissing(string fontFamily)
		{
			// Assumes that the fontFamily is NOT the name of a real font on the machine running the test
			IEnumerable<string> fontNamesUsedInBook = new string[] { fontFamily };

			List<string> missingFontsResult = FontChecker.GetMissingFonts(fontNamesUsedInBook);

			Assert.That(missingFontsResult.Count, Is.EqualTo(0), "No error should be reported if the only missing font is font family");
		}
	}
}
