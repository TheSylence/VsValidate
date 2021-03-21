using System.Linq;
using System.Xml.Linq;
using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public class ProjectTests
	{
		[Fact]
		public void AllPropertyGroupsShouldBeRead()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("Property1", "1")
				.WithPropertyGroup()
				.WithProperty("Property2", "2")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			Assert.Equal(2, sut.PropertyGroups.Count);
			var propertyNames = sut.PropertyGroups.SelectMany(x => x.Properties).Select(x => x.Name).ToList();
			Assert.Contains("Property1", propertyNames);
			Assert.Contains("Property2", propertyNames);
		}

		[Fact]
		public void ConditionOfPropertyGroupShouldContainExpressionWhenItExists()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithPropertyGroup("condition-value")
				.WithProperty("ExistingProperty", "value")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var condition = Assert.Single(sut.PropertyGroups)!.Condition;
			Assert.NotNull(condition);
			Assert.Equal("condition-value", condition!.Expression);
		}

		[Fact]
		public void FindPropertyByNameShouldNotFindNonMatchingProperties()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("Property", "1")
				.WithPropertyGroup()
				.WithProperty("Property2", "2")
				.WithPropertyGroup()
				.WithProperty("Property", "2")
				.BuildXml();

			var sut = new Project(xml);

			// Act
			var actual = sut.FindPropertyByName("Property");

			// Assert
			Assert.Equal(2, actual.Count());
		}

		[Fact]
		public void FindPropertyByNameShouldReturnAllFoundProperties()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("Property", "1")
				.WithPropertyGroup()
				.WithProperty("Property", "2")
				.BuildXml();

			var sut = new Project(xml);

			// Act
			var actual = sut.FindPropertyByName("Property");

			// Assert
			Assert.Equal(2, actual.Count());
		}

		[Fact]
		public void GroupOfPropertyShouldHaveCorrectReference()
		{
			// Arrange
			var sut = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("ExistingProperty", "value").Build();

			// Act
			var actual = sut.PropertyGroups.Single().Properties.Single();

			// Assert
			Assert.Same(sut.PropertyGroups.Single(), actual.Group);
		}

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
		public void ProjectReferenceShouldBeReadWhenStoredInMultipleGroups()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup()
				.WithProjectReference("project1")
				.WithItemGroup()
				.WithProjectReference("project2")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			Assert.Contains(sut.ProjectReferences, x => x.Name == "project1");
			Assert.Contains(sut.ProjectReferences, x => x.Name == "project2");
		}

		[Fact]
		public void ProjectReferenceShouldBeReadWhenStoredInSingleGroup()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup()
				.WithProjectReference("project/path.file")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var project = Assert.Single(sut.ProjectReferences);
			Assert.Equal("project/path.file", project!.Name);
			Assert.Null(project.Condition);
		}

		[Fact]
		public void ProjectReferenceShouldContainConditionOfItemGroup()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.WithItemGroup("condition-value")
				.WithProjectReference("project/path.file")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var project = Assert.Single(sut.ProjectReferences);
			Assert.NotNull(project!.Condition);
			Assert.Equal("condition-value", project.Condition!.Expression);
		}

		[Fact]
		public void PropertyValueShouldBeNullForEmptyProject()
		{
			// Arrange
			var xml = new XDocument();
			var sut = new Project(xml);

			// Act
			var actual = sut.PropertyValue("property");

			// Assert
			Assert.Null(actual);
		}

		[Fact]
		public void PropertyValueShouldBeNullWhenNotFound()
		{
			// Arrange
			var sut = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("ExistingProperty", "value").Build();

			// Act
			var actual = sut.PropertyValue("NonExistingProperty");

			// Assert
			Assert.Null(actual);
		}

		[Fact]
		public void PropertyValueShouldHaveCorrectValueWhenFound()
		{
			// Arrange
			var sut = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("ExistingProperty", "value").Build();

			// Act
			var actual = sut.PropertyValue("ExistingProperty");

			// Assert
			Assert.Equal("value", actual);
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