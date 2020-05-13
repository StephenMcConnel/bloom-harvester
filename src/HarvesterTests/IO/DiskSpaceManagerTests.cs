using System;
using System.Collections.Generic;
using System.Linq;
using BloomHarvester.IO;
using BloomHarvester.Logger;
using BloomHarvester.WebLibraryIntegration;
using NSubstitute;
using NUnit.Framework;

namespace BloomHarvesterTests.IO
{
	[TestFixture]
	class DiskSpaceManagerTests
	{
		IDriveInfo _fakeDriveInfo;
		IIssueReporter _issueReporter;
		IMonitorLogger _logger;

		[SetUp]
		public void SetupBeforeEachTest()
		{
			_fakeDriveInfo = Substitute.For<IDriveInfo>();
			_issueReporter = Substitute.For<IIssueReporter>();
			_logger = Substitute.For<IMonitorLogger>();
		}

		[TestCase(0)]
		[TestCase(9.9)]
		public void CleanupIfNeeded_DiskSpaceLow_PerformsCleanupAndReportsState(double numGigabytes)
		{
			_fakeDriveInfo.AvailableFreeSpace.Returns(ConvertGBToBytes(numGigabytes));

			var manager = new DiskSpaceManager(_fakeDriveInfo, _logger, _issueReporter);

			var wasCleaned = manager.CleanupIfNeeded();

			Assert.That(wasCleaned, Is.True);
			_issueReporter.ReceivedWithAnyArgs(1).ReportError(default, default, default, default);
			_logger.Received(1).TrackEvent("Low Disk Space");
		}

		
		[Test]
		public void CleanupIfNeeded_MultipleCallsWhileDiskSpaceLow_OnlyCleanedOnce()
		{
			_fakeDriveInfo.AvailableFreeSpace.Returns(1000);

			var manager = new DiskSpaceManager(_fakeDriveInfo, _logger, _issueReporter);

			var wasCleaned = manager.CleanupIfNeeded();
			Assert.That(wasCleaned, Is.True, "Test setup failure: Initial cleaning did not happen.");

			// System under test - the 2nd call to CleanupIfNeeded
			wasCleaned = manager.CleanupIfNeeded();

			// Verification
			Assert.That(wasCleaned, Is.False, "Second time should return false because not enough time passed since previous cleanup");
			_issueReporter.ReceivedWithAnyArgs(1).ReportError(default, default, default, default);
			_logger.Received(1).TrackEvent("Low Disk Space");
		}

		[TestCase(10)]
		[TestCase(10.1)]
		[TestCase(10000)]
		public void CleanupIfNeeded_DiskSpaceSufficient_NoCleanup(double numGigabytes)
		{
			_fakeDriveInfo.AvailableFreeSpace.Returns(ConvertGBToBytes(numGigabytes));

			var manager = new DiskSpaceManager(_fakeDriveInfo, _logger, _issueReporter);

			var wasCleaned = manager.CleanupIfNeeded();

			Assert.That(wasCleaned, Is.False);
			_issueReporter.DidNotReceiveWithAnyArgs().ReportError(default, default, default, default);
			_logger.DidNotReceiveWithAnyArgs().TrackEvent(default);
		}

		private static long ConvertGBToBytes(double numGigabytes)
		{
			const int bytesPerGigabyte = 1073741824;
			return (long)(numGigabytes * bytesPerGigabyte);	// truncation is good enough for this
		}
	}
}
