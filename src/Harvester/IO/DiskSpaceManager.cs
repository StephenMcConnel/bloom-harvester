using System;
using BloomHarvester.Logger;
using BloomHarvester.WebLibraryIntegration;

namespace BloomHarvester.IO
{
	interface IDiskSpaceManager
	{
		bool CleanupIfNeeded();
	}

	internal class DiskSpaceManager : IDiskSpaceManager
	{
		private readonly TimeSpan kMinTimeBetweenCleanups = new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0);

		private DateTime LastCleanedTime { get; set; } = DateTime.MinValue;

		public DiskSpaceManager(IDriveInfo driveInfo, IMonitorLogger logger, IIssueReporter issueReporter)
		{
			this.DriveInfo = driveInfo;
			this.Logger = logger;
			this.IssueReporter = issueReporter;
		}

		public IDriveInfo DriveInfo { get; set; }
		public IMonitorLogger Logger { get; set; }
		public IIssueReporter IssueReporter { get; set; }

		/// <summary>
		/// Checks if cleanup is necessary and allowed.
		/// If so, performs it.
		/// </summary>
		/// <returns>True if cleanup was actually performed. False otherwise.</returns>
		public bool CleanupIfNeeded()
		{
			bool isDiskSpaceLow = IsDiskSpaceLow();

			if (isDiskSpaceLow && HasSufficientTimeElapsedSinceLastCleanup())
			{
				Cleanup();
				return true;
			}

			return false;
		}

		private bool IsDiskSpaceLow()
		{
			// For now, just say < 10 GB is low
			return this.DriveInfo.AvailableFreeSpace < 10737418240;
		}

		private bool HasSufficientTimeElapsedSinceLastCleanup()
		{
			var now = DateTime.Now;
			var timeSinceLastCleanup = now.Subtract(LastCleanedTime);
			return timeSinceLastCleanup >= kMinTimeBetweenCleanups;
		}

		private void Cleanup()
		{
			this.Logger.TrackEvent("Low Disk Space");

			// ENHANCE: In the future, you could delete some old books out of the Harvester directory
			// But let's not bother with any risky stuff like deleting stuff off the hard drive.
			// For now, we'll just yell about the situation
			string description = $"Harvester only has {this.DriveInfo.AvailableFreeSpace} bytes left. Please cleanup some space soon.";
			this.IssueReporter.ReportError("Low Disk Space", description, "");

			LastCleanedTime = DateTime.Now;
		}
	}
}
