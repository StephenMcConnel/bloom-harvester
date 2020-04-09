using Bloom;
using Bloom.WebLibraryIntegration;

namespace BloomHarvester.WebLibraryIntegration
{
	internal interface IBookTransfer
	{
		string HandleDownloadWithoutProgress(string url, string destRoot);
	}

	/// <summary>
	/// This class is basically just a wrapper around Bloom's version of BookTransfer
	/// that marks that it implements the IBookTransfer interface (to make our unit testing life easier)
	/// </summary>
	class HarvesterBookTransfer : BookTransfer, IBookTransfer
	{
		internal HarvesterBookTransfer(BloomParseClient parseClient, BloomS3Client bloomS3Client, BookThumbNailer htmlThumbnailer)
			: base(parseClient, bloomS3Client, htmlThumbnailer, new Bloom.BookDownloadStartingEvent())
		{
		}

		public new string HandleDownloadWithoutProgress(string url, string destRoot)
		{
			// Just need to declare this as public instead of internal (interfaces...)
			return base.HandleDownloadWithoutProgress(url, destRoot);
		}
	}
}
