using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bloom;
using BloomHarvester.WebLibraryIntegration;
using NUnit.Framework;


namespace BloomHarvesterTests.WebLibraryIntegration
{
	[TestFixture]
	public class YouTrackIssueConnectorTests
	{
		[Test]
		public void TestIssueSubmission()
		{
			var issueId = YouTrackIssueConnector.SubmitToYouTrack("Harvester Test Issue",
				"This is a test from Harvester, which apparently cannot be overemphasized.", "AUT");
			Assert.That(issueId, Is.Not.Null);
			Assert.That(issueId, Does.StartWith("AUT-"));

			// Creating a new submitter seems a bit wasteful, and a bit too far into implementation details,
			// but it's the only way to delete the newly created issue.
			var submitter = new Bloom.YouTrackIssueSubmitter("AUT");
			var deleted = submitter.DeleteIssue(issueId);
			Assert.That(deleted, Is.True);
		}
	}
}
