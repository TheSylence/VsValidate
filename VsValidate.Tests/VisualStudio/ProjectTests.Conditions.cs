using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public partial class ProjectTests
	{
		[Fact]
		public void ConditionShouldBeAppliedToItemGroupWhenNestedInChooseElement()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.BeginChoose()
				.WithWhen("variable == value")
				.WithItemGroup()
				.WithPackageReference("package", "1.2")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var reference = Assert.Single(sut.PackageReferences);
			Assert.NotNull(reference!.Condition);
			Assert.Equal("variable == value", reference.Condition!.Expression);
		}

		[Fact]
		public void ConditionShouldBeAppliedToPropertyGroupWhenNestedInChooseElement()
		{
			// Arrange
			var xml = new ProjectBuilder()
				.BeginChoose()
				.WithWhen("variable == value")
				.WithPropertyGroup()
				.WithProperty("prop", "value")
				.BuildXml();

			// Act
			var sut = new Project(xml);

			// Assert
			var propertyGroup = Assert.Single(sut.PropertyGroups);
			Assert.NotNull(propertyGroup!.Condition);
			Assert.Equal("variable == value", propertyGroup.Condition!.Expression);
		}
	}
}