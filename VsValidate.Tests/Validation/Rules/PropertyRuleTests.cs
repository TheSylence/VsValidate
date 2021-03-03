using System.Threading.Tasks;
using VsValidate.Validation.Rules;
using Xunit;

namespace VsValidate.Tests.Validation.Rules
{
	public class PropertyRuleTests
	{
		[Fact]
		public async Task ValidateShouldFailWhenForbiddenPropertyIsFound()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithPropertyGroup().WithProperty("Forbidden-Property", "true").Build();

			var data = new PropertyRuleData
			{
				Forbidden = true,
				Name = "Forbidden-Property"
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenNonOptionalPropertyIsMissing()
		{
			// Arrange
			var project = new ProjectBuilder().Build();

			var data = new PropertyRuleData
			{
				Name = "Required-Property",
				Required = true
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenPropertyHasWrongValue()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("Property", "invalid")
				.Build();

			var data = new PropertyRuleData
			{
				Name = "Property",
				Value = "valid"
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenPropertyIsFoundTooFewTimes()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("Property", "value")
				.WithPropertyGroup()
				.WithProperty("Property", "value")
				.Build();

			var data = new PropertyRuleData
			{
				Name = "Property",
				MinimumOccurrences = 3
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenPropertyIsFoundTooManyTimes()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("Property", "value")
				.WithPropertyGroup()
				.WithProperty("Property", "value")
				.Build();

			var data = new PropertyRuleData
			{
				Name = "Property",
				MaximumOccurrences = 1
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenPropertyExists()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("TreatWarningsAsErrors", "true")
				.Build();

			var data = new PropertyRuleData
			{
				Name = "TreatWarningsAsErrors",
				Required = true
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenPropertyHasCorrectValue()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithPropertyGroup()
				.WithProperty("TreatWarningsAsErrors", "true")
				.Build();

			var data = new PropertyRuleData
			{
				Name = "TreatWarningsAsErrors",
				Required = true,
				Value = "true"
			};

			var sut = new PropertyRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}
	}
}