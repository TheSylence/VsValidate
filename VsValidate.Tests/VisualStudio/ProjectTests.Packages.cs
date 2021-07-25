using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public partial class ProjectTests
	{
		[Fact]
		public void PackageReferenceShouldBeReadWhenStoredInMultipleGroups()
		{
			// Arrange			
			var xml = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package.Name", "1.2.3.4-pre5")
				.WithItemGroup()
				.WithPackageReference("Package.Two", "0.1")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			Assert.Contains(sut.PackageReferences, x => x.Name == "Package.Name" && x.Version == "1.2.3.4-pre5");
			Assert.Contains(sut.PackageReferences, x => x.Name == "Package.Two" && x.Version == "0.1");
		}

		[Fact]
		public void PackageReferenceShouldHaveCorrectVersionWhenNestedAsTag()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package.Name", "1.0", true)
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			Assert.Contains(sut.PackageReferences, x => x.Name == "Package.Name" && x.Version == "1.0");
		}

		[Fact]
		public void PackageReferenceShouldHaveEmptyVersionWhenNotSpecified()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package.Name", null)
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			Assert.Contains(sut.PackageReferences, x => string.IsNullOrEmpty(x.Version));
		}

		[Fact]
		public void PackageReferencesShouldBeReadWhenStoredInSingleGroup()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package.Name", "1.2.3.4-pre5")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var package = Assert.Single(sut.PackageReferences);
			Assert.Equal("Package.Name", package!.Name);
			Assert.Equal("1.2.3.4-pre5", package.Version);
			Assert.Null(package.Condition);
		}

		[Fact]
		public void PackageReferenceShouldContainConditionOfItemGroup()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup("condition-value")
				.WithPackageReference("Package.Name", "1.2.3.4-pre5")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var package = Assert.Single(sut.PackageReferences);
			Assert.NotNull(package!.Condition);
			Assert.Equal("condition-value", package.Condition!.Expression);
		}
	}
}