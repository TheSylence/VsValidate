using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public partial class ProjectTests
	{
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
	}
}