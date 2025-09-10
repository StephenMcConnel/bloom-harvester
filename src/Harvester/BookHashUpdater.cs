using BloomHarvester.IO;
using BloomHarvester.LogEntries;
using BloomHarvester.Logger;
using BloomHarvester.Parse;
using BloomHarvester.WebLibraryIntegration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace BloomHarvester
{
	public class BookHashUpdater : IDisposable
	{
		EnvironmentSetting _environment;
		IMonitorLogger _logger = new ConsoleLogger();
		ParseClient _parseClient;
		private DateTime _initTime;
		private bool disposedValue;
		HarvesterS3Client _s3DownloadClient;
		HarvesterBookDownload _downloadClient;
		IFileIO _fileIO = new FileIO();
		UpdateHashesInParseOptions _options;

		public static void UpdateHashes(UpdateHashesInParseOptions options)
		{
			using (var updater = new BookHashUpdater(options))
			{
				updater.Run();
			}
		}

		private BookHashUpdater(UpdateHashesInParseOptions options)
		{
			_options = options;
			_environment = options.Environment;
			_parseClient = new ParseClient(_environment, _logger);
			(string downloadBucketName, string uploadBucketName) = Harvester.GetS3BucketNames(_environment);
			_s3DownloadClient = new HarvesterS3Client(downloadBucketName, _environment, true);
			_downloadClient = new HarvesterBookDownload(_s3DownloadClient);
			_initTime = DateTime.Now;
		}

		private void Run()
		{
			var bookList = _parseClient.GetBooks(out bool didExitPrematurely, _options.QueryWhere);
			if (didExitPrematurely)
			{
				_logger.LogError("Exiting prematurely due to an error while fetching books from Parse.");
				return;
			}
			if (bookList == null || bookList.Count == 0)
			{
				_logger.LogError("No books found in Parse.");
				return;
			}
			foreach (var bookModel in bookList)
			{
				try
				{
					var book = new Book(bookModel, _logger, _fileIO);
					ProcessOneBook(book);
				}
				catch (Exception e)
				{
					_logger.LogError($"Error processing book {bookModel.ObjectId}: {e.Message}");
					continue;
				}
			}
		}

		private void ProcessOneBook(Book book)
		{
			string collectionBookDir = null;
			try
			{
				string message = $"Processing: {book.Model.BaseUrl}";
				_logger.LogVerbose(message);

				_logger.TrackEvent("ProcessOneBook Start");
				string decodedUrl = HttpUtility.UrlDecode(book.Model.BaseUrl);
				// For this process, we usually want to skip downloads as much as possible.
				collectionBookDir = Harvester.DownloadBookAndCopyToCollectionFolder(book, decodedUrl, book.Model,
					_logger, null, _downloadClient, _environment, _options.ForceDownload, true);
				var analyzer = BookAnalyzer.FromFolder(collectionBookDir);
				book.Analyzer = analyzer;
				var logEntries = new List<LogEntry>();
				var isSuccessful = Harvester.UpdateHashes(book, analyzer, collectionBookDir, logEntries, _logger, null);
				if (isSuccessful)
				{
					_logger.TrackEvent("Writing updated hashes to parse");
					if (!_options.DryRun)
						book.Model.FlushUpdateToDatabase(_parseClient);
				}
			}
			catch (Exception e)
			{
				_logger.LogError($"Error processing book {book.Model.BaseUrl}: {e.Message}");
				return;
			}
			finally
			{
				// clean up after ourselves: we only need to preserve the copy in the download cache folder.
				if (Directory.Exists(collectionBookDir))
					Directory.Delete(collectionBookDir, true);
			}
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_logger.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
