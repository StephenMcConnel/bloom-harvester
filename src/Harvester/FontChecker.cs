using BloomHarvester.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BloomHarvester
{
	interface IFontChecker
	{
		List<string> GetMissingFonts(string bookPath, out bool success);
	}

	internal class FontChecker : IFontChecker
	{
		private readonly int _kGetFontsTimeoutSecs;
		private IBloomCliInvoker _bloomCli;
		private IMonitorLogger _logger;

		public FontChecker(int getFontsTimeoutSecs, IBloomCliInvoker bloomCli, IMonitorLogger logger)
		{
			_kGetFontsTimeoutSecs = getFontsTimeoutSecs;
			_bloomCli = bloomCli;
			_logger = logger ?? NullLogger.Instance;
		}

		/// <summary>
		/// Gets the names of the fonts referenced in the book but not found on this machine.
		/// </summary>
		/// <param name="bookPath">The path to the book folder</param>
		/// Returns a list of the fonts that the book reference but which are not installed, or null if there was an error
		public List<string> GetMissingFonts(string bookPath, out bool success)
		{
			var missingFonts = new List<string>();

			using (var reportFile = SIL.IO.TempFile.CreateAndGetPathButDontMakeTheFile())
			{
				string bloomArguments = $"getfonts --bookpath \"{bookPath}\" --reportpath \"{reportFile.Path}\"";
				bool subprocessSuccess = _bloomCli.StartAndWaitForBloomCli(bloomArguments, _kGetFontsTimeoutSecs * 1000, out int exitCode, out string stdOut, out string stdError);

				if (!subprocessSuccess || !SIL.IO.RobustFile.Exists(reportFile.Path))
				{
					_logger.LogError("Error: Could not determine fonts from book located at " + bookPath);
					_logger.LogVerbose("Standard output:\n" + stdOut);
					_logger.LogVerbose("Standard error:\n" + stdError);

					success = false;
					return missingFonts;
				}

				var bookFontNames = GetFontsFromReportFile(reportFile.Path);
				missingFonts = GetMissingFonts(bookFontNames);
			}

			success = true;
			return missingFonts;
		}

		internal static List<string> GetMissingFonts(IEnumerable<string> bookFontNames)
		{
			var computerFontNames = GetInstalledFontNames();

			var missingFonts = new List<string>();
			foreach (var bookFontName in bookFontNames)
			{
				if (bookFontName == "serif" || bookFontName == "sans-serif" || bookFontName == "monospace")
				{
					// These are fallback families. We don't need to verify the existence of these fonts.
					// The browser or epub reader will automatically supply a fallback font for them.
					continue;
				}
				if (!String.IsNullOrEmpty(bookFontName) && !computerFontNames.Contains(bookFontName))
				{
					missingFonts.Add(bookFontName);
				}
			}

			return missingFonts;
		}

		/// <summary>
		/// Gets the fonts referenced by a book baesd on a "getfonts" report file. 
		/// </summary>
		/// <param name="filePath">The path to the report file generated from Bloom's "getfonts" CLI command. Each line of the file should correspond to 1 font name.</param>
		/// <returns>A list of strings, one for each font referenced by the book.</returns>
		private static List<string> GetFontsFromReportFile(string filePath)
		{
			// Precondition: Caller should guarantee that filePath exists
			var referencedFonts = new List<string>();

			string[] lines = File.ReadAllLines(filePath);   // Not expecting many lines in this file

			if (lines != null)
			{
				foreach (var fontName in lines)
				{
					referencedFonts.Add(fontName);
				}
			}

			return referencedFonts;
		}

		// Returns the names of each of the installed font families as a set of strings
		private static HashSet<string> GetInstalledFontNames()
		{
			var installedFontCollection = new System.Drawing.Text.InstalledFontCollection();

			var fontFamilyDict = new HashSet<string>(installedFontCollection.Families.Select(x => x.Name), StringComparer.OrdinalIgnoreCase);
			return fontFamilyDict;
		}
	}
}
