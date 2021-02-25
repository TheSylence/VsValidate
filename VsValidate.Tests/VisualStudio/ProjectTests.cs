using System.Linq;
using System.Xml.Linq;
using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public class ProjectTests
	{
		private static Project ConstructSimpleProject()
		{
			var xml = new XDocument(new XElement("Project",
				new XElement("PropertyGroup", new XElement("ExistingProperty", "value"))));
			return new Project(xml);
		}

		[Fact]
		public void AllPropertyGroupsShouldBeRead()
		{
			// Arrange
			var xml = new XDocument(new XElement("Project",
				new XElement("PropertyGroup",
					new XElement("Property1", "1")),
				new XElement("PropertyGroup",
					new XElement("Property2", "2"))));

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
			var xml = new XDocument(new XElement("Project",
				new XElement("PropertyGroup", new XAttribute("Condition", "condition-value"),
					new XElement("ExistingProperty", "value"))));

			// Act
			var sut = new Project(xml);

			// Assert
			var condition = Assert.Single(sut.PropertyGroups)!.Condition;
			Assert.NotNull(condition);
			Assert.Equal("condition-value", condition!.Expression);
		}

		[Fact]
		public void GroupOfPropertyShouldHaveCorrectReference()
		{
			// Arrange
			var sut = ConstructSimpleProject();

			// Act
			var actual = sut.PropertyGroups.Single().Properties.Single();

			// Assert
			Assert.Same(sut.PropertyGroups.Single(), actual.Group);
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
			var sut = ConstructSimpleProject();

			// Act
			var actual = sut.PropertyValue("NonExistingProperty");

			// Assert
			Assert.Null(actual);
		}

		[Fact]
		public void PropertyValueShouldHaveCorrectValueWhenFound()
		{
			// Arrange
			var sut = ConstructSimpleProject();

			// Act
			var actual = sut.PropertyValue("ExistingProperty");

			// Assert
			Assert.Equal("value", actual);
		}
	}
}