using System.Threading.Tasks;
using VsValidate.Validation.Rules;
using Xunit;

namespace VsValidate.Tests.Validation.Rules
{
	public class PackageReferenceRuleTests
	{
		[Fact]
		public async Task ValidateShouldFailWhenForbiddenReferenceExists()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.0")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Forbidden = true,
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenPackageVersionIsWrong()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Version = "^2.0.0",
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

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

			var data = new PackageReferenceRuleData
			{
				Required = true,
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenVersionIsInvalidSemanticVersion()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Version = "^1.0",
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenOptionalPackageIsReferenced()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Name = "Package",
				Required = false
			};

			var sut = new PackageReferenceRule(data);

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

			var data = new PackageReferenceRuleData
			{
				Required = false,
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenPackageHasCorrectVersion()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Version = "^1.0.0",
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}
	}
}