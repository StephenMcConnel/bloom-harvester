using BloomHarvester.Parse.Model;
using NUnit.Framework;
using VSUnitTesting = Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using BloomHarvester;

namespace BloomHarvesterTests.Parse.Model
{
	[TestFixture]
	public class BookModelTests
	{
		[TestCase("HarvestState")]
		[TestCase("HarvesterId")]
		[TestCase("HarvesterMajorVersion")]
		[TestCase("HarvesterMinorVersion")]
		[TestCase("HarvestStartedAt")]
		[TestCase("HarvestLogEntries")]
		[TestCase("Features")]
		[TestCase("Tags")]
		[TestCase("Show")]
		public void BookModel_GetWriteableMembers_IncludesExpectedMember(string expectedMemberName)
		{
			var book = new BookModel();
			var invoker = new VSUnitTesting.PrivateObject(book);
			var writeableMembers = invoker.Invoke("GetWriteableMembers") as HashSet<string>;

			Assert.That(writeableMembers.Contains(expectedMemberName), Is.True);
		}
		
		[Test]
		public void BookModel_NonEmptyTag_SerializedInJson()
		{
			var book = new BookModel();
			book.Tags = new string[] { "computedLevel:1" };

			string bookJson = JsonConvert.SerializeObject(book);
			string expectedJsonFragment = "\"tags\":[\"computedLevel:1\"]";

			Assert.That(bookJson.Contains(expectedJsonFragment), Is.True);
		}

		[Test]
		public void BookModel_GetNewBookUpdateOperation_AddsUpdateSource()
		{
			// System under test
			var bookUpdateOp = BookModel.GetNewBookUpdateOperation();
			var result = bookUpdateOp._updatedFieldValues;

			// Verification
			var expectedResult = new Dictionary<string, string>();
			expectedResult.Add("updateSource", "\"bloomHarvester\"");

			CollectionAssert.AreEquivalent(expectedResult, result);
		}

		#region GetBloomLibraryBookDetailLink
		[Test]
		public void Book_GetBloomLibraryBookDetailLink_Prod_PopulatesLink()
		{
			var bookModel = new BookModel() { ObjectId = "myObjectId" };

			string url = bookModel.GetDetailLink(EnvironmentSetting.Prod);

			Assert.That(url, Is.EqualTo("https://bloomlibrary.org/browse/detail/myObjectId"));
		}

		[Test]
		public void Book_GetBloomLibraryBookDetailLink_Dev_PopulatesLink()
		{
			var bookModel = new BookModel() { ObjectId = "myObjectId" };

			string url = bookModel.GetDetailLink(EnvironmentSetting.Dev);

			Assert.That(url, Is.EqualTo("https://dev.bloomlibrary.org/browse/detail/myObjectId"));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void Book_GetBloomLibraryBookDetailLink_BadInput_ErrorReported(string badObjectId)
		{
			var book = new BookModel() { ObjectId = badObjectId };

			string url = book.GetDetailLink(EnvironmentSetting.Dev);

			Assert.That(url, Is.Null);
		}
		#endregion
	}
}
