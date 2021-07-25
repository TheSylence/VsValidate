using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public partial class ProjectTests
	{
		[Fact]
		public void FileNameShouldBeSet()
		{
			// Arrange
			var xml = new ProjectBuilder().BuildXml();
			const string expected = "expected";

			var sut = new Project(xml, expected);

			// Act
			var actual = sut.FileName;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void SdkShouldBeRead()
		{
			// Arrange
			var xml = new ProjectBuilder("Sdk.Name")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			Assert.Equal("Sdk.Name", sut.Sdk);
		}
	}
}