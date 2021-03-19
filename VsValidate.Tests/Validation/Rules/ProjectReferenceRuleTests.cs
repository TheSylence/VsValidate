using System.Threading.Tasks;
using VsValidate.Validation.Rules;
using Xunit;

namespace VsValidate.Tests.Validation.Rules
{
	public class ProjectReferenceRuleTests
	{
		[Fact]
		public async Task ValidateShouldFailWhenForbiddenReferenceExists()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithProjectReference("Project")
				.Build();

			var data = new ProjectReferenceRuleData
			{
				Forbidden = true,
				Name = "Project"
			};

			var sut = new ProjectReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenRequiredReferenceIsMissing()
		{
			// Arrange
			var project = new ProjectBuilder().Build();

			var data = new ProjectReferenceRuleData
			{
				Required = true,
				Name = "Project"
			};

			var sut = new ProjectReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenOptionalProjectIsReferenced()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithProjectReference("Project")
				.Build();

			var data = new ProjectReferenceRuleData
			{
				Name = "Project",
				Required = false
			};

			var sut = new ProjectReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenOptionalReferenceIsMissing()
		{
			// Arrange
			var project = new ProjectBuilder().Build();

			var data = new ProjectReferenceRuleData
			{
				Required = false,
				Name = "Project"
			};

			var sut = new ProjectReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}
	}
}